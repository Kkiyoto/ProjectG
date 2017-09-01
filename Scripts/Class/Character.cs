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
    public int x, y;//x:左から,y:下から。0スタート
    float speed;
    GameObject chara;//chara:画像を持つもの,arrow:方向を持つもの
    Sprite img;
    int count;
    public Common.Direction move_from,move_to;

    public Character(GameObject Image_obj)
    {
        chara = Image_obj;
    }
    
    public void set_position(int X,int Y,Common.Direction entrance,Common.Direction exit)
    {
        set_curve(X, Y, entrance, exit);
        if (entrance == Common.Direction.Down)
        {
            chara.transform.position = new Vector3(X, Y - 0.5f, 0);
        }
        else if (entrance == Common.Direction.Up)
        {
            chara.transform.position = new Vector3(X, Y + 0.5f, 0);
        }
        else if (entrance == Common.Direction.Right)
        {
            chara.transform.position = new Vector3((float)X + 0.5f, Y, 0);
        }
        else if (entrance == Common.Direction.Left)
        {
            chara.transform.position = new Vector3(X - 0.5f, Y, 0);
        }

    }

    public void set_Speed(float Speed)//速さを変える
    {
        speed = Speed;
    }

    /*
    public bool Move(Common.Direction Curve_direction) //曲がる方向を入れたらまわっていく
    {
        float curve = 0;
        Vector3 delta = new Vector3(0, 1f / speed, 0);
        if (Curve_direction == Common.Direction.Right)
        {
            curve = -90f / speed;
            delta = new Vector3(0, Mathf.Sin(curve), 0);
        }
        else if (Curve_direction == Common.Direction.Left)
        {
            curve = 90f / speed;
            delta = new Vector3(0, Mathf.Sin(curve), 0);
        }
        arrow.transform.Rotate(new Vector3(0, 0, curve));
        arrow.transform.Translate(delta);
        chara.transform.localRotation = new Quaternion(0, 0, 0, 0);
        count++;
        bool EndBool = false;
        if (count > speed)
        {
            count = 0;
            EndBool = true;
        }
        return EndBool;
    }
    */

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
        chara.transform.Translate(delta);
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
