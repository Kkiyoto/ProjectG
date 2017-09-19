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
    RectTransform carsol;
    float height, width;

    // Use this for initialization
    void Start()
    {
        height = Screen.height;
        width = Screen.width;
        for (int i = 0; i < 3; i++)
        {
            RectTransform o = GameObject.Find("Party" + i).GetComponent<RectTransform>();
            o.localPosition = new Vector3(0, (0.3f - i * 0.2f) * height);
            o.sizeDelta = new Vector2(0.6f * width, 0.3f * width);
        }
        carsol = GameObject.Find("Carsol").GetComponent<RectTransform>();
        carsol.sizeDelta = new Vector2(0.7f * width, 0.4f * width);
        Set_Party(0);
    }
	
	// Update is called once per frame
	void Update ()
    {

    }

    public void Get_Ready(bool Yes_or_No)
    {
        if(Yes_or_No) SceneManager.LoadScene("Tutorial");
        else SceneManager.LoadScene("start");
    }

    public void Set_Party(int num)
    {
        if (num == 0)
        {
            PlayerPrefs.SetInt("Party0", 2);
            PlayerPrefs.SetInt("Party1", 3);
            PlayerPrefs.SetInt("Party2", 4);
            carsol.localPosition = new Vector3(0, 0.3f * height);
        }
        else if (num == 1)
        {
            PlayerPrefs.SetInt("Party0", 2);
            PlayerPrefs.SetInt("Party1", 2);
            PlayerPrefs.SetInt("Party2", 2);
            carsol.localPosition = new Vector3(0, 0.1f * height);
        }
        else if (num == 2)
        {
            PlayerPrefs.SetInt("Party0", 3);
            PlayerPrefs.SetInt("Party1", 2);
            PlayerPrefs.SetInt("Party2", 4);
            carsol.localPosition = new Vector3(0, -0.1f * height);
        }
    }
}
