using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Start : MonoBehaviour {

    [SerializeField] ParticleSystem touchEffect;    // タッチの際のエフェクト
    [SerializeField] Camera _camera;                // カメラの座標
    public GameObject FadePanel;
    public GameObject Character;
    public float x, y, z;
    //public float setScale;

    private RectTransform hoge;
    private RectTransform hogehoge;

    private bool TouchFlag = false;
    private float div = 50;
    private Vector3 pos_diff;
    //private Vector3 scale_diff;
    private float chardown=-450;
    private float down_diff;


    // Update is called once per frame
    void Update () {
        // 画面のどこでもタッチで起動
        if (Input.GetMouseButtonDown(0))
        {
            // マウスのワールド座標までパーティクルを移動,エフェクトを1つ生成する
            var pos = _camera.ScreenToWorldPoint(Input.mousePosition + _camera.transform.forward * 10);
            touchEffect.transform.position = pos;
            touchEffect.Emit(1);
            // シーン遷移,フェードのトリガー起動
            if (!TouchFlag)
            {
                FadePanel.GetComponent<Animator>().SetTrigger("FadeTrigger");
                // Hikousen.GetComponent<Animator>().SetTrigger("HikousenMove"); // 今は亡きトリガー
                Invoke("SceneChanger", 5.0f);  // 1秒後にシーン遷移
            }
            TouchFlag = true;
        }

        if (TouchFlag)
        {
            // 飛行船をゆったり移動 ※現状は直線移動
            hoge = GameObject.Find("Hikousen").GetComponent<RectTransform>();
            pos_diff = (new Vector3(x, y, z) - hoge.localPosition) /div;
            //scale_diff=(new Vector3(setScale,setScale,setScale) - hoge.localScale) /div; // 動かない



            if (Mathf.Abs(pos_diff.x *div) > 0.7 && Mathf.Abs(pos_diff.y *div) > 0.7 && Mathf.Abs(pos_diff.z *div) > 0.7)
            {
                hoge.localPosition += pos_diff;
                //hoge.localScale += scale_diff; // 本当に動かない
            }
            // 目標到達後のキャラ落下
            else if(!Character.activeSelf)
            {
                Character.SetActive(true);
            }
            else
            {
                hogehoge = GameObject.Find("Character").GetComponent<RectTransform>();
                down_diff = (chardown - hogehoge.localPosition.y) / 50;
                hogehoge.localPosition += new Vector3(0,down_diff,0);
            }
        }
    }

    void SceneChanger()
    {
        FadeManager.Instance.LoadScene("Tutorial", 3.0f);
        //SceneManager.LoadScene("Tutorial"); // いにしえのシーン遷移
    }
}
