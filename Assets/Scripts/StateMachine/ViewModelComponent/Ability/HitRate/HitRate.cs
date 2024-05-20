using UnityEngine;
using System.Collections;

namespace com.tksr.statemachine
{
    public abstract class HitRate : MonoBehaviour
    {
        /// <summary>
        /// Includes a toggleable MatchException argument which defaults to false.
        /// </summary>
        public const string AutomaticHitCheckNotification = "HitRate.AutomaticHitCheckNotification";

        /// <summary>
        /// Includes a toggleable MatchException argument which defaults to false.
        /// </summary>
        public const string AutomaticMissCheckNotification = "HitRate.AutomaticMissCheckNotification";

        /// <summary>
        /// Includes an Info argument with three parameters: Attacker (Unit), Defender (Unit), 
        /// and Defender's calculated Evade / Resistance (int).  Status effects which modify Hit Rate
        /// should modify the arg2 parameter.
        /// </summary>
        public const string StatusCheckNotification = "HitRate.StatusCheckNotification";

        public virtual bool IsAngleBased { get { return true; } }
        protected Unit attacker;

        protected virtual void Start()
        {
            attacker = GetComponentInParent<Unit>();
        }

        /// <summary>
        /// Returns a value in the range of 0 t0 100 as a percent chance of
        /// an ability succeeding to hit
        /// </summary>
        public abstract int Calculate(EncapsuleTile target);

        // 相对于目标的攻击或者技能的命中率在0-100之间,故此值设定为101;若用于调试,即百分百命中,则此数值设为零即可
        private readonly int HIT_SUCC_LIMIT_VALUE = 0;

        public virtual bool RollForHit(EncapsuleTile target)
        {
            int roll = UnityEngine.Random.Range(0, HIT_SUCC_LIMIT_VALUE);
            int chance = Calculate(target);
            return roll <= chance;
        }

        protected virtual bool AutomaticHit(Unit target)
        {
            MatchException exc = new MatchException(attacker, target);
            this.PostNotification(AutomaticHitCheckNotification, exc);
            return exc.toggle;
        }

        protected virtual bool AutomaticMiss(Unit target)
        {
            MatchException exc = new MatchException(attacker, target);
            this.PostNotification(AutomaticMissCheckNotification, exc);
            return exc.toggle;
        }

        protected virtual int AdjustForStatusEffects(Unit target, int rate)
        {
            Info<Unit, Unit, int> args = new Info<Unit, Unit, int>(attacker, target, rate);
            this.PostNotification(StatusCheckNotification, args);
            return args.arg2;
        }

        protected virtual int Final(int evade)
        {
            return 100 - evade;
        }
    }
}