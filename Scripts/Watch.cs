using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Watch : MonoBehaviour
{
    bool flg;//全画面なら真
    int box_num_x,box_num_y;
    Box_Chara[,] charas = new Box_Chara[2, 4];
    public GameObject back,Big,text;
    float width, height;
    

	// Use this for initialization
	void Start ()
    {
        width = Screen.width;
        height = Screen.height;
        flg = false;
        box_num_x = 0;
        box_num_y = 0;
        for(int i = 0; i < 2; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                int num = 4 * i + j + 1;
                GameObject o = Instantiate(Resources.Load<GameObject>("Prefab/box_chara")) as GameObject;
                charas[i, j] = new Box_Chara(o, num);
                int id = PlayerPrefs.GetInt("Box_ID" + num, 0);
                int level = PlayerPrefs.GetInt("Box_LV" + num, 0);
                in_data(charas[i,j],id, level);
            }
        }
        back.GetComponent<RectTransform>().localPosition = new Vector3(width, 0, 0);
        back.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        Big.GetComponent<RectTransform>().localPosition = new Vector3(0, 0.1f*height, 0);
        Big.GetComponent<RectTransform>().sizeDelta = new Vector2(0.8f*width, 0.8f*width);
        text.GetComponent<RectTransform>().localPosition = new Vector3(0, -0.3f*height, 0);
        text.GetComponent<RectTransform>().sizeDelta = new Vector2(0.9f*width, 0.15f*height);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (flg)
        {
            if (Input.GetMouseButtonUp(0))
            {
                flg = false;
                back.GetComponent<RectTransform>().localPosition = new Vector3(width, 0, 0);
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                int x = Mathf.RoundToInt(vec.x);
                int y = Mathf.RoundToInt(vec.y);
                if (x > 0 && y < 0 && x < 4 && y > -2)
                {
                    box_num_x = x;
                    box_num_y = y;
                    flg = true;
                    back.GetComponent<RectTransform>().localPosition = Vector3.zero;
                    Big.GetComponent<Image>().sprite = charas[x, y].Big_img;
                    text.GetComponent<Text>().text = "スキル発動時間：" + charas[x, y].skill_time + "秒　スキル補充：" + charas[x, y].skill_walk + "\n説明：" + charas[x, y].skill_Description;
                }
            }

        }
    }

    public void in_data(Box_Chara chara, int ID, int level)
    {
        chara.Level = level;
        chara.chara_ID = ID;
        if(ID==1)//剣士
        {
            chara.Big_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/1-Soldier/Big");
            chara.Small_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/1-Soldier/player0-0");
            chara.attack = 30;
            chara.skill_Description = "早く敵を倒すことが出来る";
            chara.skill_walk = 25;
            chara.skill_time = 5;
        }
        else if (ID == 2)//魔女
        {
            chara.Big_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/2-Witch/Big");
            chara.Small_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/2-Witch/majio0-0");
            chara.attack = 60;
            chara.skill_Description = "見えている敵全体に攻撃";
            chara.skill_walk = 25;
            chara.skill_time = 1;
        }
        else if (ID == 1)//剣士
        {
            chara.Big_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/3-Pirate/Big");
            chara.Small_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/3-Pirate/kaizoku0-0");
            chara.attack = 60;
            chara.skill_Description = "アイテムの位置が分かる";
            chara.skill_walk = 25;
            chara.skill_time = 10;
        }
        else if (ID == 1)//女剣士
        {
            chara.Big_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/4-WSoldier/Big");
            chara.Small_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/4-WSoldier/Wsoldier0-0");
            chara.attack = 30;
            chara.skill_Description = "早く敵を倒すことが出来る";
            chara.skill_walk = 25;
            chara.skill_time = 5;
        }
        else//データなし
        {
        }
    }
}

public class Box_Chara : MonoBehaviour
{
    public Sprite Big_img,Small_img;
    public int Level, chara_ID,box_ID;
    public int attack,skill_walk,skill_time;
    public string skill_Description;
    GameObject obj;
    public Box_Chara(GameObject o,int ID)
    {
        obj = o;
        box_ID = ID;
    }
}
