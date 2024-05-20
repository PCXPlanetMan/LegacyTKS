using System;
using com.tksr.schema;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using com.tksr.document;
using com.tksr.statemachine;
using com.tksr.statemachine.defines;
using UnityEngine;
using UnityEngine.U2D;
using com.tksr.data;

/// <summary>
/// 主要管理战斗场景中的角色(剧情动画的角色一般在ScenarioManager中管理,直接在UnityScene中通过Timeline加载)
/// </summary>
public class CharactersManager : Singleton<CharactersManager>
{
    
    private SchemaStateMachine schemaStateMachine;
    private GameObject charTemplatePrefab;
    

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    #region Schema Config Data
    private SchemaCharacter schemaCharacter;

    public void LoadCharacterSchema(string jsonCharacter)
    {
        schemaCharacter = JsonConvert.DeserializeObject<SchemaCharacter>(jsonCharacter);
    }

    public void LoadStateMachineSchema(string jsonStateMachine)
    {
        schemaStateMachine = JsonConvert.DeserializeObject<SchemaStateMachine>(jsonStateMachine);
    }

    public CharResItem GetCharacterResById(int Id)
    {
        if (schemaCharacter != null)
        {
            if (schemaCharacter.characterResource.ContainsKey(Id.ToString()))
            {
                return schemaCharacter.characterResource[Id.ToString()];
            }
        }
        return null;
    }

    public CharInstanceItem GetCharacterInstanceById(int Id)
    {
        if (schemaCharacter != null)
        {
            if (schemaCharacter.characterInstances.ContainsKey(Id.ToString()))
            {
                return schemaCharacter.characterInstances[Id.ToString()];
            }
        }

        return null;
    }

    public CharDataItem GetCharacterDataItemById(int Id)
    {
        if (schemaCharacter != null)
        {
            if (schemaCharacter.characterData.ContainsKey(Id.ToString()))
            {
                return schemaCharacter.characterData[Id.ToString()];
            }
        }

        return null;
    }
    #endregion

    #region Characters Resources
    public void LoadCharTemplatePrefab(GameObject prefab)
    {
        charTemplatePrefab = prefab;
    }


    private SpriteAtlas portraitAtlasOfAllChars;

    public void LoadAllCharsPortraitAtlas(SpriteAtlas atlas)
    {
        portraitAtlasOfAllChars = atlas;
    }

    public Sprite ReadFullPortraitById(int Id)
    {
        var instanceItem = GetCharacterInstanceById(Id);
        if (instanceItem != null)
        {
            int resId = instanceItem.AttachResID;
            var resItem = GetCharacterResById(resId);
            if (resItem != null && !string.IsNullOrEmpty(resItem.FullPortraitPath))
            {
                if (portraitAtlasOfAllChars != null)
                {
                    return portraitAtlasOfAllChars.GetSprite(resItem.FullPortraitPath);
                }
            }
        }

        return null;
    }
    #endregion


    #region Scenario Characters
    private CharMainController mainCharController;
    public CharMainController MainRoleController
    {
        get { return mainCharController; }
        private set { mainCharController = value; }
    }

    public void DisableAllCharactersInit()
    {
        var goNPCs = GameObject.FindGameObjectWithTag(EnumGameTagType.GOContainNPCs.ToString());
        var goTeam = GameObject.FindGameObjectWithTag(EnumGameTagType.GOContainTeam.ToString());
        if (goNPCs != null)
        {
            for (int i = 0; i < goNPCs.transform.childCount; i++)
            {
                var child = goNPCs.transform.GetChild(i);
                child.gameObject.SetActive(false);
            }
        }

        if (goTeam != null)
        {
            for (int i = 0; i < goTeam.transform.childCount; i++)
            {
                var child = goTeam.transform.GetChild(i);
                child.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 剧情动画开始之前需要正确的加载此剧情所需的人物资源
    /// 如果没有剧情(例如加载存档或者本身没有剧情动画),则需要手动调用此函数加载人物资源
    /// </summary>
    /// <param name="listNPC"></param>
    /// <param name="listTeam"></param>
    public void ScenarioLoadCharsGameObjects(List<GameObject> listNPC, List<GameObject> listTeam)
    {
        //charactersInScene.Clear();

        List<IGameCharacterRenderer> npcs = new List<IGameCharacterRenderer>();
        for (int i = 0; i < listNPC.Count; i++)
        {
            var npc = listNPC[i].gameObject.GetComponent<IGameCharacterRenderer>();
            npc.gameObject.SetActive(true);
            npcs.Add(npc);
        }
        AddNPCs(npcs.ToArray());

        if (listTeam.Count > 0)
        {
            var mainRole = listTeam[0];
            mainRole.gameObject.SetActive(true);
            var mcc = mainRole.gameObject.GetComponent<CharMainController>();
            AddMainRole(mcc);
        }

        // 屏蔽所有未在本Scenario中出现的角色
        //var goNPCs = GameObject.FindGameObjectWithTag(EnumGameTagType.GOContainNPCs.ToString());
        //var goTeam = GameObject.FindGameObjectWithTag(EnumGameTagType.GOContainTeam.ToString());
        //if (goNPCs != null)
        //{
        //    for (int i = 0; i < goNPCs.transform.childCount; i++)
        //    {
        //        var child = goNPCs.transform.GetChild(i);
        //        if (listNPC.Exists(x => x == child.gameObject))
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            child.gameObject.SetActive(false);
        //        }
        //    }
        //}

        //if (goTeam != null)
        //{
        //    for (int i = 0; i < goTeam.transform.childCount; i++)
        //    {
        //        var child = goTeam.transform.GetChild(i);
        //        if (listTeam.Exists(x => x == child.gameObject))
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            child.gameObject.SetActive(false);
        //        }
        //    }
        //}
    }

    public void ScenarioUnloadNPCGameObjects(List<GameObject> listNPC)
    {
        for (int i = listNPC.Count - 1; i >= 0; i--)
        {
            var npc = listNPC[i];
            charactersInScene.Remove(npc.gameObject);
            ScenarioManager.Instance.ClearCharDialogUI(npc.gameObject.GetComponent<IGameCharacterRenderer>());
            npc.gameObject.SetActive(false);
        }
    }

    public void ScenarioUnloadCharById(int charId)
    {
        if (charId == mainCharController.CharID)
        {
            ScenarioManager.Instance.ClearCharDialogUI(mainCharController.gameObject.GetComponent<IGameCharacterRenderer>());
            mainCharController = null;
        }
        else
        {
            var foundChar = charactersInScene.Find(x => x.gameObject.GetComponent<IGameCharacterRenderer>().CharID == charId);
            if (foundChar != null)
            {
                charactersInScene.Remove(foundChar.gameObject);
                ScenarioManager.Instance.ClearCharDialogUI(foundChar.gameObject.GetComponent<IGameCharacterRenderer>());
                foundChar.gameObject.SetActive(false);
            }
        }
    }

    public void ScenarioLoadCharNPCGameObject(GameObject npcGO)
    {
        List<IGameCharacterRenderer> npcs = new List<IGameCharacterRenderer>();
        var npc = npcGO.gameObject.GetComponent<IGameCharacterRenderer>();
        npc.gameObject.SetActive(true);
        npcs.Add(npc);
        AddNPCs(npcs.ToArray());
    }

    public void ScenarioAddHiddenGameObject(GameObject hiddenObj)
    {
        if (hiddenObj != null)
        {
            var hidden = hiddenObj.gameObject.GetComponent<IGameCharacterRenderer>();
            AddNPCs(new List<IGameCharacterRenderer>() { hidden }.ToArray(), true);
        }
    }

    public void ScenarioLoadCharMainRoleGameObject(GameObject mainGO)
    {
        var mainRole = mainGO;
        mainRole.gameObject.SetActive(true);
        var mcc = mainRole.gameObject.GetComponent<CharMainController>();
        AddMainRole(mcc);
    }

    /// <summary>
    /// 记录场景中的NPC,并在UI中创建对应的Dialog资源.
    /// </summary>
    /// <param name="charRender"></param>
    private void AddNPCs(IGameCharacterRenderer[] charRenders, bool isHidden = false)
    {
        foreach (var charRender in charRenders)
        {
            charactersInScene.Add(charRender.gameObject);
            if (!isHidden)
                AttachMovementControllerToChar(charRender);
            ScenarioManager.Instance.CreateCharDialogUI(charRender);
        }
    }


    /// <summary>
    /// 记录MainRole,并在UI中创建对应的Dialog资源
    /// </summary>
    /// <param name="cmc"></param>
    public void AddMainRole(CharMainController cmc)
    {
        if (cmc != null)
        {
            if (mainCharController != null)
                charactersInScene.Remove(mainCharController.gameObject);

            mainCharController = cmc;
            var mainCharacterRender = cmc.GetComponent<IGameCharacterRenderer>();
            charactersInScene.Add(mainCharacterRender.gameObject);
            AttachMovementControllerToChar(mainCharacterRender);
            ScenarioManager.Instance.CreateCharDialogUI(mainCharacterRender);
        }
    }

    /// <summary>
    /// 为角色添加动作控制器
    /// </summary>
    /// <param name="charRender"></param>
    private void AttachMovementControllerToChar(IGameCharacterRenderer charRender)
    {
        var moveController = charRender.gameObject.GetComponent<CharMovementController>();
        if (moveController == null)
        {
            moveController = charRender.gameObject.AddComponent<CharMovementController>();
            charRender.MovementController = moveController;
        }
    }

    public IGameCharacterRenderer ScenarioFindCharById(int id)
    {
        var foundCharGO = charactersInScene.Find(x => x.GetComponent<IGameCharacterRenderer>().CharID == id);
        if (foundCharGO != null)
        {
            return foundCharGO.GetComponent<IGameCharacterRenderer>();
        }

        return null;
    }

    /// <summary>
    /// 卸载已经加载的Character及其相关依赖的资源
    /// </summary>
    public void ScenarioUnLoadCharsGameObjects()
    {
        charactersInScene.Clear();
        mainCharController = null;

        ScenarioManager.Instance.ClearAllCharsDialogUI();
    }

    /// <summary>
    /// 场景中非剧情动画下响应鼠标移动主角(过程中可能会与场景元素产生交互)
    /// </summary>
    /// <param name="mousePos"></param>
    public void DriveMainRoleByInputPos(Vector3 mousePos)
    {
        if (MainRoleController != null)
        {
            MainRoleController.UpdateActionByInputScreenPos(mousePos);
        }
    }

    /// <summary>
    /// 某些剧情场景的动画可能需要强制修改其层级,才能保证正常的遮挡关系
    /// </summary>
    /// <param name="charId"></param>
    /// <param name="sortingLayer"></param>
    public void UpdateCharSpriteSortingLayer(int charId, int sortingLayer)
    {
        SpriteRenderer sr = null;
        if (charId == ResourceUtils.MAINROLE_ID)
        {
            sr = MainRoleController.gameObject.GetComponent<SpriteRenderer>();
        }
        else
        {
            var foundChar = ScenarioFindCharById(charId);
            if (foundChar != null)
            {
                sr = foundChar.gameObject.GetComponent<SpriteRenderer>();
            }
        }

        if (sr != null)
        {
            sr.sortingOrder = sortingLayer;
        }
    }
    #endregion


    public void ClearExistCharacters()
    {
        foreach (var character in charactersInScene)
        {
            GameObject.Destroy(character);
        }

        // TODO:释放资源
        charactersInScene.Clear();
    }

    private List<GameObject> charactersInScene = new List<GameObject>();
    public GameObject FindCharacterById(int id)
    {
        foreach (var character in charactersInScene)
        {
            var cac = character.gameObject.GetComponent<CharMainController>();
            if (cac.CharID == id)
            {
                return character;
            }
        }

        GameObject goChar = null;

        CharInstanceItem schemaInstanceItem = GetCharacterInstanceById(id);
        if (schemaInstanceItem != null)
        {
            int resId = schemaInstanceItem.AttachResID;

            CharResItem schemaResourceItem = GetCharacterResById(resId);

            if (schemaResourceItem != null)
            {
                if (charTemplatePrefab != null)
                {
                    goChar = GameObject.Instantiate(charTemplatePrefab);
                    goChar.name = schemaInstanceItem.InstanceName;
                    goChar.transform.parent = this.transform;

                    CharMainController cac = goChar.GetComponent<CharMainController>();
                    if (cac != null)
                    {
                        CharDataItem schemaDataItem = GetCharacterDataItemById(resId); // 并不是每个角色都有Data(例如NPC)
                        cac.LoadResourceInBattleFromSchema(id, schemaResourceItem, schemaDataItem);
                    }

                    charactersInScene.Add(goChar);
                }
            }
        }

        

        if (goChar != null)
        {
            GameObject goCharacters = GameObject.Find("Characters");
            if (goCharacters == null)
            {
                goCharacters = new GameObject("Characters");
            }
            if (goCharacters != null)
            {
                goChar.transform.parent = goCharacters.transform;
            }
        }
        return goChar;
    }

    public CharJob ReadJobData(string strJob)
    {
        if (schemaStateMachine.jobData.ContainsKey(strJob))
        {
            return schemaStateMachine.jobData[strJob];
        }
        return null;
    }

    public CharJobGrow ReadJobGrow(string strJob)
    {
        if (schemaStateMachine.jobGrow.ContainsKey(strJob))
        {
            return schemaStateMachine.jobGrow[strJob];
        }
        return null;
    }

    public CharJobInfo ReadJobInfo(string strJob)
    {
        if (schemaStateMachine.jobInfo.ContainsKey(strJob))
        {
            return schemaStateMachine.jobInfo[strJob];
        }
        return null;
    }

    public CharAbilityCatalogRecipes ReadAbilityCatalog(string strCatalog)
    {
        if (schemaStateMachine.abilityCatalogRecipes.ContainsKey(strCatalog))
        {
            return schemaStateMachine.abilityCatalogRecipes[strCatalog];
        }
        return null;
    }

    public CharAbilityCategory ReadAbilityCategory(string strCategory)
    {
        if (schemaStateMachine.abilityCategory.ContainsKey(strCategory))
        {
            return schemaStateMachine.abilityCategory[strCategory];
        }
        return null;
    }

    public CharAbilityData ReadAbilityData(string strAbility)
    {
        if (schemaStateMachine.abilityData.ContainsKey(strAbility))
        {
            return schemaStateMachine.abilityData[strAbility];
        }
        return null;
    }

    public CharAbilityParam ReadAbilityParam(string strAbility, string strParam)
    {
        // TODO:Hardcore技能_参数
        string strAbilityParam = string.Format("{0}_{1}", strAbility, strParam);
        if (schemaStateMachine.abilityParam.ContainsKey(strAbilityParam))
        {
            return schemaStateMachine.abilityParam[strAbilityParam];
        }
        return null;
    }

    public CharAttackPattern ReadAttackPattern(string strPatterName)
    {
        if (schemaStateMachine.attackPatterns.ContainsKey(strPatterName))
        {
            return schemaStateMachine.attackPatterns[strPatterName];
        }
        return null;
    }

    

    public string ReadCharNameById(int Id)
    {
        var instanceItem = GetCharacterInstanceById(Id);
        if (instanceItem != null)
        {
            return instanceItem.InstanceName;
        }
        return null;
    }

    public int LoadMaxHP(int id, int level)
    {
        CharDataItem cdi = GetCharacterDataItemById(id);
        if (cdi != null)
        {
            var job = ReadJobData(cdi.Job);
            if (job != null)
            {

            }
        }
        return 0;
    }

    /// <summary>
    /// TKS解析人物等级升级造成的属性变更比率
    /// </summary>
    /// <param name="growStats"></param>
    /// <returns></returns>
    public Dictionary<int, int> ParseGrowStatsVariables(string growStats)
    {
        if (!string.IsNullOrEmpty(growStats))
        {
            Dictionary<int, int> resultDict = new Dictionary<int, int>();
            var array = growStats.Split(';');
            for (int i = 0; i < array.Length; i++)
            {
                string strData = array[i];
                if (string.IsNullOrEmpty(strData))
                {
                    continue;
                }

                string[] number = strData.Split('.');
                if (number.Length != 2)
                {
                    continue;
                }

                int whole = int.Parse(number[0]);
                int fraction = int.Parse(number[1]);

                resultDict.Add(whole, fraction);
            }
            return resultDict.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
        }
        return null;
    }

    /// <summary>
    /// 结合存档和配置计算当前等级/装备下人物的属性状态
    /// </summary>
    /// <param name="charId"></param>
    /// <returns></returns>
    public DataCharStatsInfo ParseDataCharStatsInfo(GameCharInfo docCharInfo)
    {
        DataCharStatsInfo info = new DataCharStatsInfo();
        if (docCharInfo != null)
        {
            int charId = docCharInfo.CharId;
            info.CharID = charId;
            info.Level = docCharInfo.Level;
            info.HP = docCharInfo.HP;
            info.MP = docCharInfo.MP;

            // TODO:还要考虑装备对于属性的贡献
            var characterInstance = GetCharacterInstanceById(charId);
            CharDataItem cdi = GetCharacterDataItemById(characterInstance.AttachResID);
            if (cdi != null)
            {
                var job = ReadJobData(cdi.Job);
                var jobGrow = ReadJobGrow(cdi.Job);
                if (job != null && jobGrow != null)
                {
                    // MHP
                    var dictMHPGrow = ParseGrowStatsVariables(jobGrow.MHPGrow);
                    info.MaxHP = CalcActiveStatValue(job.MHP, dictMHPGrow, 1, info.Level);
                    // MHP
                    var dictMMPGrow = ParseGrowStatsVariables(jobGrow.MMPGrow);
                    info.MaxMP = CalcActiveStatValue(job.MMP, dictMMPGrow, 1, info.Level);
                    // Attack
                    var dictATKGrow = ParseGrowStatsVariables(jobGrow.ATKGrow);
                    info.Attack = CalcActiveStatValue(job.ATK, dictATKGrow, 1, info.Level);
                    // Defense
                    var dictDEFGrow = ParseGrowStatsVariables(jobGrow.DEFGrow);
                    info.Defense = CalcActiveStatValue(job.DEF, dictDEFGrow, 1, info.Level);
                    // HitRatio
                    var dictHITGrow = ParseGrowStatsVariables(jobGrow.HITGrow);
                    info.HitRatio = CalcActiveStatValue(job.HIT, dictHITGrow, 1, info.Level);
                    // Dodge
                    var dictEVDGrow = ParseGrowStatsVariables(jobGrow.EVDGrow);
                    info.Dodge = CalcActiveStatValue(job.EVD, dictEVDGrow, 1, info.Level);
                    // Speed
                    info.Speed = job.SPD;
                    info.Speed += (info.Level - 1);

                    info.Luck = job.LUCK;
                    info.Understanding = job.USD;

                    // Move
                    info.Move = job.MOV;
                }
            }
        }

        return info;
    }

    private int CalcActiveStatValue(int baseValue, Dictionary<int, int> dictGrow, int oldLevel, int newLevel)
    {
        int value = Job.LevelUpgradeByGrowRatio(baseValue, dictGrow, oldLevel, newLevel);
        return value;
    }

    public List<GameSkillInfo> ParseSkillsTreeFromSchema(int id)
    {
        var cdi = GetCharacterDataItemById(id);
        if (cdi == null)
        {
            return null;
        }

        var jobInfo = ReadJobInfo(cdi.Job);
        if (jobInfo == null)
        {
            return null;
        }

        List<GameSkillInfo> skillsTree = new List<GameSkillInfo>();
        string strSpell = jobInfo.BornSkillsSpell;
        string strWuShu = jobInfo.BornSkillsWuShuDetail;
        string strShooting = jobInfo.BornSkillsShootingDetail;
        string strAuxiliary = jobInfo.BornSkillsAuxiliaryDetail;
        string strSpecial = jobInfo.BornSkillsSpecial;
        string strExclusive = jobInfo.BornSkillsExclusiveDetail;
        string strActive1 = jobInfo.BornSkillsActive1Detail;
        string strActive2 = jobInfo.BornSkillsActive2Detail;
        string strPassive = jobInfo.BornSkillsPassiveDetail;
        string strMovement = jobInfo.BornSkillsMovementDetail;
        var skillList = SkillsManager.Instance.ParseSkillInfoByTypeFromSchema(strSpell);
        if (skillList != null)
            skillsTree.AddRange(skillList);
        skillList = SkillsManager.Instance.ParseSkillInfoByParamFromSchema(strWuShu);
        if (skillList != null)
            skillsTree.AddRange(skillList);
        skillList = SkillsManager.Instance.ParseSkillInfoByParamFromSchema(strShooting);
        if (skillList != null)
            skillsTree.AddRange(skillList);
        skillList = SkillsManager.Instance.ParseSkillInfoByParamFromSchema(strAuxiliary);
        if (skillList != null)
            skillsTree.AddRange(skillList);
        skillList = SkillsManager.Instance.ParseSkillInfoByTypeFromSchema(strSpecial);
        if (skillList != null)
            skillsTree.AddRange(skillList);
        skillList = SkillsManager.Instance.ParseSkillInfoByParamFromSchema(strExclusive);
        if (skillList != null)
            skillsTree.AddRange(skillList);
        skillList = SkillsManager.Instance.ParseSkillInfoByParamFromSchema(strActive1);
        if (skillList != null)
            skillsTree.AddRange(skillList);
        skillList = SkillsManager.Instance.ParseSkillInfoByParamFromSchema(strActive2);
        if (skillList != null)
            skillsTree.AddRange(skillList);
        skillList = SkillsManager.Instance.ParseSkillInfoByParamFromSchema(strPassive);
        if (skillList != null)
            skillsTree.AddRange(skillList);
        skillList = SkillsManager.Instance.ParseSkillInfoByParamFromSchema(strMovement);
        if (skillList != null)
            skillsTree.AddRange(skillList);

        return skillsTree;
    }
}
