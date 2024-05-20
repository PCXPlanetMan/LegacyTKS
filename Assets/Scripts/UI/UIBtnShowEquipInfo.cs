using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBtnShowEquipInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image Background;
    public Text TextName;
    public Image Icon;

    [HideInInspector]
    public bool IsPointerEnter = false;

    [HideInInspector] public int EquipID = 0;
    [HideInInspector] public int BtnIndex = -1;

    public UIInfoPanelEquipments ParentInfoPanelEquips;
    public UIEquipsContentContainer ParentContentEquips;

    // Start is called before the first frame update
    void Start()
    {
        Background.enabled = false;
        TextName.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isInEquiping)
            return;

        Background.enabled = true;
        TextName.color = Color.red;
        IsPointerEnter = true;

        ParentInfoPanelEquips.ShowEquipmentDiff(EquipID);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isInEquiping)
            return;
        Background.enabled = false;
        TextName.color = Color.white;
        IsPointerEnter = false;
        if (isShowingTips)
        {
            isShowingTips = false;
            if (ParentInfoPanelEquips != null)
            {
                ParentInfoPanelEquips.ShowEquipTips(false, Vector3.zero);
            }
        }
        ParentInfoPanelEquips.ShowEquipmentDiff(0);
    }

    private bool isShowingTips = false;
    private bool isInEquiping = false;

    // TODO:鼠标右键点击到Item中间的空隙时会导致直接关闭UI
    void Update()
    {
        if (IsPointerEnter)
        {
            if (Input.GetMouseButton(1))
            {
                //Debug.Log("Right Click Item : " + this.transform.position);

                if (!isShowingTips && !isInEquiping)
                {
                    isShowingTips = true;
                    if (ParentInfoPanelEquips != null)
                    {
                        Vector3 vecTipsPos = this.transform.position;
                        // TODO:HardCode, 最后四个Item的Tips的位置
                        if (BtnIndex < 8)
                        {
                            vecTipsPos += new Vector3(-12, -40, 0);
                        }
                        else
                        {
                            vecTipsPos += new Vector3(-12, 180, 0);
                        }
                        ParentInfoPanelEquips.ShowEquipTips(true, vecTipsPos, EquipID);
                    }
                }
            }
            else if (Input.GetMouseButtonUp(1))
            {
                if (isShowingTips)
                {
                    isShowingTips = false;
                    if (ParentInfoPanelEquips != null)
                    {
                        ParentInfoPanelEquips.ShowEquipTips(false, Vector3.zero);
                    }
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (ParentInfoPanelEquips != null && ParentContentEquips != null)
                {
                    ParentContentEquips.SetCurrentInUseEquipIndex(BtnIndex);
                    ParentInfoPanelEquips.ShowComButtonWhenUseEquip(true);

                    isInEquiping = true;

                    Background.enabled = true;
                    TextName.color = Color.red;
                }
            }
        }
    }

    public void UnUseEquip()
    {
        isInEquiping = false;
        IsPointerEnter = false;
    }
}
