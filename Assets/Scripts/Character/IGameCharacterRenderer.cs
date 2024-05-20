using System.Collections;
using System.Collections.Generic;
using com.tksr.property;
using com.tksr.schema;
using UnityEngine;

public class IGameCharacterRenderer : MonoBehaviour
{
    public int CharID;
    public int TemplateID;

    public CharAnimationRender CharAnimRender;
    public TaskHandler HandleScenarioTask;
    public EffectPlayer EffectPlayer;
    public Transform CharCollider;
    public Transform Shadow;
    private EnumDirection AnimDirection;

    protected CharMovementController movementController;
    public CharMovementController MovementController
    {
        set { movementController = value; }
        get { return movementController; }
    }

    void Awake()
    {
        HandleScenarioTask.objId = CharID;
    }

    private float dlgLocalX;
    private float dlgLocalY;
    public void ParseDialogPosParamNormalDisplay(bool bShowLeft)
    {
        var item = CharactersManager.Instance.GetCharacterResById(TemplateID);
        if (item != null)
        {
            dlgLocalX = item.DlgLocalX;
            if (bShowLeft)
            {
                dlgLocalX = -dlgLocalX;
            }
            dlgLocalY = item.DlgLocalY;
        }
        else
        {
            Debug.LogError("Not found TemplateID: " + TemplateID);
        }
    }

    public void ParseDialogPosParamOSDisplay()
    {
        var item = CharactersManager.Instance.GetCharacterResById(TemplateID);
        if (item != null)
        {
            dlgLocalX = 0;
            dlgLocalY = item.DlgLocalY;
        }
        else
        {
            var charIns = CharactersManager.Instance.GetCharacterInstanceById(CharID);
            if (charIns != null && charIns.IsHiddenObj > 0)
            {
                Debug.Log("This is a HiddenObj no need Template");
            }
            else
            {
                Debug.LogError("Not found TemplateID: " + TemplateID);
            }
        }
    }

    public void ParseDialogPosParamSighDisplay()
    {
        var item = CharactersManager.Instance.GetCharacterResById(TemplateID);
        if (item != null)
        {
            dlgLocalX = 0;
            dlgLocalY = item.DlgLocalY;
        }
        else
        {
            Debug.LogError("Not found TemplateID: " + TemplateID);
        }
    }

    public void ParseDialogPosParamSelectionDisplay()
    {
        var item = CharactersManager.Instance.GetCharacterResById(TemplateID);
        if (item != null)
        {
            dlgLocalX = 0;
            dlgLocalY = item.DlgLocalY;
        }
        else
        {
            Debug.LogError("Not found TemplateID: " + TemplateID);
        }
    }

    public Vector3 CalcUIDialogWorldPos()
    {
        Vector3 vecLocalPos = Vector3.zero;
        vecLocalPos = new Vector3(dlgLocalX, dlgLocalY, 0);
        Vector3 vecWorldPos = this.transform.TransformPoint(vecLocalPos);
        return vecWorldPos;
    }

    public void ShowShadow(bool bShow)
    {
        if (Shadow)
        {
            Shadow.gameObject.SetActive(bShow);
        }
    }

    public void LoadRenderInScenario(EnumDirection direction, EnumAnimAction action = EnumAnimAction.Static,
        float speed = 1f)
    {
        AnimDirection = direction;

        LoadRenderResource();
        if (CharAnimRender != null)
        {
            CharAnimRender.SetDirection(action, AnimDirection, speed);
        }
    }

    public void LoadRenderResource(bool forceReload = false)
    {
        int id = TemplateID;
        CharResItem schemaResourceItem = CharactersManager.Instance.GetCharacterResById(id);
        if (schemaResourceItem != null && CharAnimRender != null)
        {
            Animator animator = CharAnimRender.gameObject.GetComponent<Animator>();
            if (animator != null)
            {
                if (animator.runtimeAnimatorController == null || forceReload == true)
                {
                    string strABName = ResourceUtils.AB_PREFIX_ANIM_CHAR + schemaResourceItem.AssetBundleName;
                    string strAssetName = schemaResourceItem.WorldController;
                    var charAnimator = GameAssetBundlesManager.Instance.LoadAnimCtrlSync(strABName, strAssetName);
                    animator.runtimeAnimatorController = charAnimator;
                }
            }
        }
    }

    public void StandFaceToTarget(Transform target)
    {
        Vector2 vecTarget = new Vector2();
        vecTarget.x = target.position.x;
        vecTarget.y = target.position.y;
        Vector2 vecSelf = new Vector2();
        vecSelf.x = transform.position.x;
        vecSelf.y = transform.position.y;
        Vector2 vecDirection = vecTarget - vecSelf;
        StandFaceToByDirection(vecDirection);
    }

    public void StandFaceToTarget(Vector2 vecTarget)
    {
        Vector2 vecSelf = new Vector2();
        vecSelf.x = transform.position.x;
        vecSelf.y = transform.position.y;
        Vector2 vecDirection = vecTarget - vecSelf;
        StandFaceToByDirection(vecDirection);
    }

    public void StandFaceToByDirection(Vector2 vecDirection)
    {
        vecDirection = Vector2.ClampMagnitude(vecDirection, 1f);
        var enumDirection = CharAnimRender.CalcDirectionByVector(vecDirection);
        Debug.LogFormat("CharID = {0}, TemplateID = {1}, StandFaceToByDirection = {2}", CharID, TemplateID, enumDirection);
        CharAnimRender.SetDirection(EnumAnimAction.Static, enumDirection);
    }

    public void StandFaceToByEnumDirection(EnumDirection enumDirection)
    {
        CharAnimRender.SetDirection(EnumAnimAction.Static, enumDirection);
    }

    public void StandFaceToTarget(FaceParam param)
    {
        if (param.faceType == FaceParam.FaceType.ToTransform)
        {
            StandFaceToTarget(param.target);
        }
        else if (param.faceType == FaceParam.FaceType.ToPosition)
        {
            StandFaceToTarget(param.destination);
        }
        else
        {
            Debug.LogError("Error Face Type");
        }
    }

    public void MoveFaceToTarget(FaceParam param)
    {
        if (param.faceType == FaceParam.FaceType.ToTransform)
        {
            MoveFaceToTarget(param.target);
        }
        else if (param.faceType == FaceParam.FaceType.ToPosition)
        {
            MoveFaceToTarget(param.destination);
        }
        else
        {
            Debug.LogError("Error Face Type");
        }
    }

    public void MoveFaceToTarget(Transform target)
    {
        Vector2 vecTarget = new Vector2();
        vecTarget.x = target.position.x;
        vecTarget.y = target.position.y;
        Vector2 vecSelf = new Vector2();
        vecSelf.x = transform.position.x;
        vecSelf.y = transform.position.y;
        Vector2 vecDirection = vecTarget - vecSelf;
        MoveFaceToByDirection(vecDirection);
    }

    public void MoveFaceToTarget(Vector2 vecTarget)
    {
        Vector2 vecSelf = new Vector2();
        vecSelf.x = transform.position.x;
        vecSelf.y = transform.position.y;
        Vector2 vecDirection = vecTarget - vecSelf;
        MoveFaceToByDirection(vecDirection);
    }

    public void MoveFaceToByDirection(Vector2 vecDirection, float speed = 1f)
    {
        vecDirection = Vector2.ClampMagnitude(vecDirection, 1f);
        var enumDirection = CharAnimRender.CalcDirectionByVector(vecDirection);
        CharAnimRender.SetDirection(EnumAnimAction.Run, enumDirection, speed);
    }

    public void StandHere()
    {
        var direction = CharAnimRender.GetLastDirection();
        CharAnimRender.SetDirection(EnumAnimAction.Static, direction);
    }

    public void TimelinePlayAnim(EnumAnimAction anim, EnumDirection direction)
    {
        CharAnimRender.SetDirection(anim, direction);
    }

    public void TimelinePlayAnim(string strAnim)
    {
        if (Application.isEditor && !Application.isPlaying)
        {
            var animator = CharAnimRender.GetComponent<Animator>();
            animator.Play(strAnim);
        }
    }

    /// <summary>
    /// 某些剧情会触发人物时间停止在当前动画帧
    /// TODO:目前简单的使用Stand
    /// </summary>
    public void TimeStopAnimationFrame()
    {
        if (CharID != ResourceUtils.MAINROLE_ID)
        {
            // this is NPC
            NPCMainController npc = this as NPCMainController;
            npc.DestroyPathTween();
        }
        StandHere();
    }
}
