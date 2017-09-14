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

    public float skills;//後でキャラによって変更、多分配列化（今考えているのはCharactorスクリプトに移植）

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
        /*GameObject o = GameObject.Find("Background");  //これで配置していく
        o.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Background/Back" + 0);
        o.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        o.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);*/
        //o = GameObject.Find("Effect");  //これで配置していく
        //o.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        //o.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
        #endregion
        #region キャラクターのアニメ
        for (int i = 0; i < 3; i++)
        {
            chara[i] = GameObject.Find("Chara" + i);
            chara_ID[i] = PlayerPrefs.GetInt("Party" + i, 1);
            chara[i].GetComponent<Animator>().SetInteger("Chara_Int", chara_ID[i]); //1はデータナンバー。PlayerPrefsでボックスで
            chara[i].GetComponent<RectTransform>().sizeDelta = new Vector2(width * 0.36f, width * 0.4f);
            chara[i].GetComponent<RectTransform>().localPosition = new Vector3(width * 0.65f * (3 - i), height * 0.277f);
        }
        /*PlayerPrefs.SetInt("Party0", 3);
        PlayerPrefs.SetInt("Party1", 1);
        PlayerPrefs.SetInt("Party2", 2);
        PlayerPrefs.SetInt("Coin", 234);
        PlayerPrefs.SetInt("treasure0", 2);
        PlayerPrefs.SetInt("treasure1", 1);
        PlayerPrefs.SetInt("Time", 357);
        PlayerPrefs.SetInt("Life", 2);
        PlayerPrefs.SetInt("enemy", 5);*/ //ここを使うとResultリセット
        #endregion
        pause_bool = false;
        timer_bool = false;
        bg_bool = false;
        time = 600;
        Time_text = GameObject.Find("Time").GetComponent<Text>();
        Life_point = 2;
        #region UIオブジェクトをheight,widthで整理します。（ずれないのなら）Mapのとこより下は消しても可
        GameObject.Find("Map_base").GetComponent<RectTransform>().localPosition = new Vector3(0.35f * width, 0.5f * height - 0.15f * width);
        GameObject.Find("Map_base").GetComponent<RectTransform>().sizeDelta = new Vector2(0.3f * width, 0.3f * width);
        GameObject.Find("Image").GetComponent<RectTransform>().localPosition = new Vector3(0, 0.465f * height);
        GameObject.Find("Image").GetComponent<RectTransform>().sizeDelta = new Vector2(width, 0.07f * height);
        GameObject.Find("Pause").GetComponent<RectTransform>().localPosition = new Vector3(0.03f*height-0.5f * width, 0.465f * height);
        GameObject.Find("Pause").GetComponent<RectTransform>().sizeDelta = new Vector2(0.05f * height, 0.05f * height);
        GameObject.Find("Time").GetComponent<RectTransform>().localPosition = new Vector3(-0.2f * width, 0.465f * height);
        GameObject.Find("Time").GetComponent<RectTransform>().sizeDelta = new Vector2(0.4f * width, 0.07f * height);
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                GameObject o = Instantiate(Resources.Load<GameObject>("Prefab/Small_map"));
                o.transform.parent = GameObject.Find("Map_base").transform;
                o.GetComponent<RectTransform>().localPosition = new Vector3(0.09f * (i-1) * width, 0.09f * (j-1) * width);
                o.GetComponent<RectTransform>().sizeDelta = new Vector2(0.09f * width, 0.09f * width);
                o.name = "Small_map" + i + "-" + j;
            }
        }
        #endregion
        //GameObject.Find("Icon").GetComponent<RectTransform>().sizeDelta = new Vector2(0.05f * width, 0.05f * width);
        //Change_icon(1);
        skills = 0;
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
        if (skills > 0) skills -= Time.deltaTime;
        #endregion
    }

    // すみません、作っちゃいました...。
    public void All_pause_flg(bool pause)
    {
        pause_bool = pause;
        timer_bool = pause;
        bg_bool = pause;
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

    public bool To_result(int Effect_or_Treasure)
    {
        if (Effect_or_Treasure == 0)
        {
            AnimatorStateInfo info = GameObject.Find("Effect").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
            return (info.IsName("Game_Over_Time") || info.IsName("Game_Over_Life")) && info.normalizedTime > 0.5f;
        }
        else return GameObject.Find("Goal").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Treasure_Open")
        && GameObject.Find("Goal").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1f;
    }

    public void Set_Button()
    {
        Color col= GameObject.Find("GameOver_text").GetComponent<Image>().color + new Color(0, 0, 0, 0.01f);
        GameObject.Find("GameOver_text").GetComponent<Image>().color = col;
        GameObject.Find("Game_over").GetComponent<Image>().color = col;
        GameObject.Find("Retry").GetComponent<Image>().color = col;
        GameObject.Find("GameOver_text").GetComponent<RectTransform>().localPosition = new Vector3(0, 0.1f * height, 0);
        GameObject.Find("Game_over").GetComponent<RectTransform>().localPosition = new Vector3(0, -0.2f * height, 0);
        GameObject.Find("Retry").GetComponent<RectTransform>().localPosition = new Vector3(0, -0.08f * height, 0);
    }

    public void Effect(string s)
    {
        GameObject o = GameObject.Find("Effect");
            o.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
            o.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        o.GetComponent<Animator>().SetTrigger(s);
        if(s!= "Goal_Trigger")
        {
            GameObject.Find("GameOver_text").GetComponent<Image>().color = new Color(1, 1, 1, 0);
            GameObject.Find("Game_over").GetComponent<Image>().color = new Color(1, 1, 1, 0);
            GameObject.Find("Retry").GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }
    }

    public void Small_map(int x, int y)
    {
        GameObject o = GameObject.Find("Small_map" + x + "-" + y);
        o.GetComponent<Image>().color = new Color(1, 0.8f, 0, 1);
    }

    /*public void Change_icon(int ID)
    {
        GameObject.Find("Icon").GetComponent<Animator>().SetInteger("Chara_Int", ID);
    }*/

    public void Retry()
    {
        time = 600;
        Life_point = 2;
        GameObject.Find("Effect").GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0);
        GameObject.Find("Game_over").GetComponent<RectTransform>().localPosition = new Vector3(0, height, 0);
        GameObject.Find("GameOver_text").GetComponent<RectTransform>().localPosition = new Vector3(0, height, 0);
        GameObject.Find("Retry").GetComponent<RectTransform>().localPosition = new Vector3(0, height, 0);
        for (int i = 0; i < 3; i++)
        {
            chara[i].GetComponent<RectTransform>().localPosition = new Vector3(width * 0.65f * (3 - i), height * 0.277f);
            chara[i].GetComponent<Animator>().SetTrigger("Retry_Trigger");
            Anime(i, Common.Action.Walk);
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
            o.GetComponent<RectTransform>().localScale = (24f * vec + new Vector3(width * 0.01f, width * 0.01f)) / 25f;
            if ((vec - new Vector3(width * 0.01f, width * 0.01f)).magnitude < 0.01f)
            {
                o.GetComponent<RectTransform>().localScale = new Vector3(width * 0.01f, width * 0.01f);
                return true;
            }
            else return false;
        }
        else
        {
            Vector3 vec = o.GetComponent<RectTransform>().localPosition;
            if (vec.magnitude < 0.55f)
            {
                o.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
                return true;
            }
            else
            {
                vec = vec / (vec.magnitude);
                o.GetComponent<RectTransform>().localPosition -= vec; return false;
            }
        }
    }

    public void Gage(float gage)
    {
        GameObject.Find("Gage").GetComponent<Image>().fillAmount = gage / 25f;
    }

    public void Change_Chara()
    {
        GameObject GO_tmp = chara[0];
        int ID_tmp = chara_ID[0];
        for(int i=1;i<Life_point; i++)
        {
            chara[i] = chara[i - 1];
            chara_ID[i] = chara_ID[i - 1];
        }
        chara[Life_point] = GO_tmp;
        chara_ID[Life_point] = ID_tmp;
    }

    public void SE_on()
    {

    }
}