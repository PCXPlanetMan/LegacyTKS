using System.Collections.Generic;

namespace com.tksr.schema
{
    #region SchemaSceneMap
    [System.Serializable]
    public class SceneMapItem
    {
        public int Id;
        public string Name;
        public string Map;
        public string UnityScene;
        public string SceneAssetBundle;
        public string MapType;
        public string ScenariosList;
        public string DisplayName;
        public int BgMusicId;
    }

    public enum EnumBattleResultAction
    {
        None,
        SwitchSceneOpenScenario,
    }

    [System.Serializable]
    public class BattleFieldItem
    {
        public int Id;
        public string Name;
        public string BattleFieldRes;
        public string BattleFieldAssetBundle;
        public string RequiredChars;
        public string OptionalTemplateChars;
        public int OptionalMin;
        public int OptionalMax;
        public string ResultSuccAction;
        public string ResultFailedAction;
    }

    [System.Serializable]
    public class SceneEntryItem
    {
        public int Id;
        public string EntryName;
        public int ConnectToEntry;
        public int BelongToScenario;
    }

    [System.Serializable]
    public class SchemaSceneMap
    {
        public Dictionary<string, SceneMapItem> maps;
        public Dictionary<string, SceneEntryItem> entries;
        public Dictionary<string, BattleFieldItem> battlefields;
    }

    public enum EnumMapType
    {
        Invalid = -1,
        BigMap = 0,
        Scenarios,
        Battle
    }
    #endregion

    #region SchemaStoryLine

    public enum EnumEventType
    {
        Invalid,
        MainStoryLine,
        SubTask,
        HelpScenario
    }

    public enum EnumScenarioCondition
    {
        Opened,
        PreFinished,
        InScene,
        InSceneDoing,
        InSceneAndPreFinished,
        FromEntryDoing,
        InSceneInTeam,
        InSceneNotInTeam,
        EventsDone,
        LevelBelow,
        LevelNotBelow
    }

    public enum EnumScenarioDoTaskContent
    {
        None,
        PlayTimeline,
        ShowConversations,
        ShowSelections,
        JumpToPost,
        ShowToast
    }

    public enum EnumScenarioPostTasks
    {
        None,
        Repeat,
        PushTasks,
        UseMoneyDoConversations,
        DoTasks,
        NextScenarios,
        NextScene,
        InitGameEvents,
        UpdateTasks,
        Pop,
        OpenBattle,
        CloseScenarios,
        OpenScenarios,
        ChkScenariosRunningToDoTasks,
        OpenLittleGameUI,
        PopTasks,
        UnloadChars,
        ChkItemsOwnedByCharToDoTasks,
        ConsumeItemsToTaskChar,
        OpenSceneEntries,
        GotItems,
        UpdateScenariosByTasksDone,
        ChkTasksRunningToDoTasks,
        ChkTasksRunningToOpenScenarios
    }

    public enum EnumStoryActionType
    {
        None,
        CameraFollowMainRole,
        OpenEnvironmentColliders,
        CloseEnvironmentColliders,
        OpenSceneEntryColliders,
        CloseSceneEntryColliders,
        TimeStopMainRoleFrame,
        TimeStopNPCFrame,
        FreeActivity,
        UnloadScenarioChars, // Timeline结束时,可以释放当前Timeline所使用的Char
        ResetDeployments,
        PlaceMainRoleInEntry
    }

    [System.Serializable]
    public class StoryScenarioItem
    {
        public int Id;
        public string Name;
        public int ParentEventId;
        public string PreScenarioIds;
        public string NextScenarioIds;
        public string DeployChars;
        public string InitAssignedTasks;
        public string Conditions;
        public string Notes;
    }

    [System.Serializable]
    public class StoryDeploymentItem
    {
        public int Id;
        public int BelongScene;
        public string Deployments;
        public string StoryActions;
    }

    public enum EnumTaskObjBelongType
    {
        Char,
        HiddenObj,
        Entry,
        Scene
    }

    [System.Serializable]
    public class StoryTaskItem
    {
        public int Id;
        public int BelongObjId;
        public string BelongObjType;
        public int BelongScenario;
        public string TaskContent;
        public string PostTasks;
    }

    public enum EnumFinishedCondition
    {
        AllScenariosDone,
        ScenariosDone,
        NeverDone,
    }

    [System.Serializable]
    public class StoryEventItem
    {
        public int Id;
        public string EventType;
        public string PossibleScenarios;
        public string FinishedCondition;
        public string DefaultToOpenScenarios;
    }

    [System.Serializable]
    public class LittleGameItem
    {
        public int Id;
        public string Type;
        public string ResultSuccTasks;
        public string ResultFailedTasks;
    }

    [System.Serializable]
    public class SchemaStoryLine
    {
        public Dictionary<string, StoryEventItem> events;
        public Dictionary<string, StoryScenarioItem> scenarios;
        public Dictionary<string, StoryDeploymentItem> deployments;
        public Dictionary<string, StoryTaskItem> tasks;
        public Dictionary<string, LittleGameItem> littlegames;
    }
    #endregion

    #region SchemaTexts
    public enum EnumTextContentType
    {
        Normal,
        Format
    }

    [System.Serializable]
    public class TextsInformationItem
    {
        public int Id;
        public string Contents;
        public string Type;
    }

    [System.Serializable]
    public class TextsDialogItem
    {
        public int Id;
        public string Contents;
        public int Speaker;
        public string Type;
        public string ExtraActions;
    }

    public enum EnumDialogContentType
    {
        Normal,
        FullName,
        FirstName,
        LastName,
        LastNameX2,
        SplitName,
        OS,
        Selections,
        Sigh,
        BlackMask,
    }

    public enum EnumDialogExtraAction
    {
        None,
        FaceTo,
        KeepAnim
    }

    [System.Serializable]
    public class TextsHistoryItem
    {
        public int Id;
        public string Contents;
    }

    [System.Serializable]
    public class TextsNoteItem
    {
        public int Id;
        public string Contents;
        public string Type;
    }

    [System.Serializable]
    public class SchemaTexts
    {
        public Dictionary<string, TextsDialogItem> dialogs;
        public Dictionary<string, TextsInformationItem> informations;
        public Dictionary<string, TextsHistoryItem> histories;
        public Dictionary<string, TextsNoteItem> notes;
    }

    #endregion

    #region SchemaCharacter
    [System.Serializable]
    public class CharResItem
    {
        public int Id;
        public string TemplateName;
        public string AssetBundleName;
        public string WorldController;
        public string BattleController;
        public string FullPortraitPath;
        public string HalfPortraitPath;
        public float DlgLocalX;
        public float DlgLocalY;
    }

    [System.Serializable]
    public class CharDataItem
    {
        public int Id;
        public string WeaponType;
        public string Job;
        public string Attack;
        public string AbilityCatalog;
        public string Strategy;
        public string Locomotions;
        public string Alliance;
    }

    [System.Serializable]
    public class CharInstanceItem
    {
        public int CharID;
        public int AttachResID;
        public string InstanceName;
        public int IsHiddenObj;
    }

    [System.Serializable]
    public class SchemaCharacter
    {
        public Dictionary<string, CharResItem> characterResource;
        public Dictionary<string, CharDataItem> characterData;
        public Dictionary<string, CharInstanceItem> characterInstances;
    }
    #endregion

    #region SchemaStateMachine
    [System.Serializable]
    public class CharJob
    {
        public string Job;
        public int MHP;
        public int MMP;
        public int ATK;
        public int DEF;
        public int MAT; // Not Used
        public int MDF; // Not Used
        public int HIT;
        public int EVD;
        public int SPD;
        public float GMHP; // Not Used
        public float GMMP; // Not Used
        public float GATK; // Not Used
        public float GDEF; // Not Used
        public float GMAT; // Not Used
        public float GMDF; // Not Used
        public float GHIT; // Not Used
        public float GEVD; // Not Used
        public float GSPD; // Not Used
        public int MOV;
        public int USD;
        public int LUCK;
        public int INT;
        public int MOL;
        public int CRG;
        public int MED;
        public int RES; // Not Used
        public int JMP; // Not Used
    }

    // 等级范围为1-60
    // 最大生命/最大内力/攻击/防御/命中/闪避随等级上升而增加的值=目前值/基数+1
    // 速度每升一级+1
    // 幸运/悟性/移动不会随等级提升而改变
    // 50悟性为基础设定值，100悟性-25%的JP消耗，0悟性+25%的JP消耗
    [System.Serializable]
    public class CharJobGrow
    {
        public string Job;
        public string MHPGrow;
        public string MMPGrow;
        public string ATKGrow;
        public string DEFGrow;
        public string MATGrow;
        public string MDFGrow;
        public string HITGrow;
        public string EVDGrow;
        public string SPDGrow;
        public string MOVGrow;
    }

    [System.Serializable]
    public class CharJobInfo
    {
        public string Job;
        public int BornLV;
        public int BornJP;
        public int CondINT;
        public int CondMOL;
        public int CondCRG;
        public string CondSpec;
        public int BornWeapon;
        public int BornArmor;
        public string BornSkillsSpell;
        public string BornSkillsWuShuDetail;
        public string BornSkillsShootingDetail;
        public string BornSkillsAuxiliaryDetail;
        public string BornSkillsSpecial;
        public string BornSkillsExclusiveDetail;
        public string BornSkillsActive1Detail;
        public string BornSkillsActive2Detail;
        public string BornSkillsPassiveDetail;
        public string BornSkillsMovementDetail;
    }

    [System.Serializable]
    public class CharAbilityCatalogRecipes
    {
        public string AbilityCatalog;
        public string Categories;
    }

    [System.Serializable]
    public class CharAbilityCategory
    {
        public string AbilityCategory;
        public string Abilities;
    }

    [System.Serializable]
    public class CharAbilityData
    {
        public string AbilityName;
        public string AbilityPower;
        public string AbilityRange;
        public string AbilityArea;
        public int AbilityCost;
        public string AbilitySubParam;
    }

    [System.Serializable]
    public class CharAbilityParam
    {
        public string ParamName;
        public string HitRate;
        public string AbilityEffectTarget;
        public string AbilityEffect;
    }

    [System.Serializable]
    public class CharAttackPattern
    {
        public string PatternName;
        public string PatternPickers;
        public string Patterns;
    }

    [System.Serializable]
    public class SchemaStateMachine
    {
        public Dictionary<string, CharJob> jobData;
        public Dictionary<string, CharJobGrow> jobGrow;
        public Dictionary<string, CharJobInfo> jobInfo;
        public Dictionary<string, CharAbilityCatalogRecipes> abilityCatalogRecipes;
        public Dictionary<string, CharAbilityCategory> abilityCategory;
        public Dictionary<string, CharAbilityData> abilityData;
        public Dictionary<string, CharAbilityParam> abilityParam;
        public Dictionary<string, CharAttackPattern> attackPatterns;
    }
    #endregion

    #region SchemaItems
    [System.Serializable]
    public class TKRTypeItem
    {
        public int Id;
        public string Name;
        public string Type;
    }

    public enum EnumGameItemType
    {
        Invalid = 0,
        Medic,
        Prop,
        Weapon,
        Armor,
        Accessory,
        Special
    }

    [System.Serializable]
    public class TKRItem
    {
        public int Id;
        public string Icon;
        public int UseAuthority;
        public string Description;
    }

    [System.Serializable]
    public class TKRItemMedic : TKRItem
    {
        public int Price;
        public string ApplyStatus;
        public string RemoveStatus;
        public int RecoverHP;
        public int RecoverMP;
        public int IncMHP;
        public int IncMMP;
        public int IncATK;
        public int IncHIT;
    }

    [System.Serializable]
    public class TKRItemProp : TKRItem
    {
        public int Price;
        public string ApplyStatus;
        public int RecoverHP;
        public int RecoverMP;
        public int IncMHP;
        public int IncMMP;
        public int IncATK;
        public int IncDEF;
        public int IncHIT;
        public int IncEVD;
        public int IncMED;
        public int IncUSD;
        public int IncJP;
        public int IncLuck;
        public int IncSPD;
    }

    [System.Serializable]
    public class TKRItemWeapon : TKRItem
    {
        public int Price;
        public string ApplyStatus;
        public int IncATK;
        public int IncHIT;
        public string ExtraSkills;
    }

    [System.Serializable]
    public class TKRItemArmor : TKRItem
    {
        public int Price;
        public string ApplyStatus;
        public int IncDEF;
        public int IncEVD;
        public string ExtraSkills;
    }

    [System.Serializable]
    public class TKRItemAccessory : TKRItem
    {
        public int Price;
        public int IncATK;
        public int IncDEF;
        public int IncHIT;
        public int IncEVD;
        public int IncUSD;
        public int IncLuck;
        public int IncSPD;
        public int IncMOV;
        public string ExtraSkills;
        public int BattleMode;
        public int SpecRing;
    }

    [System.Serializable]
    public class SchemaItems
    {
        public Dictionary<string, TKRTypeItem> items;
        public Dictionary<string, TKRItemMedic> medics;
        public Dictionary<string, TKRItemProp> props;
        public Dictionary<string, TKRItemWeapon> weapons;
        public Dictionary<string, TKRItemArmor> armors;
        public Dictionary<string, TKRItemAccessory> accessories;
        public Dictionary<string, TKRItem> specials;
    }
    #endregion

    #region SchemaSound

    [System.Serializable]
    public class TKRMusic
    {
        public int Id;
        public string ABPath;
        public string AssetName;
        public string Desc;
    }

    [System.Serializable]
    public class TKRBgm
    {
        public int Id;
        public string EffectType;
        public string AssetBundleName;
        public string Desc;
    }

    [System.Serializable]
    public class SchemaSound
    {
        public Dictionary<string, TKRMusic> background;
        public Dictionary<string, TKRBgm> effect;
    }
    #endregion

    #region SchemaSkills
    public enum EnumGameSkillMainCategory
    {
        Invalid = -1,
        Command,
        Active,
        Passive,
        Movement
    }

    public enum EnumGameSkillSubCategory
    {
        Invalid = -1,
        Spell,
        WuShu,
        Shooting,
        Auxiliary,
        Special,
        Exclusive,
        TypeOne,
        TypeTwo
    }

    public enum EnumSkillFunction
    {
        Invalid = -1,
        XianShu,
        FangShu,
        YiShu,
        HuanLingJi,
        YeShouZhaoHuan
    }

    [System.Serializable]
    public class FunctionOfSkill
    {
        public string Function;
        public string Name;
        public string Description;
    }

    [System.Serializable]
    public class SkillSpell
    {
        public int Id;
        public string Name;
        public int DELAY;
        public int UseJP;
        public int UseMP;
        public int FixInitPt;
        public int PreItem;
        public string AdvSkillsPt;
    }

    [System.Serializable]
    public class SkillWuShu
    {
        public int Id;
        public string Name;
        public int DELAY;
        public int UseJP;
        public int UseMP;
        public int PreItem;
        public string AdvSkillsPt;
    }

    [System.Serializable]
    public class SkillShooting
    {
        public int Id;
        public string Name;
        public int DELAY;
        public int UseJP;
        public int UseMP;
        public string AdvSkillsPt;
    }

    [System.Serializable]
    public class SkillAuxiliary
    {
        public int Id;
        public string Name;
        public int DELAY;
        public int UseJP;
        public int UseMP;
        public string AdvSkillsPt;
    }

    [System.Serializable]
    public class SkillSpecial
    {
        public int Id;
        public string Name;
        public int DELAY;
        public int UseJP;
        public int UseMP;
        public int UseSouls;
        public int UseAnimals;
        public int FixInitPt;
        public int PreItem;
        public string AdvSkillsPt;
    }

    [System.Serializable]
    public class SkillExclusive
    {
        public int Id;
        public string Name;
        public int DELAY;
        public int UseJP;
        public int UseMP;
        public int UseGold;
        public int Ratio;
        public int PreItem;
        public string AdvSkillsPt;
    }

    [System.Serializable]
    public class SkillActive1
    {
        public int Id;
        public string Name;
        public int UseJP;
        public int IncHP;
        public int IncMP;
        public int IncHIT;
        public int IncEVD;
        public int IncATK;
        public int IncDEF;
        public int IncSPD;
        public int IncACNT;
        public string AdvSkillsPt;
    }

    [System.Serializable]
    public class SkillActive2
    {
        public int Id;
        public string Name;
        public int UseJP;
        public int NeiXi;
        public int GuiCai;
        public int TianCai;
        public int KangMo;
        public int Ratio;
        public string Immune;
        public string AdvSkillsPt;
    }

    [System.Serializable]
    public class SkillPassive
    {
        public int Id;
        public string Name;
        public int UseJP;
        public int Ratio;
        public string AdvSkillsPt;
    }

    [System.Serializable]
    public class SkillMovement
    {
        public int Id;
        public string Name;
        public int UseJP;
        public int IncMOV;
        public int JudgeMOV;
        public int ReHPRatio;
        public int ReMPRatio;
        public string AdvSkillsPt;
    }

    [System.Serializable]
    public class SkillGeneric
    {
        public int Id;
        public string Name;
        public string MainCategory;
        public string SubCategory;
        public string Function;
        public string Description;
    }

    [System.Serializable]
    public class SchemaSkills
    {
        public Dictionary<string, SkillGeneric> skills;
        public Dictionary<string, FunctionOfSkill> function;
        public Dictionary<string, SkillSpell> spell;
        public Dictionary<string, SkillWuShu> wushu;
        public Dictionary<string, SkillShooting> shooting;
        public Dictionary<string, SkillAuxiliary> auxiliary;
        public Dictionary<string, SkillSpecial> special;
        public Dictionary<string, SkillExclusive> exclusive;
        public Dictionary<string, SkillActive1> active1;
        public Dictionary<string, SkillActive2> active2;
        public Dictionary<string, SkillPassive> passive;
        public Dictionary<string, SkillMovement> movement;
    }
    #endregion

    #region ConfigDlgAnchor

    public class DlgAnchor
    {
        public int charTemplateId;
        public float dlgLocalX;
        public float dlgLocalY;
    }

    [System.Serializable]
    public class ConfigDlgAnchors
    {
        public List<DlgAnchor> allDlgAnchors;
    }

    #endregion
}
