using System.Collections;
using System.Collections.Generic;
using com.tksr.data;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : Singleton<CursorManager>
{
    public enum CURSOR_TYPE
    {
        Normal,
        Disable,
        Door,
        Grab,
        Dialog,
        CangMing,
        Zoom
    }

    private CURSOR_TYPE lastCursorType = CURSOR_TYPE.Normal;
    private CURSOR_TYPE curCursorType = CURSOR_TYPE.Normal;

    public Image CursorUI;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeCursorType(curCursorType);
        //Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        if (System.Math.Abs(Input.GetAxis("Mouse X"))> Mathf.Epsilon || System.Math.Abs(Input.GetAxis("Mouse Y")) > Mathf.Epsilon)
        {
            Vector3 vecPosition = Input.mousePosition;
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 vecPosition = new Vector3(touch.position.x, touch.position.y, 0);
#endif

            //Debug.Log("Input.mousePosition = " + Input.mousePosition);
            if (CursorUI != null)
            {
                CursorUI.rectTransform.position = vecPosition;
            }

            if (GameMainManager.Instance.CurGameMode == EnumGameMode.WaitingInputName)
            {
                ChangeCursorType(CURSOR_TYPE.Normal);
                return;
            }

            if (GameMainManager.Instance.CurGameMode == EnumGameMode.RunningScenario ||
                GameMainManager.Instance.CurGameMode == EnumGameMode.ScenarioDialogMoment)
            {
                ChangeCursorType(CURSOR_TYPE.CangMing);
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(vecPosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);

            UpdateCursorByHit(hit);
        }
    }

    public void ChangeCursorType(CURSOR_TYPE type)
    {
        if (curCursorType != type)
        {
            lastCursorType = curCursorType;
            curCursorType = type;
            if (CursorUI != null)
            {
                CursorUI.GetComponent<ImageCursor>().PlayCursorAnimation(type.ToString());
            }
        }   
    }

    private void UpdateCursorByHit(RaycastHit2D hit)
    {
        if (hit.collider != null)
        {
            //Debug.Log("hit.collider = " + hit.collider.name);

            if (GameMainManager.Instance.CurGameMode == EnumGameMode.MainContentUI)
            {
                return;
            }

            if (hit.collider.CompareTag(EnumGameTagType.ColliderScenarioEntry.ToString()))
            {
                ChangeCursorType(CURSOR_TYPE.Door);
            }
            else if (hit.collider.CompareTag(EnumGameTagType.HittableNPC.ToString()))
            {
                ChangeCursorType(CURSOR_TYPE.Dialog);
            }
            else if (hit.collider.CompareTag(EnumGameTagType.HittableHiddenObj.ToString()))
            {
                var npc = hit.collider.gameObject.GetComponentInParent<NPCMainController>();
                if (npc != null && npc.CharID > 0)
                {
                    var charIns = CharactersManager.Instance.GetCharacterInstanceById(npc.CharID);
                    if (charIns != null)
                    {
                        if (charIns.IsHiddenObj == 1)
                        {
                            ChangeCursorType(CURSOR_TYPE.Zoom);
                            return;
                        }
                        else if (charIns.IsHiddenObj == 2)
                        {
                            ChangeCursorType(CURSOR_TYPE.Grab);
                            return;
                        }
                        else if (charIns.IsHiddenObj == 3)
                        {
                            ChangeCursorType(CURSOR_TYPE.Normal);
                            return;
                        }
                    }
                }
                ChangeCursorType(CURSOR_TYPE.Zoom);
            }
        }
        else
        {
            ChangeCursorType(CURSOR_TYPE.Normal);
        }
    }

    public void ShowCursor(bool show)
    {
        if (CursorUI != null)
        {
            CursorUI.gameObject.SetActive(show);
        }
    }
}
