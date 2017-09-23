using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title : Functions
{
    [SerializeField]
    ParticleSystem touchEffect;    // タッチの際のエフェクト
    [SerializeField]
    Camera _camera;                // カメラの座標
    State_manage UIs;

    public GameObject FadePanel, Character, fader;
    AudioClip[] SE = new AudioClip[2];
    public AudioSource[] BGM;
    private RectTransform hoge, hogehoge;

    public float x, y, z;
    private bool isTouch = true, TouchFlag = false;
    private Vector3 pos_diff;
    private float down_diff, chardown = -450, div = 50;

    void Start()
    {
        BGM = GameObject.Find("FadeManager").GetComponents<AudioSource>();
        fader = GameObject.Find("FadeManager");
        SE[0] = Resources.Load<AudioClip>("Audio/SE/Select");
        SE[1] = Resources.Load<AudioClip>("Audio/SE/Tap");
    }

    void Update()
    {

        if (Input.GetMouseButtonUp(0))
            isTouch = false;


        if (Input.GetMouseButton(0))
        {
            // マウスのワールド座標までパーティクルを移動,エフェクトを1つ生成する
            var pos = _camera.ScreenToWorldPoint(Input.mousePosition + _camera.transform.forward * 10);
            touchEffect.transform.position = pos;
            touchEffect.Emit(1);

            // シーン遷移,フェードのトリガー起動
            if (!TouchFlag)
            {
                fader.GetComponent<AudioSource>().PlayOneShot(SE[0]);
                FadePanel.GetComponent<Animator>().SetTrigger("FadeTrigger");
                Invoke("SceneChanger", 2.5f); // 5.0->2.5に
            }
            else if (!isTouch)
            {
                //fader.GetComponent<AudioSource>().PlayOneShot(SE[1]);  // 違和感
                isTouch = true;
            }
            TouchFlag = true;
        }

        if (TouchFlag)
        {
            // 飛行船をゆったり移動 ※現状は直線移動
            //hoge = GameObject.Find("Hikousen").GetComponent<RectTransform>();
            //pos_diff = (new Vector3(x, y, z) - hoge.localPosition) /div;
            // シーン遷移,フェードのトリガー起動
            if (!TouchFlag)
            {
                fader.GetComponent<AudioSource>().PlayOneShot(SE[0]);
                FadePanel.GetComponent<Animator>().SetTrigger("FadeTrigger");
                Invoke("SceneChanger", 2.5f); // 5.0->2.5に
            }
            else if (!isTouch)
            {
                //fader.GetComponent<AudioSource>().PlayOneShot(SE[1]);  // タッチSE 違和感
                isTouch = true;
            }
            TouchFlag = true;
        }
        //if (Mathf.Abs(pos_diff.x *div) > 0.7 && Mathf.Abs(pos_diff.y *div) > 0.7 && Mathf.Abs(pos_diff.z *div) > 0.7)
        //    hoge.localPosition += pos_diff;
        //else if(!Character.activeSelf)
        //    Character.SetActive(true);  // 目標到達後のキャラ落下
        //else
        //{
        //    hogehoge = GameObject.Find("Character").GetComponent<RectTransform>();
        //    down_diff = (chardown - hogehoge.localPosition.y) / 50;
        //    hogehoge.localPosition += new Vector3(0,down_diff,0);
        BGM[1].volume -= 0.005f;
        //}
    }

    void SceneChanger()
    {
        FadeManager.Instance.LoadScene("Tutorial", 3.0f);
    }
}
