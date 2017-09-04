using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Data_box
 * 場所についてのデータを入れていくつもりです。
 * 宝とか、敵とかもここに入れていこうかと思っています。
 * アタッチなし
 */

public class Data_box : MonoBehaviour
{
    public int x, y;//どこにいるか、0~8
    public Common.Direction type;//下からの道が、0:None,1:Straight,2:Right,3:Leftで場合分け
    public Common.Condition condition;
    public int treasure;

    public Data_box()
    {
        treasure = -1;
    }
    
    public void Set_address(int p,int q)//p:左からq:下から,0~8
    {
        x = p;
        y = q;
    }

    public Common.Direction Exit_direction(Common.Direction entrance)//どこから入ったらどこに出るか
    {
        Common.Direction exit = Common.Direction.None;
        if (type == Common.Direction.Straight)//まっすぐ
        {
            if (entrance == Common.Direction.Down) exit = Common.Direction.Up;
            else if (entrance == Common.Direction.Up) exit = Common.Direction.Down;
            else if (entrance == Common.Direction.Right) exit = Common.Direction.Left;
            else if (entrance == Common.Direction.Left) exit = Common.Direction.Right;
        }
        else if (type == Common.Direction.Right)//右
        {
            if (entrance == Common.Direction.Down) exit = Common.Direction.Right;
            else if (entrance == Common.Direction.Up) exit = Common.Direction.Left;
            else if (entrance == Common.Direction.Right) exit = Common.Direction.Down;
            else if (entrance == Common.Direction.Left) exit = Common.Direction.Up;
        }
        else if (type == Common.Direction.Left)//左
        {
            if (entrance == Common.Direction.Down) exit = Common.Direction.Left;
            else if (entrance == Common.Direction.Up) exit = Common.Direction.Right;
            else if (entrance == Common.Direction.Right) exit = Common.Direction.Up;
            else if (entrance == Common.Direction.Left) exit = Common.Direction.Down;
        }
        return exit;
    }

    public Common.Direction Curve_direction(Common.Direction entrance)//どこから入ったらどう曲がるか、（使わなければ消す）
    {
        Common.Direction curve = Common.Direction.None;
        if (type == Common.Direction.Straight)//まっすぐ
        {
            curve = Common.Direction.Straight;
        }
        else if (type == Common.Direction.Right)//右
        {
            if (entrance == Common.Direction.Down || entrance == Common.Direction.Up) curve = Common.Direction.Right;
            else if (entrance == Common.Direction.Right || entrance == Common.Direction.Left) curve = Common.Direction.Left;
        }
        else if (type == Common.Direction.Left)//左
        {
            if (entrance == Common.Direction.Down || entrance == Common.Direction.Up) curve = Common.Direction.Left;
            else if (entrance == Common.Direction.Right || entrance == Common.Direction.Left) curve = Common.Direction.Right;
        }
        return curve;
    }
    
    public void Copy(Data_box original)
    {
        Set_address(original.x, original.y);
        this.type = original.type;
        this.condition = original.condition;
        this.treasure = original.treasure;
    }
}
