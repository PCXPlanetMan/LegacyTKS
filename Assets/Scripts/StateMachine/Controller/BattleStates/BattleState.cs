using com.tksr.statemachine.defines;
using System.Collections.Generic;
using UnityEngine;

namespace com.tksr.statemachine
{
    public abstract class BattleState : State
    {
        protected BattleController owner;
        protected Driver driver;

        [HideInInspector]
        public Board BoardOfTilesMap { get { return owner.BoardOfTilesMap; } }
        [HideInInspector]
        public List<EncapsuleTile> TilesData { get { return owner.TilesData; } }
        [HideInInspector]
        public Vector2Int CurrentTilePos { get { return owner.CurrentTilePos; } set { owner.CurrentTilePos = value; } }
        [HideInInspector]
        public Turn BattleTurn { get { return owner.BattleTurn; } }
        [HideInInspector]
        public List<Unit> Units { get { return owner.Units; } }
        [HideInInspector]
        public StatPanelUIController UIStatPanelController { get { return owner.UIStatPanelController; } }
        [HideInInspector]
        public AbilityMenuPanelUIController UIAbilityMenuPanelController { get { return owner.UIAbilityMenuPanelController; } }

        protected virtual void Awake()
        {
            owner = GetComponent<BattleController>();
        }

        #region 针对每个State注册消息回调,例如鼠标/触摸等交互消息
        protected override void AddListeners()
        {
            if (driver == null || driver.Current == EnumDrivers.Human)
            {
                InputController.moveEvent += OnMove;
                InputController.fireEvent += OnFire;
                InputController.mouseEvent += OnMouse;
                AbilityMenuPanelUIController.actionCommand += OnActionCommand;
            }
        }

        protected override void RemoveListeners()
        {
            InputController.moveEvent -= OnMove;
            InputController.fireEvent -= OnFire;
            InputController.mouseEvent -= OnMouse;
            AbilityMenuPanelUIController.actionCommand -= OnActionCommand;
        }
        #endregion

        public override void Enter()
        {
            driver = (BattleTurn.Actor != null) ? BattleTurn.Actor.GetComponent<Driver>() : null;
            base.Enter();
        }

        protected virtual void OnMove(object sender, InfoEventArgs<Vector2Int> e)
        {

        }

        protected virtual void OnFire(object sender, InfoEventArgs<int> e)
        {

        }

        protected virtual void OnMouse(object sender, InfoEventArgs<Vector3> e)
        {

        }

        protected virtual void SelectTile(Vector2Int p)
        {
            if (CurrentTilePos == p || !BoardOfTilesMap.DictTiles.ContainsKey(p))
                return;

            CurrentTilePos = p;

            owner.MapManager.ShowSelectIndicatorOnTileMap(p);
        }

        protected virtual void SelectTile(EncapsuleTile tile)
        {
            Vector2Int p = tile.VecPos;
            SelectTile(p);
            if (tile.content != null)
                owner.MapManager.SetCameraFollowTargetOnTileMap(tile.content.transform);
        }

        protected virtual Unit GetUnit(Vector2Int p)
        {
            EncapsuleTile t = BoardOfTilesMap.GetTile(p);
            GameObject content = t != null ? t.content : null;
            return content != null ? content.GetComponent<Unit>() : null;
        }

        protected virtual void RefreshPrimaryStatPanel(Vector2Int p)
        {
            Unit target = GetUnit(p);
            if (target != null)
                UIStatPanelController.ShowPrimary(target.gameObject);
            else
                UIStatPanelController.HidePrimary();
            showingPrimary = true;
        }

        protected virtual void RefreshSecondaryStatPanel(Vector2Int p)
        {
            Unit target = GetUnit(p);
            if (target != null)
                UIStatPanelController.ShowSecondary(target.gameObject);
            else
                UIStatPanelController.HideSecondary();
            showingPrimary = false;
        }

        protected virtual void RefreshSimpleLTPanel(Vector2Int p)
        {
            Unit target = GetUnit(p);
            if (target != null)
                UIStatPanelController.ShowSimpleLT(target.gameObject);
            else
                UIStatPanelController.HideSimpleLT();
        }

        protected virtual void RefreshSimpleLBPanel(Vector2Int p)
        {
            Unit target = GetUnit(p);
            if (target != null)
                UIStatPanelController.ShowSimpleLB(target.gameObject);
            else
                UIStatPanelController.HideSimpleLB();
        }

        private bool showingPrimary = true;
        protected virtual void ShowStatPanel(Vector2Int p)
        {
            Unit target = GetUnit(p);
            if (target != null)
            {
                Alliance alliance = target.GetComponent<Alliance>();
                if (alliance != null)
                {
                    if (alliance.type == EnumAlliances.Hero)
                    {
                        RefreshPrimaryStatPanel(p);
                        showingPrimary = true;
                    }
                    else
                    {
                        RefreshSecondaryStatPanel(p);
                        showingPrimary = false;
                    }
                }
            }
        }

        private bool showingSimpleLT = true;
        protected virtual void ShowSimplePanel(Vector2Int p, bool showLT)
        {
            Unit target = GetUnit(p);
            if (target != null)
            {
                showingSimpleLT = showLT;
                if (showingSimpleLT)
                {
                    RefreshSimpleLTPanel(BattleTurn.Actor.Tile.VecPos);
                }
                else
                {
                    RefreshSimpleLBPanel(BattleTurn.Actor.Tile.VecPos);
                }
            }
        }

        protected virtual void HideStatPanel()
        {
            if (showingPrimary)
            {
                UIStatPanelController.HidePrimary();
            }
            else
            {
                UIStatPanelController.HideSecondary();
            }
        }

        protected virtual void HideSimplePanel()
        {
            if (showingSimpleLT)
            {
                UIStatPanelController.HideSimpleLT();
            }
            else
            {
                UIStatPanelController.HideSimpleLB();
            }
        }

        protected virtual bool DidPlayerWin()
        {
            return owner.GetComponent<BaseVictoryCondition>().Victor == EnumAlliances.Hero;

        }

        protected virtual bool IsBattleOver()
        {
            return owner.GetComponent<BaseVictoryCondition>().Victor != EnumAlliances.None;
        }

        protected virtual void HighlightTiles(List<EncapsuleTile> dictSelectedTiles)
        {
            for (int i = dictSelectedTiles.Count - 1; i >= 0; --i)
            {
                var tile = dictSelectedTiles[i];

                owner.MapManager.HighlightSelectedTile(tile.VecPos);
            }
        }

        protected virtual void UnHighlightTiles(List<EncapsuleTile> dictSelectedTiles)
        {
            for (int i = dictSelectedTiles.Count - 1; i >= 0; --i)
            {
                var tile = dictSelectedTiles[i];

                owner.MapManager.UnHighlightSelectedTile(tile.VecPos);
            }
        }

        // 响应技能或命令
        protected virtual void OnActionCommand(object sender, InfoEventArgs<EnumActionCommand> e)
        {

        }
    }
}