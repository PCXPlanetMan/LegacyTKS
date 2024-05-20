using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class TurnOrderController : MonoBehaviour
    {
        private const int turnActivation = 1000;
        private const int turnCost = 500;
        private const int moveCost = 300;
        private const int actionCost = 200;

        public const string RoundBeganNotification = "TurnOrderController.roundBegan";
        public const string TurnCheckNotification = "TurnOrderController.turnCheck";
        public const string TurnBeganNotification = "TurnOrderController.TurnBeganNotification";
        public const string TurnCompletedNotification = "TurnOrderController.turnCompleted";
        public const string RoundEndedNotification = "TurnOrderController.roundEnded";

        public IEnumerator Round()
        {
            BattleController bc = GetComponent<BattleController>();
            while (true)
            {
                this.PostNotification(RoundBeganNotification);

                List<Unit> units = new List<Unit>(bc.Units);
                for (int i = 0; i < units.Count; ++i)
                {
                    Stats s = units[i].GetComponent<Stats>();
                    if (s != null)
                        s[EnumStatTypes.CTR] += s[EnumStatTypes.SPD];
                }

                units.Sort((a, b) => GetCounter(a).CompareTo(GetCounter(b)));

                for (int i = units.Count - 1; i >= 0; --i)
                {
                    if (CanTakeTurn(units[i]))
                    {
                        // 若有角色计数器到达阈值则切换成当前角色的回合
                        bc.BattleTurn.Change(units[i]);

                        units[i].PostNotification(TurnBeganNotification);

                        yield return units[i];

                        int cost = turnCost;
                        if (bc.BattleTurn.HasUnitMoved)
                            cost += moveCost;
                        if (bc.BattleTurn.HasUnitActed)
                            cost += actionCost;

                        Stats s = units[i].GetComponent<Stats>();
                        s.SetValue(EnumStatTypes.CTR, s[EnumStatTypes.CTR] - cost, false);

                        units[i].PostNotification(TurnCompletedNotification);
                    }
                }

                this.PostNotification(RoundEndedNotification);
            }
        }

        bool CanTakeTurn(Unit target)
        {
            BaseException exc = new BaseException(GetCounter(target) >= turnActivation);
            target.PostNotification(TurnCheckNotification, exc);
            return exc.toggle;
        }

        int GetCounter(Unit target)
        {
            Stats stats = target.GetComponent<Stats>();
            if (stats != null)
                return stats[EnumStatTypes.CTR];
            else
                return 0;
        }
    }
}