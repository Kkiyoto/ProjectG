using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Party
 * UI部分で動いてるキャラのGameObjectは持ってる。
 * 基本的にそれを動かすというよりは、データの管理かな
 * スキルとかHPとか、能力とか
 */

public class Party : MonoBehaviour
{

    public int chara_ID;
    public GameObject chara;

    #region ここからはPlayerPrefasとかDictionaryとかで設定
    public float Attack, HP;//Atack:バトルにかかる時間（一回あたりかな？全部ででも良い）
    public int[] skills = new int[3]; //宝、敵、道
    public string skill_Description;//20秒が最高で使える時間（30に設定でリーダースキル化）
    public int Max_gage;
    #endregion

    public Party(GameObject o,int ID)
    {
        chara = o;
        chara_ID = ID;
        Anime().SetInteger("Chara_Int", ID);
        chara.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width * 0.36f, Screen.width * 0.4f);
    }

    public Animator Anime()
    {
        return chara.GetComponent<Animator>();
    }

    public Vector3 Pos
    {
        get { return chara.GetComponent<RectTransform>().localPosition; }
        set { chara.GetComponent<RectTransform>().localPosition = value; }
    }

    public int Index
    {
        get { return chara.transform.GetSiblingIndex(); }
        set { chara.transform.SetSiblingIndex(value); }
    }
}
