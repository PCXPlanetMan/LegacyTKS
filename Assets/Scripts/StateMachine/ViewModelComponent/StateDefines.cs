using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.tksr.statemachine.defines
{
    /// <summary>
    /// 人物行走方式
    /// </summary>
    public enum EnumLocomotions
    {
        Walk,
        Fly,
        Teleport
    }

    public enum EnumAlliances
    {
        None = 0,
        Neutral = 1 << 0,
        Hero = 1 << 1,
        Enemy = 1 << 2
    }

    public enum EnumEquipSlots
    {
        None = 0,
        Primary = 1 << 0,   // usually a weapon (sword etc)
        Secondary = 1 << 1, // usually a shield, but could be another sword (dual-wield) or occupied by two-handed weapon
        Head = 1 << 2,      // helmet, hat, etc
        Body = 1 << 3,      // body armor, robe, etc
        Accessory = 1 << 4  // ring, belt, etc
    }

    public enum EnumTargets
    {
        None,
        Self,
        Ally,
        Foe,
        Tile
    }

    public enum EnumStatTypes
    {
        LVL, // Level
        EXP, // Experience
        HP,  // Hit Points
        MHP, // Max Hit Points
        MP,  // Magic Points
        MMP, // Max Magic Points
        ATK, // Physical Attack
        DEF, // Physical Defense
        MAT, // Magic Attack - Not Used
        MDF, // Magic Defense - Not Used
        HIT, // Hit Rate
        EVD, // Evade
        SPD, // Speed
        MOV, // Move Range
        RES, // Status Resistance
        JMP, // Jump Height
        CTR, // Counter - for turn order
        Count
    }

    /// <summary>
    /// 状态机中的计算的人物朝向
    /// 基于二维笛卡尔坐标系
    /// </summary>
    public enum EnumStateDirections
    {
        North,  // Positive Y
        East,   // Positive X 
        South,  // Negative Y
        West    // Negative X
    }

    /// <summary>
    /// 状态驱动器(我方手动或者AI,敌方一定是AI)
    /// </summary>
    public enum EnumDrivers
    {
        None,
        Human,
        Computer
    }

    public enum EnumFacings
    {
        Front,
        Side,
        Back
    }

    public enum EnumActionCommand
    {
        Invalid = -1,
        Move,
        Attack,
        Slot1,
        Slot2,
        Defense,
        Wait
    }
}
