using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Charactor
 * GameObjectの動き回る部分で、プレイヤー、敵キャラ両方に使うクラス
 * キャラ部分のGameObjectのかわりに使う。
 * objの部分に実態を入れる。アタッチなし
 */

public class Character : Functions
{
    public int x, y;//x:左から,y:下から。0~8
    public int pre_x, pre_y;//1つ前の座標、0~8
    float speed;
    GameObject obj,map;//chara:画像を持つもの,map:左上にあるやつ
    int count;
    public Common.Direction move_from,move_to;
    public Common.Action act;
    public Common.Type type;
    Color col = Color.white;

    public Character(GameObject Image_obj,Common.Type t)
    {
        obj = Image_obj;
        act = Common.Action.Walk;
        type = t;
        map = Instantiate(Resources.Load<GameObject>("Prefab/Icon")) as GameObject;
        map.transform.parent = GameObject.Find("Map_base").transform;
        if (t != Common.Type.Player)
        {
            Set_Chara((int)t);
            //obj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/Charactor/Enemy_sprite/Enemy" + (int)t+"_1");
            map.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/GameScene/Small_enemy");
        }
    }

    public void set_position(int X,int Y,Common.Direction entrance,Common.Direction exit)
    {
        set_curve(X, Y, entrance, exit);
        obj.transform.position = new Vector3(X, Y, 0)+Dire_to_Vec(entrance)/2f;
    }

    public void set_Speed(float Speed)//速さを変える
    {
        speed = Speed;
    }

    public bool Move(Vector3 field_pos,bool move) //円状に曲がる
    {
        bool EndBool = false;
        if (count < speed)
        {
            float theta = Mathf.PI * count / 2f / speed;
            Vector3 delta = Dire_to_Vec(move_from) * (1f - Mathf.Sin(theta)) + Dire_to_Vec(move_to) * (1f - Mathf.Cos(theta));
            obj.transform.position = field_pos + delta/2f;
        }
        else
        {
            count = -1;
            EndBool = true;
        }
        if(!move)count++;
        return EndBool;
    }

    public void set_curve(int X,int Y,Common.Direction entrance, Common.Direction exit)
    {
        x = X;
        y = Y;
        move_from = entrance;
        move_to = exit;
    }

    public void On_Map(bool show)
    {
        if (show)
        {
            float delta = Screen.width * 0.022f;
            map.GetComponent<RectTransform>().localPosition = new Vector3((Pos.x - 5) * delta, (Pos.y - 5) * delta);
            map.GetComponent<Image>().color = col;
        }
        else map.GetComponent<Image>().color = new Color(0, 0, 0, 0);
    }

    public void map_color(float a)
    {
        col = new Color(1, 1, 1, a);
    }

    public Vector3 Pos
    {
        set { obj.transform.position = value; }
        get { return obj.transform.position; }
    }

    public SpriteRenderer Sprite()
    {
        return obj.GetComponent<SpriteRenderer>();
    }

    public void Set_Chara(int ID)
    {
        obj.GetComponent<Animator>().SetInteger("Chara_Int", ID);
    }

    public void Anime(Common.Action action)
    {
        obj.GetComponent<Animator>().SetInteger("Move_Int", (int)action);
    }

    public void OutScreen()
    {
        obj.GetComponent<Animator>().SetTrigger("Out_Trigger");
    }

    public bool Wait_chara()
    {
        bool check = obj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("change");
        return check;
    }
}
