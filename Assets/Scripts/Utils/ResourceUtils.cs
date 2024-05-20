using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ResourceUtils
{
    #region 游戏配置常量
    public static string AB_PREFABS = "prefabs";
    public static string AB_FONTS = "fonts";
    public static string AB_PREFIX_ANIM_CHAR = "anim/char/";
    public static string AB_CFG_DATA = "cfg/data";
    public static string AB_UI_ROOT = "ui/root";
    public static string AB_UI_DIALOG = "ui/dialog";
    public static string AB_RESOURCE_ICONS_CHAR = "atlas/icons/char";
    public static string AB_RESOURCE_ICONS_ITEM = "atlas/icons/item";

    public static string ASSET_SCHEMA_STORYLINE = "SchemaStoryLine";
    public static string ASSET_SCHEMA_TEXTS = "SchemaTexts";
    public static string ASSET_SCHEMA_SCENEMAP = "SchemaSceneMap";
    public static string ASSET_SCHEMA_CHARACTER = "SchemaCharacter";
    public static string ASSET_SCHEMA_STATEMACHINE = "SchemaStateMachine";
    public static string ASSET_SCHEMA_TKRITEM = "SchemaItems";
    public static string ASSET_SCHEMA_SKILLS = "SchemaSkills";
    public static string ASSET_SCHEMA_SOUND = "SchemaSound";
    public static string ASSET_CHAR_TEMPLATE = "CharTemplate";
    public static string ASSET_UI_CANVAS = "Canvas";
    public static string ASSET_UI_DIALOG = "Dialog";
    public static string ASSET_ICON_CHAR_PORTRAITS = "CharPortraits";
    public static string ASSET_ICON_ITEM = "ItemIcons";
    #endregion


    #region 游戏表现相关常量
    public static string ANIM_UI_TOAST_AUTO = "ToastAuto";
    #endregion


    #region 游戏内容常量
    public static int MAINROLE_ID = 1001; // 主角实例ID
    public static int NEW_GAME_INTO_SCENE_ID = 1002; // 新游戏进入的场景ID(虚空)
    //public static string DEFAULT_MAINROLE_NAME = "無名";
    public static string DEFAULT_MAINROLE_FIRST_NAME = "名";
    public static string DEFAULT_MAINROLE_LAST_NAME = "無";
    public static int NEW_GAME_SCENARIO_ID = 9001; // 新游戏第一个剧情动画
    public static int DEV_BATTLE_DEMO_SCENE_ID = 3001;
    public static int DEV_EMPTY_DEMO_SCENE_ID = 3002;
    public static int DEV_BATTLE_TEST_PARAM_ID = 5001;

    public static int SORTING_LAYER_OF_SCENARIO_SUB_TIMELINE_ANIM = 10;
    public static int SORTING_LAYER_OF_CHARACTER = 5;

    /*
     * 主角战斗状态动画控制器
     */
    public static string MAINROLE_BATTLE_ANIM_CONTROLLER_NONE = "BattleMainRoleNoneController";
    public static string MAINROLE_BATTLE_ANIM_CONTROLLER_ARROW = "BattleMainRoleArrowController";
    public static string MAINROLE_BATTLE_ANIM_CONTROLLER_BLADE = "BattleMainRoleBroadswordController";
    public static string MAINROLE_BATTLE_ANIM_CONTROLLER_BROADSWORD = "BattleMainRoleBroadswordController";
    public static string MAINROLE_BATTLE_ANIM_CONTROLLER_PIKE = "BattleMainRolePikeController";
    public static string MAINROLE_BATTLE_ANIM_CONTROLLER_SPEAR = "BattleMainRoleSpearController";
    public static string MAINROLE_BATTLE_ANIM_CONTROLLER_SWORD = "BattleMainRoleSwordController";

    #endregion


    #region UI格式化字符串
    public static readonly string FORMAT_DOC_CHAR_LV = "Lv   {0}";
    public static readonly string FORMAT_DATE_YMD = "{0:D4}年{1:D1}月{2:D1}日";
    public static readonly string FORMAT_DATE_HMS = "{0:D2}:{1:D2}:{2:D2}";
    public static readonly string FORMAT_STATS_VALUE = "{0}/{1}";
    public static readonly string FORMAT_UI_PAGE = "{0} / {1}";
    public static readonly string FORMAT_FULL_NAME = "{0}{1}";
    public static readonly string FORMAT_TEAM_MEMBER_LV = "L{0:D2}";
    public static readonly string FORMAT_DOC_LOAD = "讀檔";
    public static readonly string FORMAT_DOC_SAVE = "存檔";
    public static readonly string FORMAT_UI_ALL_PAGES = "第{0}頁 共{1}頁";
    #endregion

    #region 部分配置数据
    public static string PREFS_SYSTEM_MUSIC_ONOFF = "musicSwitch";
    public static string PREFS_SYSTEM_MUSIC_VOLUME = "musicVolume";
    public static string PREFS_SYSTEM_BGM_ONOFF = "bgmSwitch";
    public static string PREFS_SYSTEM_BGM_VOLUME = "bgmVolume";
    #endregion


    public static string DOCUMENT_ARCHIVES_FILE_NAME = "save.json";
    public static string CONFIG_CHAR_DLG_ANCHORS = "char_anchors.json";
}
