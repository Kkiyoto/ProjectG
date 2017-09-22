using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Home : MonoBehaviour
{
    public GameObject select;//Canvas
    public GameObject BG;//BackCanvas
    public GameObject detail, Big_img, text;//キャラ詳細
    Box_Chara[] charas = new Box_Chara[8];
    public GameObject[] party_chara = new GameObject[3];
    int pos_num, tap_num, target_num;//pos:カメラの位置, tap:触ったやつ, target:tapに対して何するか
    Vector3[] Camera_Pos = new Vector3[5];
    float width, height;
    float tap_second;
    int[] party_ID = new int[3];

    int in_flg;
    bool is_tap;

    // Use this for initialization
    void Start()
    {
        width = Screen.width;
        height = Screen.height;
        Watch watch = GameObject.Find("Watch").GetComponent<Watch>();
        charas = watch.get_Chara(8);
        pos_num = 0;
        for (int i = 0; i < 5; i++) Camera_Pos[i] = new Vector3((2 - i) * width, 0, 0);
        is_tap = false;
        for (int i = 0; i < 3; i++)
        {
            party_ID[i] = PlayerPrefs.GetInt("Party" + i, i + 1);
            party_chara[i].GetComponent<Image>().sprite = charas[party_ID[i] - 1].Small_img;
        }
        in_flg = 0;
    }

    // Update is called once per frame
    void Update()
    {
        #region いつでもやるやつ
        if (Input.GetKeyDown(KeyCode.Return)) pos_num = (pos_num + 1) % 5;
        Vector3 bg_vec = BG.GetComponent<RectTransform>().localPosition;
        BG.GetComponent<RectTransform>().localPosition = (Camera_Pos[Mathf.Abs(pos_num)] + 9f * bg_vec) / 10f;
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (vec.y < -0.4f * height)
            {
                int x = Mathf.RoundToInt(vec.x * 5f / width);
                Debug.Log(x.ToString());
            }
        }
        else if (tap_second < 1) tap_second += Time.deltaTime;
        #endregion
        #region 0 :キャラ鑑賞 & Select
        if (pos_num == 0)
        {
            if (in_flg == 1)
            {
                //if (Input.GetMouseButtonDown(0)) {select.GetComponent<RectTransform>().localPosition = new Vector3(width, 0, 0);in_flg=0;}
                if (Input.GetMouseButtonDown(0)) Game_Start();
            }
        }
        #endregion
        #region 1 :キャラ強化
        else if (pos_num == 1)
        {

        }
        #endregion
        #region -1:キャラ強化
        else if (pos_num == -1)
        {

        }
        #endregion
        #region 2 :パーティ編成
        else if (pos_num == 2)
        {
            if (is_tap && tap_second > 1)
            {
                if (in_flg == 1) Charas(tap_num - 1);
                else Charas(party_ID[target_num - 1] - 1);
            }
        }
        #endregion
        #region -2:キャラ詳細
        else if (pos_num == -2)
        {
            if (Input.GetMouseButtonDown(0))
            {
                detail.GetComponent<RectTransform>().localPosition = new Vector3(width, 0, 0);
                pos_num = 2;
            }
        }
        #endregion
        #region 3 :ガチャ
        else if (pos_num == 3)
        {

        }
        #endregion
        #region -3:ガチャ
        else if (pos_num == -3)
        {

        }
        #endregion
        #region 4 :キャラ強化
        else if (pos_num == 4)
        {

        }
        #endregion
        #region -4:キャラ強化
        else if (pos_num == -4)
        {

        }
        #endregion
    }

    #region 全部の関数
    public void Menu()
    {
        Vector3 vec = Input.mousePosition;
        int num = Mathf.FloorToInt(vec.x * 5f / width);
        pos_num = num;
        detail.GetComponent<RectTransform>().localPosition = new Vector3(width, 0, 0);
    }

    public void Get_Button()
    {
        if (Mathf.Abs(pos_num) == 0)
        {
            select.GetComponent<RectTransform>().localPosition = Vector3.zero;
            in_flg = 1;
        }
    }
    #endregion
    #region 0の関数
    void Game_Start()
    {
        for (int i = 0; i < 3; i++) PlayerPrefs.SetInt("Party" + i, charas[party_ID[i] - 1].chara_ID);
        SceneManager.LoadScene("Tutorial");
    }
    #endregion
    #region 1の関数
    #endregion
    #region 2の関数
    void Charas(int x)
    {
        if (x >= 0 && charas[x].chara_ID != 0)
        {
            pos_num = -2;
            detail.GetComponent<RectTransform>().localPosition = Vector3.zero;
            Big_img.GetComponent<Image>().sprite = charas[x].Big_img;
            text.GetComponent<Text>().text = "攻撃力：" + charas[x].attack + "   HP：" + charas[x].HP
                + "\nスキル発動時間：" + charas[x].skill_time + "秒　発動：" + charas[x].skill_walk
                + "歩\n説明：" + charas[x].skill_Description;
        }
    }

    public void Set_ID(int ID)
    {
        tap_num = ID;
        tap_second = 0;
        is_tap = true;
    }
    public void Set_ID_Up(int ID)
    {
        if (tap_num == ID && tap_second < 1 && is_tap)
        {
            if (target_num != 0)
            {
                party_ID[target_num - 1] = tap_num;
                party_chara[target_num - 1].GetComponent<Image>().sprite = charas[tap_num - 1].Small_img;
                target_num = 0;
                tap_num = 0;
            }
        }
        is_tap = false;
    }

    public void Outer()
    {
        if (is_tap)
        {
            is_tap = false;
            tap_num = 0;
            target_num = 0;
        }
    }

    public void Set_party(int ID)
    {
        target_num = ID;
        tap_second = 0;
        is_tap = true;
    }
    public void Set_party_Up(int ID)
    {
        if (target_num == ID && tap_second < 1 && is_tap)
        {
            if (tap_num != 0)
            {
                party_ID[target_num - 1] = tap_num;
                party_chara[target_num - 1].GetComponent<Image>().sprite = charas[tap_num - 1].Small_img;
                target_num = 0;
                tap_num = 0;
            }
        }
        is_tap = false;
    }
    #endregion
    #region 3の関数
    #endregion
    #region 4の関数
    #endregion
}
