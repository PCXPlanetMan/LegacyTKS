using System.Collections.Generic;
using com.tksr.document;
using com.tksr.schema;
using UnityEngine;
using UnityEngine.UI;

public class UIInfoPanelSkills : MonoBehaviour
{
    public List<UISkillTabClickAnim> SkillButtons;
    public List<UIBtnShowSkillInfo> ListShowSkillInfoButtons;

    private readonly int MAX_SKILL_TAB_COUNT = 4;
    private readonly int MAX_SKILL_INFO_NUMBER = 10;

    private int currentCharId = 0;

    public Button BtnPageUp;
    public Button BtnPageDown;

    public Text SkillDetails;

    private EnumGameSkillMainCategory curShowUnLearnedSkillType = EnumGameSkillMainCategory.Invalid;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        ResetSkillButtons();
        ResetShowSkillInfoButtons();

        LoadSkillsTreeOfCurrentChar(ResourceUtils.MAINROLE_ID);
        // 默认显示第一个技能
        ShowUnLearnedSkillsList(EnumGameSkillMainCategory.Command);
    }

    private void ResetSkillButtons()
    {
        for (int i = 0; i < SkillButtons.Count; i++)
        {
            var anim = SkillButtons[i];
            anim.ShowClickAnimation(false);
        }
    }

    private void ResetShowSkillInfoButtons()
    {
        for (int i = 0; i < ListShowSkillInfoButtons.Count; i++)
        {
            var button = ListShowSkillInfoButtons[i];
            button.gameObject.SetActive(false);
        }

        SkillDetails.text = string.Empty;
    }

    public void OnClickSkillTabButton(int index)
    {
        var unLearnedSkillType = (EnumGameSkillMainCategory)index;
        if (curShowUnLearnedSkillType == unLearnedSkillType)
            return;

        ResetSkillButtons();
        ResetShowSkillInfoButtons();

        UpdateSkillData(curCharStatsInfo, unLearnedSkillType);
    }

    private List<GameSkillInfo> skillsCommand = new List<GameSkillInfo>();
    private List<GameSkillInfo> skillsActive = new List<GameSkillInfo>();
    private List<GameSkillInfo> skillsPassive = new List<GameSkillInfo>();
    private List<GameSkillInfo> skillsMovement = new List<GameSkillInfo>();

    private void LoadSkillsTreeOfCurrentChar(int charId)
    {
        if (currentCharId == charId)
            return;

        skillsCommand.Clear();
        skillsActive.Clear();
        skillsPassive.Clear();
        skillsMovement.Clear();

        currentCharId = charId;
        var charInfo = DocumentDataManager.Instance.FindCharInfoInTeam(currentCharId);
        var skillsTree = charInfo.SkillsTree;
        //Debug.Log("SkillTree = " + skillsTree.Count);
        for (int i = 0; i < skillsTree.Count; i++)
        {
            var skill = skillsTree[i];
            EnumGameSkillMainCategory mainCategory;
            EnumGameSkillSubCategory subCategory;
            EnumSkillFunction function;
            SkillsManager.Instance.ParseSkillTypeInfo(skill.SkillId, out mainCategory, out subCategory, out function);
            if (mainCategory == EnumGameSkillMainCategory.Command)
            {
                if (skill.LvUpPoint > SkillsManager.THRESHOLD_SKILL_LEARN_PT)
                    skillsCommand.Add(skill);
            }
            else if (mainCategory == EnumGameSkillMainCategory.Active)
            {
                if (skill.LvUpPoint > SkillsManager.THRESHOLD_SKILL_LEARN_PT)
                    skillsActive.Add(skill);
            }
            else if (mainCategory == EnumGameSkillMainCategory.Passive)
            {
                if (skill.LvUpPoint > SkillsManager.THRESHOLD_SKILL_LEARN_PT)
                    skillsPassive.Add(skill);
            }
            else if (mainCategory == EnumGameSkillMainCategory.Movement)
            {
                if (skill.LvUpPoint > SkillsManager.THRESHOLD_SKILL_LEARN_PT)
                    skillsMovement.Add(skill);
            }
        }
    }

    private DataCharStatsInfo curCharStatsInfo;
    private int curPageIndex;
    private int maxPagesCount;

    public void UpdateSkillData(DataCharStatsInfo statsInfo, EnumGameSkillMainCategory type = EnumGameSkillMainCategory.Invalid)
    {
        if (statsInfo == null)
            return;

        curCharStatsInfo = statsInfo;
        LoadSkillsTreeOfCurrentChar(statsInfo.CharID);
        curPageIndex = 0;
        maxPagesCount = 0;

        ShowUnLearnedSkillsList(type);
    }

    public EnumGameSkillMainCategory GetCurrentShowUnLearnedSkillType()
    {
        return curShowUnLearnedSkillType;
    }

    public void ResetShowUnLearnedSkillType()
    {
        curShowUnLearnedSkillType = EnumGameSkillMainCategory.Invalid;
    }

    private void ShowUnLearnedSkillsList(EnumGameSkillMainCategory type)
    {
        if (curShowUnLearnedSkillType == type)
            return;

        curShowUnLearnedSkillType = type;

        int index = (int)type;
        if (index < MAX_SKILL_TAB_COUNT && index > -1)
        {
            var button = SkillButtons[index];
            button.ShowClickAnimation(true);
        }


        BtnPageDown.gameObject.SetActive(false);
        BtnPageUp.gameObject.SetActive(false);

        List<GameSkillInfo> curSkillsList = new List<GameSkillInfo>();
        if (type == EnumGameSkillMainCategory.Command)
        {
            for (int i = 0; i < skillsCommand.Count; i++)
            {
                var skill = skillsCommand[i];
                if (skill.Learned == false)
                {
                    curSkillsList.Add(skill);
                }
            }
        }
        else if (type == EnumGameSkillMainCategory.Active)
        {
            for (int i = 0; i < skillsActive.Count; i++)
            {
                var skill = skillsActive[i];
                if (skill.Learned == false)
                {
                    curSkillsList.Add(skill);
                }
            }
        }
        else if (type == EnumGameSkillMainCategory.Passive)
        {
            for (int i = 0; i < skillsPassive.Count; i++)
            {
                var skill = skillsPassive[i];
                if (skill.Learned == false)
                {
                    curSkillsList.Add(skill);
                }
            }
        }
        else if (type == EnumGameSkillMainCategory.Movement)
        {
            for (int i = 0; i < skillsMovement.Count; i++)
            {
                var skill = skillsMovement[i];
                if (skill.Learned == false)
                {
                    curSkillsList.Add(skill);
                }
            }
        }
        else
        {
            return;
        }

        maxPagesCount = Mathf.CeilToInt(curSkillsList.Count * 1f / MAX_SKILL_INFO_NUMBER);

        for (int i = 0; i < MAX_SKILL_INFO_NUMBER; i++)
        {
            var button = ListShowSkillInfoButtons[i];
            int skillIndex = i + curPageIndex * MAX_SKILL_INFO_NUMBER;
            if (skillIndex < curSkillsList.Count)
            {
                var skillInfo = curSkillsList[skillIndex];
                var skillGeneric = SkillsManager.Instance.FindSkillGenericById(skillInfo.SkillId);
                string skillName;
                int skillJP;
                if (GetSkillUIInfo(skillInfo.SkillId, out skillName, out skillJP))
                {
                    button.gameObject.SetActive(true);
                    button.TextName.text = skillName;
                    button.TextCount.text = skillJP.ToString();
                    button.SkillID = skillInfo.SkillId;
                    button.BtnIndex = i;
                }
                else
                {
                    button.gameObject.SetActive(false);
                }
            }
            else
            {
                button.gameObject.SetActive(false);
            }
        }

        BtnPageDown.gameObject.SetActive(false);
        BtnPageUp.gameObject.SetActive(false);
        if (maxPagesCount > 1)
        {
            if (curPageIndex == 0)
            {
                BtnPageDown.gameObject.SetActive(true);
            }
            else if (curPageIndex == maxPagesCount - 1)
            {
                BtnPageUp.gameObject.SetActive(true);
            }
            else
            {
                BtnPageUp.gameObject.SetActive(true);
                BtnPageDown.gameObject.SetActive(true);
            }
        }
    }

    private bool GetSkillUIInfo(int skillId, out string strSkillName, out int nSkillJP)
    {
        strSkillName = string.Empty;
        nSkillJP = 0;

        EnumGameSkillMainCategory mainCategory;
        EnumGameSkillSubCategory subCategory;
        EnumSkillFunction function;
        SkillsManager.Instance.ParseSkillTypeInfo(skillId, out mainCategory, out subCategory, out function);
        if (subCategory == EnumGameSkillSubCategory.Spell)
        {
            var spell = SkillsManager.Instance.FindSkillSpellById(skillId);
            if (spell != null)
            {
                strSkillName = spell.Name;
                nSkillJP = spell.UseJP;
                return true;
            }
        }
        else if (subCategory == EnumGameSkillSubCategory.WuShu)
        {
            var wushu = SkillsManager.Instance.FindSkillWuShuById(skillId);
            if (wushu != null)
            {
                strSkillName = wushu.Name;
                nSkillJP = wushu.UseJP;
                return true;
            }
        }
        else if (subCategory == EnumGameSkillSubCategory.Shooting)
        {
            var shooting = SkillsManager.Instance.FindSkillShootingById(skillId);
            if (shooting != null)
            {
                strSkillName = shooting.Name;
                nSkillJP = shooting.UseJP;
                return true;
            }
        }
        else if (subCategory == EnumGameSkillSubCategory.Auxiliary)
        {
            var auxiliary = SkillsManager.Instance.FindSkillAuxiliaryById(skillId);
            if (auxiliary != null)
            {
                strSkillName = auxiliary.Name;
                nSkillJP = auxiliary.UseJP;
                return true;
            }
        }
        else if (subCategory == EnumGameSkillSubCategory.Special)
        {
            var special = SkillsManager.Instance.FindSkillSpecialById(skillId);
            if (special != null)
            {
                strSkillName = special.Name;
                nSkillJP = special.UseJP;
                return true;
            }
        }
        else if (subCategory == EnumGameSkillSubCategory.Exclusive)
        {
            var exclusive = SkillsManager.Instance.FindSkillExclusiveById(skillId);
            if (exclusive != null)
            {
                strSkillName = exclusive.Name;
                nSkillJP = exclusive.UseJP;
                return true;
            }
        }
        else if (subCategory == EnumGameSkillSubCategory.TypeOne)
        {
            var active1 = SkillsManager.Instance.FindSkillActiveTypeOneById(skillId);
            if (active1 != null)
            {
                strSkillName = active1.Name;
                nSkillJP = active1.UseJP;
                return true;
            }
        }
        else if (subCategory == EnumGameSkillSubCategory.TypeTwo)
        {
            var active2 = SkillsManager.Instance.FindSkillActiveTypeTwoById(skillId);
            if (active2 != null)
            {
                strSkillName = active2.Name;
                nSkillJP = active2.UseJP;
                return true;
            }
        }
        else if (mainCategory == EnumGameSkillMainCategory.Passive)
        {
            var passive = SkillsManager.Instance.FindSkillPassiveById(skillId);
            if (passive != null)
            {
                strSkillName = passive.Name;
                nSkillJP = passive.UseJP;
                return true;
            }
        }
        else if (mainCategory == EnumGameSkillMainCategory.Movement)
        {
            var movement = SkillsManager.Instance.FindSkillMovementById(skillId);
            if (movement != null)
            {
                strSkillName = movement.Name;
                nSkillJP = movement.UseJP;
                return true;
            }
        }

        return false;
    }

    public void OnClickPageDownButton()
    {
        if (curPageIndex < maxPagesCount - 1)
        {
            curPageIndex++;
            EnumGameSkillMainCategory oldType = curShowUnLearnedSkillType;
            curShowUnLearnedSkillType = EnumGameSkillMainCategory.Invalid;
            ShowUnLearnedSkillsList(oldType);
        }
    }

    public void OnClickPageUpButton()
    {
        if (curPageIndex > 0)
        {
            curPageIndex--;
            EnumGameSkillMainCategory oldType = curShowUnLearnedSkillType;
            curShowUnLearnedSkillType = EnumGameSkillMainCategory.Invalid;
            ShowUnLearnedSkillsList(oldType);
        }
    }

    public void ShowSkillDetails(int skillID)
    {
        if (skillID == 0)
        {
            SkillDetails.text = string.Empty;
            return;
        }

        var skillGeneric = SkillsManager.Instance.FindSkillGenericById(skillID);
        if (skillGeneric != null)
        {
            SkillDetails.text = skillGeneric.Description;
        }
        else
        {
            SkillDetails.text = string.Empty;
        }
    }

    public Transform ComBtnsPanel;
    public void ShowComButtonWhenLearnSkill(bool bShow)
    {
        ComBtnsPanel.gameObject.SetActive(bShow);
        if (!bShow)
        {
            ReleaseCurrentInLearnSkill();
        }
    }

    private void ReleaseCurrentInLearnSkill()
    {
        for (int i = 0; i < ListShowSkillInfoButtons.Count; i++)
        {
            var itemBtn = ListShowSkillInfoButtons[i];
            if (itemBtn != null)
            {
                itemBtn.UnLearnSkill();
            }
        }
        curInLearnSkillIndex = -1;
    }

    private int curInLearnSkillIndex = -1;
    public void SetCurrentInLearnSkillIndex(int index)
    {
        curInLearnSkillIndex = index;
    }

    public void LearnCurrentSelectedSkill()
    {
        //ItemsContentContainer.DoUseCurrentSelectedItem();
        if (curInLearnSkillIndex >= 0 && curInLearnSkillIndex < MAX_SKILL_INFO_NUMBER)
        {
            //if (currentItems != null && currentItems.Count > 0)
            //{
            //    int index = currentPageIndex * ITEMS_IN_PAGE + curInUseItemIndex;
            //    if (index >= 0 && index < currentItems.Count)
            //    {
            //        var item = currentItems[index];
            //        if (item != null)
            //        {
            //            // TODO: Use a item in package
            //            Debug.Log("Use Item = " + item.ItemId);
            //        }
            //    }
            //}
        }
    }
}
