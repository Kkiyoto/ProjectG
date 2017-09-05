using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/* Main
 * Directorオブジェクトにアタッチ
 * Directorはからのオブジェクト
 * 主にゲーム動かす部分にしています。
 */

public class Main : MonoBehaviour
{
    Data_box [,] Pazzle_data = new Data_box[9,9];//[左から,下から]の順、下から見て0:None,1:Straight,2:Right,3:Left
    Field[,] Pazzle_fields = new Field[3, 3]; //触る方
    Field[,] move_fields = new Field[3, 3]; //動くため
    public GameObject field_Prefab,enemy_Prefab,treasure_Prefab;
    Character player;
    Character[] enemy;
    Vector3 tap_Start;
    int Move_X, Move_Y,touch_ID;
    Common.Direction Move_direct,Field_direct;
    Common.Condition Move_condition;
    Item[] treasure;

    bool pause_bool = false; //ポーズボタン、止め方が分からないのでとりあえず止めるためのもの
    int flg = 1;//Playerのとこ、止め方が分からないのでとりあえず止めるためのもの、0:ポーズ、1:動く,2:盤変更
    //ここのやり方が分からなかったので真偽地で無理矢理止めています。時間があればいい感じに変えてください。

    // Use this for initialization
    void Start ()
    {
        #region Pazzle_dataの設定と読み取り
        for (int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                Pazzle_data[i, j] = new Data_box();
            }
        }
        Pazzle_data_set();
        for (int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                GameObject o = Instantiate(field_Prefab) as GameObject;
                Pazzle_fields[i, j] = new Field(o);
                o = Instantiate(field_Prefab) as GameObject;
                move_fields[i, j] = new Field(o);
                move_fields[i, j].Pos=new Vector3(-5,0); //見えないところへ
            }
        }
        set_block(0, 1,1);
        #endregion
        #region playerの設定
        GameObject player_obj = GameObject.Find("Player");
        player = new Character(player_obj,Common.Type.Player);
        player.x = 4;
        player.y = 3;
        player.set_position(4,3, Common.Direction.Down,Pazzle_data[4,3].Exit_direction(Common.Direction.Down));
        player.set_Speed(150f);
        Pazzle_data[4,3].condition = Common.Condition.Player;
        #endregion
        #region 宝物の設定
        treasure = new Item[3]; //ここで宝の数
        for (int i = 0; i < treasure.Length; i++)
        {
            GameObject Treasure = Instantiate(treasure_Prefab) as GameObject;
            int[] ran = Random_position();
            Pazzle_data[ran[0],ran[1]].treasure = i;
            treasure[i] = new Item(ran[0], ran[1], Treasure);
        }
        #endregion
        #region enemyの設定
        enemy = new Character[3];//ここで敵の数
        Common.Type[] types = { Common.Type.Walk, Common.Type.Stop, Common.Type.Fly };
        for (int i = 0; i < enemy.Length; i++)
        {
            GameObject enemy_obj = Instantiate(enemy_Prefab) as GameObject;
            //int type_num = Random.RandomRange(0, 3);
            enemy[i] = new Character(enemy_obj,types[i]); //typesをランダム化
            Common.Direction dire = Common.Direction.None;
            if (types[i] != Common.Type.Stop) dire = Random_direct();
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
            enemy[i].set_Speed(130f);
        }
        #endregion
        Move_X = 0;
        Move_Y = 0;
        Move_direct = Common.Direction.None;
        GameObject camera = GameObject.Find("Main Camera");
        camera.transform.position = new Vector3(3*L(player.x) + 1, 3*L(player.y) + 1.8f, -10);
    }

    // Update is called once per frame
    void Update()
    {
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
                    #region 敵の動き
                    for (int i = 0; i < enemy.Length; i++)
                    {
                        if (enemy[i].act != Common.Action.Sad)
                        {
                            Vector3 v = new Vector3(enemy[i].x, enemy[i].y, 0);
                            if (enemy[i].type != Common.Type.Fly && L(player.x) == L(enemy[i].x) && L(player.y) == L(enemy[i].y)) v = Pazzle_fields[enemy[i].x % 3, enemy[i].y % 3].Pos;
                            if (enemy[i].act != Common.Action.Sad && enemy[i].Move(v))
                            {
                                enemy[i].pre_x = enemy[i].x;
                                enemy[i].pre_y = enemy[i].y;
                                enemy[i].x += (int)Dire_to_Vec(enemy[i].move_to).x;
                                enemy[i].y += (int)Dire_to_Vec(enemy[i].move_to).y;//敵の動いた座標を更新 
                                #region 引き返し
                                if (enemy[i].x < 0 || enemy[i].x > 8 || enemy[i].y < 0 || enemy[i].y > 8)
                                {
                                    enemy[i].x = enemy[i].pre_x;
                                    enemy[i].y = enemy[i].pre_y;
                                    enemy[i].set_curve(enemy[i].x, enemy[i].y, enemy[i].move_to, Pazzle_data[enemy[i].x, enemy[i].y].Exit_direction(enemy[i].move_to));
                                }
                                else if ((Pazzle_data[enemy[i].x, enemy[i].y].condition == Common.Condition.Hole || Pazzle_data[enemy[i].x, enemy[i].y].condition == Common.Condition.Moving) && enemy[i].type != Common.Type.Fly)
                                {
                                    enemy[i].x = enemy[i].pre_x;
                                    enemy[i].y = enemy[i].pre_y;
                                    enemy[i].set_curve(enemy[i].x, enemy[i].y, enemy[i].move_to, Pazzle_data[enemy[i].x, enemy[i].y].Exit_direction(enemy[i].move_to));
                                }
                                #endregion
                                else
                                {
                                    enemy[i].set_curve(enemy[i].x, enemy[i].y, reverse(enemy[i].move_to), Get_exit(enemy[i], (reverse(enemy[i].move_to))));
                                }
                            }
                            if (Pazzle_data[enemy[i].x,enemy[i].y].condition == Common.Condition.Player)  //ここに当たった時の
                            {
                                touch_ID = i;
                                flg = 3;
                            }
                        }
                    }
                    #endregion
                    #region プレイヤーの動き
                    if (player.Move(Pazzle_fields[player.x % 3, player.y % 3].Pos)) //動き終えたらtrue
                    {
                        player.pre_x = player.x;
                        player.pre_y = player.y;
                        player.x += (int)Dire_to_Vec(player.move_to).x;
                        player.y += (int)Dire_to_Vec(player.move_to).y;//プレイヤーの動いた座標を更新 
                        if (Pazzle_data[player.pre_x, player.pre_y].condition == Common.Condition.Moving)
                        {
                            Debug.Log("Out");
                            flg = 0;
                        }
                        else
                        {
                            Pazzle_data[player.pre_x, player.pre_y].condition = Common.Condition.Normal;
                            #region if (出ていった場合)
                            if (player.pre_x % 3 == 0 && player.move_to == Common.Direction.Left)
                            {
                                if (player.x > 0)
                                {
                                    change_block(Common.Direction.Right);
                                }
                                else flg = 0;
                            }
                            else if (player.x % 3 == 0 && player.move_to == Common.Direction.Right)
                            {
                                if (player.x < 8)
                                {
                                    change_block(Common.Direction.Left);
                                }
                                else flg = 0;
                            }
                            else if (player.pre_y % 3 == 0 && player.move_to == Common.Direction.Down)
                            {
                                if (player.y > 0)
                                {
                                    change_block(Common.Direction.Up);
                                }
                                else flg = 0;
                            }
                            else if (player.y % 3 == 0 && player.move_to == Common.Direction.Up)
                            {
                                if (player.y < 8)
                                {
                                    change_block(Common.Direction.Down);
                                }
                                else flg = 0;
                            }
                            #endregion
                            else
                            {
                                if (Pazzle_data[player.x, player.y].condition == Common.Condition.Hole || Pazzle_data[player.x, player.y].condition == Common.Condition.Moving)
                                {
                                    Debug.Log("Out");
                                    flg = 0;
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
                    if (Move(Move_X, Move_Y, Move_direct, 4f)) //盤が動く、動いてる途中はfalse
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            tap_Start = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        }
                        if (Input.GetMouseButtonUp(0))
                        {
                            Vector3 tap_Tarminal = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                            Vector3 delta = tap_Tarminal - tap_Start;
                            #region 横方向スライド
                            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y) && Mathf.Abs(delta.x) > 0.8f)
                            {
                                int X = Hole_search(player.x, player.y, 0);
                                int Y = Hole_search(player.x, player.y, 1); //穴の座標,0~8
                                if (delta.x > 0 && X % 3 > 0 && Pazzle_data[X, Y].condition == Common.Condition.Hole)
                                {
                                    Move_X = X - 1;
                                    Move_Y = Y;
                                    Move_direct = Common.Direction.Right;
                                    Move_condition = Pazzle_data[X - 1, Y].condition;
                                    Pazzle_data[X, Y].condition = Common.Condition.Moving;
                                    Pazzle_data[X - 1, Y].condition = Common.Condition.Moving;
                                }
                                else if (delta.x < 0 && X % 3 < 2 && Pazzle_data[X, Y].condition == Common.Condition.Hole)
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
                            else if (Mathf.Abs(delta.x) < Mathf.Abs(delta.y) && Mathf.Abs(delta.y) > 0.8f)
                            {
                                int X = Hole_search(player.x, player.y, 0);
                                int Y = Hole_search(player.x, player.y, 1); //穴の座標,0~8
                                if (delta.y > 0 && Y % 3 > 0 && Pazzle_data[X, Y].condition == Common.Condition.Hole)
                                {
                                    Move_X = X;
                                    Move_Y = Y - 1;
                                    Move_direct = Common.Direction.Up;
                                    Move_condition = Pazzle_data[X, Y - 1].condition;
                                    Pazzle_data[X, Y].condition = Common.Condition.Moving;
                                    Pazzle_data[X, Y - 1].condition = Common.Condition.Moving;
                                }
                                else if (delta.y < 0 && Y % 3 < 2 && Pazzle_data[X, Y].condition == Common.Condition.Hole)
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
                        }
                    }
                    #endregion
                    Set_color(new Vector3(Pazzle_fields[1,1].data_x,Pazzle_fields[1,1].data_y));//敵とか透明にする
                    break;
                case 2: //盤変更
                    GameObject camera = GameObject.Find("Main Camera");
                    camera.transform.position -= Dire_to_Vec(Field_direct) /50f;
                    Set_color(camera.transform.position - new Vector3(0, 0.8f, 0));
                    #region 動き終わった後
                    if ((camera.transform.position - new Vector3(3 * L(player.x) + 1, 3 * L(player.y) + 1.8f, -10)).magnitude < 0.005f)
                    {
                        player.set_position(player.x, player.y, Field_direct, Pazzle_data[player.x, player.y].Exit_direction(Field_direct));
                        OutScreen();
                        if (Pazzle_data[player.x, player.y].condition == Common.Condition.Hole)
                        {
                            Debug.Log("Out");
                            flg = 0;
                        }
                        else
                        {
                            Pazzle_data[player.x, player.y].condition = Common.Condition.Player;
                            Field_direct = Common.Direction.None;
                            flg = 1;
                        }
                    }
                    #endregion
                    break;
                case 3: //バトル
                    if (Input.GetMouseButtonUp(0)) //今はタッチしたら終了
                    {
                        enemy[touch_ID].act = Common.Action.Sad;
                        enemy[touch_ID].Sprite().color = Color.clear;
                        flg = 1;
                    }
                    break;
                case 4: //宝ゲット
                    if (Input.GetMouseButtonUp(0)) //今はタッチしたら終了
                    {
                        treasure[touch_ID].Get_Item();
                        flg = 1;
                    }
                    break;
            }
            #region 宝の場所更新
            for (int i = 0; i < treasure.Length; i++)
            {
                if (L(player.x) == L(treasure[i].x) && L(player.y) == L(treasure[i].y)) treasure[i].Pos = Pazzle_fields[treasure[i].x % 3, treasure[i].y % 3].Pos;
                else treasure[i].Pos = new Vector3(treasure[i].x, treasure[i].y, 0);
                if (!treasure[i].get && Pazzle_data[treasure[i].x, treasure[i].y].condition == Common.Condition.Player)  //ここに当たった時の
                {
                    touch_ID = i;
                    flg = 4;
                }
            }
            #endregion
        }
    }

    public bool Move(int x, int y, Common.Direction direct, float speed)//動かしたいものの座標、0~8、x:左から,y:下から、speed小さい方が早い
    {
        if (x== -1)
        {
            return true;
        }
        else
        {
            Vector3 pos = Pazzle_fields[x % 3, y % 3].Pos;
            if (direct == Common.Direction.None)
            {
                return true;
            }
            else if ((pos - Dire_to_Vec(direct) - new Vector3(x, y, 0)).magnitude > 0.03f)
            {
                Pazzle_fields[x % 3, y % 3].Pos = (pos * speed + Dire_to_Vec(direct) + new Vector3(x, y, 0)) / (1 + speed);
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
                Move_direct = Common.Direction.None;
                Pazzle_data[x, y].condition = Common.Condition.Hole;
                Pazzle_data[p, q].condition = Move_condition;
                Pazzle_fields[x % 3, y % 3].Pos = new Vector3(x, y, 0);
                Pazzle_fields[p % 3, q % 3].Pos = new Vector3(p, q, 0);
                Pazzle_fields[x % 3, y % 3].Set_img(Pazzle_data[x, y].type);
                Pazzle_fields[p % 3, q % 3].Set_img(Pazzle_data[p, q].type);
                Pazzle_fields[p % 3, q % 3].Layer(8 - q);
                Move_X = -1; //バグ修正のため
                if (Move_condition == Common.Condition.Player)
                {
                    player.x = p;
                    player.y = q;
                }
                for (int i = 0; i < 3; i++)//敵交換
                {
                    if (enemy[i].type != Common.Type.Fly && enemy[i].x == x && enemy[i].y == y)
                    {
                        enemy[i].pre_x = enemy[i].x;
                        enemy[i].pre_y = enemy[i].y;
                        enemy[i].x = p;
                        enemy[i].y = q;
                    }
                }
                for (int i = 0; i < 3; i++)//宝交換
                {
                    if (treasure[i].x == x && treasure[i].y == y)
                    {
                        treasure[i].x = p;
                        treasure[i].y = q;
                    }
                }
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

    public Common.Direction Random_direct() //方向のランダム関数
    {
        int ran = Random.Range(2, 6);
        if (ran == 2) return Common.Direction.Right;
        else if (ran == 3) return Common.Direction.Left;
        else if (ran == 4) return Common.Direction.Up;
        else return Common.Direction.Down;
    }

    public int[] Random_position() //座標のランダム関数
    {
        int[] pos = new int[2];
        pos[0] = Random.Range(0, 9);
        pos[1] = Random.Range(0, 9);
        while (Pazzle_data[pos[0],pos[1]].type == Common.Direction.None || Pazzle_data[pos[0], pos[1]].condition == Common.Condition.Player || Pazzle_data[pos[0], pos[1]].treasure != -1)
        {
            pos[0] = Random.Range(0, 9);
            pos[1] = Random.Range(0, 9);
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
                Pazzle_data[3 * x + i, 3 * y +j].type=tmp[i+3*j];
                Pazzle_data[3 * x + i, 3 * y + j].Set_address(3 * x + i, 3 * y + j);
                Pazzle_data[3 * x + i, 3 * y + j].condition = Common.Condition.Normal;
            }
        }
        #region 穴とか宝とか特殊なもの
        Pazzle_data[3 * x + hole % 3, 3 * y + Mathf.FloorToInt(hole / 3)].condition = Common.Condition.Hole;
        #endregion
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
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Pazzle_fields[i, j].data_x=3 * x + i;
                    Pazzle_fields[i, j].data_y=3 * y + j;
                    Pazzle_fields[i, j].Set_img(Pazzle_data[3*x+i,3*y+j].type);
                    Pazzle_fields[i, j].Layer(8-3 * y - j);
                    Pazzle_fields[i, j].Pos = new Vector3(3 * x + i, 3 * y + j);
                }
            }
        }
        else if (ID == 1)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    move_fields[i, j].data_x = 3 * x + i;
                    move_fields[i, j].data_y = 3 * y + j;
                    move_fields[i, j].Set_img(Pazzle_data[3 * x + i, 3 * y + j].type);
                    move_fields[i, j].Layer(8 - 3 * y - j);
                    move_fields[i, j].Pos = new Vector3(3 * x + i, 3 * y + j);
                }
            }
        }
    }

    public void change_block(Common.Direction entrance)//キャラの座標、0~8、x:左から,y:下から
    {
        //ここにmove_fieldをつかったエフェクト
        set_block(1, L(player.pre_x), L(player.pre_y));
        set_block(0, L(player.x), L(player.y));
        Set_color(new Vector3(3 * L(player.pre_x) + 1, 3 * L(player.pre_y) + 1, 0));
        Field_direct = entrance;
        Move_X = -1;//バグ修正のため
        flg = 2;
    }

    public void Set_color(Vector3 vec) //透明化
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Pazzle_fields[i, j].Sprite().color = new Color(1, 1, 1, 2f - d_infty(Pazzle_fields[i, j].Pos, vec));
                move_fields[i, j].Sprite().color = new Color(1, 1, 1, 2f - d_infty(move_fields[i, j].Pos, vec));
                //Pazzle_fields[i, j].Sprite().color = new Color(1, 1, 1, 1);
                //move_fields[i, j].Sprite().color = new Color(1, 1, 1, 1);
            }
        }
        for (int i = 0; i < treasure.Length; i++) treasure[i].Sprite().color = new Color(1, 1, 1, 2f - d_infty(new Vector3(treasure[i].x, treasure[i].y, 0), vec)); 
        for (int i = 0; i < enemy.Length; i++)  if (enemy[i].act != Common.Action.Sad) enemy[i].Sprite().color = new Color(1, 1, 1, 6.3f - 4 * d_infty(enemy[i].Pos, vec));
    }

    public int Hole_search(int x,int y,int P) //どこにいるか入れたら穴の場所、p=0:x,p=1:y
    {
        x = L(x)* 3;
        y = L(y)*3;
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
        GameObject.Find("State_manager").GetComponent<State_manage>().Pause_flg(pause_bool);
    }

    public void OutScreen() //move_fieldsを外にやる
    {
        for(int i = 0; i < 3; i++)
        {
            for (int j= 0; j < 3; j++)
            {
                move_fields[i, j].Pos = new Vector3(-5, 0, 0);
            }
        }
    }

    #region 関数群、全部のクラスに付けたいけどつけ方が分かりません
    public Vector3 Dire_to_Vec(Common.Direction d)//ベクトル化
    {
        if (d == Common.Direction.Straight || d == Common.Direction.Up) return new Vector3(0, 1, 0);
        else if (d == Common.Direction.Down) return new Vector3(0, -1, 0);
        else if (d == Common.Direction.Right) return new Vector3(1, 0, 0);
        else if (d == Common.Direction.Left) return new Vector3(-1, 0, 0);
        else return new Vector3(0, 0, 0);
    }

    public Common.Direction reverse(Common.Direction direct)//逆向き化
    {
        if (direct == Common.Direction.Down) return Common.Direction.Up;
        else if (direct == Common.Direction.Up) return Common.Direction.Down;
        else if (direct == Common.Direction.Right) return Common.Direction.Left;
        else if (direct == Common.Direction.Left) return Common.Direction.Right;
        else return Common.Direction.None;
    }

    public int L(int small) //0~8を0~2にする(どこの塊)
    {
        if (small < 3) return 0;
        else if (small < 6) return 1;
        else return 2;
    }

    public float d_infty(Vector3 vec_1,Vector3 vec_2)
    {
        float d1 = Mathf.Abs(vec_1.x - vec_2.x);
        float d2 = Mathf.Abs(vec_1.y - vec_2.y);
        return Mathf.Max(d1, d2);
    }

    #endregion
}
