using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using com.tksr.document;
using com.tksr.schema;
using Newtonsoft.Json;
using UnityEngine;

public class SkillsManager : Singleton<SkillsManager>
{
    public static readonly int THRESHOLD_SKILL_LEARN_PT = 9;

    private SchemaSkills schemaSkills;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadSkillsSchema(string jsonSkills)
    {
        schemaSkills = JsonConvert.DeserializeObject<SchemaSkills>(jsonSkills);
    }

    public SkillGeneric FindSkillGenericById(int skillID)
    {
        if (schemaSkills == null || schemaSkills.skills == null)
            return null;

        if (schemaSkills.skills.ContainsKey(skillID.ToString()))
        {
            return schemaSkills.skills[skillID.ToString()];
        }

        return null;
    }

    public SkillSpell FindSkillSpellById(int skillID)
    {
        if (schemaSkills == null || schemaSkills.spell == null)
            return null;

        if (schemaSkills.spell.ContainsKey(skillID.ToString()))
        {
            return schemaSkills.spell[skillID.ToString()];
        }

        return null;
    }

    public SkillWuShu FindSkillWuShuById(int skillID)
    {
        if (schemaSkills == null || schemaSkills.wushu == null)
            return null;

        if (schemaSkills.wushu.ContainsKey(skillID.ToString()))
        {
            return schemaSkills.wushu[skillID.ToString()];
        }

        return null;
    }

    public SkillShooting FindSkillShootingById(int skillID)
    {
        if (schemaSkills == null || schemaSkills.shooting == null)
            return null;

        if (schemaSkills.shooting.ContainsKey(skillID.ToString()))
        {
            return schemaSkills.shooting[skillID.ToString()];
        }

        return null;
    }

    public SkillAuxiliary FindSkillAuxiliaryById(int skillID)
    {
        if (schemaSkills == null || schemaSkills.auxiliary == null)
            return null;

        if (schemaSkills.auxiliary.ContainsKey(skillID.ToString()))
        {
            return schemaSkills.auxiliary[skillID.ToString()];
        }

        return null;
    }

    public SkillSpecial FindSkillSpecialById(int skillID)
    {
        if (schemaSkills == null || schemaSkills.special == null)
            return null;

        if (schemaSkills.special.ContainsKey(skillID.ToString()))
        {
            return schemaSkills.special[skillID.ToString()];
        }

        return null;
    }

    public SkillExclusive FindSkillExclusiveById(int skillID)
    {
        if (schemaSkills == null || schemaSkills.exclusive == null)
            return null;

        if (schemaSkills.exclusive.ContainsKey(skillID.ToString()))
        {
            return schemaSkills.exclusive[skillID.ToString()];
        }

        return null;
    }

    public SkillActive1 FindSkillActiveTypeOneById(int skillID)
    {
        if (schemaSkills == null || schemaSkills.active1 == null)
            return null;

        if (schemaSkills.active1.ContainsKey(skillID.ToString()))
        {
            return schemaSkills.active1[skillID.ToString()];
        }

        return null;
    }

    public SkillActive2 FindSkillActiveTypeTwoById(int skillID)
    {
        if (schemaSkills == null || schemaSkills.active2 == null)
            return null;

        if (schemaSkills.active2.ContainsKey(skillID.ToString()))
        {
            return schemaSkills.active2[skillID.ToString()];
        }

        return null;
    }

    public SkillPassive FindSkillPassiveById(int skillID)
    {
        if (schemaSkills == null || schemaSkills.passive == null)
            return null;

        if (schemaSkills.passive.ContainsKey(skillID.ToString()))
        {
            return schemaSkills.passive[skillID.ToString()];
        }

        return null;
    }

    public SkillMovement FindSkillMovementById(int skillID)
    {
        if (schemaSkills == null || schemaSkills.movement == null)
            return null;

        if (schemaSkills.movement.ContainsKey(skillID.ToString()))
        {
            return schemaSkills.movement[skillID.ToString()];
        }

        return null;
    }

    private List<SkillGeneric> FindSkillsByFunction(EnumSkillFunction function)
    {
        if (schemaSkills == null || schemaSkills.skills == null || schemaSkills.skills.Count == 0)
            return null;

        var foundSkills = schemaSkills.skills.Values.ToList()
            .FindAll(x => x.Function.CompareTo(function.ToString()) == 0);

        List<SkillGeneric> resultSkills = new List<SkillGeneric>();
        resultSkills.AddRange(foundSkills);
        return resultSkills;
    }

    public List<GameSkillInfo> ParseSkillInfoByTypeFromSchema(string strSkillType)
    {
        if (string.IsNullOrEmpty(strSkillType))
            return null;

        List<GameSkillInfo> skillsInfo = new List<GameSkillInfo>();
        var skillFixTypes = strSkillType.Split(';');
        for (int i = 0; i < skillFixTypes.Length; i++)
        {
            EnumSkillFunction skillFunction;
            if (!Enum.TryParse(skillFixTypes[i], out skillFunction))
            {
                continue;
            }

            var foundSkills = FindSkillsByFunction(skillFunction);
            for (int j = 0; j < foundSkills.Count; j++)
            {
                var skill = foundSkills[j];

                EnumGameSkillMainCategory mainCategory;
                if (Enum.TryParse(skill.MainCategory, out mainCategory))
                {
                    EnumGameSkillSubCategory subCategory;
                    if (Enum.TryParse(skill.SubCategory, out subCategory))
                    {
                        if (subCategory == EnumGameSkillSubCategory.Spell)
                        {
                            var spell = FindSkillSpellById(skill.Id);
                            if (spell != null)
                            {
                                if (spell.PreItem != 0)
                                {
                                    skillsInfo.Add(new GameSkillInfo()
                                        { SkillId = spell.Id, LvUpPoint = 0, Learned = false });
                                }
                                else
                                {
                                    skillsInfo.Add(new GameSkillInfo()
                                        { SkillId = spell.Id, LvUpPoint = spell.FixInitPt, Learned = false });
                                }
                            }
                        }
                        else if (subCategory == EnumGameSkillSubCategory.Special)
                        {
                            var special = FindSkillSpecialById(skill.Id);
                            if (special != null)
                            {
                                if (special.PreItem != 0)
                                {
                                    skillsInfo.Add(new GameSkillInfo()
                                        { SkillId = special.Id, LvUpPoint = 0, Learned = false });
                                }
                                else
                                {
                                    skillsInfo.Add(new GameSkillInfo()
                                        { SkillId = special.Id, LvUpPoint = special.FixInitPt, Learned = false });
                                }
                            }
                        }
                    }
                }
            }
        }

        return skillsInfo;
    }

    public List<GameSkillInfo> ParseSkillInfoByParamFromSchema(string strSkillParam)
    {
        if (string.IsNullOrEmpty(strSkillParam))
            return null;

        List<GameSkillInfo> skillTree = new List<GameSkillInfo>();
        var skillParams = strSkillParam.Split(';');
        for (int i = 0; i < skillParams.Length; i++)
        {
            var param = skillParams[i];
            if (string.IsNullOrEmpty(param))
                continue;

            var data = param.Split(':');
            if (data.Length != 2)
                continue;

            int skillID = int.Parse(data[0]);
            int skillPt = int.Parse(data[1]);
            EnumGameSkillMainCategory mainCategory;
            EnumGameSkillSubCategory subCategory;
            EnumSkillFunction function;
            ParseSkillTypeInfo(skillID, out mainCategory, out subCategory, out function);
            GameSkillInfo skillInfo = null;
            if (subCategory == EnumGameSkillSubCategory.WuShu)
            {
                var wushu = FindSkillWuShuById(skillID);
                if (wushu != null)
                {
                    if (wushu.PreItem != 0)
                    {
                        skillInfo = new GameSkillInfo() { SkillId = skillID, LvUpPoint = 0, Learned = false };
                    }
                    else
                    {
                        skillInfo = new GameSkillInfo() { SkillId = skillID, LvUpPoint = skillPt, Learned = false };
                    }
                }
            }
            else if (subCategory == EnumGameSkillSubCategory.Exclusive)
            {
                var exclusive = FindSkillExclusiveById(skillID);
                if (exclusive != null)
                {
                    if (exclusive.PreItem != 0)
                    {
                        skillInfo = new GameSkillInfo() { SkillId = skillID, LvUpPoint = 0, Learned = false };
                    }
                    else
                    {
                        skillInfo = new GameSkillInfo() { SkillId = skillID, LvUpPoint = skillPt, Learned = false };
                    }
                }
            }
            else
            {
                skillInfo = new GameSkillInfo() { SkillId = skillID, LvUpPoint = skillPt, Learned = false };
            }

            skillTree.Add(skillInfo);
        }

        return skillTree;
    }

    public void ParseSkillTypeInfo(int skillId, out EnumGameSkillMainCategory mainCategory,
        out EnumGameSkillSubCategory subCategory, out EnumSkillFunction function)
    {
        mainCategory = EnumGameSkillMainCategory.Invalid;
        subCategory = EnumGameSkillSubCategory.Invalid;
        function = EnumSkillFunction.Invalid;
        if (schemaSkills == null || schemaSkills.skills == null)
        {
            return;
        }

        if (schemaSkills.skills.ContainsKey(skillId.ToString()))
        {
            var skill = schemaSkills.skills[skillId.ToString()];
            if (!string.IsNullOrEmpty(skill.MainCategory))
            {
                Enum.TryParse(skill.MainCategory, out mainCategory);
            }

            if (!string.IsNullOrEmpty(skill.SubCategory))
            {
                Enum.TryParse(skill.SubCategory, out subCategory);
            }

            if (!string.IsNullOrEmpty(skill.Function))
            {
                Enum.TryParse(skill.Function, out function);
            }
        }
    }
}
