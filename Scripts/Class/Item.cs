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
    public int rare;

    public Item(int pos_x,int pos_y,GameObject o)
    {
        x = pos_x;
        y = pos_y;
        obj = o;
        get = false;
        find = false;
        map = Instantiate(Resources.Load<GameObject>("Prefab/Icon")) as GameObject;
        map.transform.parent = GameObject.Find("Map_base").transform;
        map.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/GameScene/Small_coin");
    }

    public void Get_Item()
    {
        get = true;
        obj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/GameScene/Treasure_get");
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
            float delta = Screen.width * 0.027f;
            map.GetComponent<Image>().color = Color.yellow;
            map.GetComponent<RectTransform>().localPosition = new Vector3((Pos.x - 5) * delta, (Pos.y - 5) * delta);
        }
        else map.GetComponent<Image>().color = Color.clear;
    }
}
