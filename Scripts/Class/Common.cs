using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Common
 * 何かしらの用語の定義とかに使えたらと、、
 * 上手い使い方は分かっていないです
 */ 

public class Common : MonoBehaviour
{
    public enum Direction //向き＆盤のタイプ
    {
        None = 0,
        Straight,
        Right,
        Left,
        Up,// or Mount(引き返し)
        Down, // or Goal
    }

    public enum Action //キャラの動作（敵含む）
    {
        Walk=0, //行動
        Battle, //バトル時
        Happy, //勝利、宝、ゴール
        Stop, //構え、一時停止時
        Sad, //タイムオーバー
    }

    public enum Type //キャラ（敵）
    {
        Player=0,
        Walk,
        Fly,
        Stop
    }

    public enum Condition //足場
    {
        Normal=0,
        Hole,
        Moving, 
        Player,
        //Enemy,
    }

    public enum Thema //背景
    {
        Sky=0,
        Mine,
        Magma,
        Poison
    }
}
