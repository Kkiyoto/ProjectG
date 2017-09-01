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

    public enum Action
    {
        Walk=0, //行動
        Battle, //バトル時
        Happy, //勝利、宝、ゴール
        Stop, //構え、一時停止時
        Sad, //タイムオーバー
    }

    public enum Condition
    {
        None=0,
        Enemy,
        Treasure,
        Block,
    }
}
