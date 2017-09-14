using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

// シーン遷移時のフェードイン・アウトを制御
public class FadeManager : MonoBehaviour
{
	#region Singleton
	private static FadeManager instance;
	public static FadeManager Instance {
		get {
			if (instance == null) {
				instance = (FadeManager)FindObjectOfType (typeof(FadeManager));
				if (instance == null) {
					Debug.LogError (typeof(FadeManager) + "is nothing");
				}
			}
			return instance;
		}
	}
	#endregion Singleton
	private float fadeAlpha = 0;          // フェード中の透明度
    private bool isFading = false;        // フェード中かどうか
    public Color fadeColor = Color.black; // フェード色,今回は黒

    public void Awake ()
	{
		if (this != Instance) {
			Destroy (this.gameObject);
			return;
		}
		DontDestroyOnLoad (this.gameObject);
    }

    // フェード
	public void OnGUI ()
	{
		if (this.isFading) {   // α値を更新,描写
			this.fadeColor.a = this.fadeAlpha; 
			GUI.color = this.fadeColor;
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
		}
	}

	// 画面遷移 シーン名+遷移時間
	public void LoadScene (string scene, float interval)
	{
		StartCoroutine (TransScene (scene, interval));
	}

    // シーン遷移用 シーン名+遷移時間
    private IEnumerator TransScene (string scene, float interval)
	{
		//暗く
		this.isFading = true;
		float time = 0;
		while (time <= interval) {
			this.fadeAlpha = Mathf.Lerp (0f, 1f, time / interval);
			time += Time.deltaTime;
			yield return 0;
		}

        //シーン切替
        SceneManager.LoadScene (scene);

		//明るく
		time = 0;
		while (time <= interval) {
			this.fadeAlpha = Mathf.Lerp (1f, 0f, time / interval);
			time += Time.deltaTime;
			yield return 0;
		}
		this.isFading = false;
        Destroy(this.gameObject);
    }
    
}
