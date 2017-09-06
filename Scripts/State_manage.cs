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
    public bool timer_bool,bg_bool;
    Text Time_text;
    int Life_point;
    GameObject[] chara = new GameObject[3];
    int[] chara_ID = new int[3];

    // Use this for initialization
    void Start ()
    {
        width = Screen.width;
        height = Screen.height;
        #region アニメの背景1
        Back_anime = GameObject.Find("Back_anime");
        Back_anime.GetComponent<Image>().sprite= Resources.Load<Sprite>("Images/Background/Bg"+0);
        Back_anime.GetComponent<RectTransform>().sizeDelta = new Vector2(height*2.8f, height*0.28f);
        Back_anime.GetComponent<RectTransform>().localPosition = new Vector3(0,height*0.292f);
        #endregion
        #region アニメの背景2
        Front_anime = GameObject.Find("Back_front");
        Front_anime.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Background/Bg_front" + 0);
        Front_anime.GetComponent<RectTransform>().sizeDelta = new Vector2(height * 2.79f, height * 0.37f);
        Front_anime.GetComponent<RectTransform>().localPosition = new Vector3(0, height * 0.297f);
        #endregion
        #region ゲームの後ろの背景
        GameObject o = GameObject.Find("Background");  //これで配置していく
        o.GetComponent<Image>().sprite= Resources.Load<Sprite>("Images/Background/Back"+0);
        o.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        o.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
        o = GameObject.Find("Effect");  //これで配置していく
        o.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        o.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
        #endregion
        #region キャラクターのアニメ
        for (int i = 0; i < 3; i++)
        {
            chara[i] = GameObject.Find("Chara" + i); //3つをfor文にするつもり
            chara_ID[i] = PlayerPrefs.GetInt("Party"+i, 1);
            chara[i].GetComponent<Animator>().SetInteger("Chara_Int",chara_ID[i]); //1はデータナンバー。PlayerPrefsでボックスで
            chara[i].GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.36f, width * 0.4f);
            chara[i].GetComponent<RectTransform>().localPosition = new Vector3(width * 0.15f*(2-i), height * 0.25f);
        }
        #endregion
        pause_bool = false;
        timer_bool = true;
        bg_bool = true;
        time = 615;
        Time_text = GameObject.Find("Time").GetComponent<Text>();
        Life_point = 2;
    }

    // Update is called once per frame
    void Update()
    {
        #region 背景の動画
        if (!pause_bool&&bg_bool)
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
            if ((vec - new Vector3(width * 0.15f * (Life_point - i), height * 0.25f)).magnitude < 0.01f)
                chara[i].GetComponent<RectTransform>().localPosition = new Vector3(width * 0.15f * (Life_point - i), height * 0.25f);
            else
                chara[i].GetComponent<RectTransform>().localPosition = (39f * vec + new Vector3(width * 0.15f * (Life_point - i), height * 0.25f)) / 40f;
        }
        #endregion
        #region 時間表示
        if (!pause_bool&&timer_bool) time -= Time.deltaTime;
        if (time < 0) //GameOver
        {
            time = 0;
            GameObject.Find("Director").GetComponent<Main>().Goal(false);
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

    public void Lose_Time(float minus)
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

    public void Anime(int ID,Common.Action action) //ID:見えてるうち、後ろから何番目？、Commn.Action.Battleとかを入れたら遷移するようにする
    {
        chara[ID].GetComponent<Animator>().SetInteger("Move_Int", (int)action);
    }

    public bool To_result()
    {
        return GameObject.Find("Effect").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Next");
    }

    public void Effect(string s)
    {
        GameObject.Find("Effect").GetComponent<Animator>().SetTrigger(s);
    }
}
