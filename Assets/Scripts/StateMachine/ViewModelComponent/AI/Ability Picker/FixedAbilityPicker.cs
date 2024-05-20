using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class FixedAbilityPicker : BaseAbilityPicker
    {
        public EnumTargets target;
        public string ability;

        public override void Pick(PlanOfAttack plan)
        {
            plan.target = target;
            plan.ability = Find(ability);

            if (plan.ability == null)
            {
                plan.ability = Default();
                plan.target = EnumTargets.Foe;
            }
        }
    }
}