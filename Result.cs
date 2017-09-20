using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/* Result
 * ゴール後のリザルト画面でのDirector
 * Directorにアタッチ
 * 上から順に何をさせようか見せてます。
 */

public class Result : Functions
{
    float width, height;
    int flg,count,num,coin;
    GameObject obj,o,Audio;
    float delta_time;
    int[] datas = new int[6]; //順にtime,Life,Enemy,Coin,treasure普通、レア
    Text text;

    public AudioSource[] Result_BGM;
    AudioClip[] Result_SE = new AudioClip[3];
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
        obj.GetComponent<RectTransform>().localPosition = new Vector3(0.18f * width, 0.34f * height);
        obj.GetComponent<RectTransform>().sizeDelta = new Vector2(0.4f * width, 0.1f * height);
        flg =0;
        count = 0;
        num = 0;
        delta_time = 0;
        text = obj.GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        Debug.Log("flg=" + flg);
        if (Input.GetKeyDown(KeyCode.Return)) Next();
        #region flg=-1:  イメージ画像のアニメ
        if (flg < 0)
        {
            RectTransform tra = obj.GetComponent<RectTransform>();
            if (tra.localPosition.magnitude > width / 61f) tra.Translate(new Vector3(width / 30f, 0));
            else if (tra.sizeDelta.y < 2f * height) tra.sizeDelta = (31f * tra.sizeDelta - new Vector2(0.36f * width, 0.063f * height)) / 30f;
            else
            {
                GameObject.Find("Title").GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Background/Result");
                Destroy(obj);
                flg++;
                count = 0;
                obj = GameObject.Find("end");
                obj.GetComponent<RectTransform>().localPosition = new Vector3(-width, 0.5f * height);
                obj.GetComponent<RectTransform>().sizeDelta = new Vector2(0.4f * width, 0.07f * height);
                text = obj.GetComponent<Text>();
            }
        }
        #endregion
        #region flg=0:  Time
        else if (flg == 0)
        {
            if (count < datas[0]) count += Random.Range(3, 8);
            else if (count > datas[0]) count--;
            int m = Mathf.FloorToInt(count / 60);
            int s = count % 60;
            text.text = (m.ToString().PadLeft(2, '0') + " : " + s.ToString().PadLeft(2, '0'));
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
            if (delta_time == 0)
            {
                Result_BGM[2].Stop();
                Audio.GetComponent<AudioSource>().PlayOneShot(Result_SE[0]); // ドン！
            }
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
            if (delta_time == 0)
                Audio.GetComponent<AudioSource>().PlayOneShot(Result_SE[0]); // ドン！
            delta_time += Time.deltaTime;
            if (delta_time > 1)
            {
                flg++;
                obj = GameObject.Find("Coin");
                obj.GetComponent<RectTransform>().localPosition = new Vector3(0.13f * width, 0.065f * height);
                obj.GetComponent<RectTransform>().sizeDelta = new Vector2(0.4f * width, 0.1f * height);
                text = obj.GetComponent<Text>();
            }
        }
        #endregion
        #region flg=3:  Coin
        else if (flg == 3)
        {
            if (count < datas[3]) count += Random.Range(10, 20);
            else if (count > datas[3]) count--;
            text.text = (count.ToString());
            if (count == datas[3])
            {
                flg++;
                count = 0;
                obj = GameObject.Find("Treasure");
                obj.GetComponent<RectTransform>().sizeDelta = new Vector3(0.45f * width, 0.45f * width);
                GameObject.Find("Box").GetComponent<RectTransform>().sizeDelta = new Vector3(0.45f * width, 0.45f * width);
                obj.GetComponent<RectTransform>().localPosition = new Vector3(width, -0.15f * height);/*
                for (int i = 0; i < datas[4]; i++)
                {
                    o = Instantiate(Resources.Load<GameObject>("Prefab/Get")) as GameObject;
                    o.name = "Get" + i;
                    o.transform.parent = obj.transform;
                    o.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Item/sword1");
                    o.transform.SetSiblingIndex(0);
                    o.GetComponent<RectTransform>().localPosition = new Vector3(width, height, 0);
                }*/
                o = GameObject.Find("Coin_Plus"); 
                o.GetComponent<RectTransform>().sizeDelta = new Vector2(0.4f * width, 0.1f * height);
                coin = Random.Range(200, 700);
                o.GetComponent<Text>().text = coin.ToString();
            }
        }
        #endregion
        #region flg=4:  Treasure_Coin
        else if (flg == 4)
        {
            if (num < datas[4]) //宝が残ってる
            {
                Vector3 vec = obj.GetComponent<RectTransform>().localPosition;
                obj.GetComponent<RectTransform>().localPosition = (14f * vec + new Vector3(0, -0.15f * height, 0)) / 15f;
                if ((vec - new Vector3(0, -0.15f * height, 0)).magnitude < 0.05f)//宝移動後
                {
                    if (obj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Closed_pink"))
                    {
                        obj.GetComponent<RectTransform>().localPosition = new Vector3(0, -0.15f * height, 0);
                        obj.GetComponent<Animator>().SetBool("Open_Bool", true);
                    }
                    else if (obj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Openning_pink")
                && obj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 2f)//開いた後
                    {
                        count++;
                        if (count < 45)
                        {
                            o.GetComponent<RectTransform>().localPosition = new Vector3(0, -height * 0.06f * Mathf.Cos(Mathf.PI / 45f * count), 0);
                            Vector2 scale = o.GetComponent<RectTransform>().sizeDelta;
                            if(count>30)
                            {
                                obj.GetComponent<RectTransform>().sizeDelta -= new Vector2(2, 2);
                            }
                        }
                        else
                        {
                            obj.GetComponent<RectTransform>().sizeDelta -= new Vector2(2, 2);
                            Vector3 pos = o.GetComponent<RectTransform>().localPosition;
                            o.GetComponent<RectTransform>().localPosition = (9f * pos + new Vector3(0.13f * width, -0.065f * height))/10f;
                            Debug.Log(" m "+ (o.GetComponent<RectTransform>().localPosition -new Vector3(0.13f * width, -0.065f * height)).magnitude);
                            if ((o.GetComponent<RectTransform>().localPosition - new Vector3(0.13f * width, -0.065f * height)).magnitude < 0.1f)
                            {
                                o.GetComponent<RectTransform>().localPosition = new Vector3(0.13f * width, -0.065f * height);
                                if (coin > 0)
                                {
                                    int ran = Random.Range(5, 10);
                                    coin -= ran;
                                    datas[3] += ran;
                                    text.text = datas[3].ToString();
                                    o.GetComponent<Text>().text = coin.ToString();
                                }
                                else if (coin < 0)
                                {
                                    coin++;
                                    datas[3]--;
                                    text.text = datas[3].ToString();
                                    o.GetComponent<Text>().text = coin.ToString();
                                }
                                else
                                {
                                    num++;
                                    obj.GetComponent<RectTransform>().sizeDelta = new Vector3(0.45f * width, 0.45f * width);
                                    obj.GetComponent<Animator>().SetBool("Open_Bool", false);
                                    obj.GetComponent<RectTransform>().localPosition = new Vector3(width, -0.15f * height);
                                    coin = Random.Range(200, 700);
                                    o.GetComponent<Text>().text = coin.ToString();
                                    count = 0;
                                    o.GetComponent<RectTransform>().localPosition = new Vector3(width, 0, 0);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                flg++;
                count = 0;
                obj.GetComponent<Animator>().SetTrigger("Next_Trigger");
                for (int i = 0; i < datas[5]; i++)
                {
                    o = Instantiate(Resources.Load<GameObject>("Prefab/Get")) as GameObject;
                    o.name = "GetRare" + i;
                    o.transform.parent = GameObject.Find("Treasure").transform;
                    o.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Item/Item"+Random.Range(0,5));
                }
                GameObject.Find("Box").GetComponent<Image>().color = Color.white;
            }
        }
        #endregion
        #region flg=5:  Treasure_Rare
        else if (flg == 5)
        {
            if (num < datas[5]) //宝が残ってる
            {
                Vector3 vec = obj.GetComponent<RectTransform>().localPosition;
                obj.GetComponent<RectTransform>().localPosition = (14f * vec + new Vector3(0, -0.15f * height, 0)) / 15f;
                if ((vec - new Vector3(0, -0.15f * height, 0)).magnitude < 0.05f)//宝移動後
                {
                    if (obj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Closed"))
                    {
                        obj.GetComponent<RectTransform>().localPosition = new Vector3(0, -0.15f * height, 0);
                        obj.GetComponent<Animator>().SetBool("Open_Bool", true);
                        o = GameObject.Find("Get" + num);
                        o.GetComponent<Image>().color = Color.white;
                    }
                    else if (obj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Openning")
                && obj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 2f)//開いた後
                    {
                        count++;
                        if (count < 90)
                        {
                            o.GetComponent<RectTransform>().localPosition = new Vector3(0, -height * 0.06f * Mathf.Cos(Mathf.PI / 45f * count), 0);
                            Vector2 scale = o.GetComponent<RectTransform>().sizeDelta;
                            //Debug.Log("name : " + o.name+"   "+scale);
                            if (scale.x < 0.5f * height)//大きくする
                            {
                                scale += new Vector2(2, 2);
                                o.GetComponent<RectTransform>().sizeDelta = scale;
                            }
                            else
                            {
                                obj.GetComponent<RectTransform>().sizeDelta -= new Vector2(2, 2);
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
                                obj.GetComponent<RectTransform>().sizeDelta = new Vector3(0.45f * width, 0.45f * width);
                                obj.GetComponent<Animator>().SetBool("Open_Bool", false);
                                GameObject.Find("Box").GetComponent<RectTransform>().sizeDelta = new Vector3(0.45f * width, 0.45f * width);
                                obj.GetComponent<RectTransform>().localPosition = new Vector3(width, -0.15f * height);
                                count = 0;
                            }
                        }
                    }
                }
            }
            else
            {
                flg++;
                count = 0;
                GameObject.Find("End").GetComponent<Animator>().SetInteger("Stamp_Int", -10);

            }
        }
        #endregion
        #region flg=6:  次へ
        else if (flg == 6)
        {
            if (Input.GetMouseButtonUp(0)) Next();
        }
        #endregion

        #region タッチエフェクト
        // 画面のどこでもタッチでエフェクト
        if (Input.GetMouseButton(0))
        {
            // マウスのワールド座標までパーティクルを移動,エフェクトを1つ生成する
            var pos = _camera.ScreenToWorldPoint(Input.mousePosition + _camera.transform.forward * 10);
            touchEffect.transform.position = pos;
            touchEffect.Emit(1);
        }
        // 使用する際はSub_cameraとTouch_particleオブジェクトを追加してください
        #endregion
    }

    public void Next()
    {
        FadeManager.Instance.LoadScene("start", 3.0f);
        SceneManager.LoadScene("start");
    }
}