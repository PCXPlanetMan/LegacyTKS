using UnityEngine;
using System.Collections;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    /// <summary>
    /// 地位等同于主控制器BattleController,主要用来监控是否有角色的血量降为0,从而驱动增加KnockOutStatusEffect而播放死亡动画
    /// </summary>
    public class AutoStatusController : MonoBehaviour
    {
        void OnEnable()
        {
            this.AddObserver(OnHPDidChangeNotification, Stats.DidChangeNotification(EnumStatTypes.HP));
        }

        void OnDisable()
        {
            this.RemoveObserver(OnHPDidChangeNotification, Stats.DidChangeNotification(EnumStatTypes.HP));
        }

        void OnHPDidChangeNotification(object sender, object args)
        {
            Stats stats = sender as Stats;
            if (stats[EnumStatTypes.HP] == 0)
            {
                Status status = stats.GetComponentInChildren<Status>();
                // TODO:StatComparisonCondition添加的HP的Observer貌似并不会被执行
                StatComparisonCondition c = status.Add<KnockOutStatusEffect, StatComparisonCondition>();
                c.Init(EnumStatTypes.HP, 0, c.EqualTo);
            }
        }
    }
}