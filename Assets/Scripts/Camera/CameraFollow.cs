using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

/// <summary>
/// TODO:关于PixelPerfectCamera的属性以及Camera数值和PPU之间的关系目前还不清楚.
/// 目前只是采用经验值按照实际设备分辨率的比率进行等比缩放
/// </summary>
public class CameraFollow : MonoBehaviour
{
    private static readonly int PPU_DESIGN = 64;
    private static readonly int REF_RESOLUTION_X = 640;
    private static readonly int REF_RESOLUTION_Y = 480;

    public int MapWidth;
    public int MapHeight;

    private Vector3 startingPosition;

    [HideInInspector]
    public Transform FollowTarget
    {
        get { return _follow; }
        set
        {
            _follow = value;
            if (value != null)
            {
                exploringMap = false;
            }
        }
    }

    private Transform _follow;

    private Vector3 targetPos;
    public float moveSpeed = 10f;

    private Vector2 vecCameraMaxMoveDistance = Vector2.zero;

    private PixelPerfectCamera ppCamera;

    void Awake()
    {
        ppCamera = this.gameObject.GetComponent<PixelPerfectCamera>();
    }


    // Start is called before the first frame update
    void Start()
    {
        startingPosition = Vector3.zero;
        ResizeCameraMaxMoveSize();
    }

    public void ResizeCameraMaxMoveSize()
    {
        vecCameraMaxMoveDistance.x = MapWidth * 1f / PPU_DESIGN / 2 - ppCamera.refResolutionX * 1f / ppCamera.assetsPPU / 2;
        vecCameraMaxMoveDistance.y = MapHeight * 1f / PPU_DESIGN / 2 - ppCamera.refResolutionY * 1f / ppCamera.assetsPPU / 2;
    }

    // 相机聚焦在人物脚底阴影使得画面中人物表现过于偏上,因此添加经验值使得相机聚焦在身体中部,看起来较为和谐
    private readonly float CAMERA_FOLLOW_HEIGHT_OFFSET = 0.75f;
    // Update is called once per frame
    void Update()
    {
        if (FollowTarget != null)
        {
            targetPos = new Vector3(FollowTarget.position.x, FollowTarget.position.y + CAMERA_FOLLOW_HEIGHT_OFFSET, transform.position.z);
            targetPos = CheckInCameraMaxRange(targetPos);
            Vector3 velocity = (targetPos - transform.position) * moveSpeed;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 1.0f, Time.deltaTime);
        }
        else
        {
            if (exploringMap)
            {
                manualMoveFocus = CheckInCameraMaxRange(manualMoveFocus);
                Vector3 velocity = (manualMoveFocus - transform.position) * moveSpeed;
                transform.position = Vector3.SmoothDamp(transform.position, manualMoveFocus, ref velocity, 1.0f, Time.deltaTime);
            }
        }
    }

    private Vector3 CheckInCameraMaxRange(Vector3 vecTarget)
    { 
        Vector2 vecDistance = vecTarget - startingPosition;
        if (vecDistance.x > vecCameraMaxMoveDistance.x)
        {
            vecTarget.x = vecCameraMaxMoveDistance.x;
        }
        if (vecDistance.x < -vecCameraMaxMoveDistance.x)
        {
            vecTarget.x = -vecCameraMaxMoveDistance.x;
        }
        if (vecDistance.y > vecCameraMaxMoveDistance.y)
        {
            vecTarget.y = vecCameraMaxMoveDistance.y;
        }
        if (vecDistance.y < -vecCameraMaxMoveDistance.y)
        {
            vecTarget.y = -vecCameraMaxMoveDistance.y;
        }

        return vecTarget;
    }


    private Vector3 manualMoveFocus;
    // 响应键盘移动的相机经验移动距离
    private readonly float EXP_MoveCameraStep = 2f;
    private bool exploringMap = false;

    /// <summary>
    /// 用于开启地图探索模式,可以浏览整个地图
    /// </summary>
    /// <param name="vecDirection"></param>
    public void ExploreBattleMap(Vector2Int vecDirection)
    {
        if (FollowTarget != null)
        {
            FollowTarget = null;
        }

        exploringMap = true;

        Vector3 curFocus = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        if (vecDirection.x == 1)
        {
            curFocus.x += EXP_MoveCameraStep;
        }
        else if (vecDirection.x == -1)
        {
            curFocus.x -= EXP_MoveCameraStep;
        }

        if (vecDirection.y == 1)
        {
            curFocus.y += EXP_MoveCameraStep / 2f;
        }
        else if (vecDirection.y == -1)
        {
            curFocus.y -= EXP_MoveCameraStep / 2f;
        }

        manualMoveFocus = curFocus;
    }
}
