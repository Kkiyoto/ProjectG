using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Result : Functions
{
    float width, height;
    int flg,count,num;
    GameObject obj,o;
    float delta_time;
    int[] datas = new int[6]; //順にtime,Life,Enemy,Coin,treasure普通、レア

    // Use this for initialization
    void Start()
    {
        width = Screen.width;
        height = Screen.height;
        o = GameObject.Find("Title");
        o.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
        o.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
        datas[0] = PlayerPrefs.GetInt("Time", 0);
        datas[1] = PlayerPrefs.GetInt("Life", 0);
        datas[2] = PlayerPrefs.GetInt("enemy", 0);
        datas[3] = PlayerPrefs.GetInt("Coin", 0);
        datas[4] = PlayerPrefs.GetInt("treasure0", 0);
        datas[5] = PlayerPrefs.GetInt("treasure1", 0);
        obj = GameObject.Find("Time");
        obj.GetComponent<RectTransform>().localPosition = new Vector3(0.2f * width, 0.3f  * height);
        obj.GetComponent<RectTransform>().sizeDelta = new Vector2(0.4f * width, 0.1f * height);
        flg = 0;
        count = 0;
        num = 0;
        delta_time = 0;
    }
	
	// Update is called once per frame
	void Update ()
    {
        Debug.Log("flg=" + flg);
        if (Input.GetMouseButtonUp(0)) SceneManager.LoadScene("start");
        #region flg=-1:  イメージ画像のアニメ 考えていないのでパス
        if (flg < 0)
        {
            GameObject Title = GameObject.Find("Title");
            Vector3 vec = Title.GetComponent<RectTransform>().localPosition;
            Title.GetComponent<RectTransform>().localPosition = 14f * vec / 15f;
            if (vec.magnitude < 0.05f)
            {
                Title.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
                if (count >= datas[flg])
                {
                    flg++;
                    count = 0;
                }
            }
        }
        #endregion
        #region flg=0:  Time
        else if (flg == 0)
        {
            if (count < datas[0]) count += Random.Range(3,8);
            else if (count > datas[0]) count--;
            int m = Mathf.FloorToInt(count / 60);
            int s = count % 60;
            obj.GetComponent<Text>().text = (m.ToString().PadLeft(2, '0') + " : " + s.ToString().PadLeft(2, '0'));
            if (count == datas[0])
            {
                flg++;
                count = 0;
                obj = GameObject.Find("Life");
                obj.GetComponent<Animator>().SetInteger("Stamp_Int", datas[1]);
            }
        }
        #endregion
        #region flg=1:  Life
        else if (flg == 1)
        {
            delta_time += Time.deltaTime;
            if (delta_time > 1)
            {
                flg++;
                obj = GameObject.Find("Enemy");
                obj.GetComponent<Animator>().SetInteger("Stamp_Int", datas[2]);
                delta_time = 0;
            }
        }
        #endregion
        #region flg=2:  Enemy
        else if (flg == 2)
        {
            delta_time += Time.deltaTime;
            if (delta_time > 1)
            {
                flg++;
                obj = GameObject.Find("Coin");
                obj.GetComponent<RectTransform>().localPosition = new Vector3(0.2f * width, 0.1f * height);
                obj.GetComponent<RectTransform>().sizeDelta = new Vector2(0.4f * width, 0.1f * height);
            }
        }
        #endregion
        #region flg=3:  Coin
        else if (flg == 3)
        {
            if (count < datas[3]) count += Random.Range(10, 20);
            else if (count > datas[3]) count--;
            obj.GetComponent<Text>().text = (count.ToString());
            if (count == datas[3])
            {
                flg++;
                count = 0;
                obj = GameObject.Find("Treasure");
                //obj.transform.parent = GameObject.Find("Canvas").transform;
            }
        }
        #endregion
        #region flg=4:  Treasure_Coin
        else if (flg == 4)
        {
            Vector3 vec = obj.transform.position;
            obj.transform.position = (14f * vec + new Vector3(0, -1, 0))/15f;
            if ((vec - new Vector3(0, -1, 0)).magnitude < 0.05f&& obj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Treasure"))
            {
                obj.transform.position = new Vector3(0, -1, 0);
                obj.GetComponent<Animator>().SetBool("Open_Bool", true);
                o = Instantiate(Resources.Load<GameObject>("Prefab/Get")) as GameObject;
                o.name = "Treasure" + num;
                num++;
            }
            if (obj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Treasure_Open")
        && obj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 2f)
            {
                count++;
                o.transform.position = new Vector3(0, -0.5f - Mathf.Cos(count), 0);
                if (count > 40)
                {
                    o.GetComponent<SpriteRenderer>().sortingOrder = 3;
                    Vector3 scale = o.GetComponent<RectTransform>().localScale;
                    if (scale.magnitude<4)
                    {
                        scale += scale / scale.magnitude * 0.5f;
                        o.GetComponent<RectTransform>().localScale = scale;
                    }
                }
            }

            if (count == datas[3])
            {
                flg++;
                count = 0;
                obj = GameObject.Find("Treasure");
            }
        }
        #endregion

    }
}