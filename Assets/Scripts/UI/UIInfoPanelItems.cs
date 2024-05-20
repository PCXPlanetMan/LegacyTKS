using com.tksr.schema;
using UnityEngine;
using UnityEngine.UI;

public class UIInfoPanelItems : MonoBehaviour
{
    private EnumGameItemType curPanelItemType = EnumGameItemType.Medic;

    public Button BtnMedicTab;
    public Button BtnPropTab;
    public Button BtnWeaponTab;
    public Button BtnArmorTab;
    public Button BtnAccessoryTab;
    public Button BtnSpecialTab;

    public UIItemsContentContainer ItemsContentContainer;

    public Image Tips;
    public Text TipOfItem;

    public Button BtnPageUp;
    public Button BtnPageDown;

    public Transform ComBtnsPanel;

    void Awake()
    {
        Tips.gameObject.SetActive(false);
        BtnPageDown.onClick.AddListener(OnClickBtnPageDown);
        BtnPageUp.onClick.AddListener(OnClickBtnPageUp);
    }

    void OnEnable()
    {
        UpdatePanelData();
    }

    private void UpdatePanelData()
    {
        ShowTabContents(curPanelItemType);
    }

    public void OnClickTabButton(int index)
    {
        EnumGameItemType type = (EnumGameItemType) index;
        ShowTabContents(type);
    }

    private void DisableAllTabButtons()
    {
        Color hideColor = Color.white;
        hideColor.a = 0f;
        BtnMedicTab.GetComponent<Image>().color = hideColor;
        BtnPropTab.GetComponent<Image>().color = hideColor;
        BtnWeaponTab.GetComponent<Image>().color = hideColor;
        BtnArmorTab.GetComponent<Image>().color = hideColor;
        BtnAccessoryTab.GetComponent<Image>().color = hideColor;
        BtnSpecialTab.GetComponent<Image>().color = hideColor;

        BtnPageUp.gameObject.SetActive(false);
        BtnPageDown.gameObject.SetActive(false);
    }

    private void ShowTabContents(EnumGameItemType type)
    {
        DisableAllTabButtons();

        curPanelItemType = type;

        switch (curPanelItemType)
        {
            case EnumGameItemType.Medic:
                {
                    BtnMedicTab.GetComponent<Image>().color = Color.white;
                }
                break;
            case EnumGameItemType.Prop:
                {
                    BtnPropTab.GetComponent<Image>().color = Color.white;
                }
                break;
            case EnumGameItemType.Weapon:
            {
                BtnWeaponTab.GetComponent<Image>().color = Color.white;
            }
                break;
            case EnumGameItemType.Armor:
            {
                BtnArmorTab.GetComponent<Image>().color = Color.white;
            }
                break;
            case EnumGameItemType.Accessory:
            {
                BtnAccessoryTab.GetComponent<Image>().color = Color.white;
            }
                break;
            case EnumGameItemType.Special:
            {
                BtnSpecialTab.GetComponent<Image>().color = Color.white;
            }
                break;
        }

        ItemsContentContainer.UpdateItemsContentData(curPanelItemType);
        UpdatePageButtons(ItemsContentContainer.GetCurrentPageIndex(), ItemsContentContainer.GetMaxPageCount());
    }

    private void UpdatePageButtons(int currentPageIndex, int maxPagesCount)
    {
        ShowPageDownButton(false);
        ShowPageUpButton(false);
        if (maxPagesCount > 1)
        {
            if (currentPageIndex == 0)
            {
                ShowPageDownButton(true);
            }
            else if (currentPageIndex == maxPagesCount - 1)
            {
                ShowPageUpButton(true);
            }
            else
            {
                ShowPageDownButton(true);
                ShowPageUpButton(true);
            }
        }
    }

    public bool IsRightMouseOnItemButton()
    {
        return ItemsContentContainer.IsAnyButtonPointerEnter();
    }

    public void ShowItemTips(bool bShow, Vector3 vecTipsPos, int itemID = 0)
    {
        if (Tips.gameObject.activeInHierarchy != bShow)
        {
            Tips.gameObject.SetActive(bShow);
            if (bShow)
            {
                Tips.transform.position = vecTipsPos;
                EnumGameItemType type;
                string name;
                var item = ItemsManager.Instance.GetTKRItemById(itemID, out type, out name);
                if (item != null)
                {
                    TipOfItem.text = item.Description;
                }
            }
            else
            {
                TipOfItem.text = string.Empty;
            }
        }
    }

    private void ShowPageUpButton(bool bShow)
    {
        BtnPageUp.gameObject.SetActive(bShow);
    }

    private void ShowPageDownButton(bool bShow)
    {
        BtnPageDown.gameObject.SetActive(bShow);
    }

    private void OnClickBtnPageDown()
    {
        bool bRes = ItemsContentContainer.DoPageDown();
        if (bRes)
        {
            UpdatePageButtons(ItemsContentContainer.GetCurrentPageIndex(), ItemsContentContainer.GetMaxPageCount());
        }
    }

    private void OnClickBtnPageUp()
    {
        bool bRes = ItemsContentContainer.DoPageUp();
        if (bRes)
        {
            UpdatePageButtons(ItemsContentContainer.GetCurrentPageIndex(), ItemsContentContainer.GetMaxPageCount());
        }
    }

    public void ShowComButtonWhenUseItem(bool bShow)
    {
        ComBtnsPanel.gameObject.SetActive(bShow);
        if (!bShow)
        {
            ItemsContentContainer.ReleaseCurrentInUseItem();
        }
    }

    public void UseCurrentSelectedItem()
    {
        ItemsContentContainer.DoUseCurrentSelectedItem();
    }
}
