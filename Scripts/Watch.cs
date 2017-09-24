using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Watch : MonoBehaviour
{
    public GameObject detail, Big, text, Menu, Title;//Canvas
    public GameObject BG,carsol,home_img;//BackCanvas
    public GameObject select, back_home, stage1;//Select
    public GameObject[] box_Chara = new GameObject[8];
    public GameObject[] party_Chara = new GameObject[3];



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
        stage1.GetComponent<RectTransform>().localPosition = new Vector3(0, -0.43f * height, 0);
        stage1.GetComponent<RectTransform>().sizeDelta = new Vector2(0.2f*width, 0.15f * height);

        detail.GetComponent<RectTransform>().localPosition = max_vec;
        detail.GetComponent<RectTransform>().sizeDelta = max_vec;
        Big.GetComponent<RectTransform>().localPosition = new Vector3(0, 0.1f * height, 0);
        Big.GetComponent<RectTransform>().sizeDelta = new Vector2(0.8f * width, width);
        text.GetComponent<RectTransform>().localPosition = new Vector3(0, -0.3f * height, 0);
        text.GetComponent<RectTransform>().sizeDelta = new Vector2(0.9f * width, 0.15f * height);

        Title.GetComponent<RectTransform>().localPosition = new Vector3(0, 0.45f * height, 0);
        Title.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 0.1f * height);
        Menu.GetComponent<RectTransform>().localPosition = new Vector3(0, -0.43f * height, 0);
        Menu.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 0.14f * height);

        BG.GetComponent<RectTransform>().sizeDelta = new Vector2(5f * width, height);
        BG.GetComponent<RectTransform>().localPosition = new Vector2(2f * width, 0);
        carsol.GetComponent<RectTransform>().sizeDelta = new Vector2(0.24f * width, 0.2f*height);
        carsol.GetComponent<RectTransform>().localPosition = max_vec;
        home_img.GetComponent<RectTransform>().sizeDelta = new Vector2(1.4f*width, 1.1f*height);
        home_img.GetComponent<RectTransform>().localPosition = new Vector2(-2.1f * width, -0.1f*height);

        for (int n = 0; n < 8; n++)
        {
            int i = Mathf.FloorToInt(n / 4f);
            int j = Mathf.RoundToInt(n % 4f);
            box_Chara[n].GetComponent<RectTransform>().sizeDelta = new Vector2(0.25f * width, 0.25f * width);
            box_Chara[n].GetComponent<RectTransform>().localPosition = new Vector3((-0.375f + j * 0.25f) * width, -0.05f * height - i * 0.25f * width);
        }
        party_Chara[0].GetComponent<RectTransform>().sizeDelta = new Vector2(0.33f * width, 0.33f * width);
        party_Chara[0].GetComponent<RectTransform>().localPosition = new Vector3(0.33f * width, 0.25f * height);
        party_Chara[1].GetComponent<RectTransform>().sizeDelta = new Vector2(0.33f * width, 0.33f * width);
        party_Chara[1].GetComponent<RectTransform>().localPosition = new Vector3(0, 0.25f * height);
        party_Chara[2].GetComponent<RectTransform>().sizeDelta = new Vector2(0.33f * width, 0.33f * width);
        party_Chara[2].GetComponent<RectTransform>().localPosition = new Vector3(-0.33f * width, 0.25f * height);


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
            chara.Big_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/1-Soldier/Big");
            chara.Small_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/1-Soldier/player0-1");
            chara.attack = 95;//+2*level;
            chara.HP = 50;//+level
            chara.skill_Description = "早く敵を倒すことが出来る";
            chara.skill_walk = 25;
            chara.skill_time = 5;
        }
        else if (ID == 2)//魔女
        {
            chara.Big_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/2-Witch/Big");
            chara.Small_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/2-Witch/majio0-1");
            chara.attack = 50;//+level
            chara.HP = 80;//+4*level;
            chara.skill_Description = "見えている敵全体に攻撃";
            chara.skill_walk = 25;
            chara.skill_time = 1;
        }
        else if (ID == 3)//海賊
        {
            chara.Big_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/3-Pirate/Big");
            chara.Small_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/3-Pirate/kaizoku0-1");
            chara.attack = 60;//+level;
            chara.HP = 160;//+7*level
            chara.skill_Description = "アイテムの位置が分かる";
            chara.skill_walk = 25;
            chara.skill_time = 10;
        }
        else if (ID == 4)//女剣士
        {
            chara.Big_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/4-WSoldier/Big");
            chara.Small_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/4-WSoldier/Wsoldier0-1");
            chara.attack = 90;//+2*level
            chara.HP = 60;//+level
            chara.skill_Description = "早く敵を倒すことが出来る";
            chara.skill_walk = 25;
            chara.skill_time = 8;
        }
        else//データなし
        {
            chara.Big_img = Resources.Load<Sprite>("Images/GameScene/Road0");
            chara.Small_img = Resources.Load<Sprite>("Images/GameScene/Road0");
            chara.attack = 1;
            chara.HP = 1;
            chara.skill_Description = "データなし";
            chara.skill_walk = 100;
            chara.skill_time = 0;
        }
    }

    public Box_Chara[] get_Chara(int num)
    {
        Box_Chara[] chara = new Box_Chara[num];
        for (int i = 0; i < num; i++)
        {
            chara[i] = new Box_Chara(box_Chara[i], i + 1);
            int id = PlayerPrefs.GetInt("Box_ID" + (i + 1), 0);
            int level = PlayerPrefs.GetInt("Box_LV" + (i + 1), 0);
            in_data(chara[i], id, level);
            chara[i].obj.GetComponent<Image>().sprite = chara[i].Small_img;
        }
        return chara;
    }
}

public class Box_Chara : MonoBehaviour
{
    public Sprite Big_img, Small_img;
    public int Level, chara_ID, box_ID;
    public int attack, HP, skill_walk, skill_time;
    public string skill_Description;
    public GameObject obj;

    public Box_Chara(GameObject o, int ID)
    {
        obj = o;
        box_ID = ID;
    }
}
