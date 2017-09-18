using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/* Select
 * ステージセレクト画面のManagerにアタッチ
 * 特には説明書かなくても分かるかな？
 */ 

public class Select : MonoBehaviour
{
    GameObject button;
    float height, width;
    bool flg = true;

	// Use this for initialization
	void Start ()
    {
        height = Screen.height;
        width = Screen.width;
        button = GameObject.Find("Start");
        button.GetComponent<RectTransform>().localPosition = new Vector3(0, 0.1f * height,-5);
        button.GetComponent<RectTransform>().sizeDelta = new Vector2(0.9f*width, 0.9f * height);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyUp(KeyCode.Return)) SceneManager.LoadScene("Select");
        if (Input.GetKeyUp(KeyCode.Space)) flg=!flg;
        Vector3 vec = button.GetComponent<RectTransform>().localPosition;
        if (flg&&vec.z < 0)
        {
            float x = vec.z * 11f;
            x++;
            button.GetComponent<RectTransform>().localPosition = new Vector3(0,x*(x+20)*0.0005f*height , x/11f);
            if (Mathf.Abs(x) < 0.1) vec.z = 0;
        }
        else if (!flg&&vec.z > -5)
        {
            float x = vec.z * 11f;
            x--;
            button.GetComponent<RectTransform>().localPosition = new Vector3(0, x * (x + 20) * 0.0005f * height, x / 11f);
        }
    }

    public void Get_Ready()
    {
        SceneManager.LoadScene("Tutorial");
    }
}
