﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* State_manage
 * ゲーム自体に関係なくても必要かなという部分を書こうかと
 * 主にUIかと思います
 * バックミュージックもここ?
 */

public class State_manage : Functions
{
    GameObject Back_anime, Front_anime, Pause_Menu;
    float width, height, time;
    //float Max_Time, needle;RectTransform Needle; 時計盤の準備
    bool pause_bool;
    public bool timer_bool, bg_bool;
    Text Time_text, Skill_text;
    int Life_point;
    int Road_count;
    Party[] Chara = new Party[3];
    AudioClip[] SEs = new AudioClip[6];//音増えるごとに追加お願いします
    AudioClip[] BGM = new AudioClip[5];

    GameObject Battle_enemy;
    public float skill_time;//後でキャラによって変更、多分配列化（今考えているのはCharactorスクリプトに移植 ★Partyにします）

    // Use this for initialization
    void Start()
    {
        width = Screen.width;
        height = Screen.height;
        #region アニメの背景1
        Back_anime = GameObject.Find("Back_anime");
        Back_anime.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Background/Bg" + 0);
        Back_anime.GetComponent<RectTransform>().sizeDelta = new Vector2(height * 2.8f, height * 0.28f);
        Back_anime.GetComponent<RectTransform>().localPosition = new Vector3(0, height * 0.292f);
        #endregion
        #region アニメの背景2
        Front_anime = GameObject.Find("Back_front");
        Front_anime.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Background/Bg_front" + 0);
        Front_anime.GetComponent<RectTransform>().sizeDelta = new Vector2(height * 2.79f, height * 0.37f);
        Front_anime.GetComponent<RectTransform>().localPosition = new Vector3(0, height * 0.2986f);
        #endregion
        #region ゲームの後ろの背景
        /*GameObject o = GameObject.Find("Background");  //これで配置していく
        o.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Background/Back" + 0);
        o.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        o.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);*/
        //o = GameObject.Find("Effect");  //これで配置していく
        //o.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        //o.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
        #endregion
        #region キャラクターのアニメ
        for (int i = 0; i < 3; i++)
        {
            GameObject o = GameObject.Find("Chara" + i);
            int ID = PlayerPrefs.GetInt("Party" + i, i + 2);
            Chara[i] = new Party(o, ID);
            GameObject.Find("dictionary").GetComponent<Dictionary>().Set_Box(Chara[i], ID);
            Chara[i].Pos = new Vector3(width * 0.8f * (i + 1), height * 0.277f);
        }
        /*PlayerPrefs.SetInt("Party0", 3);
        PlayerPrefs.SetInt("Party1", 1);
        PlayerPrefs.SetInt("Party2", 2);
        PlayerPrefs.SetInt("Coin", 234);
        PlayerPrefs.SetInt("treasure0", 2);
        PlayerPrefs.SetInt("treasure1", 1);
        PlayerPrefs.SetInt("Time", 357);
        PlayerPrefs.SetInt("Life", 2);
        PlayerPrefs.SetInt("enemy", 5);  //ここを使うとResultリセット
        PlayerPrefs.SetInt("Party" + 0, 2);
        PlayerPrefs.SetInt("Party" + 1, 4);
        PlayerPrefs.SetInt("Party" + 2, 3);*/
        #endregion
        #region SEの設定
        SEs[0] = Resources.Load<AudioClip>("Time");//例として置いときます。名前も数も変えておいてください
        #endregion
        #region スキル
        Skill_text = GameObject.Find("Skill_Text").GetComponent<Text>();
        Road_count = 0;
        skill_time = 20;
        Skill_text.text = Chara[0].skill_Description;
        #endregion
        pause_bool = false;
        timer_bool = false;
        bg_bool = false;
        time = 300;
        /*Max_Time = 600; //タイム表示が変更になった時に入れる
        needle = 600;
        Needle = GameObject.Find("Needle").GetComponent<RectTransform>();
        Needle.sizeDelta = new Vector2(0.5f * width, 0.5f * width);
        Needle.localPosition = new Vector3(0, 0.5f * height);*/
        Time_text = GameObject.Find("Time").GetComponent<Text>();
        Life_point = 2;
        Battle_enemy = GameObject.Find("BattleEnemy");
        #region UIオブジェクトをheight,widthで整理します。（ずれないのなら）Mapのとこより下は消しても可
        GameObject.Find("Map_base").GetComponent<RectTransform>().localPosition = new Vector3(0.395f * width, 0.5f * height - 0.105f * width);
        GameObject.Find("Map_base").GetComponent<RectTransform>().sizeDelta = new Vector2(0.21f * width, 0.21f * width);
        GameObject.Find("Image").GetComponent<RectTransform>().localPosition = new Vector3(0, 0.465f * height);
        GameObject.Find("Image").GetComponent<RectTransform>().sizeDelta = new Vector2(width, 0.07f * height);
        GameObject.Find("Pause").GetComponent<RectTransform>().localPosition = new Vector3(0.03f * height - 0.5f * width, 0.465f * height);
        GameObject.Find("Pause").GetComponent<RectTransform>().sizeDelta = new Vector2(0.05f * height, 0.05f * height);
        GameObject.Find("Time").GetComponent<RectTransform>().localPosition = new Vector3(-0.2f * width, 0.465f * height);
        GameObject.Find("Time").GetComponent<RectTransform>().sizeDelta = new Vector2(0.4f * width, 0.07f * height);
        GameObject.Find("Skill").GetComponent<RectTransform>().localPosition = new Vector3(-0.13f * width, -0.46f * height);
        GameObject.Find("Skill").GetComponent<RectTransform>().sizeDelta = new Vector2(0.7f * width, 0.08f * height);
        GameObject.Find("Skill_Text").GetComponent<RectTransform>().localPosition = new Vector3(0.06f * width, 0.02f * height);
        GameObject.Find("Skill_Text").GetComponent<RectTransform>().sizeDelta = new Vector2(0.58f * width, 0.06f * height);
        GameObject.Find("Gage").GetComponent<RectTransform>().sizeDelta = new Vector2(0.7f * width, 0.08f * height);
        GameObject.Find("button").GetComponent<RectTransform>().sizeDelta = new Vector2(0.7f * width, 0.08f * height);
        GameObject.Find("Outer").GetComponent<RectTransform>().sizeDelta = new Vector2(0.7f * width, 0.08f * height);
        GameObject.Find("Change").GetComponent<RectTransform>().localPosition = new Vector3(0.48f * width - 0.04f * height, -0.46f * height);
        GameObject.Find("Change").GetComponent<RectTransform>().sizeDelta = new Vector2(0.08f * height, 0.08f * height);
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

        /*Pause_Menu = GameObject.Find("Pause_Menu");
        Pause_Menu.GetComponent<RectTransform>().sizeDelta=new Vector2(0.9f*width,0.9f*height);
        Pause_Menu.GetComponent<RectTransform>().localPosition = new Vector3(0, height, -5);*/

        #region BGMの設定
        BGM[0] = Resources.Load<AudioClip>("Audio/fantasy13"); //まだ
        BGM[1] = Resources.Load<AudioClip>("Audio/fantasy13"); //まだ
        BGM[2] = Resources.Load<AudioClip>("Audio/fantasy13");
        BGM[3] = Resources.Load<AudioClip>("Audio/fantasy12");
        BGM[0] = Resources.Load<AudioClip>("Audio/fantasy13"); //まだ
        #endregion

    }

    // Update is called once per frame
    void Update()
    {
        #region 背景の動画
        if (!pause_bool && bg_bool)
        {
            Back_anime.GetComponent<RectTransform>().Translate(new Vector3(height * 0.001f, 0, 0));
            Front_anime.GetComponent<RectTransform>().Translate(new Vector3(height * 0.001f, 0, 0));
            if (Back_anime.GetComponent<RectTransform>().localPosition.x > 0.7f * height)
            {
                Back_anime.GetComponent<RectTransform>().position -= new Vector3(1.4f * height, 0, 0);
                Front_anime.GetComponent<RectTransform>().position -= new Vector3(1.4f * height, 0, 0);
            }
        }
        #endregion
        #region キャラクター移動
        for (int i = 0; i <= Life_point; i++)
        {
            Vector3 vec = Chara[i].Pos;
            if ((vec - new Vector3(width * 0.2f * (i - 0.2f), height * 0.277f)).magnitude < 0.01f)
                Chara[i].Pos = new Vector3(width * 0.2f * (i - 0.2f), height * 0.277f);
            else
                Chara[i].Pos = (39f * vec + new Vector3(width * 0.2f * (i - 0.2f), height * 0.277f)) / 40f;
        }
        #endregion
        #region 時間表示
        if (!pause_bool && timer_bool) time -= Time.deltaTime;//is_Skill(n2)
        if (time < 0) //GameOver
        {
            time = 0;
            GameObject.Find("Director").GetComponent<Main>().Goal(1);
        }
        int m = Mathf.FloorToInt(time / 60f);
        int s = Mathf.FloorToInt(time % 60f);
        Time_text.text = ("Time   " + m.ToString().PadLeft(2, '0') + " : " + s.ToString().PadLeft(2, '0'));

        //if (time < needle) needle -= 0.1f;
        //Needle.localRotation=new 
        #endregion
        #region スキル
        Gage();
        if (skill_time < 20 && !pause_bool && timer_bool) skill_time += Time.deltaTime;
        #endregion
        /*#region ポーズメニュー　
         * ★pause_boolに対応して作ろうかと思っています。スタート時ポーズを使わないで実装することは可能でしょうか?もし難しそうであれば、新しく変数作ります。
         * ★どんなのか見てみたいのであれば、Pause_Menuという名のImageをキャンバスに載せて、StartのPause_Menuあたりのコメントアウトを消してください。
        Vector3 v = Pause_Menu.GetComponent<RectTransform>().localPosition;
        if (pause_bool && v.z < 0)
        {
            float x = v.z * 11f;
            x++;
            Pause_Menu.GetComponent<RectTransform>().localPosition = new Vector3(0, x * (x + 20) * 0.0005f * height, x / 11f);
            if (Mathf.Abs(v.z) < 0.1f) Pause_Menu.GetComponent<RectTransform>().localPosition = Vector3.zero;
        }
        else if (!pause_bool && v.z > -5)
        {
            float x = v.z * 11f;
            x--;
            Pause_Menu.GetComponent<RectTransform>().localPosition = new Vector3(0, x * (x + 20) * 0.0005f * height, x / 11f);
            if (Mathf.Abs(v.z+5) <0.1f)
            {
                Pause_Menu.GetComponent<RectTransform>().localPosition = new Vector3(0, height, -5.1f);
                GameObject.Find("Director").GetComponent<Main>().Pause_button_down();
            }
        }
        #endregion*/
    }

    // すみません、作っちゃいました...。
    public void All_pause_flg(bool pause)
    {
        transform.Translate(1.0f * Time.deltaTime, 0, 0); // update内部で有効
        timer_bool = pause;
        bg_bool = pause;
    }

    public void Pause_flg(bool pause)
    {
        pause_bool = pause;
    }

    public void Lose_Time(float minus) //単位は秒
    {
        time -= minus;
    }

    public bool Damage()
    {
        Chara[0].Anime().SetTrigger("Out_Trigger");
        Life_point--;
        Party tmp = Chara[0];
        Chara[0] = Chara[1];
        Chara[1] = Chara[2];
        Chara[2] = tmp;
        if (Life_point < 0)//GameOver
        {
            Skill_text.text = "";
            return true;
        }
        else
        {
            Skill_text.text = Chara[0].skill_Description;
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
        o.GetComponent<Image>().color = new Color(1, 0.8f, 0, 1);
    }

    public void Item_Life()
    {
        if (Life_point < 2)
        {
            Life_point++;
            Chara[Life_point].Pos = new Vector3(width * 0.65f * (3 - Life_point), height * 0.277f);
            Chara[Life_point].Anime().SetTrigger("Retry_Trigger");
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

    public void Effect(string s) //全画面に出したいとき
    {
        GameObject o = GameObject.Find("Effect");
        o.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
        o.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        o.GetComponent<Animator>().SetTrigger(s);
        if (s == "Goal_Trigger")
        {
            GameObject.Find("Start_and_End_anim").GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/GameScene/end");
            GameObject.Find("Start_and_End_anim").GetComponent<RectTransform>().localPosition = new Vector3(-width, 0.3f * height);
            GameObject.Find("Start_and_End_anim").GetComponent<RectTransform>().sizeDelta = new Vector3(0.8f * width, 0.13f * height);
            o = Instantiate(Resources.Load<GameObject>("Prefab/Big_Treasure")) as GameObject;
            o.name = "Goal";
            o.transform.parent = GameObject.Find("Canvas").transform;
            o = Instantiate(Resources.Load<GameObject>("Prefab/Get")) as GameObject;
            o.transform.parent = GameObject.Find("Canvas").transform;
            o.name = "light";
            o.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/GameScene/light");
            o.GetComponent<RectTransform>().localPosition = new Vector3(0, 0.05f * height);
            o.GetComponent<RectTransform>().sizeDelta = new Vector2(0.005f * width, 0.005f * width);
            o.GetComponent<Image>().color = new Color(1, 1, 0.5f);
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
        time = 600;
        Life_point = 2;
        GameObject.Find("Effect").GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        GameObject.Find("Game_over").GetComponent<RectTransform>().localPosition = new Vector3(0, height, 0);
        GameObject.Find("GameOver_text").GetComponent<RectTransform>().localPosition = new Vector3(0, height, 0);
        GameObject.Find("Retry").GetComponent<RectTransform>().localPosition = new Vector3(0, height, 0);
        for (int i = 0; i < 3; i++)
        {
            Chara[i].Pos = new Vector3(width * 0.65f * (3 - i), height * 0.277f);
            Chara[i].Anime().SetTrigger("Retry_Trigger");
            Anime(i, Common.Action.Walk);
        }
        Change_Chara();
        timer_bool = true;
        bg_bool = true;
    }

    public bool To_goal(int Scale_or_Pos) //最後の宝箱の動きが終わったかどうか
    {
        GameObject o = GameObject.Find("Goal");
        if (Scale_or_Pos == 0)
        {
            Vector3 vec = o.GetComponent<RectTransform>().localScale;
            o.GetComponent<RectTransform>().localScale = (24f * vec + new Vector3(width * 0.01f, width * 0.01f)) / 25f;
            if ((vec - new Vector3(width * 0.01f, width * 0.01f)).magnitude < 0.01f)
            {
                o.GetComponent<RectTransform>().localScale = new Vector3(width * 0.01f, width * 0.01f);
                return true;
            }
            else return false;
        }
        else
        {
            Vector3 vec = o.GetComponent<RectTransform>().localPosition;
            if (vec.magnitude < 0.55f)
            {
                o.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
                return true;
            }
            else
            {
                vec = vec / (vec.magnitude);
                o.GetComponent<RectTransform>().localPosition -= vec; return false;
            }
        }
    }
    #endregion

    #region　スキル
    public void Gage() //スキルゲージを
    {
        GameObject o = GameObject.Find("Gage");
        if (Chara[0].Max_count > skill_time)
        {
            o.GetComponent<Image>().fillAmount = 1 - skill_time / Chara[0].Max_count;
        }
        else
        {
            float max = Chara[0].Max_gage;
            float child = Road_count;
            o.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            o.GetComponent<Image>().fillAmount = child / max;
        }
    }

    public void Road_counter()
    {
        if (Chara[0].Max_count < skill_time)
        {
            Road_count++;
        }
    }

    public void Skill_On() //スキルボタンを押したとき
    {
        if (Road_count >= Chara[0].Max_gage)
        {
            skill_time = 0;//秒
            Road_count = 0;
            GameObject.Find("Gage").GetComponent<Image>().color = new Color(0, 1, 1, 1);
        }
    }

    public bool is_Skill(int n)
    {
        return skill_time < Chara[0].skills[n];
    }
    #endregion

    public void Change_Chara()
    {
        Party tmp = Chara[0];
        for (int i = 0; i < Life_point; i++)
        {
            Chara[i] = Chara[i + 1];
        }
        Chara[Life_point] = tmp;
        Chara[Life_point].Index = 3;
        GameObject.Find("Player").GetComponent<Animator>().SetInteger("Chara_Int", Top_ID());
        Skill_text.text = Chara[0].skill_Description;
        skill_time = 20;
    }

    public void SE_on(Common.SE music)
    {
        GetComponent<AudioSource>().PlayOneShot(SEs[(int)music]);
    }

    public void BGM_on(Common.BGM music)
    {
        // フェードの変更がoneshotでは厳しいため、AudioSourceに変更中
        //BGM[music].play();

        GetComponent<AudioSource>().PlayOneShot(BGM[(int)music]);
    }

}