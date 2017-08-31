using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public int x, y;//x:左から,y:下から。0スタート
    float speed;
    GameObject chara, arrow;//chara:画像を持つもの,arrow:方向を持つもの
    Sprite img;
    int count;

    public Character(GameObject ImageObj,GameObject ArrowObj)
    {
        chara = ImageObj;
        arrow = ArrowObj;
    }
    
    public void SetSpeed(float Speed)//速さを変える
    {
        speed = Speed;
    }

    public bool Move(Common.Direction CurveDirection) //曲がる方向を入れたらまわっていく
    {
        float curve = 0;
        Vector3 delta = new Vector3(0, 1f / speed,0);
        if (CurveDirection == Common.Direction.Right)
        {
            curve = -90f / speed;
            delta = new Vector3(0, Mathf.Sin(curve), 0);
        }
        else if (CurveDirection == Common.Direction.Left)
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
}
