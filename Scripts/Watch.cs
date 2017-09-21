using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Watch : MonoBehaviour
{
    public GameObject back,Big,text;
    public GameObject[,] box_Chara = new GameObject[2,4];



    // Use this for initialization
    void Start ()
    {
        float width = Screen.width;
        float height = Screen.height;
        float W_height = 10;
        float W_width = width / height * W_height;
        back.GetComponent<RectTransform>().localPosition = new Vector3(width, 0, 0);
        back.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        Big.GetComponent<RectTransform>().localPosition = new Vector3(0, 0.1f*height, 0);
        Big.GetComponent<RectTransform>().sizeDelta = new Vector2(0.8f*width, 0.8f*width);
        text.GetComponent<RectTransform>().localPosition = new Vector3(0, -0.3f*height, 0);
        text.GetComponent<RectTransform>().sizeDelta = new Vector2(0.9f*width, 0.15f*height);
        for(int i = 0; i < 2; i++)
        {
            for(int j = 0; j < 4; j++)
            {
                box_Chara[i, j].GetComponent<RectTransform>().localPosition = new Vector3((2.1f + j) * W_width, (0.1f - i) * W_height);
            }
        }


        Destroy(this.gameObject, 1f);
    }
	
	// Update is called once per frame
	void Update ()
    {

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
            chara.skill_time = 8;
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

    public Box_Chara[,] get_Chara(int x, int y)
    {
        Box_Chara[,] chara = new Box_Chara[x, y];
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                int num = 4 * i + j + 1;
                chara[i,j] = new Box_Chara(box_Chara[i,j], num);
                int id = PlayerPrefs.GetInt("Box_ID" + num, 0);
                int level = PlayerPrefs.GetInt("Box_LV" + num, 0);
                in_data(chara[i, j], id, level);
            }
        }
        return chara;
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
