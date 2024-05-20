using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.tksr.property;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class ComputerPlayer
    {
        BattleController bc;
        Unit Actor { get { return bc.BattleTurn.Actor; } }
        Alliance alliance { get { return Actor.GetComponent<Alliance>(); } }
        Unit nearestFoe;

        public ComputerPlayer(BattleController battleController)
        {
            bc = battleController;
        }

        public PlanOfAttack Evaluate()
        {
            PlanOfAttack poa = new PlanOfAttack();
            AttackPattern pattern = Actor.GetComponentInChildren<AttackPattern>();
            if (pattern)
                pattern.Pick(poa);
            else
                DefaultAttackPattern(poa);

            if (IsPositionIndependent(poa))
                PlanPositionIndependent(poa);
            else if (IsDirectionIndependent(poa))
                PlanDirectionIndependent(poa);
            else
                PlanDirectionDependent(poa);

            if (poa.ability == null)
                MoveTowardOpponent(poa);

            return poa;
        }

        void DefaultAttackPattern(PlanOfAttack poa)
        {
            // Just get the first "Attack" ability
            poa.ability = Actor.GetComponentInChildren<Ability>();
            poa.target = EnumTargets.Foe;
        }

        bool IsPositionIndependent(PlanOfAttack poa)
        {
            AbilityRange range = poa.ability.GetComponent<AbilityRange>();
            return range.positionOriented == false;
        }

        bool IsDirectionIndependent(PlanOfAttack poa)
        {
            AbilityRange range = poa.ability.GetComponent<AbilityRange>();
            return !range.directionOriented;
        }

        void PlanPositionIndependent(PlanOfAttack poa)
        {
            List<EncapsuleTile> moveOptions = GetMoveOptions();
            EncapsuleTile tile = moveOptions[Random.Range(0, moveOptions.Count)];
            poa.moveLocation = poa.fireLocation = tile.VecPos;
        }

        void PlanDirectionIndependent(PlanOfAttack poa)
        {
            EncapsuleTile startTile = Actor.Tile;
            Dictionary<EncapsuleTile, AttackOption> map = new Dictionary<EncapsuleTile, AttackOption>();
            AbilityRange ar = poa.ability.GetComponent<AbilityRange>();
            List<EncapsuleTile> moveOptions = GetMoveOptions();

            for (int i = 0; i < moveOptions.Count; ++i)
            {
                EncapsuleTile moveTile = moveOptions[i];
                Actor.Place(moveTile);
                List<EncapsuleTile> fireOptions = ar.GetTilesInRange(bc.BoardOfTilesMap);

                for (int j = 0; j < fireOptions.Count; ++j)
                {
                    EncapsuleTile fireTile = fireOptions[j];
                    AttackOption ao = null;
                    if (map.ContainsKey(fireTile))
                    {
                        ao = map[fireTile];
                    }
                    else
                    {
                        ao = new AttackOption();
                        map[fireTile] = ao;
                        ao.target = fireTile;
                        ao.direction = Actor.Dir;
                        RateFireLocation(poa, ao);
                    }

                    ao.AddMoveTarget(moveTile);
                }
            }

            Actor.Place(startTile);
            List<AttackOption> list = new List<AttackOption>(map.Values);
            PickBestOption(poa, list);
        }

        void PlanDirectionDependent(PlanOfAttack poa)
        {
            EncapsuleTile startTile = Actor.Tile;
            EnumStateDirections startDirection = Actor.Dir;
            List<AttackOption> list = new List<AttackOption>();
            List<EncapsuleTile> moveOptions = GetMoveOptions();

            for (int i = 0; i < moveOptions.Count; ++i)
            {
                EncapsuleTile moveTile = moveOptions[i];
                Actor.Place(moveTile);

                for (int j = 0; j < 4; ++j)
                {
                    Actor.Dir = (EnumStateDirections)j;
                    AttackOption ao = new AttackOption();
                    ao.target = moveTile;
                    ao.direction = Actor.Dir;
                    RateFireLocation(poa, ao);
                    ao.AddMoveTarget(moveTile);
                    list.Add(ao);
                }
            }

            Actor.Place(startTile);
            Actor.Dir = startDirection;
            PickBestOption(poa, list);
        }

        bool IsAbilityTargetMatch(PlanOfAttack poa, EncapsuleTile tile)
        {
            bool isMatch = false;
            if (poa.target == EnumTargets.Tile)
                isMatch = true;
            else if (poa.target != EnumTargets.None)
            {
                Alliance other = tile.content.GetComponentInChildren<Alliance>();
                if (other != null && alliance.IsMatch(other, poa.target))
                    isMatch = true;
            }

            return isMatch;
        }

        List<EncapsuleTile> GetMoveOptions()
        {
            return Actor.GetComponent<Movement>().GetTilesInRange(bc.BoardOfTilesMap);
        }

        void RateFireLocation(PlanOfAttack poa, AttackOption option)
        {
            AbilityArea area = poa.ability.GetComponent<AbilityArea>();
            List<EncapsuleTile> tiles = area.GetTilesInArea(bc.BoardOfTilesMap, option.target.VecPos);
            option.areaTargets = tiles;
            option.isCasterMatch = IsAbilityTargetMatch(poa, Actor.Tile);

            for (int i = 0; i < tiles.Count; ++i)
            {
                EncapsuleTile tile = tiles[i];
                if (Actor.Tile == tiles[i] || !poa.ability.IsTarget(tile))
                    continue;

                bool isMatch = IsAbilityTargetMatch(poa, tile);
                option.AddMark(tile, isMatch);
            }
        }

        void PickBestOption(PlanOfAttack poa, List<AttackOption> list)
        {
            int bestScore = 1;
            List<AttackOption> bestOptions = new List<AttackOption>();
            for (int i = 0; i < list.Count; ++i)
            {
                AttackOption option = list[i];
                int score = option.GetScore(Actor, poa.ability);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestOptions.Clear();
                    bestOptions.Add(option);
                }
                else if (score == bestScore)
                {
                    bestOptions.Add(option);
                }
            }

            if (bestOptions.Count == 0)
            {
                poa.ability = null; // Clear ability as a sign not to perform it
                return;
            }

            List<AttackOption> finalPicks = new List<AttackOption>();
            bestScore = 0;
            for (int i = 0; i < bestOptions.Count; ++i)
            {
                AttackOption option = bestOptions[i];
                int score = option.bestAngleBasedScore;
                if (score > bestScore)
                {
                    bestScore = score;
                    finalPicks.Clear();
                    finalPicks.Add(option);
                }
                else if (score == bestScore)
                {
                    finalPicks.Add(option);
                }
            }

            AttackOption choice = finalPicks[UnityEngine.Random.Range(0, finalPicks.Count)];
            poa.fireLocation = choice.target.VecPos;
            poa.attackDirection = choice.direction;
            poa.moveLocation = choice.bestMoveTile.VecPos;
        }

        void FindNearestFoe()
        {
            nearestFoe = null;
            bc.BoardOfTilesMap.Search(Actor.Tile, delegate (EncapsuleTile arg1, EncapsuleTile arg2)
            {
                if (nearestFoe == null && arg2.content != null)
                {
                    Alliance other = arg2.content.GetComponentInChildren<Alliance>();
                    if (other != null && alliance.IsMatch(other, EnumTargets.Foe))
                    {
                        Unit unit = other.GetComponent<Unit>();
                        Stats stats = unit.GetComponent<Stats>();
                        if (stats[EnumStatTypes.HP] > 0)
                        {
                            nearestFoe = unit;
                            return true;
                        }
                    }
                }
                return nearestFoe == null;
            });
        }

        void MoveTowardOpponent(PlanOfAttack poa)
        {
            List<EncapsuleTile> moveOptions = GetMoveOptions();
            FindNearestFoe();
            if (nearestFoe != null)
            {
                EncapsuleTile toCheck = nearestFoe.Tile;
                while (toCheck != null)
                {
                    if (moveOptions.Contains(toCheck))
                    {
                        poa.moveLocation = toCheck.VecPos;
                        return;
                    }
                    toCheck = toCheck.PrevTile;
                }
            }

            poa.moveLocation = Actor.Tile.VecPos;
        }

        public EnumStateDirections DetermineEndFacingDirection()
        {
            EnumStateDirections dir = (EnumStateDirections)UnityEngine.Random.Range(0, 4);
            FindNearestFoe();
            if (nearestFoe != null)
            {
                EnumStateDirections start = Actor.Dir;
                for (int i = 0; i < 4; ++i)
                {
                    Actor.Dir = (EnumStateDirections)i;
                    if (nearestFoe.GetFacing(Actor) == EnumFacings.Front)
                    {
                        dir = Actor.Dir;
                        break;
                    }
                }
                Actor.Dir = start;
            }
            return dir;
        }
    }
}