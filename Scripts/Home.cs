using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Home : MonoBehaviour
{
    public GameObject select,Audio;
    public RectTransform BG,detail,Ready,Hikousen,stage0,stage1,stage2,stage3;
    public Text  text,Dtitle,HP;
    public Image home_img, Big_img,title,menu;
    public Sprite[] Menu_img = new Sprite[5], Title_img = new Sprite[5];
    public Text[] Ptext = new Text[3];
    Box_Chara[] charas = new Box_Chara[12];
    public Image[] party_chara = new Image[3];
    int pos_num, tap_num, target_num;//pos:カメラの位置, tap:触ったやつ, target:tapに対して何するか
    Vector3[] Camera_Pos = new Vector3[5];
    float width, height;
    float tap_second;
    int[] party_ID = new int[3];
    Vector3 out_vec;
    bool volume_set = false, scene_fin=false;
    float time_pass=0.0f,select_time=0f;

    Vector3 hikousen_vec;
    int in_flg;
    bool is_tap;
    public AudioSource[] Home_BGM = new AudioSource[2];
    public AudioClip[] Home_SE = new AudioClip[4];//BGM2つ,Button,シュ,

    // Use this for initialization
    void Start()
    {
        width = Screen.width;
        height = Screen.height;
        Watch watch = GameObject.Find("Watch").GetComponent<Watch>();
        charas = watch.get_Chara(6);
        pos_num = 0;
        for (int i = 0; i < 5; i++) Camera_Pos[i] = new Vector3((2 - i) * width, 0, 0);
        is_tap = false;
        for (int i = 0; i < 3; i++)
        {
            party_ID[i] = PlayerPrefs.GetInt("Box_Party" + i, i + 1);
            party_chara[i].sprite = charas[party_ID[i] - 1].Middle_img;
            charas[party_ID[i] - 1].img.color = Color.gray;
            Ptext[i].text = "Lv."+(charas[party_ID[i] - 1].Level+1);
        }
        home_img.sprite = charas[party_ID[0] - 1].Home_img;
        in_flg = 0;
        out_vec = new Vector3(0, height, 0);
        Home_BGM = GameObject.Find("EventSystem").GetComponents<AudioSource>();
        HP.text = "HP合計（制限時間）: " + (charas[party_ID[0] - 1].HP + charas[party_ID[1] - 1].HP + charas[party_ID[2] - 1].HP);
        hikousen_vec = new Vector3(-width, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        #region いつでもやるやつ
        if (Input.GetKeyDown(KeyCode.Return)) pos_num = (pos_num + 1) % 5;
        Vector3 v= Hikousen.localPosition;
        Hikousen.localPosition = (v * 6 + hikousen_vec) / 7f;
        Vector3 bg_vec = BG.localPosition;
        BG.localPosition = (Camera_Pos[Mathf.Abs(pos_num)] + 9f * bg_vec) / 10f;
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
            select_time += Time.deltaTime;

            stage3.GetComponent<RectTransform>().localPosition = new Vector3(0.17f * width, 0.35f * height, 0) + 0.02f*width*new Vector3(Mathf.Cos(select_time / 1.7f), Mathf.Sin(select_time * 1.3f));
            stage3.GetComponent<RectTransform>().sizeDelta = new Vector2(0.52f * width, 0.35f * height) *(1 + 0.05f  * Mathf.Cos(select_time));
            stage2.GetComponent<RectTransform>().localPosition = new Vector3(-0.22f * width, 0.07f * height, 0) + 0.02f * width * new Vector3(Mathf.Cos(select_time/1.4f), Mathf.Sin(select_time*1.2f));
            stage2.GetComponent<RectTransform>().sizeDelta = new Vector2(0.50f * width, 0.3f * height) * (1 + 0.05f  * Mathf.Cos(select_time*1.05f));
            stage1.GetComponent<RectTransform>().localPosition = new Vector3(0.23f * width, -0.1f * height, 0) + 0.02f * width * new Vector3(Mathf.Cos(select_time/1.6f), Mathf.Sin(select_time*1.1f));
            stage1.GetComponent<RectTransform>().sizeDelta = new Vector2(0.48f * width, 0.2f * height) * (1 + 0.05f*Mathf.Cos(select_time*0.97f));
            stage0.GetComponent<RectTransform>().localPosition = new Vector3(-0.22f * width, -0.3f * height, 0) + 0.02f * width * new Vector3(Mathf.Cos(select_time/1.5f), Mathf.Sin(select_time*1.4f));
            stage0.GetComponent<RectTransform>().sizeDelta = new Vector2(0.4f * width, 0.17f * height) * (1 + 0.05f  * Mathf.Cos(select_time*1.07f));
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
                detail.localPosition = out_vec;
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

        #region BGM関連
        if (!volume_set) time_pass += Time.deltaTime;
        if (time_pass <= 1.4) Home_BGM[1].volume = time_pass/2;
        else volume_set = true;

        if (scene_fin)  Home_BGM[1].volume -=0.003f;
        

        #endregion
    }

    #region 全部の関数
    public void Menu(int n)
    {
        int num = n;
        if (n > 4)
        {
            Vector3 vec = Input.mousePosition;
            num = Mathf.FloorToInt(vec.x * 5f / width);
        }
        //else Audio.GetComponent<AudioSource>().clip = Home_SE[0];
        Audio.GetComponent<AudioSource>().PlayOneShot(Home_SE[2]);//Button
        pos_num = num;
        title.sprite = Title_img[num];
        menu.sprite = Menu_img[num];
        detail.localPosition = out_vec;
        select.GetComponent<RectTransform>().localPosition = out_vec;
        if (num == 0) Ready.localPosition = new Vector3(0.3f * width, -0.26f * height);
        else Ready.localPosition = out_vec;
    }

    public void Get_Button()
    {
        if (Mathf.Abs(pos_num) == 0)
        {
            Audio.GetComponent<AudioSource>().PlayOneShot(Home_SE[2]);//Button
            select.GetComponent<RectTransform>().localPosition = Vector3.zero;
            //Audio.GetComponent<AudioSource>().clip = Home_SE[1];
        }
    }
    #endregion
    #region 0の関数
    public void Game_Start(int easy)
    {
        for (int i = 0; i < 3; i++)
        {
            PlayerPrefs.SetInt("Party" + i, charas[party_ID[i] - 1].chara_ID);
            PlayerPrefs.SetInt("Box_Party" + i, party_ID[i]);
            PlayerPrefs.SetInt("Level" + i, charas[party_ID[i] - 1].Level);
        }
        //SceneManager.LoadScene("Tutorial");
        if (easy == 0)
        {
            hikousen_vec = new Vector3(-0.22f * width, -0.3f * height, 0);
            Fader.Instance.LoadScene("Mini", 3.0f);
        }
        else
        {
            hikousen_vec = new Vector3(0.23f * width, -0.1f * height, 0);
            Fader.Instance.LoadScene("Tutorial", 3.0f);
        }
        scene_fin = true;
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
            detail.localPosition = Vector3.zero;
            Big_img.sprite = charas[x].Big_img;
            Dtitle.text = charas[x].Name + "\nLv." + (charas[x].Level + 1);
            text.text = "攻撃力：" + charas[x].attack + "   HP：" + charas[x].HP
                + "\n\nリーダー：" + charas[x].leader_Description
                + "\nスキル：" + charas[x].skill_time + "秒　発動：" + charas[x].skill_walk
                + "歩\n" + charas[x].skill_Description;
        }
    }

    public void Set_ID(int ID)
    {
        if (tap_num > 0&& charas[tap_num - 1].img.color != Color.gray) charas[tap_num - 1].img.color = Color.white;
        tap_num = ID;
        tap_second = 0;
        is_tap = true;
        in_flg = 1;
        if (charas[ID - 1].chara_ID != 0 && charas[tap_num - 1].img.color != Color.gray)
        {
            //carsol.GetComponent<RectTransform>().localPosition = new Vector3((-0.375f + (ID - 1) % 4 * 0.25f) * width, -0.05f * height - Mathf.FloorToInt((ID - 1) / 4) * 0.25f * width);
            
            charas[tap_num - 1].img.color = new Color(1,0.6f,0.6f,1);
        }
        //else carsol.GetComponent<RectTransform>().localPosition = out_vec;
    }
    public void Set_ID_Up(int ID)
    {
        if (tap_num == ID && tap_second < 1 && is_tap)
        {
            if (target_num != 0)
            {
                party_chara[target_num - 1].color = Color.white;
                if (charas[tap_num - 1].chara_ID != 0 && party_ID[0] != tap_num && party_ID[1] != tap_num && party_ID[2] != tap_num)
                {
                    charas[tap_num - 1].img.color = Color.white;
                    party_chara[target_num - 1].color = Color.white;
                    charas[party_ID[target_num - 1] - 1].img.color = Color.white;
                    charas[tap_num - 1].img.color = Color.gray;
                    party_ID[target_num - 1] = tap_num;
                    party_chara[target_num - 1].sprite = charas[tap_num - 1].Middle_img;
                    home_img.sprite = charas[party_ID[0] - 1].Home_img;
                    Ptext[target_num-1].text = "Lv." + (charas[party_ID[target_num-1] - 1].Level + 1);
                    HP.text = "HP合計（制限時間）: " + (charas[party_ID[0] - 1].HP + charas[party_ID[1] - 1].HP + charas[party_ID[2] - 1].HP);
                    tap_num = 0;
                }
                target_num = 0;
            }
        }
        is_tap = false;
    }

    public void Outer()
    {
        if (is_tap)
        {
            if(tap_num >0&&charas[tap_num - 1].img.color != Color.gray) charas[tap_num - 1].img.color = Color.white;
            if(target_num>0)party_chara[target_num - 1].color = Color.white;
            is_tap = false;
            tap_num = 0;
            target_num = 0;
        }
    }

    public void Set_party(int ID)
    {
        int tmp = target_num;
        target_num = ID;
        tap_second = 0;
        in_flg = 0;
        //if (party_chara[tap_num - 1].img.color != Color.gray)
        {
            if(tmp>0) party_chara[tmp - 1].color = Color.white;
            party_chara[target_num - 1].color = new Color(1, 0.6f, 0.6f, 1);
        }
        is_tap = true;
    }
    public void Set_party_Up(int ID)
    {
        if (target_num == ID && tap_second < 1 && is_tap)
        {
            if (tap_num != 0 && charas[tap_num - 1].chara_ID != 0 && party_ID[0] != tap_num && party_ID[1] != tap_num && party_ID[2] != tap_num)
            {
                charas[tap_num - 1].img.color = Color.white;
                party_chara[target_num - 1].color = Color.white;
                charas[party_ID[target_num - 1] - 1].img.color = Color.white;
                charas[tap_num - 1].img.color = Color.gray;
                party_ID[target_num - 1] = tap_num;
                home_img.sprite = charas[party_ID[0] - 1].Home_img;
                    Ptext[target_num-1].text = "Lv." + (charas[party_ID[target_num-1] - 1].Level + 1);
        HP.text = "HP合計（制限時間）: " + (charas[party_ID[0] - 1].HP + charas[party_ID[1] - 1].HP + charas[party_ID[2] - 1].HP);
                party_chara[target_num - 1].sprite = charas[tap_num - 1].Middle_img;
                target_num = 0;
            }
            tap_num = 0;
        }
        is_tap = false;
    }
    #endregion
    #region 3の関数
    #endregion
    #region 4の関数
    #endregion
}
