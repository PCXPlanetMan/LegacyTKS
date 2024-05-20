using UnityEngine;
using System.Collections;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class Health : MonoBehaviour
    {
        public int HP
        {
            get { return stats[EnumStatTypes.HP]; }
            set { stats[EnumStatTypes.HP] = value; }
        }

        public int MHP
        {
            get { return stats[EnumStatTypes.MHP]; }
            set { stats[EnumStatTypes.MHP] = value; }
        }

        public int MinHP = 0;
        Stats stats;

        void Awake()
        {
            stats = GetComponent<Stats>();
        }

        void OnEnable()
        {
            this.AddObserver(OnHPWillChange, Stats.WillChangeNotification(EnumStatTypes.HP), stats);
            this.AddObserver(OnMHPDidChange, Stats.DidChangeNotification(EnumStatTypes.MHP), stats);
        }

        void OnDisable()
        {
            this.RemoveObserver(OnHPWillChange, Stats.WillChangeNotification(EnumStatTypes.HP), stats);
            this.RemoveObserver(OnMHPDidChange, Stats.DidChangeNotification(EnumStatTypes.MHP), stats);
        }

        void OnHPWillChange(object sender, object args)
        {
            ValueChangeException vce = args as ValueChangeException;
            vce.AddModifier(new ClampValueModifier(int.MaxValue, MinHP, stats[EnumStatTypes.MHP]));
        }

        void OnMHPDidChange(object sender, object args)
        {
            int oldMHP = (int)args;
            if (MHP > oldMHP)
                HP += MHP - oldMHP;
            else
                HP = Mathf.Clamp(HP, MinHP, MHP);
        }
    }
}