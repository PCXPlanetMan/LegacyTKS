using UnityEngine;
using System.Collections;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class Rank : MonoBehaviour
    {
        public const int minLevel = 1;
        public const int maxLevel = 99;
        public const int maxExperience = 999999;

        public int LVL
        {
            get { return stats[EnumStatTypes.LVL]; }
        }

        public int EXP
        {
            get { return stats[EnumStatTypes.EXP]; }
            set { stats[EnumStatTypes.EXP] = value; }
        }

        public float LevelPercent
        {
            get { return (float)(LVL - minLevel) / (float)(maxLevel - minLevel); }
        }

        Stats stats;

        void Awake()
        {
            stats = GetComponent<Stats>();
        }

        void OnEnable()
        {
            this.AddObserver(OnExpWillChange, Stats.WillChangeNotification(EnumStatTypes.EXP), stats);
            this.AddObserver(OnExpDidChange, Stats.DidChangeNotification(EnumStatTypes.EXP), stats);
        }

        void OnDisable()
        {
            this.RemoveObserver(OnExpWillChange, Stats.WillChangeNotification(EnumStatTypes.EXP), stats);
            this.RemoveObserver(OnExpDidChange, Stats.DidChangeNotification(EnumStatTypes.EXP), stats);
        }


        void OnExpWillChange(object sender, object args)
        {
            ValueChangeException vce = args as ValueChangeException;
            vce.AddModifier(new ClampValueModifier(int.MaxValue, EXP, maxExperience));
        }

        void OnExpDidChange(object sender, object args)
        {
            stats.SetValue(EnumStatTypes.LVL, LevelForExperience(EXP), false);
        }


        public static int ExperienceForLevel(int level)
        {
            float levelPercent = Mathf.Clamp01((float)(level - minLevel) / (float)(maxLevel - minLevel));

            float start = 0, end = maxExperience, value = levelPercent;
            end -= start;
            return (int)EasingEquations.EaseInQuad(0, maxExperience, levelPercent);
        }

        public static int LevelForExperience(int exp)
        {
            int lvl = maxLevel;
            for (; lvl >= minLevel; --lvl)
                if (exp >= ExperienceForLevel(lvl))
                    break;
            return lvl;
        }

        public void Init(int level)
        {
            stats.SetValue(EnumStatTypes.LVL, level, false);
            stats.SetValue(EnumStatTypes.EXP, ExperienceForLevel(level), false);
        }
    }
}