using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBtnShowItemInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image Background;
    public Text TextName;
    public Text TextCount;
    public Image Icon;

    [HideInInspector]
    public bool IsPointerEnter = false;

    [HideInInspector] public int ItemID = 0;
    [HideInInspector] public int BtnIndex = -1;

    public UIInfoPanelItems ParentInfoPanelItems;
    public UIItemsContentContainer ParentContentItems;

    // Start is called before the first frame update
    void Start()
    {
        Background.enabled = false;
        TextName.color = Color.white;
        TextCount.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isInUsing)
            return;

        Background.enabled = true;
        TextName.color = Color.red;
        TextCount.color = Color.red;
        IsPointerEnter = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isInUsing)
            return;
        Background.enabled = false;
        TextName.color = Color.white;
        TextCount.color = Color.white;
        IsPointerEnter = false;
        if (isShowingTips)
        {
            isShowingTips = false;
            if (ParentInfoPanelItems != null)
            {
                ParentInfoPanelItems.ShowItemTips(false, Vector3.zero);
            }
        }
    }

    private bool isShowingTips = false;
    [HideInInspector]
    public bool isInUsing = false;

    // TODO:鼠标右键点击到Item中间的空隙时会导致直接关闭UI
    void Update()
    {
        if (IsPointerEnter)
        {
            if (Input.GetMouseButton(1))
            {
                //Debug.Log("Right Click Item : " + this.transform.position);
                
                if (!isShowingTips && !isInUsing)
                {
                    isShowingTips = true;
                    if (ParentInfoPanelItems != null)
                    {
                        Vector3 vecTipsPos = this.transform.position;
                        // TODO:HardCode, 最后三个Item的Tips的位置
                        if (BtnIndex < 7)
                        {
                            vecTipsPos += new Vector3(-12, -40, 0);
                        }
                        else
                        {
                            vecTipsPos += new Vector3(-12, 180, 0);
                        }
                        ParentInfoPanelItems.ShowItemTips(true, vecTipsPos, ItemID);
                    }
                }
            }
            else if (Input.GetMouseButtonUp(1))
            {

                if (isShowingTips)
                {
                    isShowingTips = false;
                    if (ParentInfoPanelItems != null)
                    {
                        ParentInfoPanelItems.ShowItemTips(false, Vector3.zero);
                    }
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (ParentInfoPanelItems != null && ParentContentItems != null)
                {
                    ParentContentItems.SetCurrentInUseItemIndex(BtnIndex);
                    ParentInfoPanelItems.ShowComButtonWhenUseItem(true);

                    isInUsing = true;

                    Background.enabled = true;
                    TextName.color = Color.red;
                    TextCount.color = Color.red;
                }
            }
        }
    }

    public void UnUsingItem()
    {
        isInUsing = false;
        IsPointerEnter = false;
    }
}
