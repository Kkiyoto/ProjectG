using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Dictionary
 * Dictionaryにアタッチするけど速攻Destroy
 * Partyにデータの引継ぎをする用
 * これでPlayerPrefasは2つずつぐらいで済むかな(図鑑IDとレベル)
 */

public class Dictionary : MonoBehaviour
{
    float width, height;
    //なるべく同じ名前にします。consoleから入れるのお願いします。
    public GameObject Sentou_kaisi;

    private void Start()
    {
        width = Screen.width;
        height = Screen.height;

        Sentou_kaisi.GetComponent<RectTransform>().localPosition = new Vector3(0, -0.04f * height);
        Sentou_kaisi.GetComponent<RectTransform>().sizeDelta = new Vector2(0.76f * width, 0.248f * width);

        GameObject.Find("Map_base").GetComponent<RectTransform>().localPosition = new Vector3(0.395f * width, 0.5f * height - 0.105f * width);
        GameObject.Find("Map_base").GetComponent<RectTransform>().sizeDelta = new Vector2(0.21f * width, 0.21f * width);
        GameObject.Find("Image").GetComponent<RectTransform>().sizeDelta = new Vector2(width, 0.24f * height);
        GameObject.Find("Time_base").GetComponent<RectTransform>().sizeDelta = new Vector2(width, 0.24f * height);
        GameObject.Find("Time_needle").GetComponent<RectTransform>().sizeDelta = new Vector2(0.3f*height, 0.25f * height);
        GameObject.Find("Time_needle").GetComponent<RectTransform>().localPosition = new Vector3(0, -0.013f * height);
        GameObject.Find("Time_gage").GetComponent<RectTransform>().localPosition = new Vector3(0, -0.013f * height);
        GameObject.Find("Time_gage").GetComponent<RectTransform>().sizeDelta = new Vector2(0.2f*height, 0.2f * height);
        GameObject.Find("Pause").GetComponent<RectTransform>().localPosition = new Vector3( - 0.42f * width, 0.462f * height);
        GameObject.Find("Pause").GetComponent<RectTransform>().sizeDelta = new Vector2(0.14f *width, 0.064f * height);
        GameObject.Find("Skill").GetComponent<RectTransform>().localPosition = new Vector3(-0.13f * width, -0.46f * height);
        GameObject.Find("Skill").GetComponent<RectTransform>().sizeDelta = new Vector2(0.7f * width, 0.08f * height);
        GameObject.Find("Skill_Text").GetComponent<RectTransform>().localPosition = new Vector3(0.044f * width, 0.02f * height);
        GameObject.Find("Skill_Text").GetComponent<RectTransform>().sizeDelta = new Vector2(0.528f * width, 0.06f * height);
        GameObject.Find("Gage").GetComponent<RectTransform>().sizeDelta = new Vector2(0.7f * width, 0.08f * height);
        GameObject.Find("Skill_Icon").GetComponent<RectTransform>().sizeDelta = new Vector2(0.055f * height, 0.055f * height);
        GameObject.Find("Skill_Icon").GetComponent<RectTransform>().localPosition = new Vector2(-0.285f * width,-0.0015f*height);
        GameObject.Find("Outer").GetComponent<RectTransform>().sizeDelta = new Vector2(0.7f * width, 0.08f * height);
        GameObject.Find("Change").GetComponent<RectTransform>().localPosition = new Vector3(0.48f * width - 0.04f * height, -0.46f * height);
        GameObject.Find("Change").GetComponent<RectTransform>().sizeDelta = new Vector2(0.07f * height, 0.07f * height);

        GameObject.Find("Time").GetComponent<RectTransform>().localPosition = new Vector3(0, 0.42f * height);
        GameObject.Find("Time").GetComponent<RectTransform>().sizeDelta = new Vector2(0.6f * width, 0.1f * height);
        GameObject.Find("Item_coin").GetComponent<RectTransform>().localPosition = new Vector3(0, 0.3f * height);
        GameObject.Find("Item_coin").GetComponent<RectTransform>().sizeDelta = new Vector2(0.45f*height, 0.15f * height);
        GameObject.Find("Item_weapon").GetComponent<RectTransform>().localPosition = new Vector3(0, 0.15f * height);
        GameObject.Find("Item_weapon").GetComponent<RectTransform>().sizeDelta = new Vector2(0.45f * height, 0.15f * height);
        GameObject.Find("Walk_F").GetComponent<RectTransform>().localPosition = new Vector3(0, 0.03f * height);
        GameObject.Find("Walk_F").GetComponent<RectTransform>().sizeDelta = new Vector2(0.8f * width, 0.08f * height);
        GameObject.Find("Walk").GetComponent<RectTransform>().localPosition = new Vector3(0, -0.03f * height);
        GameObject.Find("Walk").GetComponent<RectTransform>().sizeDelta = new Vector2(0.8f*width, 0.08f * height);
        GameObject.Find("To_Game").GetComponent<RectTransform>().localPosition = new Vector3(0, -0.12f * height);
        GameObject.Find("To_Game").GetComponent<RectTransform>().sizeDelta = new Vector2(0.6f * width, 0.1f * height);
        GameObject.Find("To_Menu").GetComponent<RectTransform>().localPosition = new Vector3(0, -0.25f * height);
        GameObject.Find("To_Menu").GetComponent<RectTransform>().sizeDelta = new Vector2(0.8f * width, 0.1f * height);
        GameObject.Find("Option").GetComponent<RectTransform>().localPosition = new Vector3(0.4f*width-0.05f*height, -0.38f * height);
        GameObject.Find("Option").GetComponent<RectTransform>().sizeDelta = new Vector2(0.1f*height, 0.1f * height);
        GameObject.Find("Help").GetComponent<RectTransform>().localPosition = new Vector3(-0.05f*height-0.025f*width, -0.38f * height);
        GameObject.Find("Help").GetComponent<RectTransform>().sizeDelta = new Vector2(0.75f*width-0.1f*height, 0.1f * height);

        GameObject.Find("Flame").GetComponent<RectTransform>().localPosition = new Vector3(0, -0.03f * height);
        GameObject.Find("Flame").GetComponent<RectTransform>().sizeDelta = new Vector2(width, 0.94f * height);
        GameObject.Find("Skill_Flame").GetComponent<RectTransform>().localPosition = new Vector3(0, -height/2f);
        GameObject.Find("Skill_Flame").GetComponent<RectTransform>().sizeDelta = new Vector2(width, 2f * height);

        GameObject.Find("Treasure_count").GetComponent<RectTransform>().localPosition = new Vector3(-0.3f * width, 0.47f * height);
        GameObject.Find("Treasure_count").GetComponent<RectTransform>().sizeDelta = new Vector2(0.08f * width, 0.045f * height);
        GameObject.Find("Count_Text").GetComponent<RectTransform>().localPosition = new Vector3(0.05f*width, -0.015f * height);
        GameObject.Find("Count_Text").GetComponent<RectTransform>().sizeDelta = new Vector2(0.1f * width, 0.05f * height);

        Destroy(this.gameObject, 1f);
    }

    public void Set_Box(Party chara,int ID,int level)
    {
        //スキルについて、20未満だとその秒数がタップスキルになる（組み合わせ可能）、35にすると常時発動（リーダースキル化、組み合わせ可能）
        #region ID=1:剣士
        if (ID == 1)
        {
            chara.Attack = 95+2*level;
            chara.HP = 50+level;
            chara.skills[0] = 35; //敵を早く倒す 
            chara.skills[1] = 0; //その場の敵を一掃
            chara.skills[2] = 0; //宝が見える
            chara.skills[3] = 0; //敵を留める
            chara.skills[4] = 0; //立ち止まる
            chara.skills[5] = 0; //時間が減るのが遅くなる
            chara.skills[6] = 5; //走る
            chara.skills[7] = 0; //敵と会いにくくなる
            /*chara.skills[n0] = 0; //敵が見える
            chara.skills[n1] = 0; //敵と会いにくくなる
            chara.skills[n5] = 0; //敵に完全に合わなくなる（n3,n4と組み合わせで道を自由に)単体だと強すぎる気もする
            chara.skills[n6] = 0; //見たやつを覚える
            chara.skills[n7] = 0; //引き返す
            chara.skills[n8] = 0; //Coinが増える
            chara.skills[n9] = 0; //盤が1瞬で動く
            chara.skills[n10] = 0; //周りが見える（却下っぽい。。）*/
            chara.skill_Description = "走り抜ける";
            chara.Max_gage = 25;
            chara.Max_second = 5;
            chara.Action = Common.SE.Sword;
            chara.skill_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/1-Soldier/Skill");
        }
        #endregion
        #region ID=2:魔女
        else if (ID == 2)
        {
            chara.Attack = 32+level;
            chara.HP = 80+4*level;
            chara.skills[0] = 0; //敵を早く倒す
            chara.skills[1] = 0; //その場の敵を一掃
            chara.skills[2] = 0; //宝が見える
            chara.skills[3] = 10; //敵を留める
            chara.skills[4] = 0; //立ち止まる
            chara.skills[5] = 35; //時間が減るのが遅くなる
            chara.skills[6] = 0; //走る
            chara.skills[7] = 10; //敵と会いにくくなる
            chara.skill_Description = "敵の動きを留める";
            chara.Max_gage = 25;
            chara.Max_second = 10;
            chara.Action = Common.SE.Fire;
            chara.skill_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/2-Witch/Skill");
            chara.skill_SE = Resources.Load<AudioClip>("Audio/SE/Skill-ice");
        }
        #endregion
        #region ID=3:海賊
        else if (ID == 3)
        {
            chara.Attack = 60+level;
            chara.HP = 160+7*level;
            chara.skills[0] = 0; //敵を早く倒す
            chara.skills[1] = 1; //その場の敵を一掃
            chara.skills[2] = 35; //宝が見える
            chara.skills[3] = 1; //敵を留める
            chara.skills[4] = 1; //立ち止まる
            chara.skills[5] = 0; //時間が減るのが遅くなる
            chara.skills[6] = 0; //走る
            chara.skills[7] = 0; //敵と会いにくくなる
            chara.skill_Description = "見えている敵を一掃する";
            chara.Max_gage = 25;
            chara.Max_second = 1;
            chara.Action = Common.SE.Gun;
            chara.skill_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/3-Pirate/Skill");
            chara.skill_SE = Resources.Load<AudioClip>("Audio/SE/Skill-gun");
        }
        #endregion
        #region ID=4:女剣士
        else if (ID == 4)
        {
            chara.Attack = 100+2*level;
            chara.HP = 60+level;
            chara.skills[0] = 35; //敵を早く倒す
            chara.skills[1] = 0; //その場の敵を一掃
            chara.skills[2] = 0; //宝が見える
            chara.skills[3] = 0; //敵を留める
            chara.skills[4] = 0; //立ち止まる
            chara.skills[5] = 0; //時間が減るのが遅くなる
            chara.skills[6] = 5; //走る
            chara.skills[7] = 0; //敵と会いにくくなる
            chara.skill_Description = "いつもより速く駆ける";
            chara.Max_gage = 25;
            chara.Max_second = 5;
            chara.Action = Common.SE.Sword;
            chara.skill_img = Resources.Load<Sprite>("Images/Charactor/Chara_sprite/4-WSoldier/Skill");
            chara.skill_SE = Resources.Load<AudioClip>("Audio/SE/Skill");
        }
        #endregion
        #region ID=?:図鑑に入っていない場合（IDが考えられていない）
        else
        {
            chara.Attack = 1;
            chara.HP = 1;
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
            chara.Max_gage = 100;
            chara.Max_second = 0;
        }
        #endregion

    }
}
