using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Item
 * コインや宝箱等、ゲットしていくものを入れておきます。
 * 何がゲットできるのかとかもここに入れたらなと
 */

public class Item : Functions
{
    public int x, y;
    GameObject obj,map;
    public bool get,find;
    public Common.Treasure type; 
    Color col=Color.white;
    float delta, scale;

    public Item(int pos_x,int pos_y,GameObject o,Common.Treasure t,float map_num,float sc)
    {
        x = pos_x;
        y = pos_y;
        obj = o;
        get = false;
        find = false;
        map = Instantiate(Resources.Load<GameObject>("Prefab/Icon")) as GameObject;
        map.transform.parent = GameObject.Find("Map_base").transform;
        map.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/GameScene/Small_coin");
        map.GetComponent<RectTransform>().sizeDelta = new Vector2(map_num, map_num);
        type = t;
        obj.GetComponent<Animator>().SetInteger("Item_Int" , (int)type);
        delta = map_num;
        scale = sc;
    }

    public void Get_Item()
    {
        get = true;
        obj.GetComponent<Animator>().SetTrigger("Get_Trigger");
    }

    public Vector3 Pos
    {
        set { obj.transform.position = value; }
        get { return obj.transform.position; }
    }

    public void OutScreen()
    {
        obj.transform.position = new Vector3(0, -10);
    }

    public SpriteRenderer Sprite()
    {
        return obj.GetComponent<SpriteRenderer>();
    }

    public void On_Map(bool show,bool skill)
    {
        if ((skill||(show&&find))&&!get)
        {
            map.GetComponent<Image>().color = col;
            map.GetComponent<RectTransform>().localPosition = new Vector3((Pos.x - scale) * delta, (Pos.y - scale) * delta);
        }
        else map.GetComponent<Image>().color = Color.clear;
    }

    public void map_color(float a)
    {
        col = new Color(1, 1, 1, a);
    }
}
