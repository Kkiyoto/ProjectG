using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Result : MonoBehaviour
{
    float width, height;
    int flg,count;
    //GameObject Text_box;
    //Text text;
    int[] datas = new int[6];
    Line[,] lines = new Line[9, 9];
    GameObject Title;
    int[] light_direct = new int[2];
    int[] pos = new int[2];

	// Use this for initialization
	void Start ()
    {
        width = Screen.width;
        height = Screen.height;
        int goal = PlayerPrefs.GetInt("result", 0);
        Title = GameObject.Find("Title");
        Title.GetComponent<RectTransform>().localPosition = new Vector3(0, -height);
        Title.GetComponent<RectTransform>().sizeDelta = new Vector2(0.8f*width, 0.8f*width);
        Title.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Result/Result"+goal);
        int time =PlayerPrefs.GetInt("Time",0);
        int m = Mathf.FloorToInt(time / 60f);
        int s = Mathf.FloorToInt(time % 60f);
        GameObject.Find("Time").GetComponent<Text>().text= ("Time   " + m.ToString().PadLeft(2, '0') + " : " + s.ToString().PadLeft(2, '0'));
        /*
        datas[1]=PlayerPrefs.GetInt("Life", 0);
        datas[2] = PlayerPrefs.GetInt("Coin", 0);
        datas[3] = PlayerPrefs.GetInt("treasure0", 0);
        datas[4]=PlayerPrefs.GetInt("treasure1", 0);
        datas[5]=PlayerPrefs.GetInt("enemy",0);
        datas[6]=PlayerPrefs.GetInt("Length,0);
        
        for (int i = 0; i < 6; i++)
        {
            o = GameObject.Find("Num"+i);
            o.GetComponent<RectTransform>().localPosition = new Vector3(0.1f*width, (0.3f-0.07f*i) * height);
            o.GetComponent<RectTransform>().sizeDelta = new Vector2(0.5f*width, 0.07f * height);
            o.GetComponent<Text>().text = "";
            o = GameObject.Find("Text" + i);
            o.GetComponent<RectTransform>().localPosition = new Vector3(width, (0.3f - 0.07f * i) * height);
            o.GetComponent<RectTransform>().sizeDelta = new Vector2(0.4f * width, 0.08f * height);
        }*/
        flg = 0;
        count = 0;
        //text = GameObject.Find("Num0").GetComponent<Text>();
        //Text_box = GameObject.Find("Text0");
        for(int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                lines[i, j] = new Line(new Vector3(i, j, 0));
            }
        }
        pos[0] = 4;
        pos[1] = 3;
        light_direct[1] = (int)Common.Direction.Up;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetMouseButtonUp(0)) SceneManager.LoadScene("Tutorial");
        #region flg=0:  イメージ画像
        if (flg < 1)
        {
            //if (count < datas[flg]) count++;
            //text.text = count.ToString();
            Vector3 vec = Title.GetComponent<RectTransform>().localPosition;
            Title.GetComponent<RectTransform>().localPosition = (14f * vec + new Vector3(0,0.2f * height)) / 15f;
            if ((vec - new Vector3(0, 0.2f * height)).magnitude < 0.05f)
            {
                Debug.Log("end" + flg);
                Title.GetComponent<RectTransform>().localPosition = new Vector3(0, 0.2f * height);
                if (count >= datas[flg])
                {
                    flg++;
                    count = 0;
                    if (flg < 6)
                    {
                        //text = GameObject.Find("Num" + flg).GetComponent<Text>();
                        //Text_box = GameObject.Find("Text" + flg);
                    }
                }
            }
        }
        #endregion
        #region flg=1:  軌跡
        else if (flg == 1)
        {
            light_direct[0] = reverse(light_direct[1]);
            light_direct[1] = PlayerPrefs.GetInt("Road" + count, 0);
            PlayerPrefs.DeleteKey("Road" + count);//デバック終了時には使う。消さないで!
            count++;
            if (light_direct[1] == 0)
            {
                flg++;
                count = 0;
            }
            else
            {
                if(pos[0]>=0&&pos[0]<9&&pos[1]>=0&&pos[1]<9)lines[pos[0], pos[1]].Sprite(which_curve(light_direct[0], light_direct[1]));
                Pos_change(light_direct[1]);
            }
        }
        #endregion
    }

    public int which_curve(int start,int goal) //2:Right.3:Left,4:Up,5:Down
    {
        if (start == 2) //右から
        {
            if (goal == 3) return 4;
            else if (goal == 4) return 3;
            else if (goal == 5) return 2;
            else return 6;
        }
        else if (start == 3) //左から
        {
            if (goal == 2) return 4;
            else if (goal == 4) return 0;
            else if(goal==5)return 5;
            else return 6;
        }
        else if(start==4) //上から
        {
            if (goal == 2) return 3;
            else if (goal == 3) return 0;
            else if(goal==5) return 1;
            else return 6;
        }
        else //下から
        {
            if (goal == 2) return 2;
            else if (goal == 3) return 5;
            else if(goal==4) return 1;
            else return 6;
        }
    }

    public int reverse(int num)
    {
        if (num == 2) return 3;
        else if (num == 3) return 2;
        else if (num == 4) return 5;
        else return 4;
    }

    public void Pos_change(int direct)
    {
        if (direct == 2) pos[0]++;
        else if (direct == 3) pos[0]--;
        else if (direct == 4) pos[1]++;
        else pos[1]--;
    }
}
