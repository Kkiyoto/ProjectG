using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/* State_manage
 * ゲーム自体に関係なくても必要かなという部分を書こうかと
 * 主にUIかと思います
 * バックミュージックもここ?
 */

public class State_manage : Functions
{
    public GameObject Back_anime, Front_anime,
        time_gage, skill_Icon;//,Skill_Flame;
    public RectTransform Pause_Menu,Needle;
    public Image gage,Flame, Background;
    public Animator Player;
    public float width, height, time;
    float needle, time_delta, Max_Time;
    bool pause_bool, is_red;
    public bool timer_bool, bg_bool;
    public Text Time_text, Skill_text,Degital_Time;
    int Life_point;
    int Road_count, All_count;
    public float[] tresure = new float[2];
    public float road = 0;
    public Party[] Chara = new Party[3];
    AudioClip[] SEs = new AudioClip[17];//音増えるごとに追加お願いします
    public Main main;
    public AudioSource[] BGMs;
    private int last = 0;
    private bool battle_move = false, max_bool = false;

    GameObject Battle_enemy;
    float skill_time;//後でキャラによって変更、多分配列化

    [SerializeField]
    Camera _camera;                // カメラの座標
    [SerializeField]
    ParticleSystem skillEffect;    // スキルの際のエフェクト
    GameObject skill_effect_anim;
    GameObject skill_icon_effect;

    // Use this for initialization
    void Start()
    {
        /*#region Find系
        main = GameObject.Find("Director").GetComponent<Main>();
        gage = GameObject.Find("Gage");
        time_gage = GameObject.Find("Time_gage");
        skill_Icon = GameObject.Find("Skill_Icon");
        Flame = GameObject.Find("Flame");
        Skill_Flame = GameObject.Find("Skill_Flame");
        #endregion*/
        width = Screen.width;
        height = Screen.height;
        #region アニメの背景1
        Back_anime.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Background/Bg" + 0);
        Back_anime.GetComponent<RectTransform>().sizeDelta = new Vector2(height * 2.8f, height * 0.28f);
        Back_anime.GetComponent<RectTransform>().localPosition = new Vector3(0, height * 0.292f);
        #endregion
        #region アニメの背景2
        Front_anime.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Background/Bg_front" + 0);
        Front_anime.GetComponent<RectTransform>().sizeDelta = new Vector2(height * 2.79f, height * 0.37f);
        Front_anime.GetComponent<RectTransform>().localPosition = new Vector3(0, height * 0.2986f);
        #endregion
        #region キャラクターのアニメ
        GameObject dictionary = GameObject.Find("dictionary");
        for (int i = 0; i < 3; i++)
        {
            GameObject o = GameObject.Find("Chara" + i);
            int ID = PlayerPrefs.GetInt("Party" + i, i + 2);
            int Level = PlayerPrefs.GetInt("Level" + i,0);
            Chara[i] = new Party(o, ID);
            dictionary.GetComponent<Dictionary>().Set_Box(Chara[i], ID,Level);
            Chara[i].Pos = new Vector3(-width * 0.8f * (i + 1), height * 0.277f);
        }
        //Destroy(dictionary);
        /*PlayerPrefs.SetInt("Party0", 3);
        PlayerPrefs.SetInt("Party1", 1);
        PlayerPrefs.SetInt("Party2", 2);
        PlayerPrefs.SetInt("Coin", 234);
        PlayerPrefs.SetInt("treasure0", 2);
        PlayerPrefs.SetInt("treasure1", 1);
        PlayerPrefs.SetInt("Time", 357);
        PlayerPrefs.SetInt("Life", 2);
        PlayerPrefs.SetInt("enemy", 5);  //ここを使うとResultリセット*/
        #endregion
        #region スキル
        Road_count = 0;
        All_count = 0;
        skill_time = 20;
        Skill_text.text = Chara[0].skill_Description;
        skill_Icon.GetComponent<Animator>().SetInteger("Chara_Int", Top_ID());

        //skill_effect_anim = GameObject.Find("Skill_effect");
        #endregion
        pause_bool = false;
        timer_bool = false;
        bg_bool = false;
        time = Chara[0].HP + Chara[1].HP + Chara[2].HP;
        needle = time;
        Max_Time = time / 2f;
        Life_point = 2;
        //Battle_enemy = GameObject.Find("BattleEnemy");
        #region UIオブジェクトをheight,widthで整理します。（ずれないのなら）Mapのとこより下は消しても可 ★Dictionに移植
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                GameObject o = Instantiate(Resources.Load<GameObject>("Prefab/Small_map"));
                o.transform.parent = GameObject.Find("Map_base").transform;
                o.GetComponent<RectTransform>().localPosition = new Vector3(width, width);
                o.GetComponent<RectTransform>().sizeDelta = new Vector2(0.066f * width, 0.066f * width);
                o.name = "Small_map" + i + "-" + j;
            }
        }
        #endregion

        
        Pause_Menu.sizeDelta = new Vector2(0.9f * width, 0.95f * height);
        Pause_Menu.localPosition = new Vector3(0, height, -5);
        tresure[0] = 0;
        tresure[1] = 0;

        read_sounds();
        is_red = false;

        GameObject.Find("Map_Goal").GetComponent<RectTransform>().localPosition = new Vector3(0.1f * width, 0.066f * width);
        GameObject.Find("Map_Goal").GetComponent<RectTransform>().sizeDelta = new Vector2(0.03f * width, 0.066f * width);
    }

    // Update is called once per frame
    void Update()
    {
        #region 背景の動画
        if (!pause_bool && bg_bool)
        {
            Back_anime.GetComponent<RectTransform>().Translate(new Vector3(-height * 0.001f, 0, 0));
            Front_anime.GetComponent<RectTransform>().Translate(new Vector3(-height * 0.001f, 0, 0));
            if (Back_anime.GetComponent<RectTransform>().localPosition.x < -0.7f * height)
            {
                Back_anime.GetComponent<RectTransform>().position += new Vector3(1.4f * height, 0, 0);
                Front_anime.GetComponent<RectTransform>().position += new Vector3(1.4f * height, 0, 0);
            }
        }
        #endregion
        #region キャラクター移動
        for (int i = 0; i <= Life_point; i++)
        {
            Vector3 vec = Chara[i].Pos;
            if ((vec - new Vector3(-width * 0.2f * (i - 0.2f), height * 0.277f)).magnitude < 0.01f)
                Chara[i].Pos = new Vector3(-width * 0.2f * (i - 0.2f), height * 0.277f);
            else
                Chara[i].Pos = (39f * vec + new Vector3(-width * 0.2f * (i - 0.2f), height * 0.277f)) / 40f;
        }
        #endregion
        #region 時間表示
        if (!pause_bool && timer_bool)
        {
            if (is_Skill(5)) time += Time.deltaTime / 3f;
            time -= Time.deltaTime;
            int m = Mathf.FloorToInt(needle / 60f);
            int s = Mathf.FloorToInt(needle % 60f);
            Degital_Time.text = (m.ToString().PadLeft(2, '0') + " : " + s.ToString().PadLeft(2, '0'));
            Needle.localRotation = new Quaternion(0, 0, 1, 1 - needle / Max_Time);
            Flame.color -= new Color(0, 0, 0, 0.01f);
        }
        if (time < 0) //GameOver
        {
            time = 0;
            main.Goal(1);
        }
        if (time < needle)
        {
            time_gage.GetComponent<RectTransform>().localRotation = new Quaternion(0, 0, 1, 1 - needle / Max_Time);
            time_gage.GetComponent<Image>().fillAmount = (needle - time) / Max_Time / 4f;
            if (!pause_bool && timer_bool)
            {
                if (time_delta < 0) time_delta += Time.deltaTime;
                else needle -= 0.7f;
            }
        }
        else
        {
            time_gage.GetComponent<RectTransform>().localRotation = new Quaternion(0, 0, 1, 1 - time / Max_Time);
            time_gage.GetComponent<Image>().fillAmount = (time - needle) / Max_Time / 4f;
            if (!pause_bool && timer_bool)
            {
                if (time_delta < 0) time_delta += Time.deltaTime;
                else needle += 0.7f;
            }
        }
        if (needle < 80 && !is_red)
        {
            Back_anime.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Background/BG_red" + (int)Common.Thema.Sky);
            Front_anime.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Background/Bg_front_red" + (int)Common.Thema.Sky);
            Background.sprite = Resources.Load<Sprite>("Images/Background/Back_red" + (int)Common.Thema.Sky);
            is_red = true;
            main.To_Red(true);
        }
        #endregion
        #region スキル
        Gage();
        if (skill_time < 20 && !pause_bool && timer_bool) skill_time += Time.deltaTime;
        else if (skill_time < 0)
        {
            skill_time += Time.deltaTime;
            if (skill_time < -0.2f)
            {
                var pos = _camera.ScreenToWorldPoint(new Vector3(Random.Range(0.12f, 0.88f) * width, Random.Range(0.13f, 0.6f) * height, 10));
                skillEffect.transform.position = pos;
                skillEffect.Emit(1);

                skill_effect_anim.GetComponent<Animator>().SetInteger("Skill_int", Top_ID()); // スキルエフェクト 追加
                max_bool = false;
                skill_icon_effect.GetComponent<Animator>().SetBool("Icon_effect", false);

                Debug.Log("Skill ok");
            }
            else
            {
                Anime(0, Common.Action.Walk);
                Anime(1, Common.Action.Walk);
                Anime(2, Common.Action.Walk);
                main.Pause_button_down(false);
                bg_bool = true;
                timer_bool = true;

                skill_effect_anim.GetComponent<Animator>().SetInteger("Skill_int", 10); // スキルエフェクト 消去
            }
        }
        if (skill_time < Chara[0].Max_second)
        {
            var pos = _camera.ScreenToWorldPoint(new Vector3(Random.Range(0.12f, (1 - skill_time / Chara[0].Max_second) * 0.66f) * width, 0.04f * height, 10));
            skillEffect.transform.position = pos;
            skillEffect.Emit(1);
            /*float ran = Random.value;
            pos = _camera.ScreenToWorldPoint(new Vector3(0, 0, 10)) + new Vector3(1.4f * Mathf.Cos(ran * 2 * Mathf.PI) + 1.7f, 1.4f * Mathf.Sin(ran * 2 * Mathf.PI) + 2f);
            //pos = _camera.ScreenToWorldPoint(new Vector3(0,0,10)) + new Vector3(1.4f*skill_time * Mathf.Cos(skill_time*16) + 1.7f, 1.4f*skill_time * Mathf.Sin(skill_time*16) +2f);
            skillEffect.transform.position = pos;
            skillEffect.Emit(1);*/
            

            /* ここを消しても 
            Skill_Flame.GetComponent<Image>().color = new Color(1, 1, 1, (Chara[0].Max_second - skill_time) * 3f);
            Skill_Flame.GetComponent<RectTransform>().Translate(new Vector3(0, height / 180f, 0));
            if (Skill_Flame.GetComponent<RectTransform>().localPosition.y > height * 1.5f) Skill_Flame.GetComponent<RectTransform>().localPosition = new Vector3(0, -height * 1.5f, 0);
            Skill_Flame 止められません...。*/
        }
        else if (skill_time < Chara[0].Max_second + 0.1f)
        {
            if (Chara[0].skills[6] < 30) main.Set_Speed(90f);
        }
        //if (Chara[0].walk_count >= Chara[0].Max_gage)
        if (Road_count >= Chara[0].Max_gage)
        {

            if (!max_bool)
            {
                SE_on(Common.SE.Decision);  // 追加SE
                skill_icon_effect.GetComponent<Animator>().SetBool("Icon_effect", true);
                max_bool = true;
            }

            var pos = _camera.ScreenToWorldPoint(new Vector3(Random.Range(0.18f, 0.66f) * width, 0.04f * height, 10));
            skillEffect.transform.position = pos;
            skillEffect.Emit(1);
            if (skill_time >= 20 && !pause_bool && timer_bool) skill_time += Time.deltaTime;
            if (skill_time > 30) skill_time -= 10;
            skill_Icon.GetComponent<RectTransform>().sizeDelta = new Vector2((0.055f + 0.01f * Mathf.Sin(Mathf.PI * skill_time / 1f)) * height, (0.055f + 0.01f * Mathf.Sin(Mathf.PI * skill_time / 1f)) * height);
        }
        #endregion
        #region ポーズメニュー
        Vector3 v = Pause_Menu.localPosition;
        if (pause_bool && v.z < 0)
        {
            float x = v.z * 11f;
            x++;
            Pause_Menu.localPosition = new Vector3(0, x * (x + 20) * 0.0005f * height, x / 11f);
            if (Mathf.Abs(v.z) < 0.1f) Pause_Menu.localPosition = Vector3.zero;
        }
        else if (!pause_bool && v.z > -5)
        {
            float x = v.z * 11f;
            x--;
            Pause_Menu.localPosition = new Vector3(0, x * (x + 20) * 0.0005f * height, x / 11f);
            if (Mathf.Abs(v.z + 5) < 0.1f)
            {
                Pause_Menu.localPosition = new Vector3(0, height, -5.1f);
                main.Pause_button_down(false);
            }
        }
        #endregion
    }

    public void Pause_flg(bool pause)
    {
        pause_bool = pause;

        int m = Mathf.FloorToInt(time / 60f);
        int s = Mathf.FloorToInt(time % 60f);
        Time_text.text = ("Time   " + m.ToString().PadLeft(2, '0') + " : " + s.ToString().PadLeft(2, '0'));
        GameObject.Find("Walk_F").GetComponent<Text>().text = "踏破率：" + Mathf.RoundToInt(road / 72f * 100f) + " %";
        GameObject.Find("Walk").GetComponent<Text>().text = "歩いた距離：" + All_count + "歩";
        GameObject.Find("Item_coin").GetComponent<Image>().fillAmount = tresure[1] / 3f;
        GameObject.Find("Item_weapon").GetComponent<Image>().fillAmount = tresure[0] / 3f;
    }

    public void Lose_Time(float minus) //単位は秒
    {
        time -= minus;
        time_delta = -1;
        if (minus > 0)
        {
            time_gage.GetComponent<Image>().color = new Color(1, 0, 0, 0.5f);
            Flame.color = new Color(1, 0, 0, 1);
        }
        else
        {
            time = Mathf.Min(time, Max_Time*2f);
            time_gage.GetComponent<Image>().color = new Color(0, 1, 0, 0.5f);
            Flame.color = new Color(1, 1, 0, 1);
        }
    }

    public bool Damage()
    {
        Chara[0].Anime().SetBool("Out_Bool", true);
        Life_point--;
        skill_time = 20;
        if (Chara[1].skills[6] > 30) main.Set_Speed(55);
        else main.Set_Speed(90f);
        skill_Icon.GetComponent<Animator>().SetInteger("Chara_Int", Top_ID());
        //Skill_Flame.GetComponent<Image>().color = Color.clear;
        if (Life_point < 0)//GameOver
        {
            Skill_text.text = "";
            return true;
        }
        else
        {
            Party tmp = Chara[0];
            Chara[0] = Chara[1];
            Chara[1] = Chara[2];
            Chara[2] = tmp;
            Skill_text.text = Chara[0].skill_Description;
            skill_Icon.GetComponent<RectTransform>().sizeDelta = new Vector2(0.055f * height, 0.055f * height);
            skill_Icon.GetComponent<Animator>().SetInteger("Chara_Int", Top_ID());
            return false;
        }
    }

    public int Top_ID() //一番前のキャラの図鑑ID（アニメータ）
    {
        if (Life_point < 0) return 0;
        else return Chara[0].chara_ID;
    }

    public int get_Life()
    {
        return Life_point;
    }

    public float get_Time()
    {
        return time;
    }

    public void Anime(int ID, Common.Action action) //ID:見えてるうち、後ろから何番目？、Commn.Action.Battleとかを入れたら遷移するようにする
    {
        Chara[ID].Anime().SetInteger("Move_Int", (int)action);
    }

    public void Enemy_Anime(bool isBattle, Common.Type type)//敵にもAction増えたら追加 , Common.Action action) //多分IDは基本的にtouchID
    {
        if (isBattle)
        {
            Battle_enemy.GetComponent<Animator>().SetBool("Battle_start", true);
            Battle_enemy.GetComponent<Animator>().SetInteger("EnemyInt", (int)type);
            //Battle_enemy.GetComponent<Animator>().SetInteger("Move_Int", (int)action);
        }
        else
        {
            Battle_enemy.GetComponent<Animator>().SetBool("Battle_start", false);
            Battle_enemy.GetComponent<Animator>().SetTrigger("BattleEndTrigger");
            Battle_enemy.GetComponent<Animator>().SetInteger("EnemyInt", 0);
        }
    }

    public void Small_map(int x, int y) //右上の地図の枠
    {
        GameObject o = GameObject.Find("Small_map" + x + "-" + y);
        o.GetComponent<RectTransform>().localPosition = new Vector3(0.066f * (x - 1) * width, 0.066f * (y - 1) * width);
        o.GetComponent<Image>().color = new Color(0.7f, 0.4f, 0, 1);
    }

    public void Item_Life()
    {
        if (Life_point < 2)
        {
            Life_point++;
            Chara[Life_point].Pos = new Vector3(-width * 2f , height * 0.277f);
            Chara[Life_point].Anime().SetBool("Out_Bool", false);
            Flame.color = new Color(1, 1, 0, 1);
            Anime(Life_point, Common.Action.Walk);
        }
    }

    #region ゲーム終了らへんの操作
    public bool To_result(int Effect_or_Treasure) //シーン内遷移をしても良いか
    {
        if (Effect_or_Treasure == 0)
        {
            AnimatorStateInfo info = GameObject.Find("Effect").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
            return (info.IsName("Game_Over_Time") || info.IsName("Game_Over_Life")) && info.normalizedTime > 0.5f;
        }
        else
        {
            GameObject.Find("light").GetComponent<RectTransform>().sizeDelta *= 1.05f;
            RectTransform tra = GameObject.Find("Start_and_End_anim").GetComponent<RectTransform>();
            if (tra.localPosition.x < 0) tra.Translate(new Vector3(width / 30f, 0));
            /*return GameObject.Find("Goal").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Treasure_Open")
        && GameObject.Find("Goal").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1f;*/
            return GameObject.Find("light").GetComponent<RectTransform>().sizeDelta.y > height * 3.5f;
        }
    }

    public void Set_Button() //ゲームオーバー時のボタンを作る
    {
        Color col = GameObject.Find("GameOver_text").GetComponent<Image>().color + new Color(0, 0, 0, 0.01f);
        GameObject.Find("GameOver_text").GetComponent<Image>().color = col;
        GameObject.Find("Game_over").GetComponent<Image>().color = col;
        GameObject.Find("Retry").GetComponent<Image>().color = col;
        GameObject.Find("GameOver_text").GetComponent<RectTransform>().localPosition = new Vector3(0, 0.1f * height, 0);
        GameObject.Find("Game_over").GetComponent<RectTransform>().localPosition = new Vector3(0, -0.2f * height, 0);
        GameObject.Find("Retry").GetComponent<RectTransform>().localPosition = new Vector3(0, -0.08f * height, 0);
    }

    public void Effect(int s) //全画面に出したいとき
    {
        GameObject o = GameObject.Find("Effect");
        o.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
        o.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        o.GetComponent<Animator>().SetInteger("Change_Bool", s);
        if (s == 3)
        {
            GameObject.Find("Time_base").GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/GameScene/end");
            GameObject.Find("Time_base").GetComponent<RectTransform>().localPosition = new Vector3(-width, 0.3f * height);
            GameObject.Find("Time_base").GetComponent<RectTransform>().sizeDelta = new Vector3(0.8f * width, 0.13f * height);
            o = GameObject.Find("GameOver_text");
            o.GetComponent<RectTransform>().localPosition = new Vector3(-width, 0, 0);
            o.GetComponent<Animator>().SetTrigger("Goal_Trigger");
        }
        else
        {
            GameObject.Find("GameOver_text").GetComponent<Image>().color = new Color(1, 1, 1, 0);
            GameObject.Find("Game_over").GetComponent<Image>().color = new Color(1, 1, 1, 0);
            GameObject.Find("Retry").GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }
    }

    public void Retry() //リトライを押したとき
    {
        time = 300;
        needle = 300;
        Life_point = 2;
        GameObject.Find("Effect").GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        GameObject.Find("Game_over").GetComponent<RectTransform>().localPosition = new Vector3(0, height, 0);
        GameObject.Find("GameOver_text").GetComponent<RectTransform>().localPosition = new Vector3(0, height, 0);
        GameObject.Find("Retry").GetComponent<RectTransform>().localPosition = new Vector3(0, height, 0);
        Change_Chara(true);
        for (int i = 0; i < 3; i++)
        {
            Chara[i].Pos = new Vector3(-width * 0.65f * (4 - i), height * 0.277f);
            Chara[i].Anime().SetBool("Out_Bool", false);
            Anime(i, Common.Action.Walk);
        }
        Back_anime.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Background/BG" + (int)Common.Thema.Sky);
        Front_anime.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Background/Bg_front" + (int)Common.Thema.Sky);
        GameObject.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Background/Back" + (int)Common.Thema.Sky);
        main.To_Red(false);
        /*GameObject.Find("Player").GetComponent<Animator>().SetInteger("Chara_Int", Top_ID());
        Skill_text.text = Chara[0].skill_Description;
        skill_time = 20;*/
        timer_bool = true;
        bg_bool = true;
        BGM_on(Common.BGM.tutorial); // ここでゲームオーバーBGM定義
        skill_Icon.GetComponent<RectTransform>().sizeDelta = new Vector2(0.055f * height, 0.055f * height);
        skill_Icon.GetComponent<Animator>().SetInteger("Chara_Int", Top_ID());
    }

    public bool To_goal() //最後の宝箱の動きが終わったかどうか
    {
        RectTransform rect = GameObject.Find("GameOver_text").GetComponent<RectTransform>();
        float x = rect.localPosition.x / width;
        rect.localPosition = new Vector3((x + 0.005f) * width, (x * x - 0.2f) * height);
        rect.sizeDelta = new Vector2((x + 0.7f) * width, (x + 0.7f) * width) / 2f;
        if (x < 0) GameObject.Find("Time_base").GetComponent<RectTransform>().localPosition = (new Vector3(width * x * 2, -0.2f * height));
        else if (x > 0.5f) GameObject.Find("Time_base").GetComponent<RectTransform>().localPosition = (new Vector3(width * (x - 0.5f) * 4, -0.2f * height));
        return x > 1;
    }

    public float score()
    {
        float s = 1000f * (Life_point + 1) + 5 * time + 3 * road + All_count * All_count;
        return s;
    }

    public void To_Menu()
    {
        SceneManager.LoadScene("Home");
    }
    
    public void On_Button()
    {
        SE_on(Common.SE.Button);
    }
    #endregion

    #region　スキル
    public void Gage() //スキルゲージを
    {
        if (Chara[0].Max_second > skill_time)
        {
            gage.GetComponent<Image>().fillAmount = 1 - skill_time / Chara[0].Max_second;
        }
        else
        {
            float max = Chara[0].Max_gage;
            float child = Road_count;
            //float child = Chara[0].walk_count;
            gage.color = new Color(1, 1, 1, 1);
            gage.fillAmount = child / max;
        }
    }

    public void Road_counter()
    {
        if (Chara[0].Max_second < skill_time)
        {
            Road_count++;
            //Chara[0].walk_count++;
        }
        All_count++;
    }

    public void Skill_On() //スキルボタンを押したとき
    {
        if (Road_count >= Chara[0].Max_gage)//●押しちゃいけないとき
        {
            skill_time = -2;//秒
            Road_count = 0;
            //Chara[0].walk_count = 0;
            gage.color = new Color(0, 1, 1, 1);
            //main.Pause_button_down(true);
            pause_bool = false;
            bg_bool = false;
            timer_bool = false;
            Anime(0, Common.Action.Happy);
            Anime(1, Common.Action.Stop);
            Anime(2, Common.Action.Stop);
            skill_Icon.GetComponent<RectTransform>().sizeDelta = new Vector2(0.055f * height, 0.055f * height);
            GetComponent<AudioSource>().PlayOneShot(Chara[0].skill_SE);
            if (Chara[0].skills[6] > 0) main.Set_Speed(55f);
            if (Chara[0].skills[8] > 0) main.Back();

            /* ここを消しても 
            Skill_Flame.GetComponent<Image>().sprite = Chara[0].skill_img;
            Skill_Flame 止められません...。*/

        }
    }

    public bool is_Skill(int n)
    {
        return skill_time < Chara[0].skills[n] && (skill_time > 0||n==3||n==4);
    }
    #endregion

    public void Change_Chara(bool b)
    {
        if (main.Flg(1) || b)
        {
            Party tmp = Chara[0];
            for (int i = 0; i < Life_point; i++)
            {
                Chara[i] = Chara[i + 1];
            }
            Chara[Life_point] = tmp;
            Chara[Life_point].Index = 3;
            Player.SetInteger("Chara_Int", Top_ID());
            Skill_text.text = Chara[0].skill_Description;
            skill_time = 20;
            skill_Icon.GetComponent<RectTransform>().sizeDelta = new Vector2(0.055f * height, 0.055f * height);
            skill_Icon.GetComponent<Animator>().SetInteger("Chara_Int", Top_ID());
            //Skill_Flame.GetComponent<Image>().color = Color.clear;
            if (Chara[0].skills[6] > 30) main.Set_Speed(55f);
            else main.Set_Speed(90f);
        }
    }

    public void SE_on(Common.SE music)
    {
        GetComponent<AudioSource>().PlayOneShot(SEs[(int)music]);
    }

    public void BGM_on(Common.BGM music)
    {
        if (last == 0) // スタート時
        {
            BGMs[(int)music].Play();
        }
        if (last == 4) // リトライ時
        {
            BGMs[0].Stop();
            BGMs[(int)music].volume = 0.3f;
            BGMs[(int)music].Stop();
            BGMs[(int)music].Play();
            for (int i = 0; i < 6; i++)
                Invoke("BGM_volume_set", 0.2f * i);
        }
        else if (last == 1 && (int)music == 2)      // 通常 → 戦闘へ遷移時
        {
            BGMs[last].Pause();
            BGMs[(int)music].Play();
        }
        else if (last == 1 && (int)music == 3)       // 通常 → リザルトへ遷移時
        {
            BGMs[last].Pause();
            GetComponent<AudioSource>().PlayOneShot(SEs[13]);
        }
        else if (last == 1 && (int)music == 4)       // 通常 → リタイアへ遷移時
        {
            BGMs[last].Pause();
            BGMs[0].volume = 0.1f;
            GetComponent<AudioSource>().PlayOneShot(SEs[14]);
            for (int i = 0; i < 8; i++)
                Invoke("SE_volume_set", 0.2f * i);
        }
        else if (last == 2 && (int)music == 2)
        {              // 戦闘 → 戦闘への遷移時
            BGMs[last].Stop();
            BGMs[(int)music].Play();
        }
        else if (last == 2)
        {              // 戦闘 → 通常への遷移時 この時だけフェードイン
            BGMs[last].Stop();
            BGMs[(int)music].volume = 0.1f;
            BGMs[(int)music].UnPause();
            for (int i = 0; i < 4; i++)
                Invoke("BGM_volume_set", 0.2f * i);
        }
        last = (int)music;
    }
    // Invokeで使うので、引数なしの形です
    public void BGM_volume_set() // 最終BGM 疑似フェード
    {
        BGMs[last].volume += 0.1f;
    }
    public void BGM_volume_set_out(float volume) // BGM 疑似フェード
    {
        BGMs[1].volume += volume;
    }
    public void SE_volume_set()
    {
        BGMs[0].volume += 0.1f;
    }

    public void read_sounds()
    {
        #region SEの設定
        SEs[0] = Resources.Load<AudioClip>("Audio/SE/Time");
        SEs[1] = Resources.Load<AudioClip>("Audio/SE/Fall");
        SEs[2] = Resources.Load<AudioClip>("Audio/SE/Win");
        SEs[3] = Resources.Load<AudioClip>("Audio/SE/Get");
        SEs[4] = Resources.Load<AudioClip>("Audio/SE/Button");
        SEs[5] = Resources.Load<AudioClip>("Audio/SE/Slide");
        SEs[6] = Resources.Load<AudioClip>("Audio/SE/Fire");
        SEs[7] = Resources.Load<AudioClip>("Audio/SE/Ice");
        SEs[8] = Resources.Load<AudioClip>("Audio/SE/Sword");
        SEs[9] = Resources.Load<AudioClip>("Audio/SE/Gun");
        SEs[10] = Resources.Load<AudioClip>("Audio/SE/Coin");
        SEs[11] = Resources.Load<AudioClip>("Audio/SE/Count");
        SEs[12] = Resources.Load<AudioClip>("Audio/SE/Stamp");
        SEs[13] = Resources.Load<AudioClip>("Audio/SE/Result");
        SEs[14] = Resources.Load<AudioClip>("Audio/SE/Retired");
        SEs[15] = Resources.Load<AudioClip>("Audio/SE/Skill");
        SEs[16]= Resources.Load<AudioClip>("Audio/SE/Decision");

        #endregion

        #region BGMの設定
        BGMs = GameObject.Find("State_manager").GetComponents<AudioSource>();
        #endregion

        skill_icon_effect= GameObject.Find("Skill_Icon_effect");
        skill_effect_anim = GameObject.Find("Skill_effect");
        Battle_enemy = GameObject.Find("BattleEnemy");

    }
    #region　バトルのアニメーション関連 // 書き直し必須

    public void Battle_move_anim(int type)
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject chara = GameObject.Find("Chara" + i);

            if (type == 1)
            {      // 開始時の後退
                chara.GetComponent<RectTransform>().localPosition += new Vector3(0.02f * width, -0.02f * height);
                //Debug.Log(chara.GetComponent<RectTransform>().sizeDelta);
                chara.GetComponent<RectTransform>().sizeDelta *= 0.9f;
            }
            else if (type == 2) // 終了時の進行
            {
                chara.GetComponent<RectTransform>().localPosition += new Vector3(-0.03f * width, 0.02f * height);
                chara.GetComponent<RectTransform>().sizeDelta /= 0.9f;
            }
            else if (type == 3 && !battle_move) // バトル時の進退
            {
                //chara.GetComponent<RectTransform>().localPosition += new Vector3(0.005f * width,0);
                Move_set(chara, 0.01f);

            }
            else if (type == 3 && battle_move) // バトル時の進退
            {
                //chara.GetComponent<RectTransform>().localPosition += new Vector3(-0.005f * width, 0);
                Move_set(chara, -0.01f);

            }
        }
        battle_move = !battle_move;
    }

    public void Move_set(GameObject chara, float range)
    {
        if (range >= 0)
        {
            for (float i = 0.0f; i < range; i += 0.001f)
                chara.GetComponent<RectTransform>().localPosition += new Vector3(i * width, 0);
        }
        else
        {
            for (float i = 0.0f; i < range; i -= 0.001f)
                chara.GetComponent<RectTransform>().localPosition += new Vector3(i * width, 0);
        }


    }
    #endregion

}