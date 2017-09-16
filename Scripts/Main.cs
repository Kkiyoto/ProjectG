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
    //Field[,] Around = new Field[4, 4]; 
    Data_box [,] Pazzle_data = new Data_box[11,11];//[左から,下から]の順、下から見て0:None,1:Straight,2:Right,3:Left
    Field[,] Pazzle_fields = new Field[5,5]; //触る方
    Field[,] move_fields = new Field[5,5]; //動くため
    //Data_box[,] Mountains = new Data_box[4, 9]; //端っこ、Right,Left,Up,Downの順、(int)Direction-2しないとダメ
    Character player;
    Character[] enemy;
    Vector3 tap_Start;
    int Move_X, Move_Y, touch_ID; //touch_ID:敵とか宝に当たった時にその番号 Road_countいくつ道を通ったか(スキル用 ★StateManagerに移行)
    Common.Direction Move_direct,Field_direct;
    Common.Condition Move_condition;
    Item[] treasure;
    GameObject[] arrows = new GameObject[4];
    State_manage UIs;//UIのことはUIs.~にします。


    bool pause_bool = false; //ポーズボタン、止め方が分からないのでとりあえず止めるためのもの
    int flg = 1;//Playerのとこ、止め方が分からないのでとりあえず止めるためのもの、0:ポーズ、1:動く,2:盤変更
                //ここのやり方が分からなかったので真偽地で無理矢理止めています。時間があればいい感じに変えてください。

    #region Battle追加部分
    //GameObject Battle_enemy; //UIsに移植
    GameObject time_minus;
    float timeElapsed, timeOut = 3.0f;
    bool isBattle = false;
    #endregion
    #region タッチエフェクト追記
    [SerializeField] ParticleSystem touchEffect;    // タッチの際のエフェクト
    [SerializeField] Camera _camera;                // カメラの座標
    #endregion
    #region 初期アニメーション追加部分
    // 変数多過ぎ問題
    private RectTransform Start_and_End_anim;
    private float move_diff, char_right = 0;
    private bool Anim_start = true, mid_reach = false;
    #endregion

    // Use this for initialization
    void Start ()
    {
        UIs = GameObject.Find("State_manager").GetComponent<State_manage>();
        #region Pazzle_dataの設定と読み取り
        for (int i = 0; i < 11; i++)
        {
            for(int j = 0; j < 11; j++)
            {
                Pazzle_data[i, j] = new Data_box();
            }
        }
        Pazzle_data_set();
        for (int i = 0; i < 5; i++)
        {
            for(int j = 0; j < 5; j++)
            {
                GameObject o = Instantiate(Resources.Load<GameObject>("Prefab/Field")) as GameObject;
                Pazzle_fields[i, j] = new Field(o);
                o = Instantiate(Resources.Load<GameObject>("Prefab/Field")) as GameObject;
                move_fields[i, j] = new Field(o);
                move_fields[i, j].Pos=new Vector3(-5,0); //見えないところへ
            }
        }
        #region 山の設定
        for (int i = 1; i < 10; i++)
        {
            int ran = Random.Range(0, 5);
            if (ran < 2) { Pazzle_data[0, i].type = Common.Direction.Up; Pazzle_data[0, i].condition = Common.Condition.Normal; }
            else { Pazzle_data[0, i].type = Common.Direction.None; Pazzle_data[0, i].condition = Common.Condition.Hole; }
            ran = Random.Range(0, 5);
            if (ran < 2) { Pazzle_data[10, i].type = Common.Direction.Up; Pazzle_data[10, i].condition = Common.Condition.Normal; }
            else { Pazzle_data[10, i].type = Common.Direction.None; Pazzle_data[10, i].condition = Common.Condition.Hole; }
            ran = Random.Range(0, 5);
            if (ran < 2) { Pazzle_data[i,0].type = Common.Direction.Up; Pazzle_data[i,0].condition = Common.Condition.Normal; }
            else { Pazzle_data[i,0].type = Common.Direction.None; Pazzle_data[i,0].condition = Common.Condition.Hole; }
            ran = Random.Range(0, 5);
            if (ran < 2) { Pazzle_data[i, 10].type = Common.Direction.Up; Pazzle_data[i, 10].condition = Common.Condition.Normal; }
            else { Pazzle_data[i, 10].type = Common.Direction.None; Pazzle_data[i, 10].condition = Common.Condition.Hole; }
            Pazzle_data[0, 0].type = Common.Direction.None; Pazzle_data[0, 0].condition = Common.Condition.Hole;
            Pazzle_data[0, 10].type = Common.Direction.None; Pazzle_data[0, 10].condition = Common.Condition.Hole;
            Pazzle_data[10, 0].type = Common.Direction.None; Pazzle_data[10, 0].condition = Common.Condition.Hole;
            Pazzle_data[10, 10].type = Common.Direction.None; Pazzle_data[10, 10].condition = Common.Condition.Hole;
        }
        Pazzle_data[10, 9].type = Common.Direction.Down;
        Pazzle_data[10, 9].condition = Common.Condition.Player;//ゴール右上、右側
        #endregion
        /*
        #region 周り
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                GameObject o = Instantiate(Resources.Load<GameObject>("Prefab/Field")) as GameObject;
                Around[i, j] = new Field(o);
            }
        }
        #endregion
        */
        set_block(0, 1,1);
        #endregion
        #region playerの設定
        GameObject player_obj = GameObject.Find("Player");
        player = new Character(player_obj,Common.Type.Player);
        player.x = 5;
        player.y = 4;
        player.set_position(5,4, Common.Direction.Down,Pazzle_data[5,4].Exit_direction(Common.Direction.Down));
        player.set_Speed(90f);
        Pazzle_data[5,4].condition = Common.Condition.Player;
        player.Set_Chara(UIs.Top_ID());
        #endregion
        #region 宝物の設定
        treasure = new Item[4]; //ここで宝の数
        for (int i = 0; i < 2; i++)
        {
            GameObject Treasure = Instantiate(Resources.Load<GameObject>("Prefab/Treasure")) as GameObject;
            int[] ran = Random_position();
            Pazzle_data[ran[0],ran[1]].treasure = i;
            treasure[i] = new Item(ran[0], ran[1], Treasure,Common.Treasure.Item);
        }
        for (int i = 2; i < 4; i++)
        {
            GameObject Treasure = Instantiate(Resources.Load<GameObject>("Prefab/Treasure")) as GameObject;
            int[] ran = Random_position();
            Pazzle_data[ran[0], ran[1]].treasure = i;
            treasure[i] = new Item(ran[0], ran[1], Treasure, Common.Treasure.Life);
        }
        #endregion
        #region enemyの設定
        enemy = new Character[8];//ここで敵の数
        Common.Type[] types = { Common.Type.Walk, Common.Type.Stop, Common.Type.Fly };
        for (int i = 0; i < enemy.Length; i++)
        {
            GameObject enemy_obj = Instantiate(Resources.Load<GameObject>("Prefab/Enemy")) as GameObject;
            int type_num = Random.Range(0, 3);
            enemy[i] = new Character(enemy_obj,types[type_num]); //typesをランダム化
            Common.Direction dire = Common.Direction.None;
            if (types[type_num] != Common.Type.Stop) dire = Random_direct();
            int[] ran = Random_position();
            for(int j = 0; j < i; j++)
            {
                if (enemy[j].x == ran[0] && enemy[j].y == ran[1])
                {
                    ran = Random_position();
                    j = -1;
                }
            }
            enemy[i].x = ran[0];
            enemy[i].y = ran[1];
            enemy[i].set_position(ran[0],ran[1], dire, Get_exit(enemy[i],dire));
            enemy[i].set_Speed(110f);
        }
        #endregion
        #region 矢印の設定
        for(int i = 0; i < 4; i++)
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
        camera.transform.position = new Vector3(3*L(player.x) + 2, 3*L(player.y) + 2.8f, -10);

        #region Battle追加部分
        //Battle_enemy = GameObject.Find("BattleEnemy");
        time_minus = GameObject.Find("minus");
        #endregion
        #region 初期エフェクト追記

        pause_bool = !pause_bool;
        UIs.All_pause_flg(pause_bool); // 反抗期  // 駆け抜ける時間  // 止まらない案件  // 追記、止まりました。
        //transform.Translate(1.0f * Time.deltaTime, 0, 0); // アニメーションを止めるように(updateにこれを)

        Invoke("After_start_animation", 4.0f); // 後処理

        Set_color(new Vector3(Pazzle_fields[2,2].data_x, Pazzle_fields[2,2].data_y));//敵とか透明にする
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A)) { aaaa += 0.1f; Map_color(aaaa); }
        if (Input.GetKeyDown(KeyCode.Return)) SceneManager.LoadScene("Tutorial");
        if (Input.GetKeyDown(KeyCode.Space)) Pause_button_down();
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
                    Set_color(new Vector3(Pazzle_fields[2,2].data_x, Pazzle_fields[2,2].data_y));//敵とか透明にする
                    #region 敵の動き
                    for (int i = 0; i < enemy.Length; i++)
                    {
                        if (enemy[i].act != Common.Action.Sad)
                        {
                            Vector3 v = new Vector3(enemy[i].x, enemy[i].y, 0);
                            if (enemy[i].type != Common.Type.Fly && L(player.x) == L(enemy[i].x) && L(player.y) == L(enemy[i].y)) v = Pazzle_fields[(enemy[i].x-1) % 3+1, (enemy[i].y-1) % 3+1].Pos;
                            if (enemy[i].Move(v)) //is_Skill(n3)
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
                                    enemy[i].Sprite().color = Color.clear;//ここもアニメーションになるっぽい（アイコンになるなら）
                                }
                                else if ((enemy[i].Pos - player.Pos).magnitude < 0.6f)  //ここに当たった時の。is_Skill(n1),is_Skill(n5)
                                {
                                    touch_ID = i;
                                    flg = 3;
                                }
                            }
                        }
                    }
                    #endregion
                    #region プレイヤーの動き
                    if (player.Move(Pazzle_fields[(player.x-1) % 3+1, (player.y-1) % 3+1].Pos)) //動き終えたらtrue is_Skill(n4)
                    {
                        //PlayerPrefs.SetInt("Road" + Road_count, (int)player.move_to);
                        UIs.Road_count++;
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
                                else change_Mount(Common.Direction.Left,L(player.x),L(player.y));
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
                        if (Input.GetMouseButtonUp(0)&&tap_Start.x!=-1)
                        {
                            Vector3 tap_Tarminal = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            Vector3 delta = tap_Tarminal - tap_Start;
                            #region 横方向スライド
                            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y) && delta.magnitude > 0.6f)
                            {
                                int X = Hole_search(player.x, player.y, 0);
                                int Y = Hole_search(player.x, player.y, 1); //穴の座標,0~8
                                if (delta.x > 0 && X % 3 !=1 && Pazzle_data[X, Y].condition == Common.Condition.Hole)
                                {
                                    Move_X = X - 1;
                                    Move_Y = Y;
                                    Move_direct = Common.Direction.Right;
                                    Move_condition = Pazzle_data[X - 1, Y].condition;
                                    Pazzle_data[X, Y].condition = Common.Condition.Moving;
                                    Pazzle_data[X - 1, Y].condition = Common.Condition.Moving;
                                }
                                else if (delta.x < 0 && X % 3 !=0 && Pazzle_data[X, Y].condition == Common.Condition.Hole)
                                {
                                    Move_X = X + 1;
                                    Move_Y = Y;
                                    Move_direct = Common.Direction.Left;
                                    Move_condition = Pazzle_data[X + 1, Y].condition;
                                    Pazzle_data[X, Y].condition = Common.Condition.Moving;
                                    Pazzle_data[X + 1, Y].condition = Common.Condition.Moving;
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
                                }
                                else if (delta.y < 0 && Y % 3 !=0 && Pazzle_data[X, Y].condition == Common.Condition.Hole)
                                {
                                    Move_X = X;
                                    Move_Y = Y + 1;
                                    Move_direct = Common.Direction.Down;
                                    Move_condition = Pazzle_data[X, Y + 1].condition;
                                    Pazzle_data[X, Y].condition = Common.Condition.Moving;
                                    Pazzle_data[X, Y + 1].condition = Common.Condition.Moving;
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
                    camera.transform.position = (vec*13f+(Pazzle_fields[2,2].Pos+new Vector3(0,0.8f,-10))) /14f;
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

                        }
                    }
                    #endregion
                    break;
                case 3: //バトル
                    #region Battle追記部分

                    // using UnityEngine.UI; を追加しています
                    timeElapsed += Time.deltaTime;
                    if (!isBattle) // バトル開始直後の処理
                    {
                        isBattle = true;
                        //Battle_enemy.GetComponent<Animator>().SetBool("Battle_start", isBattle);
                        //Battle_enemy.GetComponent<Animator>().SetInteger("EnemyInt", (int)enemy[touch_ID].type);
                        UIs.Enemy_Anime(true, enemy[touch_ID].type);
                        Debug.Log((int)enemy[touch_ID].type + "did it!");
                        Invoke("Battle_time_loss", 1.0f);
                        Invoke("Battle_time_loss", 2.0f);
                        Invoke("Battle_time_loss", 3.0f);

                    }
                    //if (timeElapsed < 1.0)
                        //Battle_enemy.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, timeElapsed); // 動かぬぬ  ★Animatorの中で解決したのかな?


                    if (timeElapsed >= timeOut || Input.GetMouseButtonUp(0)) //タッチ or 3秒経過で終了
                    {
                        //Battle_enemy.GetComponent<Animator>().SetTrigger("BattleEndTrigger");
                        //Battle_enemy.GetComponent<Animator>().SetInteger("EnemyInt", 0);
                        timeElapsed = 0.0f;
                        isBattle = false;
                        //Battle_enemy.GetComponent<Animator>().SetBool("Battle_start", isBattle);
                        UIs.Enemy_Anime(false, enemy[touch_ID].type); //★UIsに移植しました

                        enemy[touch_ID].act = Common.Action.Sad;
                        enemy[touch_ID].Sprite().color = Color.clear;//ここもアニメーションになるっぽい（アイコンになるなら）
                        flg = 1;
                    }
                    #endregion
                    break;
                case 4: //宝ゲット
                    if (Input.GetMouseButtonUp(0)) //今はタッチしたら終了
                    {
                        treasure[touch_ID].Get_Item();
                        flg = 1;
                    }
                    break;
                case 5: //落下
                    if (player.Wait_chara())
                    {
                        flg = 1;
                        UIs.timer_bool = true;
                        UIs.bg_bool = true;
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
                    camera.transform.position = (vec * 15f + (Pazzle_fields[2,2].Pos-Dire_to_Vec(reverse(Field_direct)) + new Vector3(0, 0.8f, -10))) / 16f;
                    Arrow_show(false);
                    #endregion
                    #region 動き終わった後
                    if (Field_direct!=Common.Direction.Straight&&(camera.transform.position - (2f * Pazzle_fields[2,2].Pos + new Vector3(3 * L(player.x) + 2, 3 * L(player.y) + 4.4f,-30)) / 3f).magnitude < 0.015f)
                    {
                        int pre_x = player.pre_x;
                        int pre_y = player.pre_y;
                        player.pre_x = player.x;
                        player.pre_y = player.y;
                        //PlayerPrefs.SetInt("Road" + Road_count, (int)reverse(player.move_to));
                        UIs.Road_count++;
                        player.set_curve(pre_x, pre_y, player.move_to, player.move_from);
                        if (Pazzle_data[player.pre_x,player.pre_y].type == Common.Direction.None)//穴
                        {
                            Fall(Common.Condition.Normal);
                        }
                        else if(Pazzle_data[player.pre_x, player.pre_y].type == Common.Direction.Up)//引き返し
                        {
                            Field_direct = Common.Direction.Straight;
                        }
                        else if (Pazzle_data[player.pre_x, player.pre_y].type == Common.Direction.Down)//ゴール
                        {
                            Goal(0);
                        }
                    }
                    else if((camera.transform.position - Pazzle_fields[2,2].Pos-new Vector3(0,0.8f, -10)).magnitude < 0.015f)//引き返し後
                    {
                        camera.transform.position = Pazzle_fields[2,2].Pos + new Vector3(0, 0.8f, -10);
                        Pazzle_data[player.x, player.y].condition = Common.Condition.Player;
                        Field_direct = Common.Direction.None;
                        flg = 1;
                            UIs.timer_bool = true;
                    }
                        #endregion
                        break;
                case 7: //ゴール
                    //RenderSettings.fogDensity += 0.1f;
                    if (UIs.To_goal(0) & UIs.To_goal(1))
                    {
                        GameObject.Find("Goal").GetComponent<Animator>().SetBool("Open_Bool", true);
                        if (UIs.To_result(1)) SceneManager.LoadScene("Result");
                    }
                    break;
                case 8: //Game Over
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
                    treasure[i].Pos = Pazzle_fields[(treasure[i].x-1) % 3+1, (treasure[i].y-1) % 3+1].Pos;
                    treasure[i].find = true;
                }
                else treasure[i].Pos = new Vector3(treasure[i].x, treasure[i].y, 0);
                if (!treasure[i].get && (treasure[i].Pos-player.Pos).magnitude<0.3f)  //ここに当たった時の
                {
                    touch_ID = i;
                    flg = 4;
                }
            }
            #endregion
        }
            #region Mapの更新
            player.On_Map(true);
            for(int i = 0; i < treasure.Length; i++) treasure[i].On_Map((L(treasure[i].x) == L(player.x) && L(treasure[i].y) == L(player.y)),UIs.is_Skill(2));//is_Skill(n6)
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
        // とりあえずの雑実装.....。すみません..。
        if (Anim_start && !mid_reach) { 
            Start_and_End_anim = GameObject.Find("Start_and_End_anim").GetComponent<RectTransform>();
            move_diff = (char_right - Start_and_End_anim.localPosition.x) / 50;
            Start_and_End_anim.localPosition += new Vector3(move_diff, 0, 0);
            if (move_diff < 0.1) mid_reach = true;
        }else if(Anim_start && mid_reach)
        {
            Start_and_End_anim = GameObject.Find("Start_and_End_anim").GetComponent<RectTransform>();
            move_diff = (char_right+300 - Start_and_End_anim.localPosition.x) / 50;
            Start_and_End_anim.localPosition += new Vector3(move_diff, 0, 0);
            if (move_diff < 0.1)
            {
                Anim_start = false;
                mid_reach = false;
            }
        }
        #endregion

    }

    public bool Move(int x, int y, Common.Direction direct, float speed)//動かしたいものの座標、0~8、x:左から,y:下から、speed小さい方が早い
    {
        if (x== -1)
        {
            return true;
        }
        else
        {
            Vector3 pos = Pazzle_fields[(x-1) % 3+1, (y-1) % 3+1].Pos;
            if (direct == Common.Direction.None)
            {
                return true;
            }
            else if ((pos - Dire_to_Vec(direct) - new Vector3(x, y, 0)).magnitude > 0.03f)
            {
                Pazzle_fields[(x-1) % 3+1, (y-1) % 3+1].Pos = (pos * speed + Dire_to_Vec(direct) + new Vector3(x, y, 0)) / (1 + speed);
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
                Pazzle_fields[(x - 1) % 3+1, (y - 1) % 3 + 1].Pos = new Vector3(x, y, 0);
                Pazzle_fields[(p - 1) % 3 + 1, (q - 1) % 3 + 1].Pos = new Vector3(p, q, 0);
                Pazzle_fields[(x - 1) % 3 + 1, (y - 1) % 3 + 1].Set_img(Pazzle_data[x, y].type);
                Pazzle_fields[(p - 1) % 3 + 1, (q - 1) % 3 + 1].Set_img(Pazzle_data[p, q].type);
                Pazzle_fields[(p - 1) % 3 + 1, (q - 1) % 3 + 1].Layer(10 - q);
                Move_X = -1; //バグ修正のため
                if (Move_condition == Common.Condition.Player)
                {
                    player.x = p;
                    player.y = q;
                    //PlayerPrefs.SetInt("Road" + Road_count, (int)Move_direct);
                    UIs.Road_count++;
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

    public Common.Direction Get_exit(Character chara,Common.Direction entrance) //敵によって出口を変える
    {
        if (chara.type == Common.Type.Walk||chara.type == Common.Type.Player) return Pazzle_data[chara.x, chara.y].Exit_direction(entrance);
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
        pos[1] = Random.Range(1,10);
        while (Pazzle_data[pos[0],pos[1]].type == Common.Direction.None || Pazzle_data[pos[0], pos[1]].condition == Common.Condition.Player || Pazzle_data[pos[0], pos[1]].treasure != -1)
        {
            pos[0] = Random.Range(1,10);
            pos[1] = Random.Range(1,10);
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
        tmp[6]= Common.Direction.Right;
        tmp[7] = Common.Direction.Left;
        tmp[8] = Common.Direction.Left;
        int ran = Random.Range(0, 3);
        if (ran == 1) tmp[6] = Common.Direction.Left; //Leftが多い場合
        else if (ran == 2) tmp[4] = Common.Direction.Straight;//Straightが多い場合
        #endregion
        #region tmpのシャッフル
        float[] random = new float[8];
        for(int i = 0; i < 8; i++)
        {
            random[i] = Random.value;
        }
        for(int i = 1; i < 8; i++)
        {
            for(int j = i; j > 0; j--)
            {
                if (random[j] < random[j-1])
                {
                    float change = random[j];
                    random[j] = random[j-1];
                    random[j-1] = change;
                    Common.Direction Change = tmp[j];
                    tmp[j] = tmp[j+1];
                    tmp[j+1] = Change;
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
                Pazzle_data[3 * x + i+1, 3 * y +j+1].type=tmp[i+3*j];
                Pazzle_data[3 * x + i+1, 3 * y + j+1].Set_address(3 * x + i+1, 3 * y + j+1);
                Pazzle_data[3 * x + i+1, 3 * y + j+1].condition = Common.Condition.Normal;
            }
        }
        Pazzle_data[3 * x + hole % 3+1, 3 * y + Mathf.FloorToInt(hole / 3)+1].condition = Common.Condition.Hole;
    }

    public void Pazzle_data_set() //全部のブロックについて入れていく
    {
        for(int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                Block_data_set(i, j);
            }
        }
    }

    public void set_block(int ID,int x, int y) //塊としての座標、0~2、x:左から,y:下から,ID;0:Pazzle_field,1:move_field
    {
        if (ID == 0)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Pazzle_fields[i, j].data_x=3 * x + i;
                    Pazzle_fields[i, j].data_y=3 * y + j;
                    Pazzle_fields[i, j].Set_img(Pazzle_data[3*x+i,3*y+j].type);
                    Pazzle_fields[i, j].Layer(10-3 * y - j);
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
                    move_fields[i, j].Set_img(Pazzle_data[3 * x + i, 3 * y + j].type);
                    move_fields[i, j].Layer(10 - 3 * y - j);
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
        player.Set_Chara(0); //アニメーション交換
        UIs.Anime(0, Common.Action.Sad);
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
            UIs.Road_count++;
            player.set_curve(pre_x, pre_y, player.move_to, player.move_from);
            Pazzle_data[player.x, player.y].condition = Common.Condition.Player;
        }
        else if (con == Common.Condition.Moving) //のってたやつが動いてた
        {
            //PlayerPrefs.SetInt("Road" + Road_count, (int)reverse(player.move_to));
            UIs.Road_count++;
            player.set_curve(player.x, player.y, player.move_to, player.move_from);
        }
        else if(con==Common.Condition.Player)//塊が動いたら盤がなかった
        {
            int pre_x = player.pre_x;
            int pre_y = player.pre_y;
            player.pre_x = player.x;
            player.pre_y = player.y;
            //PlayerPrefs.SetInt("Road" + Road_count, (int)Field_direct);
            UIs.Road_count++;
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
                Pazzle_fields[i, j].Sprite().color = new Color(1, 1, 1, 2.5f - d_infty(Pazzle_fields[i,j].Pos, vec));
                move_fields[i, j].Sprite().color = new Color(1, 1, 1, 2.5f - d_infty(move_fields[i,j].Pos, vec));
            }
        }
        for (int i = 0; i < treasure.Length; i++) treasure[i].Sprite().color = new Color(1, 1, 1, 2f - d_infty(new Vector3(treasure[i].x, treasure[i].y, 0), vec)); 
        for (int i = 0; i < enemy.Length; i++)  if (enemy[i].act != Common.Action.Sad) enemy[i].Sprite().color = new Color(1, 1, 1, 6.3f - 4 * d_infty(enemy[i].Pos, vec));
    }

    public int Hole_search(int x,int y,int P) //どこにいるか入れたら穴の場所、p=0:x,p=1:y
    {
        x = L(x)* 3+1;
        y = L(y)*3+1;
        int ans = x;
        for(int i = x; i <x+ 3; i++)
        {
            for(int j = y; j < y+3; j++)
            {
                if (Pazzle_data[i,j].type == Common.Direction.None)
                {
                    if (P == 0) ans = i;
                    else ans = j;
                }
            }
        }
        return ans;
    }

    public void Pause_button_down()
    {
        pause_bool = !pause_bool;
        UIs.Pause_flg(pause_bool);
    }

    public void OutScreen() //move_fieldsを外にやる
    {
        for(int i = 0; i < 5; i++)
        {
            for (int j= 0; j < 5; j++)
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
        for(int i = 0; i < 4; i++)
        {
            arrows[i].transform.position = new Vector3(3*L(player.x)+2+1.5f*Mathf.Sin(i * Mathf.PI / 2f), 3 * L(player.y) + 2 + 1.5f*Mathf.Cos(i * Mathf.PI / 2f), 0);
        }
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
            UIs.Effect("Goal_Trigger");//きらきらとかつけるのかな？そのアニメで時間を取ろうとしてます。
            flg = 7;
            Vector3 vec = GameObject.Find("Main Camera").GetComponent<Camera>().WorldToScreenPoint(player.Pos + Dire_to_Vec(Field_direct) * 0.5f);
            GameObject.Find("Goal").GetComponent<RectTransform>().localPosition = vec-new Vector3(Screen.width,Screen.height)/2f;
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                UIs.Anime(i, Common.Action.Sad);
                PlayerPrefs.SetInt("result", 0);
            }
            UIs.Effect("Gameover" + goal + "_Trigger");//真っ暗とか？そのアニメで時間を取ろうとしてます。
            flg = 8;
        }
        UIs.timer_bool = false;
        UIs.bg_bool = false;
        //PlayerPrefs.SetInt("Road" + Road_count, (int)reverse(player.move_from));
        //PlayerPrefs.SetInt("Length", Road_count + 1);
        //PlayerPrefs.SetInt("Road" + (Road_count+1), 0);
        PlayerPrefs.SetInt("Time", Mathf.RoundToInt(UIs.get_Time()));
        PlayerPrefs.SetInt("Life", UIs.get_Life()+1);
        int[] get_treasure = { 0, 0 };
        for (int i = 0; i < treasure.Length; i++)
        {
            if (treasure[i].get&& (int)treasure[i].type<2) get_treasure[(int)treasure[i].type]++;
        }
        PlayerPrefs.SetInt("treasure0", get_treasure[0]);
        PlayerPrefs.SetInt("treasure1", get_treasure[1]);
        int Destroy = 0;
        for (int i = 0; i < enemy.Length; i++)
        {
            if (enemy[i].act==Common.Action.Sad) Destroy++;
        }
        PlayerPrefs.SetInt("enemy", Destroy);
        //PlayerPrefs.SetInt("Coin", coin);
    }
    
    public void change_Mount(Common.Direction entrance,int x,int y)//キャラの座標、0~8、x:左から,y:下から
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
                    move_fields[i, j].Set_img(Pazzle_data[3 * x + i, 3 * y + j].type);
                    move_fields[i, j].Layer(10 - 3 * y - j);
                    move_fields[i, j].Pos = new Vector3(3 * x + i, 3 * y + j);
                    move_fields[i, j].Sprite().color = new Color(1, 1, 1, 1);
                }
                else { move_fields[i, j].Set_img(Common.Direction.None); Debug.Log("pass "+ (3 * x + i) +"  " + (3 * y + j)); }
            }
        }
        Field_direct = entrance;
        //mount = X_or_Y;
        flg = 6;
    }

    public void Revival()
    {
        UIs.Effect("Retry_Trigger");
        UIs.Retry();
        flg = 1;
        player.Set_Chara(UIs.Top_ID());//UIsで変更
        GameObject.Find("Main Camera").transform.position = new Vector3(3 * L(player.x) + 2, 3 * L(player.y) + 2.8f, -10);
    }
    
    public void Map_color(float color_a)
    {
        for(int i = 0; i < treasure.Length; i++)
        {
            treasure[i].map_color(color_a);
        }
        for (int i = 0; i < enemy.Length; i++)
        {
            enemy[i].map_color(color_a);
        }
        player.map_color(color_a);
        GameObject.Find("Map_base").GetComponent<Image>().color = new Color(1, 1, 1, color_a);
        for(int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                GameObject.Find("Small_map" + i + "-" + j).GetComponent<Image>().color = new Color(1, 0.8f, 0, color_a);
            }
        }
    }

    #region Battle追加部分
    public void Battle_time_loss()
    {
        time_minus.GetComponent<Animator>().SetTrigger("LossTime");
        UIs.Lose_Time(30);
        //time_minus.GetComponent<Animator>().SetBool("loss", false);
    }
    #endregion

    #region 初期アニメーション終了後処理
    public void After_start_animation()
    {
        pause_bool = !pause_bool;
        //UIs.All_pause_flg(pause_bool);
        UIs.Pause_flg(false);//★向こうのpause_boolだけ逆なんです、、変えてもらってもいいですよ。region:背景の動画とregion:時間表示のところですね
    }
    #endregion
}
