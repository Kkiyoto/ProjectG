using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Dictionary
 * Dictionaryにアタッチするけど速攻Destroy
 * Partyにデータの引継ぎをする用
 * これでPlayerPrefasは2つずつぐらいで済むかな(図鑑IDとレベル)
 */

public class Dictionary : MonoBehaviour
{
    float width, height;
    //なるべく同じ名前にします。consoleから入れるのお願いします。
    public GameObject Sentou_kaisi, Time_text, Skill_Text;
    public RectTransform Map_base, Image, Time_base, Time_needle, Time_gage, Pause, Skill, Gage, Skill_Icon, Outer, Change, Time, Item_coin
        , Item_weapon, Walk_F, Walk, To_Game, To_Menu, Option, Help, Start_and_End_anim, Hikousen, Skill_effect, Battle_down_panel;

    private void Start()
    {
        width = Screen.width;
        height = Screen.height;

        Sentou_kaisi.GetComponent<RectTransform>().localPosition = new Vector3(0, -0.04f * height);
        Sentou_kaisi.GetComponent<RectTransform>().sizeDelta = new Vector2(0.76f * width, 0.248f * width);

        Map_base.localPosition = new Vector3(0.395f * width, 0.5f * height - 0.105f * width);
        Map_base.sizeDelta = new Vector2(0.21f * width, 0.21f * width);
        Image.sizeDelta = new Vector2(width, 0.24f * height);
        Time_base.sizeDelta = new Vector2(width, 0.24f * height);
        Time_needle.sizeDelta = new Vector2(0.3f*height, 0.25f * height);
        Time_needle.localPosition = new Vector3(0, -0.013f * height);
        Time_gage.localPosition = new Vector3(0, -0.013f * height);
        Time_gage.sizeDelta = new Vector2(0.2f*height, 0.2f * height);
        Time_text.GetComponent<RectTransform>().localPosition = new Vector3(0, -0.05f * height);
        Time_text.GetComponent<RectTransform>().sizeDelta = new Vector2(0.2f * width, 0.1f * height);
        Time_text.GetComponent<Text>().fontSize = Mathf.RoundToInt(0.035f * height);
        Pause.localPosition = new Vector3( - 0.42f * width, 0.462f * height);
        Pause.sizeDelta = new Vector2(0.14f *width, 0.064f * height);
        Skill.localPosition = new Vector3(-0.13f * width, -0.46f * height);
        Skill.sizeDelta = new Vector2(0.7f * width, 0.08f * height);
        Skill_Text.GetComponent<RectTransform>().localPosition = new Vector3(0.044f * width, 0.02f * height);
        Skill_Text.GetComponent<RectTransform>().sizeDelta = new Vector2(0.528f * width, 0.06f * height);
        Skill_Text.GetComponent<Text>().fontSize = Mathf.RoundToInt(0.025f * height);
        Gage.sizeDelta = new Vector2(0.7f * width, 0.08f * height);
        Skill_Icon.sizeDelta = new Vector2(0.055f * height, 0.055f * height);
        Skill_Icon.localPosition = new Vector2(-0.285f * width,-0.0015f*height);
        Outer.sizeDelta = new Vector2(0.7f * width, 0.08f * height);
        Change.localPosition = new Vector3(0.48f * width - 0.04f * height, -0.46f * height);
        Change.sizeDelta = new Vector2(0.07f * height, 0.07f * height);

        Time.localPosition = new Vector3(0, 0.42f * height);
        Time.sizeDelta = new Vector2(0.6f * width, 0.1f * height);
        Item_coin.localPosition = new Vector3(0, 0.3f * height);
        Item_coin.sizeDelta = new Vector2(0.45f*height, 0.15f * height);
        Item_weapon.localPosition = new Vector3(0, 0.15f * height);
        Item_weapon.sizeDelta = new Vector2(0.45f * height, 0.15f * height);
        Walk_F.localPosition = new Vector3(0, 0.03f * height);
        Walk_F.sizeDelta = new Vector2(0.8f * width, 0.08f * height);
        Walk.localPosition = new Vector3(0, -0.03f * height);
        Walk.sizeDelta = new Vector2(0.8f*width, 0.08f * height);
        To_Game.localPosition = new Vector3(0, -0.12f * height);
        To_Game.sizeDelta = new Vector2(0.6f * width, 0.1f * height);
        To_Menu.localPosition = new Vector3(0, -0.25f * height);
        To_Menu.sizeDelta = new Vector2(0.8f * width, 0.1f * height);
        Option.localPosition = new Vector3(0.4f*width-0.05f*height, -0.38f * height);
        Option.sizeDelta = new Vector2(0.1f*height, 0.1f * height);
        Help.localPosition = new Vector3(-0.05f*height-0.025f*width, -0.38f * height);
        Help.sizeDelta = new Vector2(0.75f*width-0.1f*height, 0.1f * height);

        Start_and_End_anim.localPosition = new Vector3(-1 * width, -0.05f * height);
        Start_and_End_anim.sizeDelta = new Vector2(0.45f * width, 0.065f * height);
        Hikousen.localPosition = new Vector3(-1 * width, 0.8f * height);
        Hikousen.sizeDelta = new Vector2(0.6f * width, 0.225f * height); // original : 0.4f * width, 0.15f * height
        GameObject.Find("FallChara").GetComponent<RectTransform>().sizeDelta = new Vector2(0.8f * width, 0.25f * height);

        Battle_down_panel.localPosition = new Vector3(0, -0.35f * height);
        Battle_down_panel.sizeDelta = new Vector2(0.1f*width, 0.01f*height);
        Skill_effect.localPosition = new Vector3(0, -0.18f * height);
        Skill_effect.sizeDelta = new Vector2( 1.05f*width,  0.65f*height);
        GameObject.Find("BattleEnemy").GetComponent<RectTransform>().localPosition = new Vector3(0.35f*width, height * 0.25f); //2.0f*width, height * 0.25f
        GameObject.Find("BattleEnemy").GetComponent<RectTransform>().sizeDelta = new Vector2(0.27f * width, 0.15f * height);
        GameObject.Find("Attack_effect").GetComponent<RectTransform>().localPosition = new Vector3(0.35f * width, height * 0.25f);
        GameObject.Find("Attack_effect").GetComponent<RectTransform>().sizeDelta = new Vector2(0.27f * width, 0.15f * height);

        GameObject.Find("TakaraBako").GetComponent<RectTransform>().localPosition = new Vector3(0.35f * width, height * 0.25f);
        GameObject.Find("TakaraBako").GetComponent<RectTransform>().sizeDelta = new Vector2(0.27f * width, 0.15f * height);
        GameObject.Find("OtakaraGet").GetComponent<RectTransform>().localPosition = new Vector3(-width, 0);
        GameObject.Find("OtakaraGet").GetComponent<RectTransform>().sizeDelta = new Vector2(0.27f * width, 0.07f * height);

        GameObject.Find("Skill_Icon_effect").GetComponent<RectTransform>().sizeDelta = new Vector2(0.2f * height, 0.2f * height);
        GameObject.Find("Skill_Icon_effect").GetComponent<RectTransform>().localPosition = new Vector2(-0.285f * width, -0.0015f * height);

        GameObject.Find("Flame").GetComponent<RectTransform>().localPosition = new Vector3(0, -0.03f * height);
        GameObject.Find("Flame").GetComponent<RectTransform>().sizeDelta = new Vector2(width, 0.94f * height);
        GameObject.Find("Skill_Flame").GetComponent<RectTransform>().localPosition = new Vector3(0, -height);
        GameObject.Find("Skill_Flame").GetComponent<RectTransform>().sizeDelta = new Vector2(1.2f*width, 4f * height);

        GameObject.Find("Treasure_count").GetComponent<RectTransform>().localPosition = new Vector3(-0.3f * width, 0.47f * height);
        GameObject.Find("Treasure_count").GetComponent<RectTransform>().sizeDelta = new Vector2(0.08f * width, 0.045f * height);
        GameObject.Find("Count_Text").GetComponent<RectTransform>().localPosition = new Vector3(0.05f*width, -0.015f * height);
        GameObject.Find("Count_Text").GetComponent<RectTransform>().sizeDelta = new Vector2(0.1f * width, 0.05f * height);
        GameObject.Find("Count_Text").GetComponent<Text>().fontSize = Mathf.RoundToInt(0.022f * height);

        GameObject.Find("GameOver_text").GetComponent<RectTransform>().sizeDelta = new Vector2(0.9f * width, 0.08f * height);
        GameObject.Find("GameOver_text").GetComponent<RectTransform>().localPosition = new Vector2(width, 0);
        GameObject.Find("Game_over").GetComponent<RectTransform>().sizeDelta = new Vector2(0.6f * width, 0.08f * height);
        GameObject.Find("Game_over").GetComponent<RectTransform>().localPosition = new Vector2(width, 0);
        GameObject.Find("Retry").GetComponent<RectTransform>().sizeDelta = new Vector2(0.6f * width, 0.08f * height);
        GameObject.Find("Retry").GetComponent<RectTransform>().localPosition = new Vector2(width, 0);

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
            chara.skills[0] = 0; //敵を早く倒す 
            chara.skills[1] = 0; //その場の敵を一掃
            chara.skills[2] = 0; //宝が見える
            chara.skills[3] = 0; //敵を留める
            chara.skills[4] = 1; //立ち止まる
            chara.skills[5] = 0; //時間が減るのが遅くなる
            chara.skills[6] = 35; //走る
            chara.skills[7] = 0; //敵と会わなくなる
            chara.skills[8] = 1; //振り返る
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
            chara.skills[7] = 10; //敵と会わなくなる
            chara.skills[8] = 0; //振り返る
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
            chara.skills[7] = 0; //敵と会わなくなる
            chara.skills[8] = 0; //振り返る
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
            chara.skills[0] = 0; //敵を早く倒す
            chara.skills[1] = 0; //その場の敵を一掃
            chara.skills[2] = 0; //宝が見える
            chara.skills[3] = 0; //敵を留める
            chara.skills[4] = 0; //立ち止まる
            chara.skills[5] = 0; //時間が減るのが遅くなる
            chara.skills[6] = 35; //走る
            chara.skills[7] = 0; //敵と会わなくなる 
            chara.skills[8] = 1; //振り返る
            chara.skill_Description = "引き返すことが出来る";
            chara.Max_gage = 25;
            chara.Max_second = 1;
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
