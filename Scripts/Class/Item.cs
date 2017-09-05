using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int x, y;
    GameObject obj;
    public bool get;

    public Item(int pos_x,int pos_y,GameObject o)
    {
        x = pos_x;
        y = pos_y;
        obj = o;
        get = false;
    }

    public void Get_Item()
    {
        get = true;
        obj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/GameScene/Treasure_get");
    }

    /*public Transform Tra()
    {
        return obj.transform;
    }*/

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
}
