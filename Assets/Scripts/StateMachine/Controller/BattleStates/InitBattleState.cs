using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using com.tksr.statemachine.defines;
using com.tksr.property;

namespace com.tksr.statemachine
{
    /// <summary>
    /// 进入战斗场景初始化,主要负责加载人物属性数据
    /// </summary>
    public class InitBattleState : BattleState
    {
        public override void Enter()
        {
            base.Enter();
            StartCoroutine(Init());
        }

        IEnumerator Init()
        {
            BoardOfTilesMap.Load(TilesData);
            SelectTile(TilesData[0]);
            SpawnBattleCharacters();
            AddVictoryCondition();
            owner.Round = owner.gameObject.AddComponent<TurnOrderController>().Round();
            yield return null;
            owner.ChangeState<CutSceneState>();
        }

        /// <summary>
        /// 生成战斗状态机所需要的角色信息
        /// </summary>
        private void SpawnBattleCharacters()
        {
            var charactersData = owner.MapManager.SMLoadAllCharsInBattle();

            List<EncapsuleTile> locations = new List<EncapsuleTile>(BoardOfTilesMap.DictTiles.Values);
            for (int i = 0; i < charactersData.Count; ++i)
            {
                var character = charactersData[i];
                GameObject goUnit = new GameObject(DefinesOfStateMachine.SM_GO_UNIT_NAME);
                goUnit.transform.SetParent(character.gameObject.transform);

                int level = (int)character.CharLevel;

                // 根据配置读取人物战斗属性模板数据
                UnitRecipe recipe = character.GetStateMachineUnitRecipe();
                // 根据模板和当前人物等级解析人物属性
                GameObject instance = UnitFactory.Create(goUnit.gameObject, recipe, level);

                // 在整个地图中生成角色的位置
                EncapsuleTile charTile = BoardOfTilesMap.DictTiles[character.BornPosOnTile];

                Unit unit = instance.GetComponent<Unit>();
                unit.Place(charTile);
                // TODO:随机生成人物在地图中的朝向
                unit.Dir = (EnumStateDirections)Random.Range(0, 4);
                unit.Match();

                Units.Add(unit);
            }

            // TODO:默认将主角放在第一个位置,首次均选择主角
            SelectTile(Units[0].Tile);
        }

        void AddVictoryCondition()
        {
            DefeatTargetVictoryCondition vc = owner.gameObject.AddComponent<DefeatTargetVictoryCondition>();
            // TODO:战斗初始化时,先创建主角再创建一个敌人,故Units.Count - 1则代表的是敌人
            Unit enemy = Units[Units.Count - 1];
            vc.target = enemy;
            Health health = enemy.GetComponent<Health>();
            health.MinHP = 0;
        }
    }
}