using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Watch : MonoBehaviour
{
    public GameObject detail, Big, text, Menu, Title,Dtitle;//Canvas
    public GameObject BG,home_img,HP;//BackCanvas
    public GameObject select, back_home, stage3,stage2,stage1,stage0;//Select
    public GameObject[] box_Chara = new GameObject[12],texts=new GameObject[12];
    public GameObject[] party_Chara = new GameObject[3],Ptext=new GameObject[3];
    public RectTransform Ready;



    // Use this for initialization
    void Start()
    {
        float width = Screen.width;
        float height = Screen.height;
        float W_height = 10;
        float W_width = width / height * W_height;
        Vector2 max_vec = new Vector2(width, height);
        select.GetComponent<RectTransform>().localPosition = max_vec;
        select.GetComponent<RectTransform>().sizeDelta = max_vec;
        back_home.GetComponent<RectTransform>().localPosition = new Vector3(-0.34f*width, -0.46f * height, 0);
        back_home.GetComponent<RectTransform>().sizeDelta = new Vector2(0.3f*width, 0.06f * height);
        stage3.GetComponent<RectTransform>().localPosition = new Vector3(0.17f * width, 0.36f * height, 0);
        stage3.GetComponent<RectTransform>().sizeDelta = new Vector2(0.52f * width, 0.35f * height);
        stage2.GetComponent<RectTransform>().localPosition = new Vector3(-0.22f * width, 0.07f * height, 0);
        stage2.GetComponent<RectTransform>().sizeDelta = new Vector2(0.50f * width, 0.3f * height);
        stage1.GetComponent<RectTransform>().localPosition = new Vector3(0.23f * width, -0.1f * height, 0);
        stage1.GetComponent<RectTransform>().sizeDelta = new Vector2(0.48f * width, 0.2f * height);
        stage0.GetComponent<RectTransform>().localPosition = new Vector3(-0.22f * width, -0.3f * height, 0);
        stage0.GetComponent<RectTransform>().sizeDelta = new Vector2(0.4f * width, 0.17f * height);

        detail.GetComponent<RectTransform>().localPosition = max_vec;
        detail.GetComponent<RectTransform>().sizeDelta = max_vec;
        Big.GetComponent<RectTransform>().localPosition = new Vector3(0, 0.1f * height, 0);
        Big.GetComponent<RectTransform>().sizeDelta = new Vector2(0.95f*width, 1.3f*width);
        Dtitle.GetComponent<RectTransform>().localPosition = new Vector3(0, 0.4f * height, 0);
        Dtitle.GetComponent<RectTransform>().sizeDelta = new Vector2(0.95f * width, 0.2f * width);
        text.GetComponent<RectTransform>().localPosition = new Vector3(0, -0.28f * height, 0);
        text.GetComponent<RectTransform>().sizeDelta = new Vector2(0.9f * width, 0.25f * height);

        Title.GetComponent<RectTransform>().localPosition = new Vector3(0, 0.451f * height, 0);
        Title.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 0.11f * height);
        Menu.GetComponent<RectTransform>().localPosition = new Vector3(0, -0.44f * height, 0);
        Menu.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 0.12f * height);

        BG.GetComponent<RectTransform>().sizeDelta = new Vector2(5f * width, height);
        BG.GetComponent<RectTransform>().localPosition = new Vector2(2f * width, 0);
        HP.GetComponent<RectTransform>().sizeDelta = new Vector2(0.6f * width, 0.1f*height);
        HP.GetComponent<RectTransform>().localPosition = new Vector3(0.2f*width,0.12f*height);
        home_img.GetComponent<RectTransform>().sizeDelta = new Vector2(1.3f*width, 1.05f*height);
        home_img.GetComponent<RectTransform>().localPosition = new Vector2(-2.1f * width, -0.1f*height);

        Ready.sizeDelta = new Vector2(0.34f * width, 0.2f * height);
        Ready.localPosition = new Vector3(0.3f * width, -0.26f * height);

        GameObject.Find("Hikousen").GetComponent<RectTransform>().sizeDelta = new Vector2(0.2f * width, 0.1f * height);


        for (int n = 0; n < 12; n++)
        {
            int i = Mathf.FloorToInt(n / 4f);
            int j = Mathf.RoundToInt(n % 4f);
            box_Chara[n].GetComponent<RectTransform>().sizeDelta = new Vector2(0.25f * width, 0.15f * height);
            box_Chara[n].GetComponent<RectTransform>().localPosition = new Vector3((-0.375f + j * 0.25f) * width, (0.02f - 0.15f * i) * height);
            texts[n].GetComponent<RectTransform>().sizeDelta = new Vector2(0.23f * width, 0.14f * height);
            texts[n].GetComponent<Text>().fontSize = Mathf.RoundToInt(0.03f * height);
            //texts[n].GetComponent<RectTransform>().localPosition = new Vector3(0,  - 0.1f  * height);
        }
        //GameObject.Find("box_C0").GetComponent<RectTransform>().sizeDelta = new Vector2(0.25f * width, 0.15f * height);
        //GameObject.Find("box_C1").GetComponent<RectTransform>().sizeDelta = new Vector2(0.25f * width, 0.15f * height);
        //GameObject.Find("box_C2").GetComponent<RectTransform>().sizeDelta = new Vector2(0.25f * width, 0.15f * height);
        party_Chara[0].GetComponent<RectTransform>().sizeDelta = new Vector2(0.33f * width, 0.25f * height);
        party_Chara[0].GetComponent<RectTransform>().localPosition = new Vector3(0.33f * width, 0.26f * height);
        party_Chara[1].GetComponent<RectTransform>().sizeDelta = new Vector2(0.33f * width, 0.25f * height);
        party_Chara[1].GetComponent<RectTransform>().localPosition = new Vector3(0, 0.26f * height);
        party_Chara[2].GetComponent<RectTransform>().sizeDelta = new Vector2(0.33f * width, 0.25f * height);
        party_Chara[2].GetComponent<RectTransform>().localPosition = new Vector3(-0.33f * width, 0.26f * height);
        Ptext[0].GetComponent<RectTransform>().sizeDelta = new Vector2(0.3f * width, 0.22f * height);
        Ptext[1].GetComponent<RectTransform>().sizeDelta = new Vector2(0.3f * width, 0.22f * height);
        Ptext[2].GetComponent<RectTransform>().sizeDelta = new Vector2(0.3f * width, 0.22f * height);
        Ptext[0].GetComponent<Text>().fontSize = Mathf.RoundToInt(0.03f * height);
        Ptext[1].GetComponent<Text>().fontSize = Mathf.RoundToInt(0.03f * height);
        Ptext[2].GetComponent<Text>().fontSize = Mathf.RoundToInt(0.03f * height);


        Destroy(this.gameObject, 1f);



        /*PlayerPrefs.SetInt("Box_ID" + 1, 3);
        PlayerPrefs.SetInt("Box_ID" + 2, 2);
        PlayerPrefs.SetInt("Box_ID" + 3, 4);
        PlayerPrefs.SetInt("Box_ID" + 4, 3);
        PlayerPrefs.SetInt("Box_ID" + 5, 2);*/
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void in_data(Box_Chara chara, int ID, int level)
    {
        chara.Level = level;
        chara.chara_ID = ID;
        if (ID == 1)//剣士
        {
            chara.Name = "剣士：";
            chara.Big_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/1-Soldier/Big");
            chara.Middle_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/1-Soldier/player0-2");
            chara.Small_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/1-Soldier/player0-1");
            chara.Home_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/1-Soldier/player0-1");
            chara.attack = 95 + 2 * level;
            chara.HP = 50 + level;
            chara.skill_Description = "いつもより早く盤面上を走り抜ける";
            chara.skill_walk = 25;
            chara.skill_time = 5;
        }
        else if (ID == 2)//魔女
        {
            chara.Name = "魔女：モニカ・アローウ";
            chara.Big_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/2-Witch/Big");
            chara.Middle_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/2-Witch/Middle");
            chara.Small_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/2-Witch/Small");
            chara.Home_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/2-Witch/Home");
            chara.attack = 32 + level;
            chara.HP = 80+4*level;
            chara.skill_Description = "敵を凍らせ、動けなくする";
            chara.leader_Description = "制限時間が減るのが遅くなる";
            chara.skill_walk = 25;
            chara.skill_time = 10;
        }
        else if (ID == 3)//海賊
        {
            chara.Name = "盗賊：ジェーン・デニス";
            chara.Big_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/3-Pirate/Big");
            chara.Middle_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/3-Pirate/Middle");
            chara.Small_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/3-Pirate/Small");
            chara.Home_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/3-Pirate/Home");
            chara.attack = 60+level;
            chara.HP = 160 + 7 * level;
            chara.skill_Description = "同じ盤面にいる敵を、全て打ち抜く";
            chara.leader_Description = "Map上でどこに宝物があるのか分かる";
            chara.skill_walk = 25;
            chara.skill_time = 10;
        }
        else if (ID == 4)//女剣士
        {
            chara.Name = "剣士：ケリー・ロゼッタ";
            chara.Big_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/4-WSoldier/Big");
            chara.Middle_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/4-WSoldier/Middle");
            chara.Small_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/4-WSoldier/Small");
            chara.Home_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/4-WSoldier/Home");
            chara.attack = 100 + 2 * level;
            chara.HP = 60 + level;
            chara.skill_Description = "進んでいる方向を変える";
            chara.leader_Description = "盤面を早く駆けぬける";
            chara.skill_walk = 25;
            chara.skill_time = 8;
        }
        else if (ID == 5)
        {
            chara.Small_img = Resources.Load<Sprite>("Images/Home/C1");
        chara.chara_ID = 0;
        }
        else if (ID == 6)
        {
            chara.Small_img = Resources.Load<Sprite>("Images/Home/C2");
        chara.chara_ID = 0;
        }
        else if (ID == 7)
        {
            chara.Small_img = Resources.Load<Sprite>("Images/Home/Cha1");
            chara.chara_ID = 0;
        }
        else if (ID == 8)
        {
            chara.Small_img = Resources.Load<Sprite>("Images/Home/Cha2");
            chara.chara_ID = 0;
        }
        else//データなし
        {
            chara.Big_img = Resources.Load<Sprite>("Images/GameScene/Road0");
            chara.Middle_img = Resources.Load<Sprite>("Images/Result/waku");
            chara.Small_img = Resources.Load<Sprite>("Images/Result/waku");
            chara.attack = 1;
            chara.HP = 1;
            chara.skill_Description = "データなし";
            chara.skill_walk = 100;
            chara.skill_time = 0;
        }
    }

    public Box_Chara[] get_Chara(int num)
    {
        Box_Chara[] chara = new Box_Chara[12];
        int[] ids = { 2, 4,3, 2, 3, 4, 7, 8, 5, 6, 0, 0 };
        int[] levels = { 0, 0, 0, 6, 3, 2, 8, 5, 1, 3, -1, -1 };
        for (int i = 0; i < num; i++)
        {
            chara[i] = new Box_Chara(box_Chara[i], i + 1);
            in_data(chara[i], ids[i], levels[i]);
            chara[i].img.sprite = chara[i].Small_img;
            if(levels[i]!=-1) texts[i].GetComponent<Text>().text = "Lv." + (levels[i] + 1);
        }
        for (int i = num; i < 12; i++)
        {
            chara[i] = new Box_Chara(box_Chara[i], i + 1);
            //Box_Chara cha = new Box_Chara(box_Chara[i], i + 1);
            in_data(chara[i],ids[i], levels[i]);
            chara[i].img.sprite = chara[i].Small_img;
            if (levels[i] != -1) texts[i].GetComponent<Text>().text = "Lv." + (levels[i] + 1);
        }
        return chara;
    }
}

public class Box_Chara : MonoBehaviour
{
    public Sprite Big_img,Middle_img, Small_img,Home_img;
    public int Level, chara_ID, box_ID;
    public int attack, HP, skill_walk, skill_time;
    public string skill_Description,leader_Description,Name;
    GameObject obj;

    public Box_Chara(GameObject o, int ID)
    {
        obj = o;
        box_ID = ID;
    }

    public Image img
    {
        get { return obj.GetComponent<Image>(); }
    }
}
