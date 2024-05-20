using com.tksr.property;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.tksr.data
{
    public enum EnumGameTagType
    {
        Unknown,

        GOGameMap,
        GOGameDesigner,

        // 游戏中特殊的GameObject
        GOContainNPCs,
        GOContainTeam,
        GOContainEntries,

        // 地图场景交互元素类型(主要是响应Cursor的形状)
        HittableNPC,
        HittableHiddenObj,


        ColliderScenarioEntry,
        ColliderHiddenObject,
        ColliderNPC,
    }

    public enum EnumGameLayer
    {
        GameAlert,
        GameChar,
        GameBorder,
    }

    /// <summary>
    /// 游戏状态
    /// </summary>
    public enum EnumGameMode
    {
        RunningScenario,
        Gameplay,
        MainContentUI,
        ScenarioDialogMoment,
        SceneTaskDialog,
        ScenarioStaticNote,
        BlackMaskForDialog,
        WaitingInputName,
        Battle,
        ScenarioDialogHasSelection,
        InLittleGameUI,
    }

    public enum EnumToastType
    {
        Normal = 0,
        TKRItem,
        Money,
        Skill,
        Char,
        IntCourageMor,
        Text
    }

    public enum EnumTinyGameUI
    {
        Invalid = 0,
        InputName,
        PuzzleBaGua,
        PuzzleHuaRongDao
    }

    public enum EnumBattleAnimStatus
    {
        None = 0,
        Open,
        EndWin,
        EndLose
    }

    // 事件状态
    public enum EnumEventStatus
    {
        NotHappened,
        Running,
        Finished,
    }

    // 剧情状态 
    public enum EnumScenarioStatus
    {
        NotHappened,
        Running,
        Finished,
    }

    public enum EnumCampType
    {
        None,
        Wei,
        Shu,
        Wu,
        Other
    }

    public static class AnimationDirectionData
    {
        #region 人物在世界场景地图中只有Static/Run两套固定动画(主角可能有8方向)
        public static Dictionary<EnumDirection, string> EightStaticDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.N, "Static N" },
            { EnumDirection.NW, "Static NW" },
            { EnumDirection.W, "Static W" },
            { EnumDirection.SW, "Static SW" },
            { EnumDirection.S, "Static S" },
            { EnumDirection.SE, "Static SE" },
            { EnumDirection.E, "Static E" },
            { EnumDirection.NE, "Static NE" }
        };
        public static Dictionary<EnumDirection, string> EightRunDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.N, "Run N" },
            { EnumDirection.NW, "Run NW" },
            { EnumDirection.W, "Run W" },
            { EnumDirection.SW, "Run SW" },
            { EnumDirection.S, "Run S" },
            { EnumDirection.SE, "Run SE" },
            { EnumDirection.E, "Run E" },
            { EnumDirection.NE, "Run NE" }
        };
        public static Dictionary<EnumDirection, string> FourStaticDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "Static NW" },
            { EnumDirection.SW, "Static SW" },
            { EnumDirection.SE, "Static SE" },
            { EnumDirection.NE, "Static NE" }
        };
        public static Dictionary<EnumDirection, string> FourRunDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "Run NW" },
            { EnumDirection.SW, "Run SW" },
            { EnumDirection.SE, "Run SE" },
            { EnumDirection.NE, "Run NE" }
        };
        #endregion

        #region 在战斗地图中,所有人物都只有4方向的Static/Run/Attack固定动画,主角可能不同的装备
        #region 徒手
        public static Dictionary<EnumDirection, string> StaticNoneDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "StaticNone NW" },
            { EnumDirection.SW, "StaticNone SW" },
            { EnumDirection.SE, "StaticNone SE" },
            { EnumDirection.NE, "StaticNone NE" }
        };
        public static Dictionary<EnumDirection, string> RunNoneDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "RunNone NW" },
            { EnumDirection.SW, "RunNone SW" },
            { EnumDirection.SE, "RunNone SE" },
            { EnumDirection.NE, "RunNone NE" }
        };
        public static Dictionary<EnumDirection, string> AttackNoneDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "AttackNone NW" },
            { EnumDirection.SW, "AttackNone SW" },
            { EnumDirection.SE, "AttackNone SE" },
            { EnumDirection.NE, "AttackNone NE" }
        };
        #endregion

        #region 剑
        public static Dictionary<EnumDirection, string> StaticSwordDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "StaticSword NW" },
            { EnumDirection.SW, "StaticSword SW" },
            { EnumDirection.SE, "StaticSword SE" },
            { EnumDirection.NE, "StaticSword NE" }
        };
        public static Dictionary<EnumDirection, string> RunSwordDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "RunSword NW" },
            { EnumDirection.SW, "RunSword SW" },
            { EnumDirection.SE, "RunSword SE" },
            { EnumDirection.NE, "RunSword NE" }
        };
        public static Dictionary<EnumDirection, string> AttackSwordDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "AttackSword NW" },
            { EnumDirection.SW, "AttackSword SW" },
            { EnumDirection.SE, "AttackSword SE" },
            { EnumDirection.NE, "AttackSword NE" }
        };
        #endregion

        #region 刀
        public static Dictionary<EnumDirection, string> StaticBladeDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "StaticBlade NW" },
            { EnumDirection.SW, "StaticBlade SW" },
            { EnumDirection.SE, "StaticBlade SE" },
            { EnumDirection.NE, "StaticBlade NE" }
        };
        public static Dictionary<EnumDirection, string> RunBladeDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "RunBlade NW" },
            { EnumDirection.SW, "RunBlade SW" },
            { EnumDirection.SE, "RunBlade SE" },
            { EnumDirection.NE, "RunBlade NE" }
        };
        public static Dictionary<EnumDirection, string> AttackBladeDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "AttackBlade NW" },
            { EnumDirection.SW, "AttackBlade SW" },
            { EnumDirection.SE, "AttackBlade SE" },
            { EnumDirection.NE, "AttackBlade NE" }
        };
        #endregion

        #region 弓箭
        public static Dictionary<EnumDirection, string> StaticArrowDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "StaticArrow NW" },
            { EnumDirection.SW, "StaticArrow SW" },
            { EnumDirection.SE, "StaticArrow SE" },
            { EnumDirection.NE, "StaticArrow NE" }
        };
        public static Dictionary<EnumDirection, string> RunArrowDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "RunArrow NW" },
            { EnumDirection.SW, "RunArrow SW" },
            { EnumDirection.SE, "RunArrow SE" },
            { EnumDirection.NE, "RunArrow NE" }
        };
        public static Dictionary<EnumDirection, string> AttackArrowDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "AttackArrow NW" },
            { EnumDirection.SW, "AttackArrow SW" },
            { EnumDirection.SE, "AttackArrow SE" },
            { EnumDirection.NE, "AttackArrow NE" }
        };
        #endregion

        #region 枪
        public static Dictionary<EnumDirection, string> StaticSpearDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "StaticSpear NW" },
            { EnumDirection.SW, "StaticSpear SW" },
            { EnumDirection.SE, "StaticSpear SE" },
            { EnumDirection.NE, "StaticSpear NE" }
        };
        public static Dictionary<EnumDirection, string> RunSpearDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "RunSpear NW" },
            { EnumDirection.SW, "RunSpear SW" },
            { EnumDirection.SE, "RunSpear SE" },
            { EnumDirection.NE, "RunSpear NE" }
        };
        public static Dictionary<EnumDirection, string> AttackSpearDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "AttackSpear NW" },
            { EnumDirection.SW, "AttackSpear SW" },
            { EnumDirection.SE, "AttackSpear SE" },
            { EnumDirection.NE, "AttackSpear NE" }
        };
        #endregion

        #region 矛
        public static Dictionary<EnumDirection, string> StaticPikeDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "StaticPike NW" },
            { EnumDirection.SW, "StaticPike SW" },
            { EnumDirection.SE, "StaticPike SE" },
            { EnumDirection.NE, "StaticPike NE" }
        };
        public static Dictionary<EnumDirection, string> RunPikeDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "RunPike NW" },
            { EnumDirection.SW, "RunPike SW" },
            { EnumDirection.SE, "RunPike SE" },
            { EnumDirection.NE, "RunPike NE" }
        };
        public static Dictionary<EnumDirection, string> AttackPikeDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "AttackPike NW" },
            { EnumDirection.SW, "AttackPike SW" },
            { EnumDirection.SE, "AttackPike SE" },
            { EnumDirection.NE, "AttackPike NE" }
        };
        #endregion

        #region 戟
        public static Dictionary<EnumDirection, string> StaticHalberdDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "StaticHalberd NW" },
            { EnumDirection.SW, "StaticHalberd SW" },
            { EnumDirection.SE, "StaticHalberd SE" },
            { EnumDirection.NE, "StaticHalberd NE" }
        };
        public static Dictionary<EnumDirection, string> RunHalberdDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "RunHalberd NW" },
            { EnumDirection.SW, "RunHalberd SW" },
            { EnumDirection.SE, "RunHalberd SE" },
            { EnumDirection.NE, "RunHalberd NE" }
        };
        public static Dictionary<EnumDirection, string> AttackHalberdDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "AttackHalberd NW" },
            { EnumDirection.SW, "AttackHalberd SW" },
            { EnumDirection.SE, "AttackHalberd SE" },
            { EnumDirection.NE, "AttackHalberd NE" }
        };
        #endregion

        #region 大刀
        public static Dictionary<EnumDirection, string> StaticBroadswordDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "StaticBroadsword NW" },
            { EnumDirection.SW, "StaticBroadsword SW" },
            { EnumDirection.SE, "StaticBroadsword SE" },
            { EnumDirection.NE, "StaticBroadsword NE" }
        };
        public static Dictionary<EnumDirection, string> RunBroadswordDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "RunBroadsword NW" },
            { EnumDirection.SW, "RunBroadsword SW" },
            { EnumDirection.SE, "RunBroadsword SE" },
            { EnumDirection.NE, "RunBroadsword NE" }
        };
        public static Dictionary<EnumDirection, string> AttackBroadswordDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "AttackBroadsword NW" },
            { EnumDirection.SW, "AttackBroadsword SW" },
            { EnumDirection.SE, "AttackBroadsword SE" },
            { EnumDirection.NE, "AttackBroadsword NE" }
        };
        #endregion

        #region 扇子
        public static Dictionary<EnumDirection, string> StaticFanDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "StaticFan NW" },
            { EnumDirection.SW, "StaticFan SW" },
            { EnumDirection.SE, "StaticFan SE" },
            { EnumDirection.NE, "StaticFan NE" }
        };
        public static Dictionary<EnumDirection, string> RunFanDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "RunFan NW" },
            { EnumDirection.SW, "RunFan SW" },
            { EnumDirection.SE, "RunFan SE" },
            { EnumDirection.NE, "RunFan NE" }
        };
        public static Dictionary<EnumDirection, string> AttackFanDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "AttackFan NW" },
            { EnumDirection.SW, "AttackFan SW" },
            { EnumDirection.SE, "AttackFan SE" },
            { EnumDirection.NE, "AttackFan NE" }
        };
        #endregion

        #region 锤
        public static Dictionary<EnumDirection, string> StaticHammerDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "StaticHammer NW" },
            { EnumDirection.SW, "StaticHammer SW" },
            { EnumDirection.SE, "StaticHammer SE" },
            { EnumDirection.NE, "StaticHammer NE" }
        };
        public static Dictionary<EnumDirection, string> RunHammerDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "RunHammer NW" },
            { EnumDirection.SW, "RunHammer SW" },
            { EnumDirection.SE, "RunHammer SE" },
            { EnumDirection.NE, "RunHammer NE" }
        };
        public static Dictionary<EnumDirection, string> AttackHammerDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "AttackHammer NW" },
            { EnumDirection.SW, "AttackHammer SW" },
            { EnumDirection.SE, "AttackHammer SE" },
            { EnumDirection.NE, "AttackHammer NE" }
        };
        #endregion

        #region 骰
        public static Dictionary<EnumDirection, string> StaticDiceDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "StaticDice NW" },
            { EnumDirection.SW, "StaticDice SW" },
            { EnumDirection.SE, "StaticDice SE" },
            { EnumDirection.NE, "StaticDice NE" }
        };
        public static Dictionary<EnumDirection, string> RunDiceDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "RunDice NW" },
            { EnumDirection.SW, "RunDice SW" },
            { EnumDirection.SE, "RunDice SE" },
            { EnumDirection.NE, "RunDice NE" }
        };
        public static Dictionary<EnumDirection, string> AttackDiceDirections = new Dictionary<EnumDirection, string>()
        {
            { EnumDirection.NW, "AttackDice NW" },
            { EnumDirection.SW, "AttackDice SW" },
            { EnumDirection.SE, "AttackDice SE" },
            { EnumDirection.NE, "AttackDice NE" }
        };
        #endregion

        #endregion
    }

    public static class GameConstData
    {
        #region 游戏内特殊GameObjects
        public static readonly string GAME_TAG_GO_AUDIO_BACKGROUND = "Background";
        public static readonly string GAME_TAG_GO_AUDIO_EFFECT = "Effect";
        #endregion

        public static int TOAST_ID_INTELLIGENCE_UP = 11001;
        public static int TOAST_ID_INTELLIGENCE_DOWN = 11002;
        public static int TOAST_ID_COURAGE_UP = 11003;
        public static int TOAST_ID_COURAGE_DOWN = 11004;
        public static int TOAST_ID_MORALITY_UP = 11005;
        public static int TOAST_ID_MORALITY_DOWN = 11006;
    }

}
