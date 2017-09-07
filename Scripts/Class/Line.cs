using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour {

    GameObject[] lines = new GameObject[2];
    Sprite[] sprites = new Sprite[8];
    bool[] is_light = new bool[6];

    public Line(Vector3 pos)
    {
        lines[0] = Instantiate(Resources.Load<GameObject>("Prefab/Lines")) as GameObject;
        lines[1] = Instantiate(Resources.Load<GameObject>("Prefab/Lines")) as GameObject;
        lines[1].transform.Rotate(new Vector3(0, 0, -90));
        lines[0].transform.position = pos;
        lines[1].transform.position = pos;
        for (int i = 0; i < 6; i++) is_light[i] = false;
        sprites[0] = Resources.Load<Sprite>("Images/Result/Line000");
        sprites[1] = Resources.Load<Sprite>("Images/Result/Line001");
        sprites[2] = Resources.Load<Sprite>("Images/Result/Line010");
        sprites[3] = Resources.Load<Sprite>("Images/Result/Line011");
        sprites[4] = Resources.Load<Sprite>("Images/Result/Line100");
        sprites[5] = Resources.Load<Sprite>("Images/Result/Line101");
        sprites[6] = Resources.Load<Sprite>("Images/Result/Line110");
        sprites[7] = Resources.Load<Sprite>("Images/Result/Line111");
    }

    public void Sprite(int n)//道の番号順に数字入れたら出てくる
    {
        if (n < 3 && !is_light[n])
        {
            is_light[n] = true;
            int num = 4*B_to_I(is_light[0]) + 2 * B_to_I(is_light[1]) +  B_to_I(is_light[2]);
            lines[0].GetComponent<SpriteRenderer>().sprite = sprites[num];
        }
        else if (n < 6 && !is_light[n])
        {
            is_light[n] = true;
            int num = 4* B_to_I(is_light[3]) + 2 * B_to_I(is_light[4]) + B_to_I(is_light[5]);
            lines[1].GetComponent<SpriteRenderer>().sprite = sprites[num];
        }
    }

    public int B_to_I(bool b)
    {
        if (b) return 1;
        else return 0;
    }
}
