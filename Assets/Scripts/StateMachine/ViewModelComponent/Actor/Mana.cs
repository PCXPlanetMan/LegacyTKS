using UnityEngine;
using System.Collections;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class Mana : MonoBehaviour
    {
        public int MP
        {
            get { return stats[EnumStatTypes.MP]; }
            set { stats[EnumStatTypes.MP] = value; }
        }

        public int MMP
        {
            get { return stats[EnumStatTypes.MMP]; }
            set { stats[EnumStatTypes.MMP] = value; }
        }

        Unit unit;
        Stats stats;

        void Awake()
        {
            stats = GetComponent<Stats>();
            unit = GetComponent<Unit>();
        }

        void OnEnable()
        {
            this.AddObserver(OnMPWillChange, Stats.WillChangeNotification(EnumStatTypes.MP), stats);
            this.AddObserver(OnMMPDidChange, Stats.DidChangeNotification(EnumStatTypes.MMP), stats);
            this.AddObserver(OnTurnBegan, TurnOrderController.TurnBeganNotification, unit);
        }

        void OnDisable()
        {
            this.RemoveObserver(OnMPWillChange, Stats.WillChangeNotification(EnumStatTypes.MP), stats);
            this.RemoveObserver(OnMMPDidChange, Stats.DidChangeNotification(EnumStatTypes.MMP), stats);
            this.RemoveObserver(OnTurnBegan, TurnOrderController.TurnBeganNotification, unit);
        }

        void OnMPWillChange(object sender, object args)
        {
            ValueChangeException vce = args as ValueChangeException;
            vce.AddModifier(new ClampValueModifier(int.MaxValue, 0, stats[EnumStatTypes.MHP]));
        }

        void OnMMPDidChange(object sender, object args)
        {
            int oldMMP = (int)args;
            if (MMP > oldMMP)
                MP += MMP - oldMMP;
            else
                MP = Mathf.Clamp(MP, 0, MMP);
        }

        void OnTurnBegan(object sender, object args)
        {
            if (MP < MMP)
                MP += Mathf.Max(Mathf.FloorToInt(MMP * 0.1f), 1);
        }
    }
}