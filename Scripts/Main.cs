using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/* Main
 * Directorオブジェクトにアタッチ
 * Directorはからのオブジェクト
 * 主にゲーム動かす部分にしています。
 */

public class Main : Functions
{
    Data_box[,] Pazzle_data = new Data_box[11, 11];//[左から,下から]の順、下から見て0:None,1:Straight,2:Right,3:Left
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
    State_manage UIs;//UIのことはUIs.~にします。



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
    private RectTransform Start_and_End_anim;
    public GameObject FallChara, FadePanel, Hikousen;

    private float move_diff, char_right = 0;
    private float down_diff, chardown = -450, div = 50;
    private bool isAnim = true, Anim_start = true, mid_reach = false;
    private bool fstFlag = false, HikousenFin = false, is1stAnim = true;
    #endregion

    // Use this for initialization
    void Start()
    {
        int easy = PlayerPrefs.GetInt("Easy", 2);
        UIs = GameObject.Find("State_manager").GetComponent<State_manage>();
        Count_Text = GameObject.Find("Count_Text").GetComponent<Text>();
        #region Pazzle_dataの設定と読み取り
        for (int i = 0; i < 11; i++)
        {
            for (int j = 0; j < 11; j++)
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
        for (int i = 1; i < 10; i++)
        {
            int random = Random.Range(0, 5);
            if (random < easy) { Pazzle_data[0, i].type = Common.Direction.Up; Pazzle_data[0, i].condition = Common.Condition.Normal; }
            else { Pazzle_data[0, i].type = Common.Direction.None; Pazzle_data[0, i].condition = Common.Condition.Hole; }
            random = Random.Range(0, 5);
            if (random < easy) { Pazzle_data[10, i].type = Common.Direction.Up; Pazzle_data[10, i].condition = Common.Condition.Normal; }
            else { Pazzle_data[10, i].type = Common.Direction.None; Pazzle_data[10, i].condition = Common.Condition.Hole; }
            random = Random.Range(0, 5);
            if (random < easy) { Pazzle_data[i, 0].type = Common.Direction.Up; Pazzle_data[i, 0].condition = Common.Condition.Normal; }
            else { Pazzle_data[i, 0].type = Common.Direction.None; Pazzle_data[i, 0].condition = Common.Condition.Hole; }
            random = Random.Range(0, 5);
            if (random < easy) { Pazzle_data[i, 10].type = Common.Direction.Up; Pazzle_data[i, 10].condition = Common.Condition.Normal; }
            else { Pazzle_data[i, 10].type = Common.Direction.None; Pazzle_data[i, 10].condition = Common.Condition.Hole; }
            Pazzle_data[0, 0].type = Common.Direction.None; Pazzle_data[0, 0].condition = Common.Condition.Hole;
            Pazzle_data[0, 10].type = Common.Direction.None; Pazzle_data[0, 10].condition = Common.Condition.Hole;
            Pazzle_data[10, 0].type = Common.Direction.None; Pazzle_data[10, 0].condition = Common.Condition.Hole;
            Pazzle_data[10, 10].type = Common.Direction.None; Pazzle_data[10, 10].condition = Common.Condition.Hole;
        }
        Pazzle_data[10, 9].type = Common.Direction.Up;
        Pazzle_data[10, 9].condition = Common.Condition.Normal;//ゴール右上、右側
        Pazzle_data[10, 8].type = Common.Direction.Down;
        Pazzle_data[10, 8].condition = Common.Condition.Player;//ゴール右上、右側
        Pazzle_data[10, 7].type = Common.Direction.Up;
        Pazzle_data[10, 7].condition = Common.Condition.Normal;//ゴール右上、右側
        #endregion
        set_block(0, 1, 1);
        #endregion
        #region playerの設定
        GameObject player_obj = GameObject.Find("Player");
        player = new Character(player_obj, Common.Type.Player);
        player.x = 5;
        player.y = 4;
        player.set_position(5, 4, Common.Direction.Down, Pazzle_data[5, 4].Exit_direction(Common.Direction.Down));
        player.set_Speed(90f);
        Pazzle_data[5, 4].condition = Common.Condition.Player;
        player.Set_Chara(UIs.Top_ID());
        #endregion
        #region 宝物の設定
        treasure = new Item[6]; //ここで宝の数
        GameObject Treasure = Instantiate(Resources.Load<GameObject>("Prefab/Treasure")) as GameObject;
        int[] ran = Random_position();
        Pazzle_data[ran[0], ran[1]].treasure = 0;
        treasure[0] = new Item(ran[0], ran[1], Treasure, Common.Treasure.Item);
        for (int i = 1; i < 3; i++)
        {
            Treasure = Instantiate(Resources.Load<GameObject>("Prefab/Treasure")) as GameObject;
            ran = Random_position();
            Pazzle_data[ran[0], ran[1]].treasure = i;
            treasure[i] = new Item(ran[0], ran[1], Treasure, Common.Treasure.Coin);
        }
        for (int i = 3; i < 5; i++)
        {
            Treasure = Instantiate(Resources.Load<GameObject>("Prefab/Treasure")) as GameObject;
            ran = Random_position();
            Pazzle_data[ran[0], ran[1]].treasure = i;
            treasure[i] = new Item(ran[0], ran[1], Treasure, Common.Treasure.Time);
        }
        Treasure = Instantiate(Resources.Load<GameObject>("Prefab/Treasure")) as GameObject;
        ran = Random_position();
        Pazzle_data[ran[0], ran[1]].treasure = 5;
        treasure[5] = new Item(ran[0], ran[1], Treasure, Common.Treasure.Life);
        #endregion
        #region enemyの設定
        int ene_num = 8;
        if (easy == 5) ene_num = 6;
        enemy = new Character[ene_num];//ここで敵の数
        Common.Type[] types = { Common.Type.Walk, Common.Type.Stop, Common.Type.Fly };
        for (int i = 0; i < enemy.Length; i++)
        {
            GameObject enemy_obj = Instantiate(Resources.Load<GameObject>("Prefab/Enemy")) as GameObject;
            int type_num = Random.Range(0, 3);
            enemy[i] = new Character(enemy_obj, types[type_num]); //typesをランダム化
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
        GameObject camera = GameObject.Find("Main Camera");
        camera.transform.position = new Vector3(3 * L(player.x) + 2, 3 * L(player.y) + 2.8f, -10);

        #region Battle追加部分
        time_minus = GameObject.Find("minus");
        Battle_effect = GameObject.Find("Attack_effect");
        Sentou_kaisi = GameObject.Find("Sentou_kaisi");
        Battle_down_panel = GameObject.Find("Battle_down_panel");
        Hako = GameObject.Find("TakaraBako");
        Get = GameObject.Find("OtakaraGet");
        player_pos_for_treasure = GameObject.Find("Player");
        #endregion
        #region 初期エフェクト追記
        pause_bool = !pause_bool;
        UIs.timer_bool = false;
        UIs.bg_bool = false;
        Set_color(new Vector3(Pazzle_fields[2, 2].data_x, Pazzle_fields[2, 2].data_y));//敵とか透明にする
        #endregion

        // BGMを抑え気味で流し始める
        UIs.BGMs[1].volume = 0.0f;
        UIs.BGM_on(Common.BGM.tutorial); // ここで最初にBGM定義
<<<<<<< HEAD

        //player_obj_color= GameObject.Find("Player");
        //player_obj_color.SetActive(false);
=======
        treasure_count = 0;
>>>>>>> d97e10e2331264522a8f9cce08a6328876ad8222
    }

    void Update()
    {
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
                        if (enemy[i].act != Common.Action.Sad)
                        {
                            Vector3 v = new Vector3(enemy[i].x, enemy[i].y, 0);
                            if (enemy[i].type != Common.Type.Fly && L(player.x) == L(enemy[i].x) && L(player.y) == L(enemy[i].y)) v = Pazzle_fields[(enemy[i].x - 1) % 3 + 1, (enemy[i].y - 1) % 3 + 1].Pos;
                            if (enemy[i].Move(v,UIs.is_Skill(3))) 
                            {
                                enemy[i].pre_x = enemy[i].x;
                                enemy[i].pre_y = enemy[i].y;
                                enemy[i].x += (int)Dire_to_Vec(enemy[i].move_to).x;
                                enemy[i].y += (int)Dire_to_Vec(enemy[i].move_to).y;//敵の動いた座標を更新 
                                #region 引き返し
                                if (enemy[i].x < 1 || enemy[i].x > 9 || enemy[i].y < 1 || enemy[i].y > 9)
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
                                if (UIs.is_Skill(1))
                                {
                                    enemy[i].act = Common.Action.Sad;//★強制送還。コインゲット等のアクションをバトルに入れた場合、ここにもお願いします
                                    //enemy[i].Sprite().color = Color.clear;//ここもアニメーションになるっぽい（アイコンになるなら）
                                    enemy[i].OutScreen();//ここもアニメーションになるっぽい（アイコンになるなら）
                                }
                                else if ((enemy[i].Pos - player.Pos).magnitude < 0.6f||(UIs.is_Skill(7)&& (enemy[i].Pos - player.Pos).magnitude < 0.25f))  //ここに当たった時の。is_Skill(n1),is_Skill(n5)
                                {
                                    touch_ID = i;
                                    flg = 3;
                                }
                            }
                        }
                    }
                    #endregion
                    #region プレイヤーの動き
                    if (player.Move(Pazzle_fields[(player.x - 1) % 3 + 1, (player.y - 1) % 3 + 1].Pos,UIs.is_Skill(4))) //動き終えたらtrue 
                    {
                        if (Pazzle_data[player.x, player.y].walk)
                        {
                            Pazzle_data[player.x, player.y].walk = false;
                            UIs.road++;
                        }
                        //PlayerPrefs.SetInt("Road" + Road_count, (int)player.move_to);
                        UIs.Road_counter();
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
                                if (player.x < 8) change_block(Common.Direction.Left);
                                else change_Mount(Common.Direction.Right, L(player.x), L(player.y));
                            }
                            else if (player.y % 3 == 0 && player.move_to == Common.Direction.Down)
                            {
                                if (player.y > 0) change_block(Common.Direction.Up);
                                else change_Mount(Common.Direction.Down, L(player.x), L(player.y));
                            }
                            else if (player.y % 3 == 1 && player.move_to == Common.Direction.Up)
                            {
                                if (player.y < 8) change_block(Common.Direction.Down);
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
                                    UIs.SE_on(Common.SE.Slide);
                                }
                                else if (delta.x < 0 && X % 3 != 0 && Pazzle_data[X, Y].condition == Common.Condition.Hole)
                                {
                                    Move_X = X + 1;
                                    Move_Y = Y;
                                    Move_direct = Common.Direction.Left;
                                    Move_condition = Pazzle_data[X + 1, Y].condition;
                                    Pazzle_data[X, Y].condition = Common.Condition.Moving;
                                    Pazzle_data[X + 1, Y].condition = Common.Condition.Moving;
                                    UIs.SE_on(Common.SE.Slide);
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
                                    UIs.SE_on(Common.SE.Slide);
                                }
                                else if (delta.y < 0 && Y % 3 != 0 && Pazzle_data[X, Y].condition == Common.Condition.Hole)
                                {
                                    Move_X = X;
                                    Move_Y = Y + 1;
                                    Move_direct = Common.Direction.Down;
                                    Move_condition = Pazzle_data[X, Y + 1].condition;
                                    Pazzle_data[X, Y].condition = Common.Condition.Moving;
                                    Pazzle_data[X, Y + 1].condition = Common.Condition.Moving;
                                    UIs.SE_on(Common.SE.Slide);
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
                    GameObject camera = GameObject.Find("Main Camera");
                    Vector3 vec = camera.transform.position;
                    camera.transform.position = (vec * 13f + (Pazzle_fields[2, 2].Pos + new Vector3(0, 0.8f, -10))) / 14f;
                    Set_color(camera.transform.position - new Vector3(0, 0.8f, 0));
                    Arrow_show(false);
                    #endregion
                    #region 動き終わった後
                    if ((camera.transform.position - new Vector3(3 * L(player.x) + 2, 3 * L(player.y) + 2.8f, -10)).magnitude < 0.015f)
                    {
                        camera.transform.position = new Vector3(3 * L(player.x) + 2, 3 * L(player.y) + 2.8f, -10);
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
                            UIs.timer_bool = true;
                            if (L(player.x)==2&&L(player.y)==2) Pazzle_fields[4, 2].Sprite().color = Color.white;
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
                        UIs.timer_bool = false;
                        UIs.bg_bool = false;
                        UIs.BGM_on(Common.BGM.battle); // ここで戦闘BGM定義
                        UIs.Battle_move_anim(1);

                        for (int i = 0; i <= UIs.get_Life(); i++)
                        {
                            width = Screen.width;
                            height = Screen.height;
                            UIs.Anime(i, Common.Action.Battle);
                        }

                        UIs.Enemy_Anime(true, enemy[touch_ID].type);
                        Sentou_kaisi.GetComponent<Animator>().SetBool("Sentou_effect", true);
                        Invoke("Battle_setup", 2.0f);
                    }
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
                            UIs.Lose_Time(-60);
                        }
                        else if (treasure[touch_ID].type == Common.Treasure.Life) // 梯子取得
                        {
                            //Hako.GetComponent<Animator>().SetInteger("Get_what", 3); 
                            Get.GetComponent<Animator>().SetInteger("Get_Lost", 1);
                            UIs.Item_Life();
                        }
                        UIs.SE_on(Common.SE.Get);
                        coin += Random.Range(5, 9); // コイン取得
                        UIs.bg_bool = false;
                        isGet = true;
                        Invoke("After_get", 1.5f);
                    }
                    #endregion
                    #region キャラのアニメーション、終了時の処理
                    for (int i = 0; i <= UIs.get_Life(); i++)
                        UIs.Anime(i, Common.Action.Happy);

                    if (timeElapsed > 1.5f) //2秒経過で終了
                    {
                        // 移植作業中..
                        for (int i = 0; i <= UIs.get_Life(); i++)
                            UIs.Anime(i, Common.Action.Walk);
                        treasure[touch_ID].Get_Item();
                        if ((int)treasure[touch_ID].type < 2) UIs.tresure[(int)treasure[touch_ID].type]++;
                        Hako.GetComponent<Animator>().SetInteger("Get_what", 0);
                        Get.GetComponent<Animator>().SetBool("Get", false);

                        isGet = false;
                        UIs.bg_bool = true;
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
                        UIs.SE_on(Common.SE.Fall);
                        UIs.timer_bool = false;
                        UIs.bg_bool = false;
                    }
                    timeElapsed += Time.deltaTime;
                    if (timeElapsed > 0.1f && timeElapsed < 0.2f) player.Set_Chara(0);
                    if (player.Wait_chara())
                    {
                        flg = 1;
                        UIs.timer_bool = true;
                        UIs.bg_bool = true;
                        timeElapsed = 0.0f;
                        if (Field_direct == Common.Direction.Straight) flg = 6;
                        else if (Field_direct != Common.Direction.None) change_block(reverse(Field_direct));
                        if (UIs.Damage())
                        {
                            Goal(2);
                        }
                        player.Set_Chara(UIs.Top_ID());//UIsで変更
                    }
                    break;
                case 6: //外へ
                    #region カメラの位置
                    camera = GameObject.Find("Main Camera");
                    vec = camera.transform.position;
                    camera.transform.position = (vec * 15f + (Pazzle_fields[2, 2].Pos - Dire_to_Vec(reverse(Field_direct)) + new Vector3(0, 0.8f, -10))) / 16f;
                    Arrow_show(false);
                    #endregion
                    #region 動き終わった後
                    if (Field_direct != Common.Direction.Straight && (camera.transform.position - (2f * Pazzle_fields[2, 2].Pos + new Vector3(3 * L(player.x) + 2, 3 * L(player.y) + 4.4f, -30)) / 3f).magnitude < 0.015f)
                    {
                        int pre_x = player.pre_x;
                        int pre_y = player.pre_y;
                        player.pre_x = player.x;
                        player.pre_y = player.y;
                        //PlayerPrefs.SetInt("Road" + Road_count, (int)reverse(player.move_to));
                        UIs.Road_counter();
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
                    else if ((camera.transform.position - Pazzle_fields[2, 2].Pos - new Vector3(0, 0.8f, -10)).magnitude < 0.015f)//引き返し後
                    {
                        camera.transform.position = Pazzle_fields[2, 2].Pos + new Vector3(0, 0.8f, -10);
                        Pazzle_data[player.x, player.y].condition = Common.Condition.Player;
                        Field_direct = Common.Direction.None;
                        flg = 1;
                        UIs.timer_bool = true;
                    }
                    #endregion
                    break;
                case 7: //ゴール
                    if (timeElapsed == 0.0f)
                        UIs.SE_on(Common.SE.Get);
                    Transform tra = GameObject.Find("Goal_ship").transform;
                    Vector3 vector = tra.position;
                    if (vector.y < 50) tra.position = (16f * vector - new Vector3(10, 7.49f)) / 15f;
                    timeElapsed += Time.deltaTime;
                    if (UIs.To_goal())
                    {
                        //GameObject.Find("Goal").GetComponent<Animator>().SetBool("Open_Bool", true);
                        //if (UIs.To_result(1)) 
                        SceneManager.LoadScene("Result");
                    }
                    break;
                case 8: //Game Over
                    if (timeElapsed == 0.0f)
                        UIs.BGM_on(Common.BGM.gameover); // ここでゲームオーバーBGM定義
                    timeElapsed += Time.deltaTime;
                    if (UIs.To_result(0))
                    {
                        UIs.Set_Button();
                    }
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
        for (int i = 0; i < treasure.Length; i++) treasure[i].On_Map((L(treasure[i].x) == L(player.x) && L(treasure[i].y) == L(player.y)), UIs.is_Skill(2));//is_Skill(n6)
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
            }
            #endregion
        }
        #endregion

    }

    public bool Move(int x, int y, Common.Direction direct, float speed)//動かしたいものの座標、0~8、x:左から,y:下から、speed小さい方が早い
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
                    UIs.Road_counter();
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

    public Common.Direction Get_exit(Character chara, Common.Direction entrance) //敵によって出口を変える
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

    public int[] Random_position() //座標のランダム関数
    {
        int[] pos = new int[2];
        pos[0] = Random.Range(1, 10);
        pos[1] = Random.Range(1, 10);
        while (Pazzle_data[pos[0], pos[1]].type == Common.Direction.None || Pazzle_data[pos[0], pos[1]].condition == Common.Condition.Player || Pazzle_data[pos[0], pos[1]].treasure != -1)
        {
            pos[0] = Random.Range(1, 10);
            pos[1] = Random.Range(1, 10);
        }
        return pos;
    }

    public void Block_data_set(int x, int y) //塊としての座標、0~2、x:左から,y:下から
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

    public void Pazzle_data_set() //全部のブロックについて入れていく
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Block_data_set(i, j);
            }
        }
    }

    public void set_block(int ID, int x, int y) //塊としての座標、0~2、x:左から,y:下から,ID;0:Pazzle_field,1:move_field
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
            UIs.Small_map(x, y);
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
            UIs.Small_map(x, y);
        }
    }

    public void change_block(Common.Direction entrance)//キャラの座標、0~8、x:左から,y:下から
    {
        if (Move_X != -1) //バグ修正のため
        {
            Pazzle_data[Move_X, Move_Y].condition = Move_condition;
            Pazzle_data[Move_X + (int)Dire_to_Vec(Move_direct).x, Move_Y + (int)Dire_to_Vec(Move_direct).y].condition = Common.Condition.Hole;
            Move_X = -1;
            Move_direct = Common.Direction.None;
        }
        UIs.timer_bool = false;
        set_block(1, L(player.pre_x), L(player.pre_y));
        set_block(0, L(player.x), L(player.y));
        Set_color(new Vector3(3 * L(player.pre_x) + 2, 3 * L(player.pre_y) + 2, 0));
        Field_direct = entrance;
        flg = 2;
    }

    public void Fall(Common.Condition con) //is_Skill(n7)
    {
        player.OutScreen();
        //player.Set_Chara(0); //アニメーション交換
        UIs.Anime(0, Common.Action.Sad);
        //UIs.SE_on(Common.SE.Fall);
        flg = 5;
        UIs.timer_bool = false;
        UIs.bg_bool = false;
        if (con == Common.Condition.Hole) //ただ穴に落ちた
        {
            int pre_x = player.pre_x;
            int pre_y = player.pre_y;
            player.pre_x = player.x;
            player.pre_y = player.y;
            //PlayerPrefs.SetInt("Road" + Road_count, (int)reverse(player.move_to));
            UIs.Road_counter();
            player.set_curve(pre_x, pre_y, player.move_to, player.move_from);
            Pazzle_data[player.x, player.y].condition = Common.Condition.Player;
        }
        else if (con == Common.Condition.Moving) //のってたやつが動いてた
        {
            //PlayerPrefs.SetInt("Road" + Road_count, (int)reverse(player.move_to));
            UIs.Road_counter();
            player.set_curve(player.x, player.y, player.move_to, player.move_from);
        }
        else if (con == Common.Condition.Player)//塊が動いたら盤がなかった
        {
            int pre_x = player.pre_x;
            int pre_y = player.pre_y;
            player.pre_x = player.x;
            player.pre_y = player.y;
            //PlayerPrefs.SetInt("Road" + Road_count, (int)Field_direct);
            UIs.Road_counter();
            player.set_curve(pre_x, pre_y, reverse(Field_direct), Pazzle_data[pre_x, pre_y].Exit_direction(reverse(Field_direct)));
        }
        else
        {
            //player.set_curve(player.pre_x, player.pre_y, reverse(Field_direct), Pazzle_data[player.pre_x, player.pre_y].Exit_direction(reverse(Field_direct)));
            Field_direct = Common.Direction.Straight;
        }
    }

    public void Set_color(Vector3 vec) //透明化
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
        for (int i = 0; i < treasure.Length; i++) treasure[i].Sprite().color = new Color(1, 1, 1, 2f - d_infty(new Vector3(treasure[i].x, treasure[i].y, 0), vec));
        for (int i = 0; i < enemy.Length; i++) if (enemy[i].act != Common.Action.Sad) enemy[i].Sprite().color = new Color(1, 1, 1, 6.3f - 4 * d_infty(enemy[i].Pos, vec));
    }

    public int Hole_search(int x, int y, int P) //どこにいるか入れたら穴の場所、p=0:x,p=1:y
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
        UIs.Pause_flg(pause_bool);
        UIs.SE_on(Common.SE.Button);
    }

    public void OutScreen() //move_fieldsを外にやる
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                move_fields[i, j].Pos = new Vector3(-5, 0, 0);
            }
        }
    }

    public void Arrow_show(bool show)
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

    public void Set_Speed(float s)
    {
        player.set_Speed(s);
    }

    public void Goal(int goal)
    {
        if (goal == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                UIs.Anime(i, Common.Action.Happy);
                PlayerPrefs.SetInt("result", 1);
            }
            //Hikousen.SetActive(true);
            //Hikousen.GetComponent<RectTransform>().localPosition = new Vector3(-width, 0, 0);
            UIs.Effect(3);//きらきらとかつけるのかな？そのアニメで時間を取ろうとしてます。
            flg = 7;
            player.Pos = new Vector3(-3, -3);
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                UIs.Anime(i, Common.Action.Sad);
                PlayerPrefs.SetInt("result", 0);
            }
            UIs.Effect(goal);//真っ暗とか？そのアニメで時間を取ろうとしてます。
            flg = 8;
        }
        UIs.timer_bool = false;
        UIs.bg_bool = false;
        //PlayerPrefs.SetInt("Road" + Road_count, (int)reverse(player.move_from));
        //PlayerPrefs.SetInt("Length", Road_count + 1);
        //PlayerPrefs.SetInt("Road" + (Road_count+1), 0);
        PlayerPrefs.SetInt("Time", Mathf.RoundToInt(UIs.get_Time()));
        PlayerPrefs.SetInt("Life", UIs.get_Life() + 1);
        int[] get_treasure = { 0, 0 };
        for (int i = 0; i < treasure.Length; i++)
        {
            if (treasure[i].get && (int)treasure[i].type < 2) get_treasure[(int)treasure[i].type]++;
        }
        PlayerPrefs.SetInt("treasure0", get_treasure[0]);
        PlayerPrefs.SetInt("treasure1", get_treasure[1]);
        int Destroy = 0;
        for (int i = 0; i < enemy.Length; i++)
        {
            if (enemy[i].act == Common.Action.Sad) Destroy++;
        }
        PlayerPrefs.SetInt("enemy", Destroy);
        //PlayerPrefs.SetInt("Coin", coin);
    }

    public void change_Mount(Common.Direction entrance, int x, int y)//キャラの座標、0~8、x:左から,y:下から
    {
        if (Move_X != -1) //バグ修正のため
        {
            Pazzle_data[Move_X, Move_Y].condition = Move_condition;
            Pazzle_data[Move_X + (int)Dire_to_Vec(Move_direct).x, Move_Y + (int)Dire_to_Vec(Move_direct).y].condition = Common.Condition.Hole;
            set_block(0, L(Move_X), L(Move_Y));
            Move_X = -1;
        }
        UIs.timer_bool = false;
        for (int i = 1; i < 4; i++)
        {
            for (int j = 1; j < 4; j++)
            {
                if (3 * x + i >= 0 && 3 * y + j >= 0 && 3 * x + i < 11 && 3 * y + j < 11)
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
        UIs.Effect(0);
        UIs.Retry();
        flg = 1;
        player.Set_Chara(UIs.Top_ID());//UIsで変更
        GameObject.Find("Main Camera").transform.position = new Vector3(3 * L(player.x) + 2, 3 * L(player.y) + 2.8f, -10);
    }

    public void To_Red(bool time)
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

    public bool Flg(int n)
    {
        return flg == n;
    }

    public void Map_color(float color_a)
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
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                GameObject.Find("Small_map" + i + "-" + j).GetComponent<Image>().color = new Color(0.7f, 0.4f, 0, color_a);
            }
        }
    }

    #region Battle追加部分
    public void Battle_setup()
    {
        UIs.timer_bool = true;
        Sentou_kaisi.GetComponent<Animator>().SetBool("Sentou_effect", false);
        Invoke("Battle_time_loss", 1.0f);
        Invoke("Battle_time_loss", 2.0f);
        Invoke("Battle_time_loss", 3.0f);

    }
    public void Battle_endset()
    {
        for (int i = 0; i <= UIs.get_Life(); i++)
            UIs.Anime(i, Common.Action.Walk);

        Battle_effect.GetComponent<Animator>().SetInteger("Battle_effect", 10);
        Battle_down_panel.GetComponent<Image>().color = new Color(0, 0, 0, 0);

        timeElapsed = 0.0f;
        isBattle = false;
        UIs.Enemy_Anime(false, enemy[touch_ID].type);
        UIs.BGM_on(Common.BGM.tutorial); // ここで通常BGM定義

        coin += Random.Range(10, 20); // コイン取得


        UIs.Battle_move_anim(2);


        UIs.bg_bool = true;
        enemy[touch_ID].act = Common.Action.Sad;
        enemy[touch_ID].OutScreen();//ここもアニメーションになるっぽい（アイコンになるなら）
        flg = 1;
    }

    public void Battle_time_loss()
    {
        time_minus.GetComponent<Animator>().SetTrigger("LossTime");
        Battle_effect.GetComponent<Animator>().SetInteger("Battle_effect", UIs.Top_ID());
        UIs.Battle_move_anim(3);
        
        if(UIs.is_Skill(0)) UIs.Lose_Time(500 / UIs.Chara[0].Attack);
        else UIs.Lose_Time(900/UIs.Chara[0].Attack);
        //UIs.SE_on(UIs.Chara[0].Action); //(Party.Action Chara[]内部に移行)
        if (UIs.Top_ID() == 1 || UIs.Top_ID() == 4)
        {
            UIs.SE_on(Common.SE.Sword);
        }
        else if (UIs.Top_ID() == 2)
        {
            UIs.SE_on(Common.SE.Fire);
        }
        else if (UIs.Top_ID() == 3)
        {
            UIs.SE_on(Common.SE.Gun);
        }
    }
    #endregion

    #region 初期アニメーション終了後処理


    public void BGM_fade_in()
    {
        UIs.BGM_volume_set_out(0.04f);
    }
    public void After_hikousen()
    {
        HikousenFin = true;
        FadePanel.GetComponent<Animator>().SetBool("FadeBool", true);
        Invoke("After_start_animation", 4.0f); // 後処理

        for (int i = 0; i < 3; i++)
            UIs.Chara[i].Pos -= new Vector3(UIs.width * 0.8f * (i + 1), 0, 0);
    }

    public void After_start_animation()
    {
        pause_bool = !pause_bool;
        UIs.timer_bool = true;
        UIs.bg_bool = true;
        flg = 1;

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
}
