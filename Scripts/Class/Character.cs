using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Charactor
 * GameObjectの動き回る部分で、プレイヤー、敵キャラ両方に使うクラス
 * キャラ部分のGameObjectのかわりに使う。
 * objの部分に実態を入れる。アタッチなし
 */

public class Character : MonoBehaviour
{
    public int x, y;//x:左から,y:下から。0~8
    public int pre_x, pre_y;//1つ前の座標、0~8
    float speed;
    GameObject obj;//chara:画像を持つもの,arrow:方向を持つもの
    Sprite img;
    int count;
    public Common.Direction move_from,move_to;

    public Character(GameObject Image_obj)
    {
        obj = Image_obj;
    }
    
    public void set_position(int X,int Y,Common.Direction entrance,Common.Direction exit)
    {
        set_curve(X, Y, entrance, exit);
        obj.transform.position = new Vector3(X%3, Y%3, 0)+Dire_to_Vec(entrance)/2f;
    }

    public void set_Speed(float Speed)//速さを変える
    {
        speed = Speed;
    }

    public bool Move() //かくかく曲がる
    {
        bool EndBool = false;
        Vector3 delta=new Vector3(0,0,0);
        if (2*count<speed)
        {
            delta = -Dire_to_Vec(move_from)/speed;
        }
        else if (count<speed)
        {
            delta = Dire_to_Vec(move_to) / speed;
        }
        else
        {
            count = -1;
            EndBool = true;
        }
        count++;
        obj.transform.Translate(delta);
        return EndBool;
    }

    public void set_curve(int X,int Y,Common.Direction entrance, Common.Direction exit)
    {
        x = X;
        y = Y;
        move_from = entrance;
        move_to = exit;
    }

    public Vector3 Dire_to_Vec(Common.Direction d)
    {
        if (d == Common.Direction.Straight || d == Common.Direction.Up) return new Vector3(0, 1, 0);
        else if (d == Common.Direction.Down) return new Vector3(0, -1, 0);
        else if (d == Common.Direction.Right) return new Vector3(1, 0, 0);
        else if (d == Common.Direction.Left) return new Vector3(-1, 0, 0);
        else return new Vector3(0, 0, 0);
    }
}
