using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* State_manage
 * ゲーム自体に関係なくても必要かなという部分を書こうかと
 * 主にUIかと思います
 * バックミュージックもここ?
 */

public class State_manage : Functions
{
    GameObject Back_anime, Front_anime;
    float width, height, time;
    bool pause_bool;
    public bool timer_bool, bg_bool;
    Text Time_text;
    int Life_point;
    GameObject[] chara = new GameObject[3];
    int[] chara_ID = new int[3];


    // Use this for initialization
    void Start()
    {
        width = Screen.width;
        height = Screen.height;
        #region アニメの背景1
        Back_anime = GameObject.Find("Back_anime");
        Back_anime.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Background/Bg" + 0);
        Back_anime.GetComponent<RectTransform>().sizeDelta = new Vector2(height * 2.8f, height * 0.28f);
        Back_anime.GetComponent<RectTransform>().localPosition = new Vector3(0, height * 0.292f);
        #endregion
        #region アニメの背景2
        Front_anime = GameObject.Find("Back_front");
        Front_anime.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Background/Bg_front" + 0);
        Front_anime.GetComponent<RectTransform>().sizeDelta = new Vector2(height * 2.79f, height * 0.37f);
        Front_anime.GetComponent<RectTransform>().localPosition = new Vector3(0, height * 0.297f);
        #endregion
        #region ゲームの後ろの背景
        GameObject o = GameObject.Find("Background");  //これで配置していく
        o.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Background/Back" + 0);
        o.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 0.93f*height);
        o.GetComponent<RectTransform>().localPosition = new Vector3(0, -0.035f*height);
        o = GameObject.Find("Effect");  //これで配置していく
        o.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        o.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
        #endregion
        #region キャラクターのアニメ
        for (int i = 0; i < 3; i++)
        {
            chara[i] = GameObject.Find("Chara" + i); //3つをfor文にするつもり
            chara_ID[i] = PlayerPrefs.GetInt("Party" + i, 1);
            chara[i].GetComponent<Animator>().SetInteger("Chara_Int", chara_ID[i]); //1はデータナンバー。PlayerPrefsでボックスで
            chara[i].GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.36f, width * 0.4f);
            chara[i].GetComponent<RectTransform>().localPosition = new Vector3(width * 0.65f * (3 - i), height * 0.277f);
        }
        #endregion
        pause_bool = false;
        timer_bool = true;
        bg_bool = true;
        time = 600;
        Time_text = GameObject.Find("Time").GetComponent<Text>();
        Life_point = 2;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                o = Instantiate(Resources.Load<GameObject>("Prefab/Small_map"));
                o.transform.parent = GameObject.Find("Map_base").transform;
                o.GetComponent<RectTransform>().localPosition = new Vector3(0.09f * (i-1) * width, 0.09f * (j-1) * width);
                o.GetComponent<RectTransform>().sizeDelta = new Vector2(0.09f * width, 0.09f * width);
                o.name = "Small_map" + i + "-" + j;
            }
        }
        //GameObject.Find("Icon").GetComponent<RectTransform>().sizeDelta = new Vector2(0.05f * width, 0.05f * width);
        //Change_icon(1);
    }

    // Update is called once per frame
    void Update()
    {
        #region 背景の動画
        if (!pause_bool && bg_bool)
        {
            Back_anime.GetComponent<RectTransform>().Translate(new Vector3(height * 0.001f, 0, 0));
            Front_anime.GetComponent<RectTransform>().Translate(new Vector3(height * 0.001f, 0, 0));
            if (Back_anime.GetComponent<RectTransform>().localPosition.x > 0.7f * height)
            {
                Back_anime.GetComponent<RectTransform>().position -= new Vector3(1.4f * height, 0, 0);
                Front_anime.GetComponent<RectTransform>().position -= new Vector3(1.4f * height, 0, 0);
            }
        }
        #endregion
        #region キャラクター移動
        for (int i = 0; i <= Life_point; i++)
        {
            Vector3 vec = chara[i].GetComponent<RectTransform>().localPosition;
            if ((vec - new Vector3(width * 0.2f * (Life_point - i - 0.2f), height * 0.277f)).magnitude < 0.01f)
                chara[i].GetComponent<RectTransform>().localPosition = new Vector3(width * 0.2f * (Life_point - i - 0.2f), height * 0.277f);
            else
                chara[i].GetComponent<RectTransform>().localPosition = (39f * vec + new Vector3(width * 0.2f * (Life_point - i - 0.2f), height * 0.277f)) / 40f;
        }
        #endregion
        #region 時間表示
        if (!pause_bool && timer_bool) time -= Time.deltaTime;
        if (time < 0) //GameOver
        {
            time = 0;
            GameObject.Find("Director").GetComponent<Main>().Goal(1);
        }
        int m = Mathf.FloorToInt(time / 60f);
        int s = Mathf.FloorToInt(time % 60f);
        Time_text.text = ("Time   " + m.ToString().PadLeft(2, '0') + " : " + s.ToString().PadLeft(2, '0'));
        #endregion
    }

    public void Pause_flg(bool pause)
    {
        pause_bool = pause;
    }

    public void Lose_Time(float minus) //単位は秒
    {
        time -= minus;
    }

    public bool Damage()
    {
        chara[Life_point].GetComponent<Animator>().SetTrigger("Out_Trigger");
        Life_point--;
        if (Life_point < 0) return true;//GameOver
        else return false;
    }

    public int Top_ID() //一番前のキャラの図鑑ID（アニメータ）
    {
        if (Life_point < 0) return 0;
        else return chara_ID[Life_point];
    }

    public int get_Life()
    {
        return Life_point;
    }

    public float get_Time()
    {
        return time;
    }

    public void Anime(int ID, Common.Action action) //ID:見えてるうち、後ろから何番目？、Commn.Action.Battleとかを入れたら遷移するようにする
    {
        chara[ID].GetComponent<Animator>().SetInteger("Move_Int", (int)action);
    }

    public bool To_result()
    {
        return GameObject.Find("Effect").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Next");
    }

    public void Set_Button()
    {
        GameObject.Find("Game_over").GetComponent<RectTransform>().localPosition = new Vector3(0, -0.08f*height, 0);
        GameObject.Find("Retry").GetComponent<RectTransform>().localPosition = new Vector3(0, -0.2f*height, 0);
    }

    public void Effect(string s)
    {
        GameObject.Find("Effect").GetComponent<Animator>().SetTrigger(s);
    }

    public void Small_map(int x, int y, string ID)
    {
        GameObject o = GameObject.Find("Small_map" + x + "-" + y);
        o.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/GameScene/Small_map");
    }

    /*public void Change_icon(int ID)
    {
        GameObject.Find("Icon").GetComponent<Animator>().SetInteger("Chara_Int", ID);
    }*/

    public void Retry()
    {
        time = 600;
        Life_point = 2;
        GameObject.Find("Game_over").GetComponent<RectTransform>().localPosition = new Vector3(0, height, 0);
        GameObject.Find("Retry").GetComponent<RectTransform>().localPosition = new Vector3(0, height, 0);
        for (int i = 0; i < 3; i++)
        {
            chara[i].GetComponent<RectTransform>().localPosition = new Vector3(width * 0.65f * (3 - i), height * 0.277f);
            chara[i].GetComponent<Animator>().SetTrigger("Out_Trigger");
        }
        timer_bool = true;
        bg_bool = true;
    }

    public bool To_goal(int Scale_or_Pos)
    {
        GameObject o = GameObject.Find("Goal");
        if (Scale_or_Pos == 0)
        {
            Vector3 vec = o.GetComponent<RectTransform>().localScale;
            o.GetComponent<RectTransform>().localScale = (14f * vec + new Vector3(width * 0.9f, width * 0.9f)) / 15f;
            if ((vec - new Vector3(width * 0.9f, width * 0.9f)).magnitude < 0.1f)
            {
                o.GetComponent<RectTransform>().localScale = new Vector3(width * 0.9f, width * 0.9f);
                return true;
            }
            else return false;
        }
        else
        {
            Vector3 vec = o.GetComponent<RectTransform>().position;
            vec = vec / (vec.magnitude) * 0.1f;
            o.GetComponent<RectTransform>().position-=vec;
            if (vec.magnitude < 0.1f)
            {
                o.GetComponent<RectTransform>().position = new Vector3(0, 0);
                return true;
            }
            else return false;
        }
    }
}