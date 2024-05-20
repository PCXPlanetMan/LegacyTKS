using UnityEngine;
using System.Collections;
using com.tksr.property;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public abstract class BaseVictoryCondition : MonoBehaviour
    {

        public EnumAlliances Victor
        {
            get { return victor; }
            protected set { victor = value; }
        }
        EnumAlliances victor = EnumAlliances.None;

        protected BattleController bc;


        protected virtual void Awake()
        {
            bc = GetComponent<BattleController>();
        }

        protected virtual void OnEnable()
        {
            this.AddObserver(OnHPDidChangeNotification, Stats.DidChangeNotification(EnumStatTypes.HP));
        }

        protected virtual void OnDisable()
        {
            this.RemoveObserver(OnHPDidChangeNotification, Stats.DidChangeNotification(EnumStatTypes.HP));
        }

        protected virtual void OnHPDidChangeNotification(object sender, object args)
        {
            CheckForGameOver();
        }

        protected virtual void CheckForGameOver()
        {
            if (PartyDefeated(EnumAlliances.Hero))
                Victor = EnumAlliances.Enemy;
        }

        protected virtual bool PartyDefeated(EnumAlliances type)
        {
            for (int i = 0; i < bc.Units.Count; ++i)
            {
                Alliance a = bc.Units[i].GetComponent<Alliance>();
                if (a == null)
                    continue;

                if (a.type == type && !IsDefeated(bc.Units[i]))
                    return false;
            }
            return true;
        }

        protected virtual bool IsDefeated(Unit unit)
        {
            Health health = unit.GetComponent<Health>();
            if (health)
                return health.MinHP == health.HP;

            Stats stats = unit.GetComponent<Stats>();
            return stats[EnumStatTypes.HP] == 0;
        }
    }
}