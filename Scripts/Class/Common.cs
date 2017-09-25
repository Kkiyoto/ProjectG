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
        Walk = 0, //行動
        Battle, //バトル時
        Happy, //勝利、宝、ゴール
        Stop, //構え、一時停止時
        Sad, //タイムオーバー
    }

    public enum Type //キャラ（敵）
    {
        Player = 0,
        Walk,
        Fly,
        Stop
    }

    public enum Condition //足場
    {
        Normal = 0,
        Hole,
        Moving,
        Player,
        //Enemy,
    }

    public enum Thema //背景
    {
        Sky = 0,
        Mine,
        Magma,
        Poison
    }

    public enum SE//音、PlayOneShotさせるやつ。何が一緒の音かとか、何があるのか分かっていないので追記どんどんお願いします。
    {
        Time = 0,//敵にたたかれているとき
        Fall,//落ちる時
        Win,//敵倒したとき
        Get,//宝箱、ゴール
        Button,//ボタンクリック音
        Slide,//盤を動かしているとき
        Fire,
        Ice,
        Sword,
        Gun,
        Coin,
        Count,
        Stamp,
        Result,
        Retired,
        Skill,
        Decision,
    }


    public enum BGM
    {
        none = 0,
        tutorial,
        battle,
        result,
        gameover,
    }

    public enum Treasure //何が入っているのか
    {
        Item = 0,//武器とか1つ
        Coin,//3つ
        Time,//2つ
        Life,//1つ
    }
}
