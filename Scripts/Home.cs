using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Home : MonoBehaviour
{
    public GameObject back, Big_img, text;
    Box_Chara[,] charas = new Box_Chara[2, 4];
    int pos_num,top_x,top_y;
    Vector3[] Camera_Pos = new Vector3[5];
    float width, height;
    float top_second;

    // Use this for initialization
    void Start()
    {
        width = Screen.width;
        height = Screen.height;
        Watch watch = GameObject.Find("Watch").GetComponent<Watch>();
        charas = watch.get_Chara(2, 4);
        pos_num = 0;
        for (int i = 0; i < 5; i++) Camera_Pos[i] = new Vector3((i - 2) * width, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        back.GetComponent<RectTransform>().localPosition = (3f * Camera_Pos[pos_num] + back.GetComponent<RectTransform>().localPosition) / 4f;
        if (Input.GetMouseButtonDown(0))
        {
            top_second = 0;
            Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (vec.y < -0.4f * height)
            {
                int x = Mathf.RoundToInt(vec.x * 5f / width);
                Debug.Log(x.ToString());
            }
        }
        else if (top_second < 1) top_second += Time.deltaTime;
        #region 0 :キャラ鑑賞
        if (pos_num == 0)
        {

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

        }
        #endregion
        #region -2:キャラ詳細
        else if (pos_num== -2)
        {
            if (Input.GetMouseButtonUp(0))
            {
                back.GetComponent<RectTransform>().localPosition = new Vector3(width, 0, 0);
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

    void Charas(int x, int y)
    {
        if (x > 0 && y < 0 && x < 4 && y > -2)
        {
            pos_num = -2;
            back.GetComponent<RectTransform>().localPosition = Vector3.zero;
            Big_img.GetComponent<Image>().sprite = charas[x, y].Big_img;
            text.GetComponent<Text>().text = "スキル発動時間：" + charas[x, y].skill_time + "秒　スキル補充：" + charas[x, y].skill_walk + "\n説明：" + charas[x, y].skill_Description;
        }
    }

    public void Set_XY(int x,int y)
    {

    }
}
