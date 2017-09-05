using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* State_manage
 * ゲーム自体に関係なくても必要かなという部分を書こうかと
 * 主にUIかと思います
 * バックミュージックもここ?
 */

public class State_manage : MonoBehaviour
{
    GameObject Back_anime,Front_anime;
    float width, height, time;
    bool pause_bool;
    Text Time_text;
    int Life_point;
    GameObject[] chara = new GameObject[3];

    // Use this for initialization
    void Start ()
    {
        width = Screen.width;
        height = Screen.height;
        #region アニメの背景1
        Back_anime = GameObject.Find("Back_anime");
        Back_anime.GetComponent<Image>().sprite= Resources.Load<Sprite>("Images/Background/Bg"+0);
        Back_anime.GetComponent<RectTransform>().sizeDelta = new Vector2(height*2.8f, height*0.28f);
        Back_anime.GetComponent<RectTransform>().localPosition = new Vector3(0,height*0.28f);
        #endregion
        #region アニメの背景2
        Front_anime = GameObject.Find("Back_front");
        Front_anime.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Background/Bg_front" + 0);
        Front_anime.GetComponent<RectTransform>().sizeDelta = new Vector2(height * 2.79f, height * 0.37f);
        Front_anime.GetComponent<RectTransform>().localPosition = new Vector3(0, height * 0.285f);
        #endregion
        #region ゲームの後ろの背景
        GameObject o = GameObject.Find("Background");  //これで配置していく
        o.GetComponent<Image>().sprite= Resources.Load<Sprite>("Images/Background/Back"+0);
        o.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        o.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
        #endregion
        #region キャラクターのアニメ
        for (int i = 0; i < 3; i++)
        {
            o = GameObject.Find("Chara" + i); //3つをfor文にするつもり
            int n = PlayerPrefs.GetInt("chara"+i, 1);
            o.GetComponent<Animator>().SetInteger("Chara_Int",n); //1はデータナンバー。PlayerPrefsでボックスで
            o.GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.36f, width * 0.4f);
            o.GetComponent<RectTransform>().localPosition = new Vector3(width * 0.15f*i, height * 0.25f);
        }
        #endregion
        pause_bool = false;
        time = 1000;
        Time_text = GameObject.Find("Time").GetComponent<Text>();
        Life_point = 3;
    }

    // Update is called once per frame
    void Update ()
    {
        if (!pause_bool)
        {
            Back_anime.GetComponent<RectTransform>().Translate(new Vector3(height * 0.001f, 0, 0));
            Front_anime.GetComponent<RectTransform>().Translate(new Vector3(height * 0.001f, 0, 0));
            time -= Time.deltaTime;
        }
            if (Back_anime.GetComponent<RectTransform>().localPosition.x > 0.7f * height)
        {
            Back_anime.GetComponent<RectTransform>().position -= new Vector3(1.4f * height, 0, 0);
            Front_anime.GetComponent<RectTransform>().position -= new Vector3(1.4f * height, 0, 0);
        }
        int m = Mathf.FloorToInt(time / 60f);
        int s = Mathf.FloorToInt(time % 60f);
        Time_text.text = ("Time   "+m + " : " + s);
    }

    public void Pause_flg(bool pause)
    {
        pause_bool = pause;
    }

    public void Time_decade(float minus)
    {
        time -= minus;
    }

    public void Damage()
    {
        Life_point--;

    }
}
