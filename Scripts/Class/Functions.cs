using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Functions : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Common.Direction Random_direct() //方向のランダム関数
    {
        int ran = Random.Range(2, 6);
        if (ran == 2) return Common.Direction.Right;
        else if (ran == 3) return Common.Direction.Left;
        else if (ran == 4) return Common.Direction.Up;
        else return Common.Direction.Down;
    }

    public int B_to_I(bool b) //boolを0,1に
    {
        if (b) return 1;
        else return 0;
    }

    public Vector3 Dire_to_Vec(Common.Direction d)//ベクトル化
    {
        if (d == Common.Direction.Up) return new Vector3(0, 1, 0);
        else if (d == Common.Direction.Down) return new Vector3(0, -1, 0);
        else if (d == Common.Direction.Right) return new Vector3(1, 0, 0);
        else if (d == Common.Direction.Left) return new Vector3(-1, 0, 0);
        else return new Vector3(0, 0, 0);
    }

    public Common.Direction reverse(Common.Direction direct)//逆向き化
    {
        if (direct == Common.Direction.Down) return Common.Direction.Up;
        else if (direct == Common.Direction.Up) return Common.Direction.Down;
        else if (direct == Common.Direction.Right) return Common.Direction.Left;
        else if (direct == Common.Direction.Left) return Common.Direction.Right;
        else return Common.Direction.None;
    }

    public int L(int small) //0~8を0~2にする(どこの塊)
    {
        int n = (small + 8) / 3;
        return Mathf.FloorToInt(n - 3);
    }

    public float d_infty(Vector3 vec_1, Vector3 vec_2)
    {
        float d1 = Mathf.Abs(vec_1.x - vec_2.x);
        float d2 = Mathf.Abs(vec_1.y - vec_2.y);
        return Mathf.Max(d1, d2);
    }
}
