using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.tksr.property
{
    /// <summary>
    /// 人物八方向
    /// </summary>
    public enum EnumDirection
    {
        N,
        NW,
        W,
        SW,
        S,
        SE,
        E,
        NE
    }

    /// <summary>
    /// 人物朝向
    /// </summary>
    public enum EnumOrientation
    {
        Center,
        Left,
        Right
    }

    /// <summary>
    /// 人物基本动画系统
    /// </summary>
    public enum EnumAnimAction
    {
        Static,
        Run,
        Attack,
        Idle,
        // Special
        Speak,
        Angry,
        Scolded,
        Disappear,
        Trapped,
        Cry,
        Sleep,
        Drinking,
        Drunk,
        Dance
    }

    /// <summary>
    /// 武器类别
    /// </summary>
    public enum EnumWeaponType
    {
        None,
        Sword,
        Blade,
        Arrow,
        Spear, // 枪
        Pike, // 矛
        Halberd, // 戟
        Broadsword, // 大刀
        Fan,
        Hammer,
        Dice,
        All
    }

    public enum EnumInteractiveAction
    {
        None,
        WalkingToTaskPos, // 走到点击NPC触发任务的位置
        DoingTask,
        IntoEntry,
    }
}
