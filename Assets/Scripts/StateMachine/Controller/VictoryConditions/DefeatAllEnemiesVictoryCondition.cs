using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class DefeatAllEnemiesVictoryCondition : BaseVictoryCondition
    {
        protected override void CheckForGameOver()
        {
            base.CheckForGameOver();
            if (Victor == EnumAlliances.None && PartyDefeated(EnumAlliances.Enemy))
                Victor = EnumAlliances.Hero;
        }
    }
}