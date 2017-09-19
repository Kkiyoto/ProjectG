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
        //スキルについて、20未満だとその秒数がタップスキルになる（組み合わせ可能）、30にすると常時発動（リーダースキル化、組み合わせ可能）
        #region ID=1:剣士
        if (ID == 1)
        {
            chara.Attack = 30;
            chara.HP = 60;
            chara.skills[0] = 5; //敵を早く倒す //★永田さんの実装を見て作ります。
            chara.skills[1] = 0; //その場の敵を一掃
            chara.skills[2] = 0; //宝が見える
            /*chara.skills[n0] = 0; //敵が見える
            chara.skills[n1] = 0; //敵と会いにくくなる
            chara.skills[n2] = 0; //時間が減るのが遅くなる
            chara.skills[n3] = 0; //敵を留める
            chara.skills[n4] = 0; //立ち止まる
            chara.skills[n5] = 0; //敵に完全に合わなくなる（n3,n4と組み合わせで道を自由に)単体だと強すぎる気もする
            chara.skills[n6] = 0; //見たやつを覚える
            chara.skills[n7] = 0; //引き返す
            chara.skills[n8] = 0; //Coinが増える
            chara.skills[n9] = 0; //盤が1瞬で動く
            chara.skills[n10] = 0; //周りが見える（却下っぽい。。）*/
            chara.skill_Description = "早く敵を倒すことが出来る";
            chara.Max_gage = 25;
            chara.Max_second = 5;
        }
        #endregion
        #region ID=2:魔女
        else if (ID == 2)
        {
            chara.Attack = 60;
            chara.HP = 60;
            chara.skills[0] = 0; //敵を早く倒す
            chara.skills[1] = 1; //その場の敵を一掃
            chara.skills[2] = 0; //宝が見える
            /*chara.skills[n0] = 0; //敵が見える
            chara.skills[n1] = 0; //敵と会いにくくなる
            chara.skills[n2] = 0; //時間が減るのが遅くなる
            chara.skills[n3] = 0; //敵を留める
            chara.skills[n4] = 0; //立ち止まる（n3と組み合わせで道を自由に)
            chara.skills[n5] = 0; //見たやつを覚える
            chara.skills[n6] = 0; //引き返す
            chara.skills[n7] = 0; //Coinが増える
            chara.skills[n8] = 0; //盤が1瞬で動く
            chara.skills[n9] = 0; //周りが見える（却下っぽい。。）*/
            chara.skill_Description = "見えている敵全体に攻撃";
            chara.Max_gage = 25;
            chara.Max_second = 1;
        }
        #endregion
        #region ID=3:海賊
        else if (ID == 3)
        {
            chara.Attack = 60;
            chara.HP = 120;
            chara.skills[0] = 0; //敵を早く倒す
            chara.skills[1] = 0; //その場の敵を一掃
            chara.skills[2] = 10; //宝が見える
            /*chara.skills[n0] = 0; //敵が見える
            chara.skills[n1] = 0; //敵と会いにくくなる
            chara.skills[n2] = 0; //時間が減るのが遅くなる
            chara.skills[n3] = 0; //敵を留める
            chara.skills[n4] = 0; //立ち止まる（n3と組み合わせで道を自由に)
            chara.skills[n5] = 0; //見たやつを覚える
            chara.skills[n6] = 0; //引き返す
            chara.skills[n7] = 0; //Coinが増える
            chara.skills[n8] = 0; //盤が1瞬で動く
            chara.skills[n9] = 0; //周りが見える（却下っぽい。。）*/
            chara.skill_Description = "アイテムの位置が分かる";
            chara.Max_gage = 25;
            chara.Max_second = 10;
        }
        #endregion
        #region ID=4:女剣士
        else if (ID == 4)
        {
            chara.Attack = 30;
            chara.HP = 60;
            chara.skills[0] = 5; //敵を早く倒す //★永田さんの実装を見て作ります。
            chara.skills[1] = 0; //その場の敵を一掃
            chara.skills[2] = 0; //宝が見える
            /*chara.skills[n0] = 0; //敵が見える
            chara.skills[n1] = 0; //敵と会いにくくなる
            chara.skills[n2] = 0; //時間が減るのが遅くなる
            chara.skills[n3] = 0; //敵を留める
            chara.skills[n4] = 0; //立ち止まる
            chara.skills[n5] = 0; //敵に完全に合わなくなる（n3,n4と組み合わせで道を自由に)単体だと強すぎる気もする
            chara.skills[n6] = 0; //見たやつを覚える
            chara.skills[n7] = 0; //引き返す
            chara.skills[n8] = 0; //Coinが増える
            chara.skills[n9] = 0; //盤が1瞬で動く
            chara.skills[n10] = 0; //周りが見える（却下っぽい。。）*/
            chara.skill_Description = "早く敵を倒すことが出来る";
            chara.Max_gage = 25;
            chara.Max_second = 5;
        }
        #endregion
        #region ID=?:図鑑に入っていない場合（IDが考えられていない）
        else
        {
            chara.Attack = 90;
            chara.HP = 60;
            chara.skills[0] = 0; //敵を早く倒す
            chara.skills[1] = 0; //その場の敵を一掃
            chara.skills[2] = 0; //宝が見える
            /*chara.skills[n0] = 0; //敵が見える
            chara.skills[n1] = 0; //敵と会いにくくなる
            chara.skills[n2] = 0; //時間が減るのが遅くなる
            chara.skills[n3] = 0; //敵を留める
            chara.skills[n4] = 0; //立ち止まる
            chara.skills[n5] = 0; //敵に完全に合わなくなる（n3,n4と組み合わせで道を自由に)単体だと強すぎる気もする
            chara.skills[n6] = 0; //見たやつを覚える
            chara.skills[n7] = 0; //引き返す
            chara.skills[n8] = 0; //Coinが増える
            chara.skills[n9] = 0; //盤が1瞬で動く
            chara.skills[n10] = 0; //周りが見える（却下っぽい。。）*/
            chara.skill_Description = "図鑑に入っていません。。";
            chara.Max_gage = 25;
            chara.Max_second = 0;
        }
        #endregion

    }
}
