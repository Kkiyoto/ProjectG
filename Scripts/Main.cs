using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public GameObject field_Prefab;
    Character player;
    //Character[] enemy;
    Vector3 tap_Start;
    int Move_X, Move_Y;
    Common.Direction Move_direct;

    bool flg = true;//Playerのとこ、止め方が分からないのでとりあえず止めるためのもの

    // Use this for initialization
    void Start ()
    {
        #region Pazzle_dataの設定と読み取り
        for(int i = 0; i < 9; i++)
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
                Pazzle_fields[i, j].Tra().position = new Vector3(i, j);
                o = Instantiate(field_Prefab) as GameObject;
                move_fields[i, j] = new Field(o);
                move_fields[i, j].Tra().position = new Vector3(5,0); //見えないところへ
            }
        }
        set_block(0, 1,1);
        #endregion
        #region playerの設定
        GameObject player_obj = GameObject.Find("Player");
        player = new Character(player_obj);
        player.x = 4;
        player.y = 3;
        player.set_position(4,3, Common.Direction.Down,Pazzle_data[4,3].Exit_direction(Common.Direction.Down));
        player.set_Speed(150f);
        Pazzle_data[4,3].condition = Common.Condition.Player;
        #endregion
        Move_X = 0;
        Move_Y = 0;
        Move_direct = Common.Direction.None;
	}
	
	// Update is called once per frame
	void Update ()
    {
        /* デバック用に置いてます
        for(int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                Debug.Log("i,j " + i + " " + j + "   t " + PazzleFields[i, j].type);
            }
        }*/
        if (flg&&player.Move())
        {
            player.pre_x = player.x;
            player.pre_y = player.y;
            player.x += (int)Dire_to_Vec(player.move_to).x;
            player.y += (int)Dire_to_Vec(player.move_to).y;//プレイヤーの動いた座標を更新 
            Pazzle_data[player.pre_x, player.pre_y].condition = Common.Condition.Normal;
            #region if (出ていった場合)
            if (player.pre_x%3 ==0&& player.move_to==Common.Direction.Left)
            {
                if (player.x > 0)
                {
                    change_block(Common.Direction.Right);
                }
                else flg = false;
            }
            else if (player.x %3 == 0 && player.move_to == Common.Direction.Right)
            {
                if (player.x <8)
                {
                    change_block(Common.Direction.Left);
                }
                else flg = false;
            }
            else if (player.pre_y %3 == 0 && player.move_to == Common.Direction.Down)
            {
                if (player.y > 0)
                {
                    change_block(Common.Direction.Up);
                }
                else flg = false;
            }
            else if (player.y %3 == 0 && player.move_to == Common.Direction.Up)
            {
                if (player.y <8)
                {
                    change_block(Common.Direction.Down);
                }
                else flg = false;
            }
            #endregion
            else
            {
                if (Pazzle_data[player.x , player.y].condition == Common.Condition.Hole || Pazzle_data[player.x , player.y ].condition == Common.Condition.Moving)
                {
                    Debug.Log("Out");
                    flg = false;
                }
                else
                {
                    player.set_curve(player.x, player.y, reverse(player.move_to), Pazzle_data[player.x, player.y].Exit_direction(reverse(player.move_to)));
                    Pazzle_data[player.x, player.y].condition = Common.Condition.Player;
                }
            }
        }
        #region 盤を動かす
        if (Move(Move_X, Move_Y, Move_direct, 6f)) //盤が動く、動いてる途中はfalse
        {
            if (Input.GetMouseButtonDown(0))
            {
                tap_Start = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            if (Input.GetMouseButtonUp(0) && Physics2D.OverlapPoint(tap_Start))
            {
                Vector3 tap_Tarminal = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 delta = tap_Tarminal - tap_Start;
                #region 横方向スライド
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y) && Mathf.Abs(delta.x) > 0.8f)
                {
                    int X = Pazzle_fields[Mathf.RoundToInt(tap_Start.x), Mathf.RoundToInt(tap_Start.y)].data_x;
                    int Y = Pazzle_fields[Mathf.RoundToInt(tap_Start.x), Mathf.RoundToInt(tap_Start.y)].data_y; //タッチしたものの座標,0~8
                    if (Pazzle_data[X, Y].condition == Common.Condition.Normal)
                    {
                        if (delta.x > 0 && X%3 < 2 && Pazzle_data[X + 1, Y].condition == Common.Condition.Hole)
                        {
                            Move_X = X;
                            Move_Y = Y;
                            Move_direct = Common.Direction.Right;
                            Pazzle_data[X, Y].condition = Common.Condition.Moving;
                            Pazzle_data[X + 1, Y].condition = Common.Condition.Moving;
                        }
                        else if (delta.x < 0 && X%3 > 0 && Pazzle_data[X - 1, Y].condition == Common.Condition.Hole)
                        {
                            Move_X = X;
                            Move_Y = Y;
                            Move_direct = Common.Direction.Left;
                            Pazzle_data[X, Y].condition = Common.Condition.Moving;
                            Pazzle_data[X - 1, Y].condition = Common.Condition.Moving;
                        }
                    }
                }
                #endregion
                #region 縦方向スライド 
                else if (Mathf.Abs(delta.x) < Mathf.Abs(delta.y) && Mathf.Abs(delta.y) > 0.8f)
                {
                    int X = Pazzle_fields[Mathf.RoundToInt(tap_Start.x), Mathf.RoundToInt(tap_Start.y)].data_x;
                    int Y = Pazzle_fields[Mathf.RoundToInt(tap_Start.x), Mathf.RoundToInt(tap_Start.y)].data_y; //タッチしたものの座標,0~8
                    if (Pazzle_data[X, Y].condition == Common.Condition.Normal)
                    {
                        if (delta.y > 0 && Y%3 < 2 && Pazzle_data[X, Y + 1].condition == Common.Condition.Hole)
                        {
                            Move_X = X;
                            Move_Y = Y;
                            Move_direct = Common.Direction.Up;
                            Pazzle_data[X, Y].condition = Common.Condition.Moving;
                            Pazzle_data[X, Y + 1].condition = Common.Condition.Moving;
                        }
                        else if (delta.y < 0 && Y%3 > 0 && Pazzle_data[X, Y - 1].condition == Common.Condition.Hole)
                        {
                            Move_X = X;
                            Move_Y = Y;
                            Move_direct = Common.Direction.Down;
                            Pazzle_data[X, Y].condition = Common.Condition.Moving;
                            Pazzle_data[X, Y - 1].condition = Common.Condition.Moving;
                        }
                    }
                    #endregion
                }
            }
        }
        #endregion
    }

    public bool Move(int x, int y, Common.Direction direct, float speed)//動かしたいものの座標、0~8、x:左から,y:下から、speed小さい方が早い
    {
        Vector3 pos = Pazzle_fields[x%3,y%3].Tra().position;
        if(direct == Common.Direction.None)
        {
            return true;
        }
        else if ((pos - Dire_to_Vec(direct) - new Vector3(x%3,y%3, 0)).magnitude > 0.05f)
        {
            Pazzle_fields[x % 3, y % 3].Tra().position = (pos * speed + Dire_to_Vec(direct) + new Vector3(x % 3, y % 3, 0)) / (1 + speed);
            return false;
        }
        else  //動き終わり、データ交換
        {
            //int x = Pazzle_fields[X, Y].data_x;
            //int y = Pazzle_fields[X, Y].data_y;//Pazzle_data用に0~8に変換 
            int p = x + (int)Dire_to_Vec(direct).x;
            int q = y + (int)Dire_to_Vec(direct).y;//元々の穴の位置(0~8)
            Pazzle_data[x,y].Set_address(p, q);
            Pazzle_data[p, q].Set_address(x, y);
            Data_box Tmp = Pazzle_data[p, q];
            Pazzle_data[p, q] = Pazzle_data[x, y];
            Pazzle_data[x, y] = Tmp; //Data_box交換
            Move_direct = Common.Direction.None;
            Pazzle_data[x, y].condition = Common.Condition.Hole;
            Pazzle_data[p, q].condition = Common.Condition.Normal;
            Pazzle_fields[x % 3, y % 3].Tra().position = new Vector3(x % 3, y % 3, 0);
            Pazzle_fields[p % 3, q % 3].Tra().position = new Vector3(p % 3, q % 3, 0);
            Pazzle_fields[x % 3, y % 3].Set_img(Pazzle_data[x, y].type);
            Pazzle_fields[p%3, q%3].Set_img(Pazzle_data[p, q].type);
            return true;
        }
    }

    /* パッと動く方、キャラ動かせたからコメントアウト（データ交換保存のため残してます、後で消す）
     * 
    public bool Move(int X, int Y, Common.Direction direct, float speed)//動かしたいものの座標、0~2、x:左から,y:下から、speed小さい方が早い
    {
        if (direct != Common.Direction.None)
        {
            int x = Pazzle_fields[X, Y].data.x;
            int y = Pazzle_fields[X, Y].data.y;//Pazzle_data用に0~8に変換 
            int p = x + (int)Dire_to_Vec(direct).x;
            int q = y + (int)Dire_to_Vec(direct).y;//元々の穴の位置(0~8)
            Pazzle_fields[X, Y].data.Set_address(p, q);
            p %= 3;
            q %= 3; //0~2に変換
            Pazzle_fields[p, q].data.Set_address(x, y);
            Pazzle_fields[X, Y].Tra().position = new Vector3(p, q, 0);
            Pazzle_fields[p, q].Tra().position = new Vector3(X, Y, 0);
            Field Tmp = Pazzle_fields[p, q];
            Pazzle_fields[p, q] = Pazzle_fields[X, Y];
            Pazzle_fields[X, Y] = Tmp; //Field交換
            Move_direct = Common.Direction.None;
        }
        return true;
    }
    */

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
                }
            }
        }
    }

    public void change_block(Common.Direction entrance)//キャラの座標、0~8、x:左から,y:下から
    {
        int Block_x = Mathf.FloorToInt(player.pre_x / 3);
        int Block_y = Mathf.FloorToInt(player.pre_y / 3);
        //ここにmove_fieldをつかったエフェクト
        /* for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Pazzle_data[3 * Block_x+i, 3 * Block_y+j].Copy(Pazzle_fields[i, j].data);
            }
        }*/
        set_block(0, L(player.x), L(player.y));
        player.set_position(player.x, player.y, entrance, Pazzle_data[player.x, player.y].Exit_direction(entrance));
        if (Pazzle_data[player.x, player.y].condition == Common.Condition.Hole)
        {
            Debug.Log("Out");
            flg = false;
        }
        else Pazzle_data[player.x, player.y].condition = Common.Condition.Player;
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

    #endregion
}
