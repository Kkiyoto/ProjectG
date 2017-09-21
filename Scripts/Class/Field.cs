using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Field
 * GameObjectのスライド部分で、実態を持つもののクラス
 * スライド部分のGameObjectのかわりに使う。
 * objの部分に実態を入れる。アタッチなし
 */

public class Field : Functions
{
    GameObject obj;
    Sprite[] sprites = new Sprite[6]; //下からの道が、0:None,1:Straight,2:Right,3:Left,4:Mountain,5:ゴール
    Sprite[] Red_sprites = new Sprite[6]; //下からの道が、0:None,1:Straight,2:Right,3:Left,4:Mountain,5:ゴール
    public int data_x,data_y; //pazzle_fields[data_x,data_y]にデータをたくさん入れてます
    public bool time;

    public Field(GameObject o)
    {
        obj = o;
        for (int i = 0; i < 6; i++)
        {
            sprites[i] = Resources.Load<Sprite>("Images/GameScene/Road"+i);
            Red_sprites[i] = sprites[i];
        }
        Red_sprites[1]= Resources.Load<Sprite>("Images/GameScene/Road_red" + 1);
        Red_sprites[2]= Resources.Load<Sprite>("Images/GameScene/Road_red" + 2);
        Red_sprites[3]= Resources.Load<Sprite>("Images/GameScene/Road_red" + 3);
    }
    
    public void Set_img(Common.Direction img_type)//形を入れたらデータを作る
    {
        if(time)obj.GetComponent<SpriteRenderer>().sprite = Red_sprites[(int)img_type];
        else obj.GetComponent<SpriteRenderer>().sprite = sprites[(int)img_type];
    }

    public void Layer(int layer)
    {
        obj.GetComponent<SpriteRenderer>().sortingOrder = layer;
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
}
