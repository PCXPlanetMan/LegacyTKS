using System;
using System.Collections;
using System.Collections.Generic;
using com.tksr.document;
using UnityEngine;
using UnityEngine.UI;

public class UIInfoPanelStatus : MonoBehaviour
{
    public Text Level;
    public Text HP;
    public Text MP;
    public Text Exp;
    public Text Upgrade;
    public Text Attack;
    public Text Defense;
    public Text HitRatio;
    public Text Dodge;
    public Text Speed;
    public Text Luck;
    public Text Understanding;
    public Text Move;
    public Text SkillPoint;

    public void UpdateStatusData(DataCharStatsInfo info)
    {
        Level.text = info.Level.ToString();
        HP.text = string.Format(ResourceUtils.FORMAT_STATS_VALUE, info.HP, info.MaxHP);
        MP.text = string.Format(ResourceUtils.FORMAT_STATS_VALUE, info.MP, info.MaxMP);
        Attack.text = info.Attack.ToString();
        Defense.text = info.Defense.ToString();
        HitRatio.text = info.HitRatio.ToString();
        Dodge.text = info.Dodge.ToString();
        Speed.text = info.Speed.ToString();
        Luck.text = info.Luck.ToString();
        Understanding.text = info.Understanding.ToString();
        Move.text = info.Move.ToString();
    }
}
