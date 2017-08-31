using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    GameObject obj;
    public int x, y;//どこにいるか、0~8
    Sprite[] sprites = new Sprite[4]; //下からの道が、0:None,1:Straight,2:Right,3:Left
    public int type; //下からの道が、0:None,1:Straight,2:Right,3:Leftで場合分け

    public Field(GameObject o)
    {
        obj = o;
        for (int i = 0; i < 4; i++)
        {
            sprites[i] = Resources.Load<Sprite>("Images/GameScene/Road"+i);
        }
    }

    public void Set(Common.Direction FromDown)//形を入れたらデータを作る
    {
        type = 0;
        if (FromDown == Common.Direction.Straight) type = 1;
        else if (FromDown == Common.Direction.Right) type = 2;
        else if (FromDown == Common.Direction.Left) type = 3;
        obj.GetComponent<SpriteRenderer>().sprite = sprites[type];
    }

    public Common.Direction ExitDirection(Common.Direction entrance)//どこから入ったらどこに出るか
    {
        Common.Direction exit=Common.Direction.None;
        if (type == 1)//まっすぐ
        {
            if (entrance == Common.Direction.Down) exit = Common.Direction.Up;
            else if (entrance == Common.Direction.Up) exit = Common.Direction.Down;
            else if (entrance == Common.Direction.Right) exit = Common.Direction.Left;
            else if (entrance == Common.Direction.Left) exit = Common.Direction.Right;
        }
        else if (type == 2)//右
        {
            if (entrance == Common.Direction.Down) exit = Common.Direction.Right;
            else if (entrance == Common.Direction.Up) exit = Common.Direction.Left;
            else if (entrance == Common.Direction.Right) exit = Common.Direction.Down;
            else if (entrance == Common.Direction.Left) exit = Common.Direction.Up;
        }
        else if (type == 3)//左
        {
            if (entrance == Common.Direction.Down) exit = Common.Direction.Left;
            else if (entrance == Common.Direction.Up) exit = Common.Direction.Right;
            else if (entrance == Common.Direction.Right) exit = Common.Direction.Up;
            else if (entrance == Common.Direction.Left) exit = Common.Direction.Down;
        }
        return exit;
    }


    public Common.Direction CurveDirection(Common.Direction entrance)//どこから入ったらどう曲がるか
    {
        Common.Direction curve = Common.Direction.None;
        if (type == 1)//まっすぐ
        {
            curve = Common.Direction.Straight;
        }
        else if (type == 2)//右
        {
            if (entrance == Common.Direction.Down || entrance == Common.Direction.Up) curve = Common.Direction.Right;
            else if (entrance == Common.Direction.Right || entrance == Common.Direction.Left) curve = Common.Direction.Left;
        }
        else if (type == 3)//右
        {
            if (entrance == Common.Direction.Down || entrance == Common.Direction.Up) curve = Common.Direction.Left;
            else if (entrance == Common.Direction.Right || entrance == Common.Direction.Left) curve = Common.Direction.Right;
        }
        return curve;
    }

    public Transform Tra()
    {
        return obj.transform;
    }
}
