using System.Collections;
using System.Collections.Generic;
using com.tksr.document;
using com.tksr.schema;
using UnityEngine;

public class UIEquipsContentContainer : MonoBehaviour
{
    public List<UIBtnShowEquipInfo> EquipList;

    private int currentPageIndex = 0;
    private int maxPagesCount = 0;
    private readonly int EQUIPMENTS_IN_PAGE = 12;

    private int curInUseEquipIndex = -1;

    private void HideAllEquips()
    {
        foreach (var equip in EquipList)
        {
            equip.gameObject.SetActive(false);
        }
    }

    private List<GameItemInfo> currentEquips;
    public void UpdateEquipsContentData(EnumGameItemType type)
    {
        HideAllEquips();
        currentPageIndex = 0;

        currentEquips = DocumentDataManager.Instance.LoadItemsFormPackageByType(type);
        if (currentEquips != null)
        {
            if (currentEquips.Count == 0)
            {
                return;
            }

            maxPagesCount = Mathf.CeilToInt(currentEquips.Count * 1f / EQUIPMENTS_IN_PAGE);

            EnumGameItemType itemType;
            string name;
            for (int i = 0; i < EQUIPMENTS_IN_PAGE; i++)
            {
                int index = currentPageIndex * EQUIPMENTS_IN_PAGE + i;
                if (index < currentEquips.Count)
                {
                    var item = currentEquips[index];
                    var itemInfo = ItemsManager.Instance.GetTKRItemById(item.ItemId, out itemType, out name);
                    UIBtnShowEquipInfo uiEquipButton = EquipList[i];
                    uiEquipButton.TextName.text = name;
                    uiEquipButton.Icon.sprite = ItemsManager.Instance.ReadItemIconSpriteById(item.ItemId);
                    uiEquipButton.EquipID = item.ItemId;
                    uiEquipButton.BtnIndex = i;
                    uiEquipButton.gameObject.SetActive(true);
                }
            }
        }
    }

    public int GetCurrentPageIndex()
    {
        return currentPageIndex;
    }

    public int GetMaxPageCount()
    {
        return maxPagesCount;
    }

    public bool IsAnyButtonPointerEnter()
    {
        foreach (var equipButton in EquipList)
        {
            if (equipButton.gameObject.activeInHierarchy == true)
            {
                if (equipButton.IsPointerEnter)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool DoPageDown()
    {
        // TODO:装备界面的翻页逻辑
        return false;
    }

    public bool DoPageUp()
    {
        // TODO:装备界面的翻页逻辑
        return false;
    }

    public void SetCurrentInUseEquipIndex(int index)
    {
        curInUseEquipIndex = index;
    }

    public void ReleaseCurrentInUseEquip()
    {
        for (int i = 0; i < EquipList.Count; i++)
        {
            var equipBtn = EquipList[i];
            if (equipBtn != null)
            {
                equipBtn.UnUseEquip();
            }
        }
        currentPageIndex = -1;
    }

    public void DoUseCurrentSelectedEquip()
    {
        if (curInUseEquipIndex >= 0 && curInUseEquipIndex < EQUIPMENTS_IN_PAGE)
        {
            if (currentEquips != null && currentEquips.Count > 0)
            {
                int index = currentPageIndex * EQUIPMENTS_IN_PAGE + curInUseEquipIndex;
                if (index >= 0 && index < currentEquips.Count)
                {
                    var item = currentEquips[index];
                    if (item != null)
                    {
                        // TODO: Use a equip in package
                        Debug.Log("Use Item = " + item.ItemId);
                    }
                }
            }
        }
    }
}
