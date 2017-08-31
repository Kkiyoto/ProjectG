using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    Common.Direction[,] PazzleData = new Common.Direction[9,9];//[左から,下から]の順、下から見て0:None,1:Straight,2:Right,3:Left
    Field[,] PazzleFields = new Field[3, 3]; //触る方
    Field[,] MoveFields = new Field[3, 3]; //動くため
    public GameObject FieldPrefab;
    Character player;
    //Character[] enemy;
    Vector3 tapStart;
    int MoveX, MoveY;
    Common.Direction MoveDirect;

    // Use this for initialization
    void Start ()
    {
        PazzleDataSet();
        for (int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                GameObject o = Instantiate(FieldPrefab) as GameObject;
                PazzleFields[i, j] = new Field(o);
                PazzleFields[i, j].Tra().position = new Vector3(i, j);
                o = Instantiate(FieldPrefab) as GameObject;
                MoveFields[i, j] = new Field(o);
                MoveFields[i, j].Tra().position = new Vector3(5,0);
            }
        }
        SetBlock(0, 1,1);
        MoveX = 0;
        MoveY = 0;
        MoveDirect = Common.Direction.None;
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

        if (Move(MoveX, MoveY, MoveDirect, 8f))
        {
            if (Input.GetMouseButtonDown(0))
            {
                tapStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            if (Input.GetMouseButtonUp(0)&& Physics2D.OverlapPoint(tapStart))
            {
                Vector3 tapTarminal = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 delta = tapTarminal - tapStart;
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y) && Mathf.Abs(delta.x) > 0.8f)
                {
                    int X = Mathf.RoundToInt(tapStart.x);
                    int Y = Mathf.RoundToInt(tapStart.y); //タッチしたものの座標,0~2
                    if (delta.x > 0 &&X<2&& PazzleFields[X + 1, Y].type == 0)
                    {
                        MoveX = X;
                        MoveY = Y;
                        MoveDirect = Common.Direction.Right;
                    }
                    else if (delta.x < 0 &&X>0&& PazzleFields[X - 1, Y].type == 0)
                    {
                        MoveX = X;
                        MoveY = Y;
                        MoveDirect = Common.Direction.Left;
                    }
                }
                else if (Mathf.Abs(delta.x) < Mathf.Abs(delta.y) && Mathf.Abs(delta.y) > 0.8f)
                {
                    int X = Mathf.RoundToInt(tapStart.x);
                    int Y = Mathf.RoundToInt(tapStart.y); //タッチしたものの座標,0~2
                    if (delta.y > 0 &&Y<2&& PazzleFields[X, Y + 1].type == 0)
                    {
                        MoveX = X;
                        MoveY = Y;
                        MoveDirect = Common.Direction.Up;
                    }
                    else if (delta.y < 0 &&Y>0&& PazzleFields[X, Y - 1].type == 0)
                    {
                        MoveX = X;
                        MoveY = Y;
                        MoveDirect = Common.Direction.Down;
                    }
                }
            }
        }
	}

    public bool Move(int X, int Y, Common.Direction direct, float speed)//動かしたいものの座標、0~2、x:左から,y:下から、speed小さい方が早い
    {
        Vector3 pos = PazzleFields[X, Y].Tra().position;
        if(direct == Common.Direction.None)
        {
            return true;
        }
        else if ((pos - DireToVec(direct) - new Vector3(X, Y, 0)).magnitude > 0.05f)
        {
            PazzleFields[X, Y].Tra().position = (pos * speed + DireToVec(direct) + new Vector3(X, Y, 0)) / (1 + speed);
            return false;
        }
        else  //動き終わり、データ交換
        {
            int x = PazzleFields[X, Y].x;
            int y = PazzleFields[X, Y].y;//PazzleData用に0~8に変換 
            int p = x + (int)DireToVec(direct).x;
            int q = y + (int)DireToVec(direct).y;//元々の穴の位置(0~8)
            Common.Direction tmp = PazzleData[p, q];
            PazzleData[p, q] = PazzleData[x, y];
            PazzleData[x, y] = tmp; //Data交換
            PazzleFields[X, Y].x = p;PazzleFields[X, Y].y = q;
            p %= 3;
            q %= 3; //0~2に変換
            PazzleFields[p, q].x = x; PazzleFields[p, q].y = y;
            PazzleFields[X, Y].Tra().position = new Vector3(p, q, 0);
            PazzleFields[p, q].Tra().position = new Vector3(X, Y, 0);
            Field Tmp = PazzleFields[p, q];
            PazzleFields[p, q] = PazzleFields[X, Y];
            PazzleFields[X, Y] = Tmp; //Field交換
            MoveDirect = Common.Direction.None;
            return true;
        }
    }

    public void BlockDataSet(int x, int y) //塊としての座標、0~2、x:左から,y:下から
    {
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
        float[] random = new float[8];//ここからシャッフル
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
        }//ここまでシャッフル
        int hole = 4;//ここが穴になる、後でランダム化
        for (int i = 0; i < hole; i++) tmp[i] = tmp[i + 1];
        tmp[hole] = Common.Direction.None;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                PazzleData[3 * x + i, 3 * y +j] = tmp[i+3*j];
            }
        }
    }

    public void PazzleDataSet()
    {
        for(int i = 0; i < 3; i++)
        {
            for(int j = 0; j < 3; j++)
            {
                BlockDataSet(i, j);
            }
        }
    }

    public void SetBlock(int ID,int x, int y) //塊としての座標、0~2、x:左から,y:下から,ID;0:PazzleField,1:MoveField
    {
        if (ID == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    PazzleFields[i, j].Set(PazzleData[3 * x + i, 3 * y + j]);
                    PazzleFields[i, j].x= 3 * x + i;
                    PazzleFields[i, j].y = 3 * y + j;
                }
            }
        }
        else if (ID == 1)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    MoveFields[i, j].Set(PazzleData[3 * x + i, 3 * y + j]);
                }
            }
        }
    }


    public Vector3 DireToVec(Common.Direction d)
    {
        if (d == Common.Direction.Straight || d == Common.Direction.Up) return new Vector3(0, 1, 0);
        else if (d == Common.Direction.Down) return new Vector3(0, -1, 0);
        else if (d == Common.Direction.Right) return new Vector3(1, 0, 0);
        else if (d == Common.Direction.Left) return new Vector3(-1, 0, 0);
        else return new Vector3(0, 0, 0);
    }
}
