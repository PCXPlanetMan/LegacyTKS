using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.tksr.statemachine.defines;

namespace com.tksr.statemachine
{
    public class Turn
    {
        public Unit Actor;
        public bool HasUnitMoved;
        public bool HasUnitActed;
        public bool LockMove;
        public Ability Ability;
        public List<EncapsuleTile> Targets;
        public PlanOfAttack Plan;

        private EncapsuleTile startTile;
        private EnumStateDirections startDir;

        public void Change(Unit current)
        {
            Actor = current;
            HasUnitMoved = false;
            HasUnitActed = false;
            LockMove = false;
            startTile = Actor.Tile;
            startDir = Actor.Dir;
            Plan = null;
        }

        public void UndoMove()
        {
            HasUnitMoved = false;
            Actor.Place(startTile);
            Actor.Dir = startDir;
            Actor.Match();
        }
    }
}