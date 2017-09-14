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

    #region タッチエフェクト追記
    [SerializeField] ParticleSystem touchEffect;    // タッチの際のエフェクト
    [SerializeField] Camera _camera;                // カメラの座標
    #endregion

    // Use this for initialization
    void Start()
    {
        width = Screen.width;
        height = Screen.height;
        /*o = GameObject.Find("Title");
        o.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
        o.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);*/
        datas[0] = PlayerPrefs.GetInt("Time", 0);
        datas[1] = PlayerPrefs.GetInt("Life", 0);
        datas[2] = PlayerPrefs.GetInt("enemy", 0);
        datas[3] = PlayerPrefs.GetInt("Coin", 0);
        datas[4] = PlayerPrefs.GetInt("treasure0", 0);
        datas[5] = PlayerPrefs.GetInt("treasure1", 0);
        obj = GameObject.Find("Time");
        obj.GetComponent<RectTransform>().localPosition = new Vector3(0.18f * width, 0.34f  * height);
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
                obj.GetComponent<RectTransform>().localPosition = new Vector3(0.13f * width, 0.065f * height);
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
                obj.GetComponent<RectTransform>().sizeDelta = new Vector3(0.35f * width, 0.35f * width);
                GameObject.Find("Box").GetComponent<RectTransform>().sizeDelta = new Vector3(0.35f * width, 0.35f * width);
                //obj.transform.parent = GameObject.Find("Canvas").transform;
                for (int i = 0; i < datas[4]; i++)
                {
                    o = Instantiate(Resources.Load<GameObject>("Prefab/Get")) as GameObject;
                    o.name = "Get" + i;
                    o.transform.parent = obj.transform;
                    o.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Item/sword1"); 
                    o.transform.SetSiblingIndex(0);
                    o.GetComponent<RectTransform>().localPosition = new Vector3(width, height, 0);
                }
            }
        }
        #endregion
        #region flg=4:  Treasure_Coin
        else if (flg == 4)
        {
            if (num < datas[4])
            {
                Vector3 vec = obj.GetComponent<RectTransform>().localPosition;
                obj.GetComponent<RectTransform>().localPosition = (14f * vec + new Vector3(0, -0.15f*height, 0)) / 15f;
                Debug.Log("g" + obj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Openning")
                + " " + (obj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 2f));
                if ((vec - new Vector3(0, -0.15f * height, 0)).magnitude < 0.05f)
                {
                    if (obj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Closed"))
                    {
                        obj.GetComponent<RectTransform>().localPosition = new Vector3(0, -0.15f * height, 0);
                        obj.GetComponent<Animator>().SetBool("Open_Bool", true);
                        o = GameObject.Find("Get" + num);
                        o.GetComponent<Image>().color = Color.white;
                    }
                    else if (obj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Openning")
                && obj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 2f)
                    {
                        count++;
                        if (count < 90)
                        {
                            o.GetComponent<RectTransform>().localPosition = new Vector3(0, -height * 0.05f * Mathf.Cos(Mathf.PI / 45f * count), 0);
                            Vector2 scale = o.GetComponent<RectTransform>().sizeDelta;
                            //Debug.Log("name : " + o.name+"   "+scale);
                            if (scale.x < 0.5f * height)
                            {
                                scale += new Vector2(2, 2);
                                o.GetComponent<RectTransform>().sizeDelta = scale;
                            }
                            if (count == 40)
                            {
                                o.transform.SetAsLastSibling();
                            }
                        }
                        else
                        {
                            obj.GetComponent<RectTransform>().sizeDelta -= new Vector2(2, 2);
                            GameObject.Find("Box").GetComponent<RectTransform>().sizeDelta = obj.GetComponent<RectTransform>().sizeDelta;
                            if (obj.GetComponent<RectTransform>().sizeDelta.x < 1)
                            {
                                o.transform.parent = GameObject.Find("Canvas").transform;
                                o.GetComponent<RectTransform>().localPosition = new Vector3(0.3f * (num - 1) * width, -0.3f * height);
                                num++;
                                obj.GetComponent<RectTransform>().sizeDelta = new Vector3(0.35f * width, 0.35f * width);
                                obj.GetComponent<Animator>().SetBool("Open_Bool", false);
                                GameObject.Find("Box").GetComponent<RectTransform>().sizeDelta = new Vector3(0.35f * width, 0.35f * width);
                                obj.GetComponent<RectTransform>().localPosition = new Vector3(width, -0.15f * height);
                                count = 0;
                                //o = GameObject.Find("Get"+num);
                            }
                        }
                    }
                }
            }
            else
            {
                flg++;
                count = 0;
                for (int i = 0; i < datas[5]; i++)
                {
                    o = Instantiate(Resources.Load<GameObject>("Prefab/Get")) as GameObject;
                    o.name = "GetRare" + i;
                    o.transform.parent = GameObject.Find("Treasure").transform;
                    o.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Item/sword2");
                }
            }
        }
        #endregion

        #region タッチエフェクト
        // 画面のどこでもタッチでエフェクト
        if (Input.GetMouseButtonDown(0))
        {
            // マウスのワールド座標までパーティクルを移動,エフェクトを1つ生成する
            var pos = _camera.ScreenToWorldPoint(Input.mousePosition + _camera.transform.forward * 10);
            touchEffect.transform.position = pos;
            touchEffect.Emit(1);
        }
        // 使用する際はSub_cameraとTouch_particleオブジェクトを追加してください
        #endregion
    }
}