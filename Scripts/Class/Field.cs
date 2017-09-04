using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Field
 * GameObjectのスライド部分で、実態を持つもののクラス
 * スライド部分のGameObjectのかわりに使う。
 * objの部分に実態を入れる。アタッチなし
 */

public class Field : MonoBehaviour
{
    GameObject obj;
    Sprite[] sprites = new Sprite[4]; //下からの道が、0:None,1:Straight,2:Right,3:Left
    public int data_x,data_y; //pazzle_fields[data_x,data_y]にデータをたくさん入れてます

    public Field(GameObject o)
    {
        obj = o;
        for (int i = 0; i < 4; i++)
        {
            sprites[i] = Resources.Load<Sprite>("Images/GameScene/Road"+i);
        }
    }
    
    public void Set_img(Common.Direction img_type)//形を入れたらデータを作る
    {
        obj.GetComponent<SpriteRenderer>().sprite = sprites[(int)img_type];
    }

    public void Layer(int layer)
    {
        obj.GetComponent<SpriteRenderer>().sortingOrder = layer;
    }

    public Transform Tra()
    {
        return obj.transform;
    }

    public SpriteRenderer Sprite()
    {
        return obj.GetComponent<SpriteRenderer>();
    }
}
