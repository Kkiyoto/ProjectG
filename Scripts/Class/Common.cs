using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Common
 * 何かしらの用語の定義とかに使えたらと、、
 * 上手い使い方は分かっていないです
 */ 
public class Common : MonoBehaviour
{
    public enum Direction
    {
        None = 0,
        Straight,
        Right,
        Left,
        Up,
        Down,
    }

    public Vector3 DireToVec(Direction d)
    {
        if(d==Direction.Straight||d==Direction.Up)return new Vector3(0, 1, 0);
        else if(d==Direction.Down)return new Vector3(0, -1, 0);
        else if(d==Direction.Right)return new Vector3(1,0, 0);
        else if(d==Direction.Left)return new Vector3(-1,0, 0);
        else return new Vector3(0, 0, 0);
    }
}
