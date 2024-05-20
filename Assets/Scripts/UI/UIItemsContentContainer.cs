using System.Collections;
using System.Collections.Generic;
using com.tksr.document;
using com.tksr.schema;
using UnityEngine;

public class UIItemsContentContainer : MonoBehaviour
{
    public List<UIBtnShowItemInfo> ItemList;

    private EnumGameItemType currentItemType;
    private int currentPageIndex = 0;
    private int maxPagesCount = 0;
    private readonly int ITEMS_IN_PAGE = 10;
    private int curInUseItemIndex = -1;

    private void HideAllItems()
    {
        foreach (var item in ItemList)
        {
            item.gameObject.SetActive(false);
        }
    }

    private List<GameItemInfo> currentItems;
    public void UpdateItemsContentData(EnumGameItemType type)
    {
        HideAllItems();
        currentPageIndex = 0;

        currentItems = DocumentDataManager.Instance.LoadItemsFormPackageByType(type);
        if (currentItems != null)
        {
            if (currentItems.Count == 0)
            {
                return;
            }

            maxPagesCount = Mathf.CeilToInt(currentItems.Count * 1f / ITEMS_IN_PAGE);
            

            EnumGameItemType itemType;
            string name;
            for (int i = 0; i < ITEMS_IN_PAGE; i++)
            {
                int index = currentPageIndex * ITEMS_IN_PAGE + i;
                if (index < currentItems.Count)
                {
                    var item = currentItems[index];
                    var itemInfo = ItemsManager.Instance.GetTKRItemById(item.ItemId, out itemType, out name);
                    UIBtnShowItemInfo uiItemButton = ItemList[i];
                    uiItemButton.TextName.text = name;
                    uiItemButton.TextCount.text = item.Count.ToString();
                    uiItemButton.Icon.sprite = ItemsManager.Instance.ReadItemIconSpriteById(item.ItemId);
                    uiItemButton.ItemID = item.ItemId;
                    uiItemButton.BtnIndex = i;
                    uiItemButton.gameObject.SetActive(true);
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
        foreach (var itemButton in ItemList)
        {
            if (itemButton.gameObject.activeInHierarchy == true)
            {
                if (itemButton.IsPointerEnter || itemButton.isInUsing)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public bool DoPageDown()
    {
        // TODO:物品界面的翻页逻辑
        return false;
    }

    public bool DoPageUp()
    {
        // TODO:物品界面的翻页逻辑
        return false;
    }

    public void SetCurrentInUseItemIndex(int index)
    {
        curInUseItemIndex = index;
    }

    public void ReleaseCurrentInUseItem()
    {
        for (int i = 0; i < ItemList.Count; i++)
        {
            var itemBtn = ItemList[i];
            if (itemBtn != null)
            {
                itemBtn.UnUsingItem();
            }
        }
        currentPageIndex = -1;
    }

    public void DoUseCurrentSelectedItem()
    {
        if (curInUseItemIndex >= 0 && curInUseItemIndex < ITEMS_IN_PAGE)
        {
            if (currentItems != null && currentItems.Count > 0)
            {
                int index = currentPageIndex * ITEMS_IN_PAGE + curInUseItemIndex;
                if (index >= 0 && index < currentItems.Count)
                {
                    var item = currentItems[index];
                    if (item != null)
                    {
                        // TODO: Use a item in package
                        Debug.Log("Use Item = " + item.ItemId);
                    }
                }
            }
        }
    }
}
