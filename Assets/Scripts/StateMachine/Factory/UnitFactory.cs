using UnityEngine;
using System.IO;
using System.Collections;
using com.tksr.statemachine.defines;
using com.tksr.property;
using System.Collections.Generic;
using System;
using System.Linq;

namespace com.tksr.statemachine
{
    /// <summary>
    /// 人物属性模板管理
    /// </summary>
    public static class UnitFactory
    {
        public static GameObject Create(GameObject obj, UnitRecipe recipe, int level)
        {
            obj.AddComponent<Unit>();
            AddStats(obj);
            AddLocomotion(obj, recipe.locomotion);
            obj.AddComponent<Status>();
            obj.AddComponent<Equipment>();
            AddJobInChild(obj, recipe.job);
            AddRank(obj, level);
            obj.AddComponent<Health>();
            obj.AddComponent<Mana>();
            AddAttackInChild(obj, recipe.attack);
            AddAbilityCatalogInChild(obj, recipe.abilityCatalog);
            AddAlliance(obj, recipe.alliance);
            AddAttackPatternInChild(obj, recipe.strategy);
            return obj;
        }

        static GameObject InstantiateGameObject(string name)
        {
            GameObject instance = new GameObject(name);
            return instance;
        }

        static void AddStats(GameObject obj)
        {
            Stats s = obj.AddComponent<Stats>();
            s.SetValue(EnumStatTypes.LVL, 1, false);
        }

        static void AddJobInChild(GameObject obj, string name)
        {
            GameObject instance = InstantiateGameObject(name);
            instance.transform.SetParent(obj.transform);
            Job job = instance.AddComponent<Job>();

            // 解析Job数据并且添加StatModifierFeature
            // TODO: To Decouping
            if (CharactersManager.Instance != null)
            {
                var jobData = CharactersManager.Instance.ReadJobData(name);
                List<EnumStatTypes> orders = new List<EnumStatTypes>(Job.statOrder);

                job.baseStats[orders.IndexOf(EnumStatTypes.MHP)] = jobData.MHP;
                job.baseStats[orders.IndexOf(EnumStatTypes.MMP)] = jobData.MMP;
                job.baseStats[orders.IndexOf(EnumStatTypes.ATK)] = jobData.ATK;
                job.baseStats[orders.IndexOf(EnumStatTypes.DEF)] = jobData.DEF;
                job.baseStats[orders.IndexOf(EnumStatTypes.MAT)] = jobData.MAT;
                job.baseStats[orders.IndexOf(EnumStatTypes.MDF)] = jobData.MDF;
                job.baseStats[orders.IndexOf(EnumStatTypes.HIT)] = jobData.HIT;
                job.baseStats[orders.IndexOf(EnumStatTypes.EVD)] = jobData.EVD;
                job.baseStats[orders.IndexOf(EnumStatTypes.SPD)] = jobData.SPD;
                job.baseStats[orders.IndexOf(EnumStatTypes.MOV)] = jobData.MOV;

                job.growStats[orders.IndexOf(EnumStatTypes.MHP)] = jobData.GMHP;
                job.growStats[orders.IndexOf(EnumStatTypes.MMP)] = jobData.GMMP;
                job.growStats[orders.IndexOf(EnumStatTypes.ATK)] = jobData.GATK;
                job.growStats[orders.IndexOf(EnumStatTypes.DEF)] = jobData.GDEF;
                job.growStats[orders.IndexOf(EnumStatTypes.MAT)] = jobData.GMAT;
                job.growStats[orders.IndexOf(EnumStatTypes.MDF)] = jobData.GMDF;
                job.growStats[orders.IndexOf(EnumStatTypes.HIT)] = jobData.GHIT;
                job.growStats[orders.IndexOf(EnumStatTypes.EVD)] = jobData.GEVD;
                job.growStats[orders.IndexOf(EnumStatTypes.SPD)] = jobData.GSPD;

                var jobGrow = CharactersManager.Instance.ReadJobGrow(name);
                job.growStatsVariables[orders.IndexOf(EnumStatTypes.MHP)] = CharactersManager.Instance.ParseGrowStatsVariables(jobGrow.MHPGrow);
                job.growStatsVariables[orders.IndexOf(EnumStatTypes.MMP)] = CharactersManager.Instance.ParseGrowStatsVariables(jobGrow.MMPGrow);
                job.growStatsVariables[orders.IndexOf(EnumStatTypes.ATK)] = CharactersManager.Instance.ParseGrowStatsVariables(jobGrow.ATKGrow);
                job.growStatsVariables[orders.IndexOf(EnumStatTypes.DEF)] = CharactersManager.Instance.ParseGrowStatsVariables(jobGrow.DEFGrow);
                job.growStatsVariables[orders.IndexOf(EnumStatTypes.MAT)] = CharactersManager.Instance.ParseGrowStatsVariables(jobGrow.MATGrow);
                job.growStatsVariables[orders.IndexOf(EnumStatTypes.MDF)] = CharactersManager.Instance.ParseGrowStatsVariables(jobGrow.MDFGrow);
                job.growStatsVariables[orders.IndexOf(EnumStatTypes.HIT)] = CharactersManager.Instance.ParseGrowStatsVariables(jobGrow.HITGrow);
                job.growStatsVariables[orders.IndexOf(EnumStatTypes.EVD)] = CharactersManager.Instance.ParseGrowStatsVariables(jobGrow.EVDGrow);
                job.growStatsVariables[orders.IndexOf(EnumStatTypes.SPD)] = CharactersManager.Instance.ParseGrowStatsVariables(jobGrow.SPDGrow);
                job.growStatsVariables[orders.IndexOf(EnumStatTypes.MOV)] = CharactersManager.Instance.ParseGrowStatsVariables(jobGrow.MOVGrow);


                //StatModifierFeature smfEVD = instance.AddComponent<StatModifierFeature>();
                //smfEVD.type = EnumStatTypes.EVD;
                //smfEVD.amount = jobData.AEVD;

                StatModifierFeature smfRES = instance.AddComponent<StatModifierFeature>();
                smfRES.type = EnumStatTypes.RES;
                smfRES.amount = jobData.RES;

                //StatModifierFeature smfMOV = instance.AddComponent<StatModifierFeature>();
                //smfMOV.type = EnumStatTypes.MOV;
                //smfMOV.amount = jobData.AMOV;

                //StatModifierFeature smfJMP = instance.AddComponent<StatModifierFeature>();
                //smfJMP.type = EnumStatTypes.JMP;
                //smfJMP.amount = jobData.AJMP;
            }

            job.Employ();
            job.LoadDefaultStats();
        }

        static void AddLocomotion(GameObject obj, EnumLocomotions type)
        {
            switch (type)
            {
                case EnumLocomotions.Walk:
                    obj.AddComponent<WalkMovement>();
                    break;
                case EnumLocomotions.Fly:
                    //obj.AddComponent<FlyMovement>();
                    break;
                case EnumLocomotions.Teleport:
                    //obj.AddComponent<TeleportMovement>();
                    break;
            }
        }

        static void AddAlliance(GameObject obj, EnumAlliances type)
        {
            Alliance alliance = obj.AddComponent<Alliance>();
            alliance.type = type;
        }

        static void AddRank(GameObject obj, int level)
        {
            Rank rank = obj.AddComponent<Rank>();
            rank.Init(level);
        }

        static void AddAttackInChild(GameObject obj, string name)
        {
            GameObject instance = InstantiateGameObject(name);
            instance.transform.SetParent(obj.transform);
            // 解析普通攻击数据(普通攻击也算是一种技能)
            LoadAbilityContent(instance, name);
        }

        static void AddAbilityCatalogInChild(GameObject obj, string name)
        {
            GameObject main = InstantiateGameObject("AbilityCatalog");
            main.transform.SetParent(obj.transform);
            main.AddComponent<AbilityCatalog>();
            // 根据人物职业类型加载器技能树
            LoadAbilityContent(main, name);
        }

        /// <summary>
        /// 为敌方角色(或某些NPC)添加战斗AI,我方角色默认手动控制(如果是自动战斗则应该动态修改我方角色为自动AI)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        static void AddAttackPatternInChild(GameObject obj, string name)
        {
            Driver driver = obj.AddComponent<Driver>();
            driver.Normal = EnumDrivers.Human;
            if (string.IsNullOrEmpty(name))
            {
                driver.Normal = EnumDrivers.Human;
            }
            else
            {
                driver.Normal = EnumDrivers.Computer;
                GameObject instance = InstantiateGameObject("AttackPattern");
                instance.transform.SetParent(obj.transform);
                instance.AddComponent<AttackPattern>();
                LoadAttackPatternContent(instance, name);
            }
        }

        private static void LoadAbilityContent(GameObject instance, string catalog)
        {
            // TODO: To Decouping of 'CharactersManager'
            if (CharactersManager.Instance != null)
            {
                var charAbilityCatalogRecipes = CharactersManager.Instance.ReadAbilityCatalog(catalog);
                if (!string.IsNullOrEmpty(charAbilityCatalogRecipes.Categories))
                {
                    var categories = charAbilityCatalogRecipes.Categories.Split(';');
                    foreach (var category in categories)
                    {
                        GameObject goCategory = new GameObject(category);
                        goCategory.transform.SetParent(instance.transform);

                        var charAbilityCategory = CharactersManager.Instance.ReadAbilityCategory(category);
                        if (!string.IsNullOrEmpty(charAbilityCategory.Abilities))
                        {
                            var abilities = charAbilityCategory.Abilities.Split(';');
                            foreach (var ability in abilities)
                            {
                                GameObject goAbility = new GameObject(ability);
                                goAbility.transform.SetParent(goCategory.transform);
                                goAbility.AddComponent<Ability>();

                                var charAbilityData = CharactersManager.Instance.ReadAbilityData(ability);

                                var abilityClassType = ParseAbilityPowerComponent(goAbility, charAbilityData.AbilityPower);
                                ParseAbilityRangeComponent(goAbility, charAbilityData.AbilityRange);
                                ParseAbilityAreaComponent(goAbility, charAbilityData.AbilityArea);
                                if (charAbilityData.AbilityCost > 0)
                                {
                                    if (abilityClassType == AbilityPowerParser.EnumAbilityPowerClass.Physical)
                                    {
                                        AbilityPhysicCost apc = goAbility.AddComponent<AbilityPhysicCost>();
                                        apc.amount = charAbilityData.AbilityCost;
                                    }
                                    else if (abilityClassType == AbilityPowerParser.EnumAbilityPowerClass.Magical)
                                    {
                                        AbilityMagicCost amc = goAbility.AddComponent<AbilityMagicCost>();
                                        amc.amount = charAbilityData.AbilityCost;
                                    }
                                }

                                var subParams = charAbilityData.AbilitySubParam.Split(';');
                                foreach (var param in subParams)
                                {
                                    var charAbilityParam = CharactersManager.Instance.ReadAbilityParam(charAbilityData.AbilityName, param);
                                    GameObject goSubParam = InstantiateGameObject(param);
                                    goSubParam.transform.SetParent(goAbility.transform);

                                    if (charAbilityParam == null || charAbilityParam.HitRate == null)
                                    {
                                        Debug.Log("111");
                                    }
                                    ParseAbilitySubParamHitRate(goSubParam, charAbilityParam.HitRate);
                                    ParseAbilitySubParamEffectTarget(goSubParam, charAbilityParam.AbilityEffectTarget);
                                    ParseAbilitySubParamEffect(goSubParam, charAbilityParam.AbilityEffect);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static AbilityPowerParser.EnumAbilityPowerClass ParseAbilityPowerComponent(GameObject goAbility, string strPowerContent)
        {
            var classType = AbilityPowerParser.ParsePowerClassType(strPowerContent);
            switch (classType)
            {
                case AbilityPowerParser.EnumAbilityPowerClass.None:
                    break;
                case AbilityPowerParser.EnumAbilityPowerClass.Physical:
                    {
                        PhysicalAbilityPower pap = goAbility.AddComponent<PhysicalAbilityPower>();
                        int nLevel = 0;
                        if (AbilityParser.ParseAbilityInt(strPowerContent, "Level", ref nLevel))
                        {
                            pap.level = nLevel;
                        }
                    }
                    break;
                case AbilityPowerParser.EnumAbilityPowerClass.Magical:
                    {
                        MagicalAbilityPower map = goAbility.AddComponent<MagicalAbilityPower>();
                        int nLevel = 0;
                        if (AbilityParser.ParseAbilityInt(strPowerContent, "Level", ref nLevel))
                        {
                            map.level = nLevel;
                        }
                    }
                    break;
                case AbilityPowerParser.EnumAbilityPowerClass.Weapon:
                    {
                        // TODO: Weapon
                    }
                    break;
            }
            return classType;
        }

        private static void ParseAbilityRangeComponent(GameObject goAbility, string strRangeContent)
        {
            var classType = AbilityRangeParser.ParseRangeClassType(strRangeContent);
            AbilityRange abilityRange = null;
            switch (classType)
            {
                case AbilityRangeParser.EnumAbilityRangeClass.Self:
                    {
                        abilityRange = goAbility.AddComponent<SelfAbilityRange>();
                    }
                    break;
                case AbilityRangeParser.EnumAbilityRangeClass.Line:
                    {
                        abilityRange = goAbility.AddComponent<LineAbilityRange>();
                    }
                    break;
                case AbilityRangeParser.EnumAbilityRangeClass.Const:
                    {
                        abilityRange = goAbility.AddComponent<ConstantAbilityRange>();
                    }
                    break;
                case AbilityRangeParser.EnumAbilityRangeClass.Cone:
                    {
                        abilityRange = goAbility.AddComponent<ConeAbilityRange>();
                    }
                    break;
                case AbilityRangeParser.EnumAbilityRangeClass.Infi:
                    {
                        abilityRange = goAbility.AddComponent<InfiniteAbilityRange>();
                    }
                    break;
            }


            int nHorizontal = 0;
            if (AbilityParser.ParseAbilityInt(strRangeContent, "H", ref nHorizontal))
            {
                abilityRange.horizontal = nHorizontal;
            }
            int nVertical = 0;
            if (AbilityParser.ParseAbilityInt(strRangeContent, "V", ref nVertical))
            {
                abilityRange.vertical = nVertical;
            }
        }

        private static void ParseAbilityAreaComponent(GameObject goAbility, string strAreaContent)
        {
            var classType = AbilityAreaParser.ParseAreaClassType(strAreaContent);
            switch (classType)
            {
                case AbilityAreaParser.EnumAbilityAreaClass.Specify:
                    {
                        SpecifyAbilityArea saa = goAbility.AddComponent<SpecifyAbilityArea>();
                        int nHorizontal = 0;
                        if (AbilityParser.ParseAbilityInt(strAreaContent, "H", ref nHorizontal))
                        {
                            saa.horizontal = nHorizontal;
                        }
                        int nVertical = 0;
                        if (AbilityParser.ParseAbilityInt(strAreaContent, "V", ref nVertical))
                        {
                            saa.vertical = nVertical;
                        }
                    }
                    break;
                case AbilityAreaParser.EnumAbilityAreaClass.Full:
                    {
                        goAbility.AddComponent<FullAbilityArea>();
                    }
                    break;
                case AbilityAreaParser.EnumAbilityAreaClass.Unit:
                    {
                        goAbility.AddComponent<UnitAbilityArea>();
                    }
                    break;
            }
        }

        private static void ParseAbilitySubParamHitRate(GameObject goSubParam, string strHitRate)
        {
            var classType = HitRateParser.ParseHitRateClassType(strHitRate);
            switch (classType)
            {
                case HitRateParser.EnumHitRateClass.A:
                    {
                        goSubParam.AddComponent<ATypeHitRate>();
                    }
                    break;
                case HitRateParser.EnumHitRateClass.S:
                    {
                        goSubParam.AddComponent<STypeHitRate>();
                    }
                    break;
                case HitRateParser.EnumHitRateClass.Full:
                    {
                        goSubParam.AddComponent<FullTypeHitRate>();
                    }
                    break;
            }
        }

        private static void ParseAbilitySubParamEffectTarget(GameObject goSubParam, string strEffectTarget)
        {
            var classType = AbilityEffectTargetParser.ParseAbilityEffectTargetClassType(strEffectTarget);
            switch (classType)
            {
                case AbilityEffectTargetParser.EnumAbilityEffectTargetClass.Default:
                    {
                        goSubParam.AddComponent<DefaultAbilityEffectTarget>();
                    }
                    break;
                case AbilityEffectTargetParser.EnumAbilityEffectTargetClass.AbsorbDamage:
                    {
                        goSubParam.AddComponent<AbsorbDamageAbilityEffectTarget>();
                    }
                    break;
                case AbilityEffectTargetParser.EnumAbilityEffectTargetClass.Enemy:
                    {
                        goSubParam.AddComponent<EnemyAbilityEffectTarget>();
                    }
                    break;
                case AbilityEffectTargetParser.EnumAbilityEffectTargetClass.KOd:
                    {
                        goSubParam.AddComponent<KOdAbilityEffectTarget>();
                    }
                    break;
                case AbilityEffectTargetParser.EnumAbilityEffectTargetClass.Undead:
                    {
                        UndeadAbilityEffectTarget uaet = goSubParam.AddComponent<UndeadAbilityEffectTarget>();
                        int nToggle = 0;
                        if (AbilityParser.ParseAbilityInt(strEffectTarget, "Toggle", ref nToggle))
                        {
                            uaet.toggle = nToggle == 0 ? false : true;
                        }
                    }
                    break;
            }
        }

        private static void ParseAbilitySubParamEffect(GameObject goSubParam, string strEffect)
        {
            var classType = AbilityEffectParser.ParseAbilityEffectClassType(strEffect);
            switch (classType)
            {
                case AbilityEffectParser.EnumAbilityEffectClass.Damage:
                    {
                        goSubParam.AddComponent<DamageAbilityEffect>();
                    }
                    break;
                case AbilityEffectParser.EnumAbilityEffectClass.Esuna:
                    {
                        goSubParam.AddComponent<EsunaAbilityEffect>();
                    }
                    break;
                case AbilityEffectParser.EnumAbilityEffectClass.Heal:
                    {
                        goSubParam.AddComponent<HealAbilityEffect>();
                    }
                    break;
                case AbilityEffectParser.EnumAbilityEffectClass.Inflict:
                    {
                        InflictAbilityEffect iae = goSubParam.AddComponent<InflictAbilityEffect>();
                        int nDuration = 0;
                        if (AbilityParser.ParseAbilityInt(strEffect, "Duration", ref nDuration))
                        {
                            iae.duration = nDuration;
                        }
                        string strStatusName = string.Empty;
                        if (AbilityParser.ParseAbilityString(strEffect, "StatusName", ref strStatusName))
                        {
                            iae.statusName = strStatusName;
                        }
                    }
                    break;
                case AbilityEffectParser.EnumAbilityEffectClass.Revive:
                    {
                        ReviveAbilityEffect rae = goSubParam.AddComponent<ReviveAbilityEffect>();
                        float fPercent = 0f;
                        if (AbilityParser.ParseAbilityFloat(strEffect, "Percent", ref fPercent))
                        {
                            rae.percent = fPercent;
                        }
                    }
                    break;
            }
        }

        private static void LoadAttackPatternContent(GameObject goAP, string strPatternName)
        {
            AttackPattern ap = goAP.gameObject.GetComponent<AttackPattern>();
            // TODO: To Decouping of 'CharactersManager'
            if (CharactersManager.Instance != null && ap != null)
            {
                if (ap.pickers == null)
                {
                    ap.pickers = new List<BaseAbilityPicker>();
                }

                var charAttackPattern = CharactersManager.Instance.ReadAttackPattern(strPatternName);
                if (!string.IsNullOrEmpty(charAttackPattern.PatternPickers))
                {
                    var pickers = charAttackPattern.PatternPickers.Split(';');
                    Dictionary<string, BaseAbilityPicker> dictPickers = new Dictionary<string, BaseAbilityPicker>();
                    foreach (var pick in pickers)
                    {
                        var kv = pick.Split(':');
                        if (kv.Length == 2)
                        {
                            string pickKey = kv[0];
                            string pickValue = kv[1];

                            GameObject goPicker = InstantiateGameObject(pickKey);
                            goPicker.transform.SetParent(goAP.transform);
                            EnumTargets targets = (EnumTargets)Enum.Parse(typeof(EnumTargets), pickValue);
                            FixedAbilityPicker fap = goPicker.AddComponent<FixedAbilityPicker>();
                            fap.ability = pickKey;
                            fap.target = targets;

                            dictPickers.Add(pickKey, fap);
                        }
                    }
                    var patterns = charAttackPattern.Patterns.Split(';');
                    if (patterns.Length != dictPickers.Count)
                    {
                        Debug.LogErrorFormat("Parse Attack Pattern error, {0}, {1}", charAttackPattern.PatternPickers, charAttackPattern.Patterns);
                    }
                    foreach (var pattern in patterns)
                    {
                        var multi = pattern.Split('|');
                        if (multi.Length == 1)
                        {
                            if (dictPickers.ContainsKey(multi[0]))
                            {
                                ap.pickers.Add(dictPickers[multi[0]]);
                            }
                        }
                        else if (multi.Length == 2)
                        {
                            if (dictPickers.ContainsKey(multi[0]) && dictPickers.ContainsKey(multi[1]))
                            {
                                string strMulti = string.Format("{0}Or{1}", multi[0], multi[1]);
                                GameObject goPicker = InstantiateGameObject(strMulti);
                                goPicker.transform.SetParent(goAP.transform);
                                RandomAbilityPicker rap = goPicker.AddComponent<RandomAbilityPicker>();
                                if (rap.pickers == null)
                                    rap.pickers = new List<BaseAbilityPicker>();
                                rap.pickers.Add(dictPickers[multi[0]]);
                                rap.pickers.Add(dictPickers[multi[1]]);

                                ap.pickers.Add(rap);
                            }
                        }
                    }
                }
            }
        }
    }
}