using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Dictionary
 * Dictionaryにアタッチするけど速攻Destroy
 * Partyにデータの引継ぎをする用
 * これでPlayerPrefasは2つずつぐらいで済むかな(図鑑IDとレベル)
 */

public class Dictionary : MonoBehaviour {

    public void Set_Box(Party chara,int ID)
    {
        #region ID=1:剣士
        if (ID == 1)
        {
            chara.Attack = 30;
            chara.HP = 60;
            chara.skills[0] = 0; //宝が見える
            chara.skills[1] = 0; //敵が見える
            chara.skills[2] = 15; //時間が短くなる
            /*chara.skills[0] = 0; //敵を留める
            chara.skills[0] = 0; //立ち止まる
            chara.skills[0] = 0; //見たやつを覚える
            chara.skills[0] = 0; //引き返す
            chara.skills[0] = 0; //道を自由に（自分も敵も止まる）
            chara.skills[0] = 0; //周りが見える*/
            chara.skill_Description = "剣士のスキル";
            chara.Max_gage = 25;
        }
        #endregion
        #region ID=2:魔女
        else if (ID == 2)
        {
            chara.Attack = 60;
            chara.HP = 60;
            chara.skills[0] = 10; //宝が見える
            chara.skills[1] = 10; //敵が見える
            chara.skills[2] = 0; //時間が短くなる
            /*chara.skills[0] = 0; //敵を留める
            chara.skills[0] = 0; //立ち止まる
            chara.skills[0] = 0; //見たやつを覚える
            chara.skills[0] = 0; //引き返す
            chara.skills[0] = 0; //道を自由に（自分も敵も止まる）
            chara.skills[0] = 0; //周りが見える*/
            chara.skill_Description = "魔女のスキル";
            chara.Max_gage = 25;
        }
        #endregion
        #region ID=3:海賊
        else if (ID == 3)
        {
            chara.Attack = 60;
            chara.HP = 120;
            chara.skills[0] = 15; //宝が見える
            chara.skills[1] = 0; //敵が見える
            chara.skills[2] = 0; //時間が短くなる
            /*chara.skills[0] = 0; //敵を留める
            chara.skills[0] = 0; //立ち止まる
            chara.skills[0] = 0; //見たやつを覚える
            chara.skills[0] = 0; //引き返す
            chara.skills[0] = 0; //道を自由に（自分も敵も止まる）
            chara.skills[0] = 0; //周りが見える*/
            chara.skill_Description = "海賊のスキル";
            chara.Max_gage = 25;
        }
        #endregion

    }
}
