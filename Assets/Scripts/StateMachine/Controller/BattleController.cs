using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace com.tksr.statemachine
{
    /// <summary>
    /// 基于状态机的战场控制器,负责战场逻辑的状态控制,协调表现层和数据层之间的通信(因此可能会有一些和表现层对象的耦合)
    /// </summary>
    public class BattleController : StateMachine
    {
        [HideInInspector]
        // TODO:解耦.负责从表现层获取TilesMap和人物信息资源
        public ScenarioMap MapManager { get; set; }


        [HideInInspector]
        public Board BoardOfTilesMap;
        [HideInInspector]
        // TilesMap中可行走的Tiles,由表现层根据地图数据解析生成赋值
        public List<EncapsuleTile> TilesData;
        [HideInInspector]
        public Vector2Int CurrentTilePos;
        [HideInInspector]
        public EncapsuleTile CurrentTile { get { return BoardOfTilesMap.GetTile(CurrentTilePos); } }
        [HideInInspector]
        public StatPanelUIController UIStatPanelController;
        [HideInInspector]
        public AbilityMenuPanelUIController UIAbilityMenuPanelController;
        // TODO:用于在地图中显示人物朝向(目前TKS没有此逻辑）
        [HideInInspector] 
        public Transform FacingIndicator;
        [HideInInspector]
        public Turn BattleTurn = new Turn();
        [HideInInspector]
        // 当前地图中加载的人物对应的抽象数据列表
        public List<Unit> Units = new List<Unit>();
        [HideInInspector]
        public IEnumerator Round;
        [HideInInspector]
        public ComputerPlayer CPU;

        [HideInInspector] 
        public DisplayBattleInfoController UIDisplayBattleInfoController;


        private GameObject battleGUI;

        private void Awake()
        {
            BoardOfTilesMap = new Board();
            CPU = new ComputerPlayer(this);

            // TODO:Dynamic Load
            battleGUI = GameObject.Find(DefinesOfStateMachine.SM_GUI_CONTROLLER);
            if (battleGUI != null)
            {
                UIStatPanelController = battleGUI.GetComponent<StatPanelUIController>();
                UIAbilityMenuPanelController = battleGUI.GetComponent<AbilityMenuPanelUIController>();
                UIDisplayBattleInfoController = battleGUI.GetComponent<DisplayBattleInfoController>();
            }
        }

        void Start()
        {
            if (battleGUI != null)
                ChangeState<InitBattleState>();
        }
    }
}