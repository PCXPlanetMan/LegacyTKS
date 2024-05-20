using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    /// <summary>
    /// 指定敌人被击破后则胜利;一般在战斗初始化之时进行设定
    /// </summary>
    public class DefeatTargetVictoryCondition : BaseVictoryCondition
    {
        public Unit target;

        protected override void CheckForGameOver()
        {
            base.CheckForGameOver();
            if (Victor == EnumAlliances.None && IsDefeated(target))
                Victor = EnumAlliances.Hero;
        }
    }
}