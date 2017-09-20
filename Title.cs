using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Title : Functions
{
    [SerializeField] ParticleSystem touchEffect;    // タッチの際のエフェクト
    [SerializeField] Camera _camera;                // カメラの座標
    State_manage UIs;

    public GameObject FadePanel,Character;
    public AudioSource[] BGM;
    private RectTransform hoge, hogehoge;

    public float x, y, z;
    private bool TouchFlag = false;
    private Vector3 pos_diff;
    private float down_diff,chardown = -450,div = 50;

    void Start()
    {
        BGM = GameObject.Find("FadeManager").GetComponents<AudioSource>();
    }
    
    void Update () {
    if (Input.GetMouseButton(0))
    {
        // マウスのワールド座標までパーティクルを移動,エフェクトを1つ生成する
        var pos = _camera.ScreenToWorldPoint(Input.mousePosition + _camera.transform.forward * 10);
        touchEffect.transform.position = pos;
        touchEffect.Emit(1);

        // シーン遷移,フェードのトリガー起動
        if (!TouchFlag)
        {
            FadePanel.GetComponent<Animator>().SetTrigger("FadeTrigger");
            Invoke("SceneChanger", 5.0f);
        }
        TouchFlag = true;
    }

    if (TouchFlag)
    {
        // 飛行船をゆったり移動 ※現状は直線移動
        hoge = GameObject.Find("Hikousen").GetComponent<RectTransform>();
        pos_diff = (new Vector3(x, y, z) - hoge.localPosition) /div;



        if (Mathf.Abs(pos_diff.x *div) > 0.7 && Mathf.Abs(pos_diff.y *div) > 0.7 && Mathf.Abs(pos_diff.z *div) > 0.7)
            hoge.localPosition += pos_diff;
        else if(!Character.activeSelf)
            Character.SetActive(true);  // 目標到達後のキャラ落下
        else
        {
            hogehoge = GameObject.Find("Character").GetComponent<RectTransform>();
            down_diff = (chardown - hogehoge.localPosition.y) / 50;
            hogehoge.localPosition += new Vector3(0,down_diff,0);
            BGM[0].volume -= 0.005f;
        }
    }
    }

    void SceneChanger()
    {
        FadeManager.Instance.LoadScene("Tutorial", 3.0f);
    }
}
