using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Small_Game : Functions
{
    public GameObject game_camera;
    Data_box[,] Pazzle_data = new Data_box[8,8];//[左から,下から]の順、下から見て0:None,1:Straight,2:Right,3:Left
    Field[,] Pazzle_fields = new Field[5, 5]; //触る方
    Field[,] move_fields = new Field[5, 5]; //動くため
    Character player;
    Character[] enemy;
    Vector3 tap_Start;
    int Move_X, Move_Y, touch_ID; //touch_ID:敵とか宝に当たった時にその番号 Road_countいくつ道を通ったか(スキル用 ★StateManagerに移行)
    Common.Direction Move_direct, Field_direct;
    Common.Condition Move_condition;
    Item[] treasure;
    GameObject[] arrows = new GameObject[4];
    Text Count_Text;
    int treasure_count;

    float score = 0,retry=0;
    float revive_time=0;
    bool pause_bool = false; //ポーズボタン、止め方が分からないのでとりあえず止めるためのもの
    int flg = 0;//Playerのとこ、止め方が分からないのでとりあえず止めるためのもの、0:ポーズ、1:動く,2:盤変更
                //ここのやり方が分からなかったので真偽地で無理矢理止めています。時間があればいい感じに変えてください。

    #region Battle追加部分

    public GameObject player_obj_color;

    GameObject time_minus;
    GameObject Battle_effect;
    GameObject Sentou_kaisi;
    GameObject Battle_down_panel;
    GameObject Hako, Get;
    GameObject Enemy;
    private RectTransform move_treasure_get;
    GameObject player_pos_for_treasure;
    float timeElapsed, timeOut = 3.0f;
    float width, height;
    bool isBattle = false;
    bool isGet = false;

    int coin = 0;
    #endregion
    #region タッチエフェクト追記
    [SerializeField]
    ParticleSystem touchEffect;    // タッチの際のエフェクト
    [SerializeField]
    Camera _camera;                // カメラの座標
    private bool isTouch = false;
    #endregion
    #region 初期アニメーション追加部分
    private Color Faller;
    private Vector3 pos_diff;
    private RectTransform Hikousen_pos, FallChara_pos;
    private GameObject Start_and_End_anim;
    public GameObject FallChara, FadePanel, Hikousen;

    private float move_diff, char_right = 0;
    private float down_diff, chardown = -450, div = 50;
    private bool isAnim = true, Anim_start = true, mid_reach = false;
    private bool fstFlag = false, HikousenFin = false, is1stAnim = true;
    #endregion



    public GameObject Back_anime, Front_anime,
        time_gage, skill_Icon;//,Skill_Flame;
    public RectTransform Pause_Menu, Needle;
    public Image gage, Flame, Background;
    public Animator Player;
    public float time;
    float needle, time_delta, Max_Time;
    bool is_red;
    public bool timer_bool, bg_bool;
    public Text Time_text, Skill_text, Degital_Time;
    int Life_point;
    int Road_count, All_count;
    public float[] tresure = new float[2];
    public float road = 0;
    public Party[] Chara = new Party[3];
    AudioClip[] SEs = new AudioClip[17];//音増えるごとに追加お願いします
    public AudioSource[] BGMs;
    private int last = 0;
    private bool battle_move = false, max_bool = false;

    GameObject Battle_enemy;
    float skill_time;//後でキャラによって変更、多分配列化
    
    [SerializeField]
    ParticleSystem skillEffect;    // スキルの際のエフェクト
    GameObject skill_effect_anim;
    GameObject skill_icon_effect;


    // Use this for initialization
    void Start()
    {
        #region 元StateManager

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
            int Level = PlayerPrefs.GetInt("Level" + i, 0);
            Chara[i] = new Party(o, ID);
            dictionary.GetComponent<Dictionary>().Set_Box(Chara[i], ID, Level);
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
                o.GetComponent<RectTransform>().sizeDelta = new Vector2(0.08f * width, 0.08f * width);
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


        GameObject.Find("Map_Goal").GetComponent<RectTransform>().localPosition = new Vector3(0.06f * width, 0.04f * width);
        GameObject.Find("Map_Goal").GetComponent<RectTransform>().sizeDelta = new Vector2(0.03f * width, 0.08f * width);
        #endregion

        #region 元Main
        Count_Text = GameObject.Find("Count_Text").GetComponent<Text>();
        #region Pazzle_dataの設定と読み取り
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Pazzle_data[i, j] = new Data_box();
            }
        }
        Pazzle_data_set();
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                GameObject o = Instantiate(Resources.Load<GameObject>("Prefab/Field")) as GameObject;
                Pazzle_fields[i, j] = new Field(o);
                o = Instantiate(Resources.Load<GameObject>("Prefab/Field")) as GameObject;
                move_fields[i, j] = new Field(o);
                move_fields[i, j].Pos = new Vector3(-5, 0); //見えないところへ
            }
        }
        #region 山の設定
        for (int i = 1; i < 8; i++)
        {
            Pazzle_data[0, i].type = Common.Direction.Up; Pazzle_data[0, i].condition = Common.Condition.Normal;
            Pazzle_data[7, i-1].type = Common.Direction.Up; Pazzle_data[7, i-1].condition = Common.Condition.Normal;
            Pazzle_data[i-1, 0].type = Common.Direction.Up; Pazzle_data[i-1, 0].condition = Common.Condition.Normal;
            Pazzle_data[i, 7].type = Common.Direction.Up; Pazzle_data[i, 7].condition = Common.Condition.Normal;
        }
        Pazzle_data[7,4].type = Common.Direction.Down;
        Pazzle_data[7,4].condition = Common.Condition.Player;//ゴール右上、右側
        Pazzle_data[7,5].type = Common.Direction.Down;
        Pazzle_data[7,5].condition = Common.Condition.Player;//ゴール右上、右側
        Pazzle_data[7,6].type = Common.Direction.Down;
        Pazzle_data[7,6].condition = Common.Condition.Player;//ゴール右上、右側
        #endregion
        set_block(0, 0, 0);
        #endregion
        #region playerの設定
        GameObject player_obj = GameObject.Find("Player");
        float map_num = Screen.width * 0.026f;
        player = new Character(player_obj, Common.Type.Player,map_num,3.5f);
        player.x = 2;
        player.y = 1;
        //player.set_position(5, 4, Common.Direction.Down, Pazzle_data[5, 4].Exit_direction(Common.Direction.Down));
        if (is_Skill(6)) player.set_Speed(55f);
        else player.set_Speed(90f);
        Pazzle_data[2,1].condition = Common.Condition.Player;
        player.Set_Chara(Top_ID());
        #endregion
        #region 宝物の設定
        treasure = new Item[5]; //ここで宝の数
        GameObject Treasure = Instantiate(Resources.Load<GameObject>("Prefab/Treasure")) as GameObject;
        int[] ran = Random_position();
        Pazzle_data[ran[0], ran[1]].treasure = 0;
        treasure[0] = new Item(ran[0], ran[1], Treasure, Common.Treasure.Item, map_num, 3.5f);
        for (int i = 1; i < 3; i++)
        {
            Treasure = Instantiate(Resources.Load<GameObject>("Prefab/Treasure")) as GameObject;
            ran = Random_position();
            Pazzle_data[ran[0], ran[1]].treasure = i;
            treasure[i] = new Item(ran[0], ran[1], Treasure, Common.Treasure.Coin,map_num, 3.5f);
        }
        Treasure = Instantiate(Resources.Load<GameObject>("Prefab/Treasure")) as GameObject;
            ran = Random_position();
            Pazzle_data[ran[0], ran[1]].treasure = 3;
            treasure[3] = new Item(ran[0], ran[1], Treasure, Common.Treasure.Time, map_num, 3.5f);
        Treasure = Instantiate(Resources.Load<GameObject>("Prefab/Treasure")) as GameObject;
        ran = Random_position();
        Pazzle_data[ran[0], ran[1]].treasure = 4;
        treasure[4] = new Item(ran[0], ran[1], Treasure, Common.Treasure.Life, map_num, 3.5f);
        #endregion
        #region enemyの設定
        enemy = new Character[5];//ここで敵の数
        Common.Type[] types = { Common.Type.Walk, Common.Type.Stop, Common.Type.Fly };
        for (int i = 0; i < enemy.Length; i++)
        {
            GameObject enemy_obj = Instantiate(Resources.Load<GameObject>("Prefab/Enemy")) as GameObject;
            int type_num = Random.Range(0, 3);
            enemy[i] = new Character(enemy_obj, types[type_num],map_num,3.5f); //typesをランダム化
            Common.Direction dire = Common.Direction.None;
            if (types[type_num] != Common.Type.Stop) dire = Random_direct();
            ran = Random_position();
            for (int j = 0; j < i; j++)
            {
                if (enemy[j].x == ran[0] && enemy[j].y == ran[1])
                {
                    ran = Random_position();
                    j = -1;
                }
            }
            enemy[i].x = ran[0];
            enemy[i].y = ran[1];
            enemy[i].set_position(ran[0], ran[1], dire, Get_exit(enemy[i], dire));
            enemy[i].set_Speed(110f);
        }
        #endregion
        #region 矢印の設定
        for (int i = 0; i < 4; i++)
        {
            arrows[i] = Instantiate(Resources.Load<GameObject>("Prefab/Arrow")) as GameObject;
            arrows[i].transform.Rotate(new Vector3(0, 0, -90 * i));
            //arrows[i].transform.position = new Vector3( Mathf.Sin(i * Mathf.PI / 2f), Mathf.Cos(i * Mathf.PI / 2f),0);
        }
        #endregion
        Move_X = -1;
        Move_Y = 0;
        Move_direct = Common.Direction.None;
        game_camera.transform.position = new Vector3(3 * L(player.x) + 2, 3 * L(player.y) + 2.8f, -10);

        #region Battle追加部分
        time_minus = GameObject.Find("minus");
        Battle_effect = GameObject.Find("Attack_effect");
        Sentou_kaisi = GameObject.Find("Sentou_kaisi");
        Battle_down_panel = GameObject.Find("Battle_down_panel");
        Hako = GameObject.Find("TakaraBako");
        Get = GameObject.Find("OtakaraGet");
        player_pos_for_treasure = GameObject.Find("Player");
        Enemy = GameObject.Find("BattleEnemy");
        #endregion
        #region 初期エフェクト追記
        Start_and_End_anim = GameObject.Find("Start_and_End_anim");
        //pause_bool = !pause_bool;
        timer_bool = false;
        bg_bool = false;
        Set_color(new Vector3(Pazzle_fields[2, 2].data_x, Pazzle_fields[2, 2].data_y));//敵とか透明にする
        #endregion

        // BGMを抑え気味で流し始める
        BGMs[1].volume = 0.0f;
        BGM_on(Common.BGM.tutorial); // ここで最初にBGM定義
        //player_obj_color= GameObject.Find("Player");
        //player_obj_color.SetActive(false);
        treasure_count = 0;
        #endregion
    }

    void Update()
    {
        #region 元StateManager
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
        }
        if (time < 0) //GameOver
        {
            time = 0;
            Goal(1);
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
        int m = Mathf.FloorToInt(time / 60f);
        int s = Mathf.FloorToInt(time % 60f);
        if (time - needle > 1.5f)
        {
            m = Mathf.FloorToInt(needle / 60f);
            s = Mathf.FloorToInt(needle % 60f);
            Degital_Time.color = Color.green;
        }
        else if (time - needle < -1.5f)
        {
            m = Mathf.FloorToInt(needle / 60f);
            s = Mathf.FloorToInt(needle % 60f);
            Degital_Time.color = Color.red;
        }
        else Degital_Time.color = Color.white;
        Degital_Time.text = (m.ToString().PadLeft(2, '0') + " : " + s.ToString().PadLeft(2, '0'));
        Needle.localRotation = new Quaternion(0, 0, 1, 1 - needle / Max_Time);
        Flame.color -= new Color(0, 0, 0, 0.01f);
        if (needle < 80 && !is_red)
        {
            Back_anime.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Background/BG_red" + (int)Common.Thema.Sky);
            Front_anime.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Background/Bg_front_red" + (int)Common.Thema.Sky);
            Background.sprite = Resources.Load<Sprite>("Images/Background/Back_red" + (int)Common.Thema.Sky);
            is_red = true;
            To_Red(true);
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
                Pause_button_down(false);
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
            if (Chara[0].skills[6] < 30) player.set_Speed(90f);
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
        Vector3 pause_v = Pause_Menu.localPosition;
        if (pause_bool && pause_v.z < 0)
        {
            float x = pause_v.z * 11f;
            x++;
            Pause_Menu.localPosition = new Vector3(0, x * (x + 20) * 0.0005f * height, x / 11f);
            if (Mathf.Abs(pause_v.z) < 0.1f) Pause_Menu.localPosition = Vector3.zero;
        }
        else if (!pause_bool && pause_v.z > -5)
        {
            float x = pause_v.z * 11f;
            x--;
            Pause_Menu.localPosition = new Vector3(0, x * (x + 20) * 0.0005f * height, x / 11f);
            if (Mathf.Abs(pause_v.z + 5) < 0.1f)
            {
                Pause_Menu.localPosition = new Vector3(0, height, -5.1f);
                Pause_button_down(false);
            }
        }
        #endregion
        #endregion

        #region 元Main
        if (Input.GetKeyDown(KeyCode.Return)) SceneManager.LoadScene("Tutorial");
        if (Input.GetKeyDown(KeyCode.Space)) Pause_button_down(!pause_bool);
        /* デバック用に置いてます
        for(int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                Debug.Log("i,j " + i + " " + j + "   t " + PazzleFields[i, j].type);
            }
        }*/
        if (!pause_bool)
        {
            switch (flg)
            {
                case 1: //ゲーム部分
                    Set_color(new Vector3(Pazzle_fields[2, 2].data_x, Pazzle_fields[2, 2].data_y));//敵とか透明にする
                    #region 敵の動き
                    for (int i = 0; i < enemy.Length; i++)
                    {
                        if (revive_time>=0&&enemy[i].act != Common.Action.Sad)
                        {
                            if (is_Skill(3)) enemy[i].Sprite().color = new Color(0, 0.5f, 1, 1);
                            Vector3 v = new Vector3(enemy[i].x, enemy[i].y, 0);
                            if (enemy[i].type != Common.Type.Fly && L(player.x) == L(enemy[i].x) && L(player.y) == L(enemy[i].y)) v = Pazzle_fields[(enemy[i].x - 1) % 3 + 1, (enemy[i].y - 1) % 3 + 1].Pos;
                            if (enemy[i].Move(v,is_Skill(3))) 
                            {
                                enemy[i].pre_x = enemy[i].x;
                                enemy[i].pre_y = enemy[i].y;
                                enemy[i].x += (int)Dire_to_Vec(enemy[i].move_to).x;
                                enemy[i].y += (int)Dire_to_Vec(enemy[i].move_to).y;//敵の動いた座標を更新 
                                #region 引き返し
                                if (enemy[i].x < 1 || enemy[i].x > 6 || enemy[i].y < 1 || enemy[i].y > 6)
                                {
                                    enemy[i].x = enemy[i].pre_x;
                                    enemy[i].y = enemy[i].pre_y;
                                    enemy[i].set_curve(enemy[i].x, enemy[i].y, enemy[i].move_to, Get_exit(enemy[i], (enemy[i].move_to)));
                                }
                                else if ((Pazzle_data[enemy[i].x, enemy[i].y].condition == Common.Condition.Hole || Pazzle_data[enemy[i].x, enemy[i].y].condition == Common.Condition.Moving) && enemy[i].type == Common.Type.Walk)
                                {
                                    enemy[i].x = enemy[i].pre_x;
                                    enemy[i].y = enemy[i].pre_y;
                                    enemy[i].set_curve(enemy[i].x, enemy[i].y, enemy[i].move_to, Get_exit(enemy[i], (enemy[i].move_to)));
                                }
                                #endregion
                                else
                                {
                                    enemy[i].set_curve(enemy[i].x, enemy[i].y, reverse(enemy[i].move_to), Get_exit(enemy[i], (reverse(enemy[i].move_to))));
                                }
                            }
                            if (L(enemy[i].x) == L(player.x) && L(enemy[i].y) == L(player.y))
                            {
                                if (is_Skill(1))
                                {
                                    enemy[i].act = Common.Action.Sad;//★強制送還。コインゲット等のアクションをバトルに入れた場合、ここにもお願いします
                                    //enemy[i].Sprite().color = Color.clear;//ここもアニメーションになるっぽい（アイコンになるなら）
                                    enemy[i].OutScreen(false);//ここもアニメーションになるっぽい（アイコンになるなら）
                                }
                                else if (!is_Skill(7)&&(enemy[i].Pos - player.Pos).magnitude < 0.6f)  //ここに当たった時の。is_Skill(n1),is_Skill(n5)
                                {
                                    touch_ID = i;
                                    flg = 3;
                                }
                            }
                        }
                    }
                    #endregion
                    #region プレイヤーの動き
                    if (revive_time < 0) revive_time += Time.deltaTime;
                    else if (player.Move(Pazzle_fields[(player.x - 1) % 3 + 1, (player.y - 1) % 3 + 1].Pos,is_Skill(4))) //動き終えたらtrue 
                    {
                        if (Pazzle_data[player.x, player.y].walk)
                        {
                            Pazzle_data[player.x, player.y].walk = false;
                            road++;
                        }
                        //PlayerPrefs.SetInt("Road" + Road_count, (int)player.move_to);
                        Road_counter();
                        if (Pazzle_data[player.x, player.y].condition == Common.Condition.Moving)
                        {
                            Fall(Common.Condition.Moving);
                        }
                        else
                        {
                            player.pre_x = player.x;
                            player.pre_y = player.y;
                            player.x += (int)Dire_to_Vec(player.move_to).x;
                            player.y += (int)Dire_to_Vec(player.move_to).y;//プレイヤーの動いた座標を更新 
                            Pazzle_data[player.pre_x, player.pre_y].condition = Common.Condition.Normal;
                            #region if (出ていった場合)
                            if (player.x % 3 == 0 && player.move_to == Common.Direction.Left)
                            {
                                if (player.x > 0) change_block(Common.Direction.Right);
                                else change_Mount(Common.Direction.Left, L(player.x), L(player.y));
                            }
                            else if (player.x % 3 == 1 && player.move_to == Common.Direction.Right)
                            {
                                if (player.x < 5) change_block(Common.Direction.Left);
                                else change_Mount(Common.Direction.Right, L(player.x), L(player.y));
                            }
                            else if (player.y % 3 == 0 && player.move_to == Common.Direction.Down)
                            {
                                if (player.y > 0) change_block(Common.Direction.Up);
                                else change_Mount(Common.Direction.Down, L(player.x), L(player.y));
                            }
                            else if (player.y % 3 == 1 && player.move_to == Common.Direction.Up)
                            {
                                if (player.y < 5) change_block(Common.Direction.Down);
                                else change_Mount(Common.Direction.Up, L(player.x), L(player.y));
                            }
                            #endregion
                            else
                            {
                                if (Pazzle_data[player.x, player.y].condition == Common.Condition.Hole || Pazzle_data[player.x, player.y].condition == Common.Condition.Moving)
                                {
                                    Fall(Common.Condition.Hole);
                                }
                                else
                                {
                                    player.set_curve(player.x, player.y, reverse(player.move_to), Pazzle_data[player.x, player.y].Exit_direction(reverse(player.move_to)));
                                    Pazzle_data[player.x, player.y].condition = Common.Condition.Player;
                                }
                            }
                        }
                    }
                    #endregion
                    #region 盤を動かす
                    Arrow_show(true);
                    if (Move(Move_X, Move_Y, Move_direct, 3f)) //盤が動く、動いてる途中はfalse is_Skill(n9)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            tap_Start = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        }
                        if (Input.GetMouseButtonUp(0) && tap_Start.x != -1)
                        {
                            Vector3 tap_Tarminal = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            Vector3 delta = tap_Tarminal - tap_Start;
                            #region 横方向スライド
                            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y) && delta.magnitude > 0.6f)
                            {
                                int X = Hole_search(player.x, player.y, 0);
                                int Y = Hole_search(player.x, player.y, 1); //穴の座標,0~8
                                if (delta.x > 0 && X % 3 != 1 && Pazzle_data[X, Y].condition == Common.Condition.Hole)
                                {
                                    Move_X = X - 1;
                                    Move_Y = Y;
                                    Move_direct = Common.Direction.Right;
                                    Move_condition = Pazzle_data[X - 1, Y].condition;
                                    Pazzle_data[X, Y].condition = Common.Condition.Moving;
                                    Pazzle_data[X - 1, Y].condition = Common.Condition.Moving;
                                    SE_on(Common.SE.Slide);
                                }
                                else if (delta.x < 0 && X % 3 != 0 && Pazzle_data[X, Y].condition == Common.Condition.Hole)
                                {
                                    Move_X = X + 1;
                                    Move_Y = Y;
                                    Move_direct = Common.Direction.Left;
                                    Move_condition = Pazzle_data[X + 1, Y].condition;
                                    Pazzle_data[X, Y].condition = Common.Condition.Moving;
                                    Pazzle_data[X + 1, Y].condition = Common.Condition.Moving;
                                    SE_on(Common.SE.Slide);
                                }
                            }
                            #endregion
                            #region 縦方向スライド 
                            else if (Mathf.Abs(delta.x) < Mathf.Abs(delta.y) && delta.magnitude > 0.6f)
                            {
                                int X = Hole_search(player.x, player.y, 0);
                                int Y = Hole_search(player.x, player.y, 1); //穴の座標,0~8
                                if (delta.y > 0 && Y % 3 != 1 && Pazzle_data[X, Y].condition == Common.Condition.Hole)
                                {
                                    Move_X = X;
                                    Move_Y = Y - 1;
                                    Move_direct = Common.Direction.Up;
                                    Move_condition = Pazzle_data[X, Y - 1].condition;
                                    Pazzle_data[X, Y].condition = Common.Condition.Moving;
                                    Pazzle_data[X, Y - 1].condition = Common.Condition.Moving;
                                    SE_on(Common.SE.Slide);
                                }
                                else if (delta.y < 0 && Y % 3 != 0 && Pazzle_data[X, Y].condition == Common.Condition.Hole)
                                {
                                    Move_X = X;
                                    Move_Y = Y + 1;
                                    Move_direct = Common.Direction.Down;
                                    Move_condition = Pazzle_data[X, Y + 1].condition;
                                    Pazzle_data[X, Y].condition = Common.Condition.Moving;
                                    Pazzle_data[X, Y + 1].condition = Common.Condition.Moving;
                                    SE_on(Common.SE.Slide);
                                }
                                #endregion
                            }
                            tap_Start.x = -1;
                        }
                    }
                    #endregion
                    break;
                case 2: //盤変更
                    #region カメラの位置
                    Vector3 vec = game_camera.transform.position;
                    game_camera.transform.position = (vec * 13f + (Pazzle_fields[2, 2].Pos + new Vector3(0, 0.8f, -10))) / 14f;
                    Set_color(game_camera.transform.position - new Vector3(0, 0.8f, 0));
                    Arrow_show(false);
                    #endregion
                    #region 動き終わった後
                    if ((game_camera.transform.position - new Vector3(3 * L(player.x) + 2, 3 * L(player.y) + 2.8f, -10)).magnitude < 0.015f)
                    {
                        game_camera.transform.position = new Vector3(3 * L(player.x) + 2, 3 * L(player.y) + 2.8f, -10);
                        player.set_position(player.x, player.y, Field_direct, Pazzle_data[player.x, player.y].Exit_direction(Field_direct));
                        OutScreen();
                        if (Pazzle_data[player.x, player.y].condition == Common.Condition.Hole)
                        {
                            Fall(Common.Condition.Player);
                        }
                        else
                        {
                            Pazzle_data[player.x, player.y].condition = Common.Condition.Player;
                            Field_direct = Common.Direction.None;
                            flg = 1;
                            timer_bool = true;
                        }
                    }
                    #endregion
                    break;
                case 3: //バトル
                    #region Battle追記部分
                    timeElapsed += Time.deltaTime;

                    if (!isBattle) // バトル開始直後の処理
                    {
                        isBattle = true;
                        timer_bool = false;
                        bg_bool = false;
                        BGM_on(Common.BGM.battle); // ここで戦闘BGM定義
                        //UIs.Battle_move_anim(1);

                        for (int i = 0; i <= Life_point; i++)
                        {
                            Anime(i, Common.Action.Stop);
                        }

                        Enemy_Anime(true, enemy[touch_ID].type);
                        Sentou_kaisi.GetComponent<Animator>().SetBool("Sentou_effect", true);
                        Invoke("Battle_setup", 2.0f);
                    }


                    Vector3 vecEne = Enemy.GetComponent<RectTransform>().localPosition; 
                    if ((vecEne - new Vector3(width * 0.35f, height * 0.25f)).magnitude < 0.01f)
                        Enemy.GetComponent<RectTransform>().localPosition = new Vector3(width * 0.35f, height * 0.25f);
                    else
                        Enemy.GetComponent<RectTransform>().localPosition = (39f * vecEne + new Vector3(width * 0.35f, height * 0.25f)) / 40f;


                    if (timeElapsed <= 0.7)
                        Battle_down_panel.GetComponent<Image>().color = new Color(0, 0, 0, timeElapsed);

                    if (timeElapsed >= timeOut + 3.0f) // 3秒経過で終了
                        Battle_endset();

                    #endregion
                    break;
                case 4: //宝ゲット
                    #region お宝Getの表示、アイテムによる処理
                    timeElapsed += Time.deltaTime;
                    if (!isGet)
                    {
                        float treasure_x = player_pos_for_treasure.transform.position.x;
                        float treasure_y = player_pos_for_treasure.transform.position.y;
                        int display_treasure_pos_x = (int)(0.5f + treasure_x - 1) % 3;
                        int display_treasure_pos_y = (int)(0.5f + treasure_y - 1) % 3;

                        move_treasure_get = Get.GetComponent<RectTransform>();
                        move_treasure_get.localPosition = new Vector3(-90 + display_treasure_pos_x * 90, -120 + display_treasure_pos_y * 80, 0);

                        if (treasure[touch_ID].type == Common.Treasure.Item)      // 宝箱取得
                        {
                            Hako.GetComponent<Animator>().SetInteger("Get_what",1);
                            Get.GetComponent<Animator>().SetInteger("Get_Lost", 0);
                        }
                        else if (treasure[touch_ID].type == Common.Treasure.Coin) // ピンク色の宝箱取得
                        {
                            Hako.GetComponent<Animator>().SetInteger("Get_what", 1);
                            Get.GetComponent<Animator>().SetInteger("Get_Lost", 0);
                        }
                        else if (treasure[touch_ID].type == Common.Treasure.Time) // 時間取得
                        {
                            Hako.GetComponent<Animator>().SetInteger("Get_what", 2);
                            Get.GetComponent<Animator>().SetInteger("Get_Lost", 1);
                            Lose_Time(-60);
                        }
                        else if (treasure[touch_ID].type == Common.Treasure.Life) // 梯子取得
                        {
                            //Hako.GetComponent<Animator>().SetInteger("Get_what", 3); 
                            Get.GetComponent<Animator>().SetInteger("Get_Lost", 1);
                            Item_Life();
                        }
                        SE_on(Common.SE.Get);
                        coin += Random.Range(5, 9); // コイン取得
                        bg_bool = false;
                        isGet = true;
                        Invoke("After_get", 1.5f);
                    }
                    #endregion
                    #region キャラのアニメーション、終了時の処理
                    for (int i = 0; i <= Life_point; i++)
                        Anime(i, Common.Action.Happy);

                    if (timeElapsed > 1.5f) //2秒経過で終了
                    {
                        // 移植作業中..
                        for (int i = 0; i <= Life_point; i++) Anime(i, Common.Action.Walk);
                        treasure[touch_ID].Get_Item();
                        if ((int)treasure[touch_ID].type < 2) tresure[(int)treasure[touch_ID].type]++;
                        Hako.GetComponent<Animator>().SetInteger("Get_what", 0);
                        Get.GetComponent<Animator>().SetBool("Get", false);

                        isGet = false;
                        bg_bool = true;
                        timeElapsed = 0.0f;
                        if (treasure[touch_ID].type == Common.Treasure.Coin || treasure[touch_ID].type == Common.Treasure.Item)
                        {
                            treasure_count++;
                            Count_Text.text = "×" + treasure_count;
                        }
                        flg = 1;
                    }
                    break;
                    #endregion
                case 5: //落下
                    if (timeElapsed == 0.0f)
                    {
                        float treasure_x = player_pos_for_treasure.transform.position.x;
                        float treasure_y = player_pos_for_treasure.transform.position.y;
                        int display_treasure_pos_x = (int)(0.5f + treasure_x - 1) % 3;
                        int display_treasure_pos_y = (int)(0.5f + treasure_y - 1) % 3;

                        move_treasure_get = Get.GetComponent<RectTransform>();
                        move_treasure_get.localPosition = new Vector3(-90 + display_treasure_pos_x * 90, -120 + display_treasure_pos_y * 80, 0);

                        Get.GetComponent<Animator>().SetInteger("Get_Lost", 2);
                        Invoke("After_get", 1.5f);
                        SE_on(Common.SE.Fall);
                        timer_bool = false;
                        bg_bool = false;
                    }
                    timeElapsed += Time.deltaTime;
                    //if (timeElapsed > 0.1f && timeElapsed < 0.2f) { player.Set_Chara(0); player.OutScreen(false); }
                    if (player.Wait_chara())
                    {
                        flg = 1;
                        timer_bool = true;
                        bg_bool = true;
                        revive_time = -0.5f;
                        timeElapsed = 0.0f;
                        if (Field_direct == Common.Direction.Straight) flg = 6;
                        else if (Field_direct != Common.Direction.None) change_block(reverse(Field_direct));
                        if (Damage())
                        {
                            Goal(2);
                        }
                        player.Set_Chara(Top_ID());//UIsで変更
                    }
                    break;
                case 6: //外へ
                    #region カメラの位置
                    vec = game_camera.transform.position;
                    game_camera.transform.position = (vec * 15f + (Pazzle_fields[2, 2].Pos - Dire_to_Vec(reverse(Field_direct)) + new Vector3(0, 0.8f, -10))) / 16f;
                    Arrow_show(false);
                    #endregion
                    #region 動き終わった後
                    if (Field_direct != Common.Direction.Straight && (game_camera.transform.position - (2f * Pazzle_fields[2, 2].Pos + new Vector3(3 * L(player.x) + 2, 3 * L(player.y) + 4.4f, -30)) / 3f).magnitude < 0.015f)
                    {
                        int pre_x = player.pre_x;
                        int pre_y = player.pre_y;
                        player.pre_x = player.x;
                        player.pre_y = player.y;
                        //PlayerPrefs.SetInt("Road" + Road_count, (int)reverse(player.move_to));
                        Road_counter();
                        player.set_curve(pre_x, pre_y, player.move_to, player.move_from);
                        if (Pazzle_data[player.pre_x, player.pre_y].type == Common.Direction.None)//穴
                        {
                            Fall(Common.Condition.Normal);
                        }
                        else if (Pazzle_data[player.pre_x, player.pre_y].type == Common.Direction.Up)//引き返し
                        {
                            Field_direct = Common.Direction.Straight;
                        }
                        else if (Pazzle_data[player.pre_x, player.pre_y].type == Common.Direction.Down)//ゴール
                        {
                            Goal(0);
                        }
                    }
                    else if ((game_camera.transform.position - Pazzle_fields[2, 2].Pos - new Vector3(0, 0.8f, -10)).magnitude < 0.015f)//引き返し後
                    {
                        game_camera.transform.position = Pazzle_fields[2, 2].Pos + new Vector3(0, 0.8f, -10);
                        Pazzle_data[player.x, player.y].condition = Common.Condition.Player;
                        Field_direct = Common.Direction.None;
                        flg = 1;
                        timer_bool = true;
                    }
                    #endregion
                    break;
                case 7: //ゴール
                    if (timeElapsed == 0.0f)
                        SE_on(Common.SE.Get);
                    Transform tra = GameObject.Find("Goal_ship").transform;
                    Vector3 vector = tra.position;
                    if (vector.y < 50) tra.position = (16f * vector - new Vector3(7, 4.99f)) / 15f;
                    timeElapsed += Time.deltaTime;
                    To_goal();
                    break;
                case 8: //Game Over
                    if (timeElapsed == 0.0f) { 
                        BGM_on(Common.BGM.gameover); // ここでゲームオーバーBGM定義
                        After_Gameover();
                     }
                    timeElapsed += Time.deltaTime;
                    To_result();
                    break;
            }
            #region 宝の場所更新
            for (int i = 0; i < treasure.Length; i++)
            {
                if (L(player.x) == L(treasure[i].x) && L(player.y) == L(treasure[i].y))
                {
                    treasure[i].Pos = Pazzle_fields[(treasure[i].x - 1) % 3 + 1, (treasure[i].y - 1) % 3 + 1].Pos;
                    treasure[i].find = true;
                }
                else treasure[i].Pos = new Vector3(treasure[i].x, treasure[i].y, 0);
                if (!treasure[i].get && (treasure[i].Pos - player.Pos).magnitude < 0.3f)  //ここに当たった時の
                {
                    touch_ID = i;
                    flg = 4;
                }
            }
            #endregion

        }
        #region Mapの更新
        player.On_Map(true);
        for (int i = 0; i < treasure.Length; i++) treasure[i].On_Map((L(treasure[i].x) == L(player.x) && L(treasure[i].y) == L(player.y)), is_Skill(2));//is_Skill(n6)
        for (int i = 0; i < enemy.Length; i++)
        {
            if (enemy[i].act == Common.Action.Sad) enemy[i].On_Map(false);
            else if (d_infty(enemy[i].Pos, new Vector3(3 * L(player.x) + 2, 3 * L(player.y) + 2)) < 1.5f) enemy[i].On_Map(true);
            else enemy[i].On_Map(false);//UIs.is_Skill(n0)),is_Skill(n6,find実装)
        }
        #endregion
        #region タッチエフェクト
        // 画面のどこでもタッチでエフェクト
        if (Input.GetMouseButton(0))
        {
            // マウスのワールド座標までパーティクルを移動,エフェクトを1つ生成する
            var pos = _camera.ScreenToWorldPoint(Input.mousePosition + _camera.transform.forward * 10);
            touchEffect.transform.position = pos;
            touchEffect.Emit(1);
        }
        // 使用する際はSub_cameraとTouch_particleオブジェクトを追加してください
        #endregion
        #region 初期アニメーション追加部分
        if (is1stAnim)
        {
            #region 飛行船のアニメーション
            if (!fstFlag)
            {
                Invoke("After_hikousen", 1.0f);
                fstFlag = true;
                for (int i = 0; i < 10; i++)
                    Invoke("BGM_fade_in", i * 0.5f); // BGM 疑似フェードイン              
            }

            Hikousen_pos = GameObject.Find("Hikousen").GetComponent<RectTransform>();
            pos_diff = (new Vector3(-90, 50, 0) - Hikousen_pos.localPosition);
            //Debug.Log(pos_diff);

            if (Mathf.Abs(pos_diff.x) < 5 && Mathf.Abs(pos_diff.z) < 5)
                FallChara.GetComponent<Animator>().SetTrigger("FallTrigger");
            #endregion
            #region 探索開始!のアニメーション
            if (HikousenFin)
            {
                /*
                if (Anim_start && !mid_reach)
                {
                    Start_and_End_anim = GameObject.Find("Start_and_End_anim").GetComponent<RectTransform>();
                    move_diff = (char_right - Start_and_End_anim.localPosition.x) / 50;
                    Start_and_End_anim.localPosition += new Vector3(move_diff, 0, 0);
                    if (move_diff < 0.1) mid_reach = true;
                }
                else if (Anim_start && mid_reach)
                {
                    Start_and_End_anim = GameObject.Find("Start_and_End_anim").GetComponent<RectTransform>();
                    move_diff = (char_right + Screen.width*2 - Start_and_End_anim.localPosition.x) / 50;
                    Start_and_End_anim.localPosition += new Vector3(move_diff, 0, 0);
                    player_obj_color.SetActive(true);
                    if (move_diff < 0.1)
                    {
                        Anim_start = false;
                        mid_reach = false;
                        isAnim = false;
                        //player_obj_color.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
                        //player_obj_color.SetActive(true);
                    }
                }
                */
                Start_and_End_anim.GetComponent<Animator>().SetBool("moveBool", true);
                isAnim = false;
            }
            #endregion
        }
        #endregion
        #endregion
    }


    #region M
    bool Move(int x, int y, Common.Direction direct, float speed)//動かしたいものの座標、0~8、x:左から,y:下から、speed小さい方が早い
    {
        if (x == -1)
        {
            return true;
        }
        else
        {
            Vector3 pos = Pazzle_fields[(x - 1) % 3 + 1, (y - 1) % 3 + 1].Pos;
            if (direct == Common.Direction.None)
            {
                return true;
            }
            else if ((pos - Dire_to_Vec(direct) - new Vector3(x, y, 0)).magnitude > 0.03f)
            {
                Pazzle_fields[(x - 1) % 3 + 1, (y - 1) % 3 + 1].Pos = (pos * speed + Dire_to_Vec(direct) + new Vector3(x, y, 0)) / (1 + speed);
                return false;
            }
            else  //動き終わり、データ交換
            {
                int p = x + (int)Dire_to_Vec(direct).x;
                int q = y + (int)Dire_to_Vec(direct).y;//元々の穴の位置(0~8)
                Pazzle_data[x, y].Set_address(p, q);
                Pazzle_data[p, q].Set_address(x, y);
                Data_box Tmp = Pazzle_data[p, q];
                Pazzle_data[p, q] = Pazzle_data[x, y];
                Pazzle_data[x, y] = Tmp; //Data_box交換
                Pazzle_data[x, y].condition = Common.Condition.Hole;
                Pazzle_data[p, q].condition = Move_condition;
                Pazzle_fields[(x - 1) % 3 + 1, (y - 1) % 3 + 1].Pos = new Vector3(x, y, 0);
                Pazzle_fields[(p - 1) % 3 + 1, (q - 1) % 3 + 1].Pos = new Vector3(p, q, 0);
                Pazzle_fields[(p - 1) % 3 + 1, (q - 1) % 3 + 1].Layer(10 - q);
                Pazzle_fields[(x - 1) % 3 + 1, (y - 1) % 3 + 1].Set_img(Pazzle_data[x, y].type);
                Pazzle_fields[(p - 1) % 3 + 1, (q - 1) % 3 + 1].Set_img(Pazzle_data[p, q].type);
                Move_X = -1; //バグ修正のため
                if (Move_condition == Common.Condition.Player)
                {
                    player.x = p;
                    player.y = q;
                    //PlayerPrefs.SetInt("Road" + Road_count, (int)Move_direct);
                    Road_counter();
                }
                Move_direct = Common.Direction.None;
                for (int i = 0; i < enemy.Length; i++)//敵交換
                {
                    if (enemy[i].type != Common.Type.Fly && enemy[i].x == x && enemy[i].y == y)
                    {
                        enemy[i].pre_x = enemy[i].x;
                        enemy[i].pre_y = enemy[i].y;
                        enemy[i].x = p;
                        enemy[i].y = q;
                    }
                }
                for (int i = 0; i < treasure.Length; i++)//宝交換
                {
                    if (treasure[i].x == x && treasure[i].y == y)
                    {
                        treasure[i].x = p;
                        treasure[i].y = q;
                    }
                }
                tap_Start.x = -1;//デバッグ
                return true;
            }
        }
    }

    Common.Direction Get_exit(Character chara, Common.Direction entrance) //敵によって出口を変える
    {
        if (chara.type == Common.Type.Walk || chara.type == Common.Type.Player) return Pazzle_data[chara.x, chara.y].Exit_direction(entrance);
        else if (chara.type == Common.Type.Fly)
        {
            Common.Direction ran = Random_direct();
            while (ran == entrance)
            {
                ran = Random_direct();
            }
            return ran;
        }
        else return Common.Direction.None;
    }

    int[] Random_position() //座標のランダム関数
    {
        int[] pos = new int[2];
        pos[0] = Random.Range(1, 6);
        pos[1] = Random.Range(1, 6);
        while (Pazzle_data[pos[0], pos[1]].type == Common.Direction.None || Pazzle_data[pos[0], pos[1]].condition == Common.Condition.Player || Pazzle_data[pos[0], pos[1]].treasure != -1)
        {
            pos[0] = Random.Range(1, 6);
            pos[1] = Random.Range(1, 6);
        }
        return pos;
    }

    void Block_data_set(int x, int y) //塊としての座標、0~2、x:左から,y:下から
    {
        #region tmp[] = 8個の道の構成
        Common.Direction[] tmp = new Common.Direction[9];
        tmp[1] = Common.Direction.Straight;
        tmp[2] = Common.Direction.Straight;
        tmp[3] = Common.Direction.Straight;
        tmp[4] = Common.Direction.Right;
        tmp[5] = Common.Direction.Right;
        tmp[6] = Common.Direction.Right;
        tmp[7] = Common.Direction.Left;
        tmp[8] = Common.Direction.Left;
        int ran = Random.Range(0, 3);
        if (ran == 1) tmp[6] = Common.Direction.Left; //Leftが多い場合
        else if (ran == 2) tmp[4] = Common.Direction.Straight;//Straightが多い場合
        #endregion
        #region tmpのシャッフル
        float[] random = new float[8];
        for (int i = 0; i < 8; i++)
        {
            random[i] = Random.value;
        }
        for (int i = 1; i < 8; i++)
        {
            for (int j = i; j > 0; j--)
            {
                if (random[j] < random[j - 1])
                {
                    float change = random[j];
                    random[j] = random[j - 1];
                    random[j - 1] = change;
                    Common.Direction Change = tmp[j];
                    tmp[j] = tmp[j + 1];
                    tmp[j + 1] = Change;
                }
                else break;
            }
        }
        #endregion
        int hole = 4;//ここが穴になる、後でランダム化
        for (int i = 0; i < hole; i++) tmp[i] = tmp[i + 1];
        tmp[hole] = Common.Direction.None;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Pazzle_data[3 * x + i + 1, 3 * y + j + 1].type = tmp[i + 3 * j];
                Pazzle_data[3 * x + i + 1, 3 * y + j + 1].Set_address(3 * x + i + 1, 3 * y + j + 1);
                Pazzle_data[3 * x + i + 1, 3 * y + j + 1].condition = Common.Condition.Normal;
            }
        }
        Pazzle_data[3 * x + hole % 3 + 1, 3 * y + Mathf.FloorToInt(hole / 3) + 1].condition = Common.Condition.Hole;
    }

    void Pazzle_data_set() //全部のブロックについて入れていく
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                Block_data_set(i, j);
            }
        }
    }

    void set_block(int ID, int x, int y) //塊としての座標、0~2、x:左から,y:下から,ID;0:Pazzle_field,1:move_field
    {
        if (ID == 0)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Pazzle_fields[i, j].data_x = 3 * x + i;
                    Pazzle_fields[i, j].data_y = 3 * y + j;
                    Pazzle_fields[i, j].Layer(10 - 3 * y - j);
                    Pazzle_fields[i, j].Set_img(Pazzle_data[3 * x + i, 3 * y + j].type);
                    Pazzle_fields[i, j].Pos = new Vector3(3 * x + i, 3 * y + j);
                }
            }
            Small_map(x, y);
        }
        else if (ID == 1)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    move_fields[i, j].data_x = 3 * x + i;
                    move_fields[i, j].data_y = 3 * y + j;
                    move_fields[i, j].Layer(10 - 3 * y - j);
                    move_fields[i, j].Set_img(Pazzle_data[3 * x + i, 3 * y + j].type);
                    move_fields[i, j].Pos = new Vector3(3 * x + i, 3 * y + j);
                }
            }
            Small_map(x, y);
        }
    }

    void Plus_enemy()
    {
        int num = -1;
        for (int i = 0; i < enemy.Length; i++)
        {
            if (enemy[i].act == Common.Action.Sad)
            {
                num = i;
                break;
            }
        }
        if (num != -1)
        {
            Common.Type[] types = { Common.Type.Walk, Common.Type.Stop, Common.Type.Fly };
            int type_num = Random.Range(0, 3);
            enemy[num].Revival(types[type_num]); //typesをランダム化
            Common.Direction dire = Common.Direction.None;
            if (types[type_num] != Common.Type.Stop) dire = Random_direct();
            int[] ran = Random_position();
            for (int j = 0; j < enemy.Length; j++)
            {
                if (j != num && enemy[j].x == ran[0] && enemy[j].y == ran[1])
                {
                    ran = Random_position();
                    j = -1;
                }
            }
            enemy[num].x = ran[0];
            enemy[num].y = ran[1];
            enemy[num].set_position(ran[0], ran[1], dire, Get_exit(enemy[num], dire));
        }
    }

    void change_block(Common.Direction entrance)//キャラの座標、0~8、x:左から,y:下から
    {
        if (Move_X != -1) //バグ修正のため
        {
            Pazzle_data[Move_X, Move_Y].condition = Move_condition;
            Pazzle_data[Move_X + (int)Dire_to_Vec(Move_direct).x, Move_Y + (int)Dire_to_Vec(Move_direct).y].condition = Common.Condition.Hole;
            Move_X = -1;
            Move_direct = Common.Direction.None;
        }
        timer_bool = false;
        set_block(1, L(player.pre_x), L(player.pre_y));
        set_block(0, L(player.x), L(player.y));
        Set_color(new Vector3(3 * L(player.pre_x) + 2, 3 * L(player.pre_y) + 2, 0));
        Field_direct = entrance;
        flg = 2;
    }

    void Fall(Common.Condition con) //is_Skill(n7)
    {
        player.OutScreen(true);
        //player.Set_Chara(0); //アニメーション交換
        Anime(0, Common.Action.Sad);
        //UIs.SE_on(Common.SE.Fall);
        flg = 5; ;
        Invoke("After_Fall", 0.1f);
        timer_bool = false;
        bg_bool = false;
        if (con == Common.Condition.Hole) //ただ穴に落ちた
        {
            int pre_x = player.pre_x;
            int pre_y = player.pre_y;
            player.pre_x = player.x;
            player.pre_y = player.y;
            //PlayerPrefs.SetInt("Road" + Road_count, (int)reverse(player.move_to));
            Road_counter();
            player.set_curve(pre_x, pre_y, player.move_to, player.move_from);
            Pazzle_data[player.x, player.y].condition = Common.Condition.Player;
        }
        else if (con == Common.Condition.Moving) //のってたやつが動いてた
        {
            //PlayerPrefs.SetInt("Road" + Road_count, (int)reverse(player.move_to));
            Road_counter();
            player.set_curve(player.x, player.y, player.move_to, player.move_from);
        }
        else if (con == Common.Condition.Player)//塊が動いたら盤がなかった
        {
            int pre_x = player.pre_x;
            int pre_y = player.pre_y;
            player.pre_x = player.x;
            player.pre_y = player.y;
            //PlayerPrefs.SetInt("Road" + Road_count, (int)Field_direct);
            Road_counter();
            player.set_curve(pre_x, pre_y, reverse(Field_direct), Pazzle_data[pre_x, pre_y].Exit_direction(reverse(Field_direct)));
        }
        else
        {
            //player.set_curve(player.pre_x, player.pre_y, reverse(Field_direct), Pazzle_data[player.pre_x, player.pre_y].Exit_direction(reverse(Field_direct)));
            Field_direct = Common.Direction.Straight;
        }
    }

    void Set_color(Vector3 vec) //透明化
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                /*if (UIs.is_Skill(n10)) //周りが見えるがスキル化するとき
                {
                    Pazzle_fields[i, j].Sprite().color = new Color(1, 1, 1, 1);
                    move_fields[i, j].Sprite().color = new Color(1, 1, 1, 1);
                }
                else
                {
                    if (i == 0 || i == 4 || j == 0 || j == 4)
                    {
                        Pazzle_fields[i, j].Sprite().color = new Color(1, 1, 1, 0);
                        move_fields[i, j].Sprite().color = new Color(1, 1, 1, 0);
                    }
                    else
                    {
                        Pazzle_fields[i, j].Sprite().color = new Color(1, 1, 1, 2f - d_infty(Pazzle_fields[1, 1].Pos, vec));
                        move_fields[i, j].Sprite().color = new Color(1, 1, 1, 2f - d_infty(move_fields[1, 1].Pos, vec));
                    }
                }*/
                Pazzle_fields[i, j].Sprite().color = new Color(1, 1, 1, 2.5f - d_infty(Pazzle_fields[i, j].Pos, vec));
                move_fields[i, j].Sprite().color = new Color(1, 1, 1, 2.5f - d_infty(move_fields[i, j].Pos, vec));
            }
        }
        if (L(player.x) == 1 && L(player.y) == 1)
        {
            Pazzle_fields[4, 1].Sprite().color = Color.white;
            Pazzle_fields[4, 2].Sprite().color = Color.white;
            Pazzle_fields[4, 3].Sprite().color = Color.white;
        }
        for (int i = 0; i < treasure.Length; i++) treasure[i].Sprite().color = new Color(1, 1, 1, 2f - d_infty(new Vector3(treasure[i].x, treasure[i].y, 0), vec));
        for (int i = 0; i < enemy.Length; i++) if (enemy[i].act != Common.Action.Sad) enemy[i].Sprite().color = new Color(1, 1, 1, 6.3f - 4 * d_infty(enemy[i].Pos, vec));
    }

    int Hole_search(int x, int y, int P) //どこにいるか入れたら穴の場所、p=0:x,p=1:y
    {
        x = L(x) * 3 + 1;
        y = L(y) * 3 + 1;
        int ans = x;
        for (int i = x; i < x + 3; i++)
        {
            for (int j = y; j < y + 3; j++)
            {
                if (Pazzle_data[i, j].type == Common.Direction.None)
                {
                    if (P == 0) ans = i;
                    else ans = j;
                }
            }
        }
        return ans;
    }

    public void Pause_button_down(bool which)
    {
        pause_bool = which;
        int m = Mathf.FloorToInt(time / 60f);
        int s = Mathf.FloorToInt(time % 60f);
        Time_text.text = ("Time   " + m.ToString().PadLeft(2, '0') + " : " + s.ToString().PadLeft(2, '0'));
        GameObject.Find("Walk_F").GetComponent<Text>().text = "踏破率：" + Mathf.RoundToInt(road / 72f * 100f) + " %";
        GameObject.Find("Walk").GetComponent<Text>().text = "歩いた距離：" + All_count + "歩";
        GameObject.Find("Item_coin").GetComponent<Image>().fillAmount = tresure[1] / 3f;
        GameObject.Find("Item_weapon").GetComponent<Image>().fillAmount = tresure[0] / 3f;
        SE_on(Common.SE.Button);
    }

    void OutScreen() //move_fieldsを外にやる
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                move_fields[i, j].Pos = new Vector3(-5, 0, 0);
            }
        }
    }

    void Arrow_show(bool show)
    {
        if (show && L(player.y) != 2) arrows[0].GetComponent<SpriteRenderer>().color = Color.white;
        else arrows[0].GetComponent<SpriteRenderer>().color = Color.clear;
        if (show && L(player.x) != 2) arrows[1].GetComponent<SpriteRenderer>().color = Color.white;
        else arrows[1].GetComponent<SpriteRenderer>().color = Color.clear;
        if (show && L(player.y) != 0) arrows[2].GetComponent<SpriteRenderer>().color = Color.white;
        else arrows[2].GetComponent<SpriteRenderer>().color = Color.clear;
        if (show && L(player.x) != 0) arrows[3].GetComponent<SpriteRenderer>().color = Color.white;
        else arrows[3].GetComponent<SpriteRenderer>().color = Color.clear;
        for (int i = 0; i < 4; i++)
        {
            arrows[i].transform.position = new Vector3(3 * L(player.x) + 2 + 1.5f * Mathf.Sin(i * Mathf.PI / 2f), 3 * L(player.y) + 2 + 1.5f * Mathf.Cos(i * Mathf.PI / 2f), 0);
        }
    }
    

    void Goal(int goal)
    {
        if (goal == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                Anime(i, Common.Action.Happy);
                PlayerPrefs.SetInt("result", 1);
            }
            //Hikousen.SetActive(true);
            //Hikousen.GetComponent<RectTransform>().localPosition = new Vector3(-width, 0, 0);
            GameObject o = GameObject.Find("Effect");
            o.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
            o.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            o.GetComponent<Animator>().SetInteger("Change_Bool", 3);
            o = GameObject.Find("Time_base");
            o.transform.parent = GameObject.Find("Canvas").transform;
            o.transform.SetAsLastSibling();
            o.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/GameScene/end");
            o.GetComponent<RectTransform>().localPosition = new Vector3(-width, 0.3f * height);
            o.GetComponent<RectTransform>().sizeDelta = new Vector3(0.8f * width, 0.13f * height);
            o = GameObject.Find("GameOver_text");
            o.GetComponent<RectTransform>().localPosition = new Vector3(-width, 0, 0);
            o.GetComponent<Animator>().SetTrigger("Goal_Trigger");
            flg = 7;
            player.Pos = new Vector3(-3, -3);
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                Anime(i, Common.Action.Sad);
                PlayerPrefs.SetInt("result", 0);
            }
            GameObject o = GameObject.Find("Effect");
            o.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
            o.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
            o.GetComponent<Animator>().SetInteger("Change_Bool", goal);
            GameObject.Find("GameOver_text").GetComponent<Image>().color = new Color(1, 1, 1, 0);
            GameObject.Find("Game_over").GetComponent<Image>().color = new Color(1, 1, 1, 0);
            GameObject.Find("Retry").GetComponent<Image>().color = new Color(1, 1, 1, 0);
            flg = 8;
        }
        timer_bool = false;
        bg_bool = false;
        //PlayerPrefs.SetInt("Road" + Road_count, (int)reverse(player.move_from));
        //PlayerPrefs.SetInt("Length", Road_count + 1);
        //PlayerPrefs.SetInt("Road" + (Road_count+1), 0);
        PlayerPrefs.SetInt("Time", Mathf.RoundToInt(time));
        PlayerPrefs.SetInt("Life", Life_point + 1);
        int[] get_treasure = { 0, 0 };
        float score_p = 1;
        for (int i = 0; i < treasure.Length; i++)
        {
            if (treasure[i].get && (int)treasure[i].type < 2) { get_treasure[(int)treasure[i].type]++; score_p *= 4; }
        }
        PlayerPrefs.SetInt("treasure0", get_treasure[0]);
        PlayerPrefs.SetInt("treasure1", get_treasure[1]);
        int Destroy = 0;
        for (int i = 0; i < enemy.Length; i++)
        {
            if (enemy[i].act == Common.Action.Sad) Destroy++;
        }
        PlayerPrefs.SetInt("enemy", Destroy);
        PlayerPrefs.SetInt("Coin", coin);
        Debug.Log(" s " + score);
        score = score * score*3f  + score_p + coin + 1000f * (Life_point + 1) + 2 * time + All_count * All_count*1.5f;
        score = Mathf.Max(score, 1200 * retry) - 1200 * retry;
        PlayerPrefs.SetInt("Score", Mathf.RoundToInt(score));

    }

    void change_Mount(Common.Direction entrance, int x, int y)//キャラの座標、0~8、x:左から,y:下から
    {
        if (Move_X != -1) //バグ修正のため
        {
            Pazzle_data[Move_X, Move_Y].condition = Move_condition;
            Pazzle_data[Move_X + (int)Dire_to_Vec(Move_direct).x, Move_Y + (int)Dire_to_Vec(Move_direct).y].condition = Common.Condition.Hole;
            set_block(0, L(Move_X), L(Move_Y));
            Move_X = -1;
        }
        timer_bool = false;
        for (int i = 1; i < 4; i++)
        {
            for (int j = 1; j < 4; j++)
            {
                if (3 * x + i >= 0 && 3 * y + j >= 0 && 3 * x + i < 8 && 3 * y + j < 8)
                {
                    move_fields[i, j].Layer(10 - 3 * y - j);
                    move_fields[i, j].Set_img(Pazzle_data[3 * x + i, 3 * y + j].type);
                    move_fields[i, j].Pos = new Vector3(3 * x + i, 3 * y + j);
                    move_fields[i, j].Sprite().color = new Color(1, 1, 1, 1);
                }
            }
        }
        Field_direct = entrance;
        //mount = X_or_Y;
        flg = 6;
    }

    public void Revival()
    {
        GameObject o = GameObject.Find("Effect");
        o.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
        o.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        o.GetComponent<Animator>().SetInteger("Change_Bool", 0);
        GameObject.Find("GameOver_text").GetComponent<Image>().color = new Color(1, 1, 1, 0);
        GameObject.Find("Game_over").GetComponent<Image>().color = new Color(1, 1, 1, 0);
        GameObject.Find("Retry").GetComponent<Image>().color = new Color(1, 1, 1, 0);
        revive_time = -0.5f;
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
        To_Red(false);
        /*GameObject.Find("Player").GetComponent<Animator>().SetInteger("Chara_Int", Top_ID());
        Skill_text.text = Chara[0].skill_Description;
        skill_time = 20;*/
        timer_bool = true;
        bg_bool = true;
        BGM_on(Common.BGM.tutorial); // ここでゲームオーバーBGM定義
        skill_Icon.GetComponent<RectTransform>().sizeDelta = new Vector2(0.055f * height, 0.055f * height);
        skill_Icon.GetComponent<Animator>().SetInteger("Chara_Int", Top_ID());
        retry++;
        flg = 1;
        player.Set_Chara(Top_ID());//UIsで変更
        game_camera.transform.position = new Vector3(3 * L(player.x) + 2, 3 * L(player.y) + 2.8f, -10);
    }

    void To_Red(bool time)
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Pazzle_fields[i, j].time = time;
                move_fields[i, j].time = time;
                Pazzle_fields[i, j].Set_img(Pazzle_data[3 * L(player.x) + i, 3 * L(player.y) + j].type);
            }
        }
    }
    

    void Map_color(float color_a)
    {
        for (int i = 0; i < treasure.Length; i++)
        {
            treasure[i].map_color(color_a);
        }
        for (int i = 0; i < enemy.Length; i++)
        {
            enemy[i].map_color(color_a);
        }
        player.map_color(color_a);
        GameObject.Find("Map_base").GetComponent<Image>().color = new Color(1, 1, 1, color_a);
        GameObject.Find("Map_Goal").GetComponent<Image>().color = new Color(1, 1, 1, color_a);
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                GameObject.Find("Small_map" + i + "-" + j).GetComponent<Image>().color = new Color(0.7f, 0.4f, 0, color_a);
            }
        }
    }

    #region Battle追加部分
    public void Battle_setup()
    {
        timer_bool = true;
        Sentou_kaisi.GetComponent<Animator>().SetBool("Sentou_effect", false);
        Invoke("Battle_time_loss", 1.0f);
        Invoke("Battle_time_loss", 2.0f);
        Invoke("Battle_time_loss", 3.0f);

    }
    public void Battle_endset()
    {
        for (int i = 0; i <= Life_point; i++)
            Anime(i, Common.Action.Walk);

        Battle_effect.GetComponent<Animator>().SetInteger("Battle_effect", 10);
        Battle_down_panel.GetComponent<Image>().color = new Color(0, 0, 0, 0);

        timeElapsed = 0.0f;
        isBattle = false;
        Enemy_Anime(false, enemy[touch_ID].type);
        BGM_on(Common.BGM.tutorial); // ここで通常BGM定義

        coin += Random.Range(10, 20); // コイン取得

        Enemy.GetComponent<RectTransform>().localPosition=new Vector3(2.0f * width, height * 0.25f);

        Battle_move_anim(2);

        score++;
        bg_bool = true;
        enemy[touch_ID].act = Common.Action.Sad;
        enemy[touch_ID].OutScreen(true);//ここもアニメーションになるっぽい（アイコンになるなら）
        flg = 1;
    }

    public void Battle_time_loss()
    {
        Anime(0, Common.Action.Battle);

        time_minus.GetComponent<Animator>().SetTrigger("LossTime");
        Battle_effect.GetComponent<Animator>().SetInteger("Battle_effect", Top_ID());
        //UIs.Battle_move_anim(3);
        
        if(is_Skill(0)) Lose_Time(500 / Chara[0].Attack);
        else Lose_Time(900/Chara[0].Attack);
        SE_on(Chara[0].Action); //(Party.Action Chara[]内部に移行)
    }
    #endregion

    #region 初期アニメーション終了後処理


    public void BGM_fade_in()
    {
        BGM_volume_set_out(0.04f);
    }
    public void After_hikousen()
    {
        HikousenFin = true;
        FadePanel.GetComponent<Animator>().SetBool("FadeBool", true);
        Invoke("After_start_animation", 4.0f); // 後処理

        for (int i = 0; i < 3; i++)
            Chara[i].Pos -= new Vector3(width * 0.8f * (i + 1), 0, 0);
    }

    public void After_start_animation()
    {
        //pause_bool = !pause_bool;
        timer_bool = true;
        bg_bool = true;
        player.set_position(2,1, Common.Direction.Down, Pazzle_data[2,1].Exit_direction(Common.Direction.Down));//From135
        flg = 1;

        revive_time = -0.5f;
        is1stAnim = false;

        FadePanel.SetActive(false);
        FallChara.SetActive(false);
        Hikousen.SetActive(false);
    }

    #endregion

    private void After_get()
    {
        Get.GetComponent<Animator>().SetInteger("Get_Lost", 5);
    }
    private void After_Gameover()
    {
        for (int i = 0; i <= Life_point; i++)
            Anime(i, Common.Action.Sad);

        Battle_effect.GetComponent<Animator>().SetInteger("Battle_effect", 10);
        timeElapsed = 0.0f;
        isBattle = false;
        Enemy_Anime(false, enemy[touch_ID].type);
        Battle_move_anim(2);
        bg_bool = true;
        enemy[touch_ID].act = Common.Action.Sad;
    }
    #endregion

    


    #region S
    
    void Lose_Time(float minus) //単位は秒
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

    bool Damage()
    {
        Chara[0].Anime().SetBool("Out_Bool", true);
        Life_point--;
        skill_time = 20;
        if (Chara[1].skills[6] > 30) player.set_Speed(55);
        else player.set_Speed(90);
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

    int Top_ID() //一番前のキャラの図鑑ID（アニメータ）
    {
        if (Life_point < 0) return 0;
        else return Chara[0].chara_ID;
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

    void Small_map(int x, int y) //右上の地図の枠
    {
        GameObject o = GameObject.Find("Small_map" + x + "-" + y);
        o.GetComponent<RectTransform>().localPosition = new Vector3(0.08f * (x - 0.5f) * width, 0.08f * (y - 0.5f) * width);
        o.GetComponent<Image>().color = new Color(0.7f, 0.4f, 0, 1);
    }

    void Item_Life()
    {
        if (Life_point < 2)
        {
            Life_point++;
            Chara[Life_point].Pos = new Vector3(-width * 2f, height * 0.277f);
            Chara[Life_point].Anime().SetBool("Out_Bool", false);
            Flame.color = new Color(1, 1, 0, 1);
            Anime(Life_point, Common.Action.Walk);
        }
    }

    #region ゲーム終了らへんの操作
    public void To_result() //シーン内遷移をしても良いか
    {
            AnimatorStateInfo info = GameObject.Find("Effect").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
            if((info.IsName("Game_Over_Time") || info.IsName("Game_Over_Life")) && info.normalizedTime > 0.5f)Set_Button();
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
    

    public void To_goal() //最後の宝箱の動きが終わったかどうか
    {
        RectTransform rect = GameObject.Find("GameOver_text").GetComponent<RectTransform>();
        float x = rect.localPosition.x / width;
        rect.localPosition = new Vector3((x + 0.005f) * width, (x * x - 0.2f) * height);
        rect.sizeDelta = new Vector2((x + 0.7f) * width, (x + 0.7f) * width) / 2f;
        if (x < 0) GameObject.Find("Time_base").GetComponent<RectTransform>().localPosition = (new Vector3(width * x * 2, 0.3f * height));
        else if (x > 0.5f) GameObject.Find("Time_base").GetComponent<RectTransform>().localPosition = (new Vector3(width * (x - 0.5f) * 4, 0.3f * height));
        if( x > 1) SceneManager.LoadScene("Result"); 
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
    void Gage() //スキルゲージを
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

    void Road_counter()
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
            if (Chara[0].skills[6] > 0) player.set_Speed(55);
            if (Chara[0].skills[8] > 0) player.back();

            /* ここを消しても 
            Skill_Flame.GetComponent<Image>().sprite = Chara[0].skill_img;
            Skill_Flame 止められません...。*/

        }
    }

    bool is_Skill(int n)
    {
        return skill_time < Chara[0].skills[n] && (skill_time > 0 || n == 3 || n == 4);
    }
    #endregion

    public void Change_Chara(bool b)
    {
        if (flg==1|| b)
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
            if (Chara[0].skills[6] > 30) player.set_Speed(55);
            else player.set_Speed(90);
        }
    }

    void SE_on(Common.SE music)
    {
        GetComponent<AudioSource>().PlayOneShot(SEs[(int)music]);
    }

    void BGM_on(Common.BGM music)
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
        SEs[16] = Resources.Load<AudioClip>("Audio/SE/Decision");

        #endregion

        #region BGMの設定
        BGMs = GetComponents<AudioSource>();
        #endregion

        skill_icon_effect = GameObject.Find("Skill_Icon_effect");
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

    void After_Fall()
    {
        player.Set_Chara(0);
        player.OutScreen(false);
    }
    #endregion

    #endregion
}