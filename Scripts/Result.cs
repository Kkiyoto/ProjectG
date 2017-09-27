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
    int flg, count, num, coin;
    GameObject obj, o, Audio,Panel;
    float delta_time;
    int[] datas = new int[7]; //順にtime,Life,Enemy,Coin,treasure普通、レア
    Text text;

    public AudioSource[] Result_BGM;
    AudioClip[] Result_SE = new AudioClip[4];
    #region タッチエフェクト追記
    [SerializeField]
    ParticleSystem touchEffect;    // タッチの際のエフェクト
    [SerializeField]
    Camera _camera;                // カメラの座標
    private bool isTouch = true;
    #endregion

    // Use this for initialization
    void Start()
    {
        width = Screen.width;
        height = Screen.height;
        o = GameObject.Find("light");
        /*o.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
        o.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);*/
        datas[0] = PlayerPrefs.GetInt("Time", 0);
        datas[1] = PlayerPrefs.GetInt("Life", 0);
        datas[2] = PlayerPrefs.GetInt("enemy", 0);
        datas[3] = PlayerPrefs.GetInt("Coin", 0);
        datas[4] = PlayerPrefs.GetInt("treasure1", 0);
        datas[5] = PlayerPrefs.GetInt("treasure0", 0);
        datas[6] = PlayerPrefs.GetInt("Score", 0);
        obj = GameObject.Find("Time");
        obj.GetComponent<RectTransform>().localPosition = new Vector3(0.18f * width, 0.34f * height);
        obj.GetComponent<RectTransform>().sizeDelta = new Vector2(0.4f * width, 0.1f * height);
        flg = -1;
        count = 0;
        num = 0;
        delta_time = 0;
        text = obj.GetComponent<Text>();
        Audio = GameObject.Find("EventSystem");
        Panel = GameObject.Find("ScenePanel");

        Result_SE[0] = Resources.Load<AudioClip>("Audio/SE/Paper");
        Result_SE[1] = Resources.Load<AudioClip>("Audio/SE/Count");
        Result_SE[2] = Resources.Load<AudioClip>("Audio/SE/Stamp");
        Result_SE[3] = Resources.Load<AudioClip>("Audio/SE/Tap");
        Audio.GetComponent<AudioSource>().PlayOneShot(Result_SE[0]); // ペラッ
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Return)) Next();
        #region flg=-1:  イメージ画像のアニメ
        if (flg < 0)
        {

            o.GetComponent<Image>().color -= new Color(0, 0, 0, 0.01f);
            if (o.GetComponent<Image>().color.a < 0.01f)
            {
                Destroy(o);
                flg++;
                count = 0;
                /*obj = GameObject.Find("end");
                obj.GetComponent<RectTransform>().localPosition = new Vector3(-width, 0.5f * height);
                obj.GetComponent<RectTransform>().sizeDelta = new Vector2(0.4f * width, 0.07f * height);*/
                text = obj.GetComponent<Text>();
            }
        }
        #endregion
        #region flg=0:  Time
        else if (flg == 0)
        {
            if (delta_time == 0)
            {
                Audio.GetComponent<AudioSource>().PlayOneShot(Result_SE[1]); // カラカラカラ
            }
            delta_time += Time.deltaTime;
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
                delta_time = 0;
            }
        }
        #endregion
        #region flg=1:  Life
        else if (flg == 1)
        {
            if (delta_time == 0)
            {
                Result_BGM[0].Stop(); //カラカラ音ミュート
                //Audio.GetComponent<AudioSource>().PlayOneShot(Result_SE[2]); // ドン！
            }
            delta_time += Time.deltaTime;
            if (delta_time > 1)
            {
                flg++;
                obj = GameObject.Find("Enemy");
                obj.GetComponent<Animator>().SetInteger("Stamp_Int", datas[2]);
                delta_time = 0;
                Audio.GetComponent<AudioSource>().PlayOneShot(Result_SE[2]); // ドン！
            }
        }
        #endregion
        #region flg=2:  Enemy
        else if (flg == 2)
        {
            //if (delta_time == 0)
            //Audio.GetComponent<AudioSource>().PlayOneShot(Result_SE[2]); // ドン！
            delta_time += Time.deltaTime;
            if (delta_time > 1)
            {
                Audio.GetComponent<AudioSource>().PlayOneShot(Result_SE[2]); // ドン！
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
                obj.GetComponent<RectTransform>().localPosition = new Vector3(width, -0.37f * height);/*
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
                coin = Random.Range(100, 200);
                datas[6] += coin;
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
                obj.GetComponent<RectTransform>().localPosition = (11f * vec + new Vector3(0, -0.37f * height, 0)) / 12f;
                if ((vec - new Vector3(0, -0.37f * height, 0)).magnitude < 0.15f)//宝移動後
                {
                    if (obj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Closed_pink"))
                    {
                        //obj.GetComponent<RectTransform>().localPosition = new Vector3(0, -0.3f * height, 0);
                        obj.GetComponent<Animator>().SetBool("Open_Bool", true);
                    }
                    else if (obj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Openning_pink")
                && obj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1.5f)//開いた後
                    {
                        count++;
                        if (count < 40)
                        {
                            o.GetComponent<RectTransform>().localPosition = new Vector3(-0.05f * width, -height * (0.03f * Mathf.Cos(Mathf.PI / 40f * count) + 0.28f), 0);
                            Vector2 scale = o.GetComponent<RectTransform>().sizeDelta;
                            if (count > 30)
                            {
                                obj.GetComponent<RectTransform>().sizeDelta -= new Vector2(2, 2);
                            }
                        }
                        else
                        {
                            obj.GetComponent<RectTransform>().sizeDelta -= new Vector2(2, 2);
                            Vector3 pos = o.GetComponent<RectTransform>().localPosition;
                            o.GetComponent<RectTransform>().localPosition = (9f * pos + new Vector3(0.13f * width, -0.038f * height)) / 10f;
                            if ((o.GetComponent<RectTransform>().localPosition - new Vector3(0.13f * width, -0.038f * height)).magnitude < 0.1f)
                            {
                                o.GetComponent<RectTransform>().localPosition = new Vector3(0.13f * width, -0.038f * height);
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
                                    obj.GetComponent<RectTransform>().localPosition = new Vector3(width, -0.37f * height);
                                    coin = Random.Range(100, 200);
                                    datas[6] += coin;
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
                    int n = Random.Range(0, 5);
                    o.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Item/Item" + n);
                    /*if(n==0)o.transform.Find("Get_Text").GetComponent<Text>().text = "蒼水剣";
                    else if (n == 1) o.transform.Find("Get_Text").GetComponent<Text>().text = "シャムシール";
                    else if (n == 2) o.transform.Find("Get_Text").GetComponent<Text>().text = "揺炎刀";
                    else if (n == 3) o.transform.Find("Get_Text").GetComponent<Text>().text = "藍玉杖";
                    else o.transform.Find("Get_Text").GetComponent<Text>().text = "翡翠杖";*/
                }
                GameObject.Find("Box").GetComponent<Image>().color = Color.white;
                obj.GetComponent<RectTransform>().localPosition = new Vector3(width, -0.3f * height);
                GameObject.Find("Box").transform.SetAsLastSibling();
                num = 0;
            }
        }
        #endregion
        #region flg=5:  Treasure_Rare
        else if (flg == 5)
        {
            if (num < datas[5]) //宝が残ってる
            {
                Vector3 vec = obj.GetComponent<RectTransform>().localPosition;
                obj.GetComponent<RectTransform>().localPosition = (9f * vec + new Vector3(0, -0.3f * height, 0)) / 10f;
                if ((vec - new Vector3(0, -0.3f * height, 0)).magnitude < 0.05f)//宝移動後
                {
                    if (obj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Closed"))
                    {
                        //obj.GetComponent<RectTransform>().localPosition = new Vector3(0, -0.15f * height, 0);
                        obj.GetComponent<Animator>().SetBool("Open_Bool", true);
                        o = GameObject.Find("GetRare" + num);
                        o.GetComponent<Image>().color = Color.white;
                    }
                    else if (obj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Openning")
                && obj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1.5f)//開いた後
                    {
                        count++;
                        if (count < 90)
                        {
                            o.GetComponent<RectTransform>().localPosition = new Vector3(0, -height * 0.06f * Mathf.Cos(Mathf.PI / 45f * count), 0);
                            Vector2 scale = o.GetComponent<RectTransform>().sizeDelta;
                            if (scale.x < 0.5f * height)//大きくする
                            {
                                scale += new Vector2(2, 2);
                                o.GetComponent<RectTransform>().sizeDelta = scale;
                            }
                            else
                            {
                                obj.GetComponent<RectTransform>().sizeDelta -= new Vector2(3, 3);
                            }
                            if (count == 40)
                            {
                                o.transform.SetAsLastSibling();
                            }
                        }
                        else
                        {
                            obj.GetComponent<RectTransform>().sizeDelta -= new Vector2(3, 3);
                            GameObject.Find("Box").GetComponent<RectTransform>().sizeDelta = obj.GetComponent<RectTransform>().sizeDelta;
                            if (obj.GetComponent<RectTransform>().sizeDelta.x < 1)
                            {
                                o.transform.parent = GameObject.Find("BackCanvas").transform;
                                o.GetComponent<RectTransform>().localPosition = new Vector3(0.27f * (num - 1) * width, -0.125f * height);
                                o.GetComponent<RectTransform>().localScale = new Vector3(0.5f, 0.5f, 0.5f);
                                num++;
                                obj.GetComponent<RectTransform>().sizeDelta = new Vector3(0.45f * width, 0.45f * width);
                                obj.GetComponent<Animator>().SetBool("Open_Bool", false);
                                GameObject.Find("Box").GetComponent<RectTransform>().sizeDelta = new Vector3(0.45f * width, 0.45f * width);
                                obj.GetComponent<RectTransform>().localPosition = new Vector3(width, -0.3f * height);
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
                //Audio.GetComponent<AudioSource>().PlayOneShot(Result_SE[2]); // ドン！
                obj = GameObject.Find("Score");
                obj.GetComponent<RectTransform>().localPosition = new Vector3(-0.3f * width, -0.4f * height);
                obj.GetComponent<RectTransform>().sizeDelta = new Vector2(0.5f * width, 0.2f * height);
                text = obj.GetComponent<Text>();
            }
        }
        #endregion
        #region flg=6:  SCORE
        else if (flg == 6)
        {
            if (count < datas[6]) count += Random.Range(40, 80);
            else if (count > datas[6]) count--;
            text.text = "Score\n"+(count.ToString());//.PadLeft(5, '0'));
            if (count == datas[6])
            {
                delta_time = 0;
                GameObject.Find("End").GetComponent<Animator>().SetInteger("Stamp_Int", -10);
                //Audio.GetComponent<AudioSource>().PlayOneShot(Result_SE[2]); // ドン！
                o = GameObject.Find("Button");
                o.GetComponent<RectTransform>().localPosition = new Vector3(0.25f * width, -0.45f * height);
                o.GetComponent<RectTransform>().sizeDelta = new Vector2(0.5f * width, 0.09f * height);
                flg++;
            }
        }
        #endregion
        #region flg=7:  次へ
        else if (flg == 7)
        {
            delta_time += Time.deltaTime;
            if (delta_time > 1)
            {
                Audio.GetComponent<AudioSource>().PlayOneShot(Result_SE[2]); // ドン！
                flg++;
                obj = GameObject.Find("Coin");
                obj.GetComponent<RectTransform>().localPosition = new Vector3(0.13f * width, 0.065f * height);
                obj.GetComponent<RectTransform>().sizeDelta = new Vector2(0.4f * width, 0.1f * height);
                text = obj.GetComponent<Text>();
            }
        }
        #endregion

        #region タッチエフェクト
        // 画面のどこでもタッチでエフェクト

        if (Input.GetMouseButtonUp(0))
        {
            isTouch = false;
        }

        if (Input.GetMouseButton(0))
        {
            // マウスのワールド座標までパーティクルを移動,エフェクトを1つ生成する
            var pos = _camera.ScreenToWorldPoint(Input.mousePosition + _camera.transform.forward * 10);
            touchEffect.transform.position = pos;
            touchEffect.Emit(1);

            if (!isTouch)
            {
                //Audio.GetComponent<AudioSource>().PlayOneShot(Result_SE[3]);  // 違和感
                isTouch = true;
            }

        }
        // 使用する際はSub_cameraとTouch_particleオブジェクトを追加してください
        #endregion
    }
    

    public void Next()
    {
        FadeManager.Instance.LoadScene("Home", 3.0f);
        Panel.GetComponent<Animator>().SetTrigger("Fader");
    }
}