using com.tksr.property;
using com.tksr.schema;
using com.tksr.statemachine.defines;
using System;
using System.Linq;
using com.tksr.data;
using DG.Tweening;
using UnityEngine;

public class CharMainController : IGameCharacterRenderer
{
    public uint CharLevel { set; get; }
    
    private bool isManualControlled = false;
    private float charColliderRadius = 0f;

    /// <summary>
    /// 设置主角是否可以受控
    /// </summary>
    /// <param name="controlled"></param>
    public void SwitchManualControlled(bool controlled)
    {
        isManualControlled = controlled;
        if (isManualControlled)
        {
            CharCollider.gameObject.SetActive(true);
            CharAnimRender.SetDirection(EnumAnimAction.Static, CharAnimRender.GetLastDirection());

            var collider = CharCollider.gameObject.GetComponent<CircleCollider2D>();
            if (collider != null)
            {
                charColliderRadius = collider.radius;
            }
        }
        else
        {
            CharCollider.gameObject.SetActive(false);
            if (movementController != null)
                movementController.ChangeMoveStatus(CharMovementController.EnumCharMoveStatus.None);
        }
    }

    private EnumInteractiveAction curInteractiveAction = EnumInteractiveAction.None;
    private NPCMainController currentActionNPC;

    /// <summary>
    /// 当NPC被点击后,NPC会停止当前的DoTween动作,并准备对话
    /// </summary>
    /// <param name="npc"></param>
    private void PrepareDoTaskWithOther(NPCMainController npc)
    {
        currentActionNPC = npc;
        npc.PauseCurrentTweenPath();
    }

    private void MakeNPCFaceToMeWhenTalkToNPC()
    {
        if (currentActionNPC)
        {
            var npc = currentActionNPC;
            // 如果是特殊动画,则无需使NPC面向主角
            if (!npc.CharAnimRender.SaveOldAnimationData())
            {
                Vector2 vecMainRole = new Vector2(this.transform.position.x, this.transform.position.y);
                Vector2 vecNPC = new Vector2(npc.transform.position.x, npc.transform.position.y);
                Vector2 vecDirection = vecMainRole - vecNPC;
                Vector2 direction = Vector2.ClampMagnitude(vecDirection, 1f);
                var enumDirection = npc.CharAnimRender.CalcDirectionByVector(direction);
                npc.CharAnimRender.SetDirection(EnumAnimAction.Static, enumDirection);
            }
        }
    }

    /// <summary>
    /// 场景中主角和NPC对话完毕后做最后的清除操作
    /// </summary>
    public void FinishDialogWithNPC()
    {
        curInteractiveAction = EnumInteractiveAction.None;
        PostProcessInteraction();
    }

    /// <summary>
    /// 当鼠标点击场景中的NPC/Item等则产生相应的交互动作
    /// </summary>
    /// <param name="hitObject"></param>
    private bool GenerateInteractiveAction(Collider2D hitObject)
    {
        bool hasHit = false;
        curInteractiveAction = EnumInteractiveAction.None;
        if (hitObject != null)
        {
            //Debug.LogFormat("Click NPC, {0}", hit.collider.tag);
            EnumGameTagType tagType = EnumGameTagType.Unknown;
            if (!Enum.TryParse(hitObject.tag, out tagType))
            {
                tagType = EnumGameTagType.Unknown;
            }
            switch (tagType)
            {
                case EnumGameTagType.HittableNPC:
                {
                    NPCMainController npc = hitObject.gameObject.GetComponentInParent<NPCMainController>();
                    bool bRun = npc.HandleScenarioTask.HasDoingTask();
                    if (bRun)
                    {
                        curInteractiveAction = EnumInteractiveAction.WalkingToTaskPos;
                        PrepareDoTaskWithOther(npc);
                    }
                    else
                    {
                        Debug.LogWarningFormat("Hit NPC but no doing task, npc = {0}", npc.CharID);
                    }
                }
                    break;
                case EnumGameTagType.HittableHiddenObj:
                {
                    NPCMainController npc = hitObject.gameObject.GetComponentInParent<NPCMainController>();
                    if (npc != null)
                    {
                        var charIns = CharactersManager.Instance.GetCharacterInstanceById(npc.CharID);
                        if (charIns.IsHiddenObj != 0)
                        {
                            bool bRun = npc.HandleScenarioTask.HasDoingTask();
                            if (bRun)
                            {
                                curInteractiveAction = EnumInteractiveAction.WalkingToTaskPos;
                                PrepareDoTaskWithOther(npc);
                            }
                            else
                            {
                                Debug.LogWarningFormat("Hit Hidden but no doing task, hidden = {0}", npc.CharID);
                            }
                        }
                    }
                }
                    break;
                default:
                    break;
            }

            hasHit = true;
        }

        // 在走向NPC未触发实际的对话之前可以随时取消
        PostProcessInteraction();
        return hasHit;
    }

    /// <summary>
    /// 处理完点击目标后的后处理
    /// </summary>
    private void PostProcessInteraction()
    {
        if (curInteractiveAction == EnumInteractiveAction.None)
        {
            if (currentActionNPC != null && currentActionNPC.gameObject.activeInHierarchy == true)
            {
                currentActionNPC.ResumeCurrentTweenPath();
            }
            currentActionNPC = null;
        }
    }

    /// <summary>
    /// 根据屏幕输入和当前游戏状态实现主角的交互(eg. NPC对话，场景物品交互，触发剧情等),实际的动作发生在主角朝NPC走过去并进入NPC交互范围之内.
    /// </summary>
    /// <param name="mousePos"></param>
    public void UpdateActionByInputScreenPos(Vector3 mousePos)
    {
        if (isManualControlled)
        {
            var screenPos = Camera.main.ScreenToWorldPoint(mousePos);
            Vector2 pos2D = new Vector2(screenPos.x, screenPos.y);
            //RaycastHit2D hit = Physics2D.Raycast(pos2D, Vector2.zero);
            var hits = Physics2D.RaycastAll(pos2D, Vector2.zero);
            if (hits.Length > 0)
            {
                RaycastHit2D hit = hits.ToArray().FirstOrDefault(x => x.collider.CompareTag(EnumGameTagType.HittableNPC.ToString()) == true 
                                                                      || x.collider.CompareTag(EnumGameTagType.HittableHiddenObj.ToString()) == true);
                // 根据鼠标点击点的对象而决定执行何种动作
                bool hitSomething = GenerateInteractiveAction(hit.collider);
                if (hit.collider != null)
                    Debug.LogFormat("Hit collider = {0}", hit.collider.name);
                else
                {
                    Debug.Log("Not found Hittable NPC");
                }
            }

            // 判断点击点在边界之外时,主角走到边界附近自动停下来
            var curMainRoleRBPos = movementController.GetRBPosition();
            Vector2 vecMainRoleToTouchPos = ((Vector2) screenPos - curMainRoleRBPos);
            float rayCheckDistance = vecMainRoleToTouchPos.magnitude;
            Vector2 vecCheckDirection = vecMainRoleToTouchPos.normalized;
            LayerMask mask = LayerMask.GetMask(EnumGameLayer.GameBorder.ToString());
            if (charColliderRadius > 0f)
            {
                RaycastHit2D hitBorder = Physics2D.CircleCast(curMainRoleRBPos, charColliderRadius, vecCheckDirection, rayCheckDistance, mask);
                if (hitBorder.collider != null)
                {
                    screenPos = hitBorder.centroid;
                    Debug.Log("Hit Border");
                    //ScenarioManager.Instance.DisappearDialogWhenClicked();
                }
            }

            // 主角朝当前目标走过去(而后根据之前确定的动作而产生最终的结果)
            movementController.DoAnimMoveChar(screenPos);
        }
    }

    /// <summary>
    /// 从配置中读取武将人物的外观/属性.(属性数据并不是每个角色都有的,例如NPC)
    /// 如果是战斗状态则还要还原状态机
    /// </summary>
    /// <param name="cri"></param>
    /// <param name="cdi"></param>
    public void LoadResourceInBattleFromSchema(int instanceId, CharResItem cri, CharDataItem cdi)
    {
        CharID = instanceId;
        TemplateID = cdi.Id;
        ParseWeaponTypeFromSchema(cdi);
        StateMachineUnitRecipeFromSchema(cdi);

        if (CharAnimRender != null)
        {
            Animator animator = CharAnimRender.gameObject.GetComponent<Animator>();
            if (animator != null)
            {
                string strABName = ResourceUtils.AB_PREFIX_ANIM_CHAR + cri.AssetBundleName;
                string strAssetName = cri.BattleController;
                if (TemplateID == 10001001) // 主角根据当前装备确定动画,配置表中的BattleController不被信任
                {
                    strAssetName = GetMainRoleRenderController(charWeapongType);
                }
                CharAnimRender.ParseAnimationDictInBattle(charWeapongType);
                var charAnimator = GameAssetBundlesManager.Instance.LoadAnimCtrlSync(strABName, strAssetName);
                animator.runtimeAnimatorController = charAnimator;


                // 战斗场景使用状态机控制移动
            }
            ShowShadow(true);
        }

        // 在战场状态下需要关闭所有碰撞器,否则Collider2D会影响人物的移动
        var colliders = this.transform.GetComponentsInChildren<Collider2D>();
        foreach (var collider in colliders)
        {
            collider.gameObject.SetActive(false);
        }
    }

    public Vector2 GetTilePosition()
    {
        return movementController.GetRBPosition();
    }

    private EnumWeaponType charWeapongType = EnumWeaponType.None;
    private EnumWeaponType ParseWeaponTypeFromSchema(CharDataItem cdi)
    {
        if (cdi != null)
        {
            charWeapongType = (EnumWeaponType)Enum.Parse(typeof(EnumWeaponType), cdi.WeaponType);
            if (cdi.Id == 10001001) // TODO:如果是主角,则不能信任配置中的武器属性,需要根据当前装备的武器判断
            {
                charWeapongType = EnumWeaponType.None;
            }
        }
        return charWeapongType;
    }

    /// <summary>
    /// 根据主角装备的武器类型确定其动画控制器
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private string GetMainRoleRenderController(EnumWeaponType type)
    {
        string strController = null;
        
        if (type == EnumWeaponType.None)
        {
            strController = ResourceUtils.MAINROLE_BATTLE_ANIM_CONTROLLER_NONE;
        }
        else if (type == EnumWeaponType.Arrow)
        {
            strController = ResourceUtils.MAINROLE_BATTLE_ANIM_CONTROLLER_ARROW;
        }
        else if (type == EnumWeaponType.Blade)
        {
            strController = ResourceUtils.MAINROLE_BATTLE_ANIM_CONTROLLER_BLADE;
        }
        else if (type == EnumWeaponType.Broadsword)
        {
            strController = ResourceUtils.MAINROLE_BATTLE_ANIM_CONTROLLER_BROADSWORD;
        }
        else if (type == EnumWeaponType.Pike)
        {
            strController = ResourceUtils.MAINROLE_BATTLE_ANIM_CONTROLLER_PIKE;
        }
        else if (type == EnumWeaponType.Spear)
        {
            strController = ResourceUtils.MAINROLE_BATTLE_ANIM_CONTROLLER_SPEAR;
        }
        else if (type == EnumWeaponType.Sword)
        {
            strController = ResourceUtils.MAINROLE_BATTLE_ANIM_CONTROLLER_SWORD;
        }

        return strController;
    }

    public void ForceChangeDirection(string strDirection)
    {
        EnumDirection direction = (EnumDirection)Enum.Parse(typeof(EnumDirection), strDirection);
        ForceChangeDirection(direction);
    }

    public void ForceChangeDirection(EnumDirection direction)
    {
        EnumAnimAction currentAnimAction = CharAnimRender.GetLastAnimAction();
        CharAnimRender.SetDirection(currentAnimAction, direction);
    }

    public float DoPlayNormalAttackAnim()
    {
        EnumDirection lastDirection = CharAnimRender.GetLastDirection();
        EnumAnimAction currentAnimAction = EnumAnimAction.Attack;
        return CharAnimRender.SetDirection(currentAnimAction, lastDirection);
    }

    public float DoPlayUnderAttackAnim()
    {
        float fShakeDuration = 1f;
        this.transform.DOShakePosition(fShakeDuration, 0.1f, 50).SetEase(Ease.InOutQuad);
        return fShakeDuration;
    }

    public float DoPlayDeadAnim()
    {
        float fDeadDuration = 1.5f;
        this.transform.DOShakePosition(fDeadDuration, 0.15f, 60).SetEase(Ease.InOutQuad);
        return fDeadDuration;
    }

    // 一旦位于战场中,那么必须要有关联的ScenarioMap,用于转换CellPostion到WorldPosition
    [HideInInspector]
    public ScenarioMap AttachedMap = null;

    /// <summary>
    /// 在TileMap中根据世界坐标放置角色
    /// </summary>
    /// <param name="worldPos"></param>
    public void PlaceAtTileByWorldPos(Vector3 worldPos)
    {
        this.transform.position = worldPos;
    }

    private Vector2Int curCellPos = Vector2Int.zero;
    /// <summary>
    /// 在TileMap中根据TileMap坐标放置角色
    /// </summary>
    /// <param name="cellPos"></param>
    public void PlaceAttTileByCellPos(Vector2Int cellPos)
    {
        if (AttachedMap == null)
        {
            Debug.LogError("No Attached Map Component, can't calculate tile position");
            return;
        }
        curCellPos = cellPos;
        Vector3 vecWorldPos = AttachedMap.GridMap.GetCellCenterWorld(new Vector3Int(cellPos.x, cellPos.y, 0));
        PlaceAtTileByWorldPos(vecWorldPos);
    }

    public Vector2Int GetCurrentCellPos()
    {
        return curCellPos;
    }

    private UnitRecipe unitRecipe;
    private UnitRecipe StateMachineUnitRecipeFromSchema(CharDataItem cdi)
    {
        if (unitRecipe == null)
            unitRecipe = new UnitRecipe();

        if (cdi != null)
        {
            unitRecipe.job = cdi.Job;
            unitRecipe.attack = cdi.Attack;
            unitRecipe.abilityCatalog = cdi.AbilityCatalog;
            unitRecipe.strategy = cdi.Strategy;
            unitRecipe.locomotion = (EnumLocomotions)Enum.Parse(typeof(EnumLocomotions), cdi.Locomotions);
            unitRecipe.alliance = (EnumAlliances)Enum.Parse(typeof(EnumAlliances), cdi.Alliance);
        }
        return unitRecipe;
    }

    public UnitRecipe GetStateMachineUnitRecipe()
    {
        return unitRecipe;
    }

    public void UpdateAnimDirectionFromSM(Vector2 vecDirection)
    {
        vecDirection = Vector2.ClampMagnitude(vecDirection, 1);
        CharAnimRender.WorldUpdateDirection(vecDirection);
    }

    public Vector2Int BornPosOnTile { get; set; }

    private enum EnumFilterCollision
    {
        NoCollision = 0x0,
        NPCCollision = 0x1,
        HiddenCollision = 0x2,
    }

    /// <summary>
    /// 当主角到达任务点后执行具体的任务
    /// </summary>
    /// <param name="collision"></param>
    private void TriggerDoTask(Collider2D collision)
    {
        EnumFilterCollision filterCollision = EnumFilterCollision.NoCollision;
        if (collision.CompareTag(EnumGameTagType.ColliderNPC.ToString()))
        {
            filterCollision = EnumFilterCollision.NPCCollision;
        }
        else if (collision.CompareTag(EnumGameTagType.ColliderHiddenObject.ToString()))
        {
            filterCollision = EnumFilterCollision.HiddenCollision;
        }

        if (filterCollision != EnumFilterCollision.NoCollision)
        {
            var npc = collision.transform.parent.GetComponent<NPCMainController>();
            if (currentActionNPC != null && npc == currentActionNPC)
            {
                // 在触发了主角和NPC的对话并走到NPC(之前NPC已经停下来了)周围后就停下来
                movementController.DoAnimParkingChar();

                // HiddenObject并不会动!!!
                if (filterCollision == EnumFilterCollision.NPCCollision)
                {
                    // 如果是通过点击触发了类似对话剧情,则无需使NPC面向对主角
                    if (!ScenarioManager.Instance.IsTaskHandlerContainPlayTimeline(currentActionNPC.HandleScenarioTask))
                    {
                        // 在对话开始时使NPC面向主角
                        MakeNPCFaceToMeWhenTalkToNPC();
                    }
                }

                curInteractiveAction = EnumInteractiveAction.DoingTask;

                ScenarioManager.Instance.DoScenarioTaskWithOther(currentActionNPC.HandleScenarioTask);
            }
        }
    }

    private EnumAnimAction lastAnimAction;
    private EnumDirection lastAnimDirection;
    /// <summary>
    /// 判断主角的碰撞对象从而决定接下来执行的动作
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GameMainManager.Instance.CurGameMode == EnumGameMode.RunningScenario)
        {
            return;
        }
        if (curInteractiveAction == EnumInteractiveAction.WalkingToTaskPos)
        {
            TriggerDoTask(collision);
        }
        else
        {
            if (collision.CompareTag(EnumGameTagType.ColliderScenarioEntry.ToString()))
            {
                // 某些出入口可能有事件触发
                ScenarioEntry entry = collision.gameObject.GetComponent<ScenarioEntry>();
                bool hasEntryTask = entry.HandleScenarioTask.HasDoingTask();

                // TODO:触发事件和进出应该没有关系
                if (hasEntryTask)
                {
                    ScenarioManager.Instance.DoScenarioTaskWithOther(entry.HandleScenarioTask);
                    //curInteractiveAction = EnumInteractiveAction.None;
                }
                else
                {
                    //curInteractiveAction = EnumInteractiveAction.IntoEntry;
                }
                curInteractiveAction = EnumInteractiveAction.IntoEntry;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (curInteractiveAction == EnumInteractiveAction.IntoEntry)
        {
            if (collision.CompareTag(EnumGameTagType.ColliderScenarioEntry.ToString()))
            {
                ScenarioEntry entry = collision.gameObject.GetComponent<ScenarioEntry>();
                Vector3 vecMainChar = transform.position;
                Vector3 vecEntryCenter = entry.transform.position;
                var polygon = entry.gameObject.GetComponent<PolygonCollider2D>();
                if (polygon != null)
                {
                    vecEntryCenter = polygon.bounds.center;
                }

                float distance = Vector3.Distance(vecMainChar, vecEntryCenter);
                if (distance < 1f)
                {
                    Debug.Log("Trigger ScenarioEntry, entryId = " + entry.entryId);
                    entry.SwitchToNextScenarioByEntry();
                    curInteractiveAction = EnumInteractiveAction.None;
                }
            }
        }
        else if (curInteractiveAction == EnumInteractiveAction.WalkingToTaskPos)
        {
            TriggerDoTask(collision);
        }
    }

    /// <summary>
    /// 无剧情时切换地图使用RB设置主角位置
    /// </summary>
    /// <param name="vecPos"></param>
    /// <param name="vecDirection"></param>
    public void BornInScenario(Vector2 vecPos, Vector2 vecDirection)
    {
        movementController.HoldPosition(vecPos);
        Vector2 direction = Vector2.ClampMagnitude(vecDirection, 1f);
        var enumDirection = CharAnimRender.CalcDirectionByVector(direction);
        CharAnimRender.SetDirection(EnumAnimAction.Static, enumDirection);
        LoadRenderInScenario(enumDirection);
    }

    /// <summary>
    /// 根据剧情动画需要设置主角位置
    /// </summary>
    /// <param name="vecPos"></param>
    /// <param name="vecDirection"></param>
    public void PlaceInScenario(Vector2 vecPos, Vector2 vecDirection)
    {
        this.transform.position = new Vector3(vecPos.x, vecPos.y, 0);
        Vector2 direction = Vector2.ClampMagnitude(vecDirection, 1f);
        var enumDirection = CharAnimRender.CalcDirectionByVector(direction);
        CharAnimRender.SetDirection(EnumAnimAction.Static, enumDirection);
        LoadRenderInScenario(enumDirection);
    }
}