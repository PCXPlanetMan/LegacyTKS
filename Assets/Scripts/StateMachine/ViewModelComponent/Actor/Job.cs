using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class Job : MonoBehaviour
    {

        public static readonly EnumStatTypes[] statOrder = new EnumStatTypes[]
        {
        EnumStatTypes.MHP,
        EnumStatTypes.MMP,
        EnumStatTypes.ATK,
        EnumStatTypes.DEF,
        EnumStatTypes.MAT,
        EnumStatTypes.MDF,
        EnumStatTypes.HIT,
        EnumStatTypes.EVD,
        EnumStatTypes.SPD,
        EnumStatTypes.MOV
        };

        public int[] baseStats = new int[statOrder.Length];
        public float[] growStats = new float[statOrder.Length];
        public Dictionary<int, int>[] growStatsVariables = new Dictionary<int, int>[statOrder.Length];
        Stats stats;


        void OnDestroy()
        {
            this.RemoveObserver(OnLvlChangeNotification, Stats.DidChangeNotification(EnumStatTypes.LVL), stats);
        }

        public void Employ()
        {
            stats = gameObject.GetComponentInParent<Stats>();
            this.AddObserver(OnLvlChangeNotification, Stats.DidChangeNotification(EnumStatTypes.LVL), stats);

            Feature[] features = GetComponentsInChildren<Feature>();
            for (int i = 0; i < features.Length; ++i)
                features[i].Activate(gameObject);
        }

        public void UnEmploy()
        {
            Feature[] features = GetComponentsInChildren<Feature>();
            for (int i = 0; i < features.Length; ++i)
                features[i].Deactivate();

            this.RemoveObserver(OnLvlChangeNotification, Stats.DidChangeNotification(EnumStatTypes.LVL), stats);
            stats = null;
        }

        public void LoadDefaultStats()
        {
            for (int i = 0; i < statOrder.Length; ++i)
            {
                EnumStatTypes type = statOrder[i];
                stats.SetValue(type, baseStats[i], false);
            }

            stats.SetValue(EnumStatTypes.HP, stats[EnumStatTypes.MHP], false);
            stats.SetValue(EnumStatTypes.MP, stats[EnumStatTypes.MMP], false);
        }

        protected virtual void OnLvlChangeNotification(object sender, object args)
        {
            int oldValue = (int)args;
            int newValue = stats[EnumStatTypes.LVL];

            //for (int i = oldValue; i < newValue; ++i)
            //    LevelUp();
            // [PPAN] TKS直接升级到某等级
            LevelUpgradeTo(oldValue, newValue);
        }

        void LevelUp()
        {
            for (int i = 0; i < statOrder.Length; ++i)
            {
                EnumStatTypes type = statOrder[i];
                int whole = Mathf.FloorToInt(growStats[i]);
                float fraction = growStats[i] - whole;

                int value = stats[type];
                value += whole;
                if (UnityEngine.Random.value > (1f - fraction))
                    value++;

                stats.SetValue(type, value, false);
            }

            stats.SetValue(EnumStatTypes.HP, stats[EnumStatTypes.MHP], false);
            stats.SetValue(EnumStatTypes.MP, stats[EnumStatTypes.MMP], false);
        }

        private readonly int MAX_CHARACTER_LEVEL = 60;

        /// <summary>
        /// TKS中人物升级增加的比率不是固定值
        /// 等级范围为1-60
        /// 最大生命/最大内力/攻击/防御/命中/闪避随等级上升而增加的值=目前值/基数+1
        /// 速度每升一级+1
        /// 幸运/悟性/移动不会随等级提升而改变
        /// </summary>
        /// <param name="newLevel"></param>
        private void LevelUpgradeTo(int oldLevel, int newLevel)
        {
            if (newLevel <= oldLevel)
            {
                Debug.LogErrorFormat("Level up parameters error. oldlv = {0}, newlv = {1}", oldLevel, newLevel);
                return;
            }

            if (newLevel > MAX_CHARACTER_LEVEL)
            {
                Debug.LogErrorFormat("Level up too big > 60. newLevel = {0}", newLevel);
                return;
            }

            for (int i = 0; i < statOrder.Length; i++)
            {
                EnumStatTypes type = statOrder[i];
                var dictGrow = growStatsVariables[i];
                int value = baseStats[i];

                if (type == EnumStatTypes.SPD)
                {
                    value += (newLevel - oldLevel);
                    stats.SetValue(type, value, false);
                    break;
                }

                if (type != EnumStatTypes.MHP && type != EnumStatTypes.MMP && type != EnumStatTypes.ATK && type != EnumStatTypes.DEF && type != EnumStatTypes.HIT && type != EnumStatTypes.EVD)
                    continue;

                value = LevelUpgradeByGrowRatio(value, dictGrow, oldLevel, newLevel);
                stats.SetValue(type, value, false);
            }
            stats.SetValue(EnumStatTypes.HP, stats[EnumStatTypes.MHP], false);
            stats.SetValue(EnumStatTypes.MP, stats[EnumStatTypes.MMP], false);
        }

        public static int LevelUpgradeByGrowRatio(int baseValue, Dictionary<int, int> dictGrow, int oldLevel, int newLevel)
        {
            int value = baseValue;
            foreach (var grow in dictGrow)
            {
                int lv = grow.Key;
                int baseNumber = grow.Value;

                if (oldLevel < lv && oldLevel < newLevel)
                {
                    int destLevel = 0;
                    if (newLevel <= lv)
                    {
                        destLevel = newLevel;
                    }
                    else
                    {
                        destLevel = lv;
                    }

                    for (int j = oldLevel + 1; j <= destLevel; j++)
                    {
                        value += (int)(value / baseNumber) + 1;
                    }

                    oldLevel = destLevel;
                }
            }

            return value;
        }
    }
}