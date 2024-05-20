using com.tksr.data;
using com.tksr.property;
using UnityEngine;

public class CharMovementController : MonoBehaviour
{
    public float movementSpeed = 3f;

    private CharAnimationRender charAnimationRender;
    private Rigidbody2D rigidBody;
    private Vector2 bornPos;

    private EnumCharMoveStatus moveStatus = EnumCharMoveStatus.None;

    void Awake()
    {
        rigidBody = GetComponentInChildren<Rigidbody2D>();
        charAnimationRender = GetComponent<CharAnimationRender>();
    }

    /// <summary>
    /// TODO:必须要在FixedUpdate中移动主角.如果在Update中做则会导致严重的Lag,原因是什么?
    /// </summary>
    void FixedUpdate()
    {
        if (moveStatus == EnumCharMoveStatus.None)
            return;

        Vector2 vecCurPos = rigidBody.position;
        float fDistance = Vector2.Distance(vecTargetPos, vecCurPos);
        if (fDistance < 0.1f)
        {
            ChangeMoveStatus(EnumCharMoveStatus.Static);
            return;
        }
        Vector2 vecInput = vecTargetPos - vecCurPos;
        //vecInput = Vector2.ClampMagnitude(vecInput, 1f);
        vecInput.Normalize();
        Vector2 movement = vecInput * movementSpeed;
        Vector2 vecNewPos = vecCurPos + movement * Time.fixedDeltaTime;
        charAnimationRender.WorldUpdateDirection(movement);
        rigidBody.MovePosition(vecNewPos);
    }

    private Vector2 vecTargetPos = Vector2.zero;
    /// <summary>
    /// 移动角色
    /// </summary>
    /// <param name="vecTarget"></param>
    /// <returns></returns>
    public float DoAnimMoveChar(Vector2 vecTarget)
    {
        ChangeMoveStatus(EnumCharMoveStatus.Run);
        vecTargetPos = vecTarget;
        // NPC是没有rigidBody的
        if (rigidBody != null)
        {
            Vector2 vecCurPos = rigidBody.position;
            return Vector2.Distance(vecCurPos, vecTargetPos) / movementSpeed;
        }
        else
        {
            return 0f;
        }
    }

    public void DoAnimParkingChar()
    {
        ChangeMoveStatus(EnumCharMoveStatus.Static);
        vecTargetPos = rigidBody.position;
    }

    public void DoAnimFaceToDirection(Vector2 direction)
    {
        var enumDirection = charAnimationRender.CalcDirectionByVector(direction);
        charAnimationRender.SetDirection(EnumAnimAction.Static, enumDirection);
        DoAnimParkingChar();
    }

    public void ChangeMoveStatus(EnumCharMoveStatus newStatus)
    {
        if (moveStatus == newStatus)
        {
            return;
        }

        if (newStatus == EnumCharMoveStatus.Static)
        {
            charAnimationRender.SetDirection(EnumAnimAction.Static, charAnimationRender.GetLastDirection());
        }


        moveStatus = newStatus;
    }

    public void HoldPosition(Vector2 vecPosition)
    {
        Debug.LogFormat("HoldPosition = {0}", vecPosition.ToString());
        rigidBody.position = vecTargetPos = vecPosition;
    }

    public Vector2 GetRBPosition()
    {
        return rigidBody.position;
    }

    public enum EnumCharMoveStatus
    {
        None = 0,
        Static,
        Run
    }
}
