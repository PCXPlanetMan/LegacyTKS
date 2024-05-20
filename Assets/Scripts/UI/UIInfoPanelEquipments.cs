using System.Collections;
using System.Collections.Generic;
using com.tksr.schema;
using UnityEngine;
using UnityEngine.UI;

public class UIInfoPanelEquipments : MonoBehaviour
{
    private EnumGameItemType curPanelEquipType = EnumGameItemType.Weapon;

    public Button BtnWeaponTab;
    public Button BtnArmorTab;
    public Button BtnAccessoryTab;
    public Button BtnPageUp;
    public Button BtnPageDown;
    public UIEquipsContentContainer EquipsContentContainer;
    public Image Tips;
    public Text TipOfItem;
    public Transform ComBtnsPanel;

    public Text Weapon;
    public Text Armor;
    public Text Accessory1;
    public Text Accessory2;


    #region Show Equipment Propery
    public Text Attack;
    public Text Defense;
    public Text Hit;
    public Text Dodge;
    public Text Speed;
    public Text Luck;
    public Text Understanding;
    public Text Move;
    public Image UpAttack;
    public Image DownAttack;
    public Image UpDefense;
    public Image DownDefense;
    public Image UpHit;
    public Image DownHit;
    public Image UpDodge;
    public Image DownDodge;
    public Image UpSpeed;
    public Image DownSpeed;
    public Image UpLuck;
    public Image DownLuck;
    public Image UpUnderstanding;
    public Image DownUnderstanding;
    public Image UpMove;
    public Image DownMove;
    #endregion


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
        ShowTabContents(curPanelEquipType);
    }

    public void OnClickTabButton(int index)
    {
        EnumGameItemType type = (EnumGameItemType)index;
        ShowTabContents(type);
    }

    private void DisableAllTabButtons()
    {
        Color hideColor = Color.white;
        hideColor.a = 0f;
        BtnWeaponTab.GetComponent<Image>().color = hideColor;
        BtnArmorTab.GetComponent<Image>().color = hideColor;
        BtnAccessoryTab.GetComponent<Image>().color = hideColor;

        BtnPageUp.gameObject.SetActive(false);
        BtnPageDown.gameObject.SetActive(false);
    }

    private void ShowTabContents(EnumGameItemType type)
    {
        DisableAllTabButtons();

        curPanelEquipType = type;

        switch (curPanelEquipType)
        {
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
        }

        if (curPanelEquipType != EnumGameItemType.Weapon && curPanelEquipType != EnumGameItemType.Armor &&
            curPanelEquipType != EnumGameItemType.Accessory)
        {
            Debug.LogError("Current type is not a Equipment");
            return;
        }

        EquipsContentContainer.UpdateEquipsContentData(curPanelEquipType);
        UpdatePageButtons(EquipsContentContainer.GetCurrentPageIndex(), EquipsContentContainer.GetMaxPageCount());
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

    public bool IsRightMouseOnEquipButton()
    {
        return EquipsContentContainer.IsAnyButtonPointerEnter();
    }

    public void ShowEquipTips(bool bShow, Vector3 vecTipsPos, int itemID = 0)
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
        bool bRes = EquipsContentContainer.DoPageDown();
        if (bRes)
        {
            UpdatePageButtons(EquipsContentContainer.GetCurrentPageIndex(), EquipsContentContainer.GetMaxPageCount());
        }
    }

    private void OnClickBtnPageUp()
    {
        bool bRes = EquipsContentContainer.DoPageUp();
        if (bRes)
        {
            UpdatePageButtons(EquipsContentContainer.GetCurrentPageIndex(), EquipsContentContainer.GetMaxPageCount());
        }
    }

    public void ShowComButtonWhenUseEquip(bool bShow)
    {
        ComBtnsPanel.gameObject.SetActive(bShow);
        if (!bShow)
        {
            EquipsContentContainer.ReleaseCurrentInUseEquip();
        }
    }

    public void UseCurrentSelectedEquip()
    {
        EquipsContentContainer.DoUseCurrentSelectedEquip();
    }

    private int curShowWeaponId;
    private int curShowArmorId;
    private int curShowAccessory1Id;
    private int curShowAccessory2Id;

    private TKRItemWeapon curCharEquipWeapon;
    private TKRItemArmor curCharEquipArmor;
    private TKRItemAccessory curCharEquipAccessory1;
    private TKRItemAccessory curCharEquipAccessory2;

    public void UpdateEquipmentData(DataCharStatsInfo info)
    {
        curShowWeaponId = info.Weapon;
        curShowArmorId = info.Armor;
        curShowAccessory1Id = info.Accessory1;
        curShowAccessory2Id = info.Accessory2;

        if (curShowWeaponId == 0)
        {
            Weapon.text = string.Empty;
            curCharEquipWeapon = null;
        }
        else
        {
            EnumGameItemType type;
            string name;
            curCharEquipWeapon = (TKRItemWeapon)ItemsManager.Instance.GetTKRItemById(curShowWeaponId, out type, out name);
            Weapon.text = name;
        }

        if (curShowArmorId == 0)
        {
            Armor.text = string.Empty;
            curCharEquipArmor = null;
        }
        else
        {
            EnumGameItemType type;
            string name;
            curCharEquipArmor = (TKRItemArmor)ItemsManager.Instance.GetTKRItemById(curShowArmorId, out type, out name);
            Armor.text = name;
        }

        if (curShowAccessory1Id == 0)
        {
            Accessory1.text = string.Empty;
            curCharEquipAccessory1 = null;
        }
        else
        {
            EnumGameItemType type;
            string name;
            curCharEquipAccessory1 = (TKRItemAccessory)ItemsManager.Instance.GetTKRItemById(curShowAccessory1Id, out type, out name);
            Accessory1.text = name;
        }

        if (curShowAccessory2Id == 0)
        {
            Accessory2.text = string.Empty;
            curCharEquipAccessory2 = null;
        }
        else
        {
            EnumGameItemType type;
            string name;
            curCharEquipAccessory2 = (TKRItemAccessory)ItemsManager.Instance.GetTKRItemById(curShowAccessory2Id, out type, out name);
            Accessory2.text = name;
        }
    }

    public void ShowEquipmentDiff(int equipId)
    {
        ReInitEquipmentValue();

        if (equipId == 0)
        {
            return;
        }

        EnumGameItemType type;
        string name;
        var preparedEquip = ItemsManager.Instance.GetTKRItemById(equipId, out type, out name);
        if (preparedEquip == null)
            return;

        int diffAttack = 0;
        int diffDefense = 0;
        int diffHit = 0;
        int diffDodge = 0;
        int diffSpeed = 0;
        int diffLuck = 0;
        int diffUnderstanding = 0;
        int diffMove = 0;
        if (type == EnumGameItemType.Weapon)
        {
            TKRItemWeapon preparedWeapon = (TKRItemWeapon) preparedEquip;
            if (curCharEquipWeapon != null)
            {
                diffAttack = curCharEquipWeapon.IncATK;
                diffHit = curCharEquipWeapon.IncHIT;
            }

            diffAttack = preparedWeapon.IncATK - diffAttack;
            diffHit = preparedWeapon.IncHIT - diffHit;
        }
        else if (type == EnumGameItemType.Armor)
        {
            TKRItemArmor preparedArmor = (TKRItemArmor)preparedEquip;
            if (curCharEquipArmor != null)
            {
                diffDefense = curCharEquipArmor.IncDEF;
                diffDodge = curCharEquipArmor.IncEVD;
            }

            diffDefense = preparedArmor.IncDEF - diffDefense;
            diffDodge = preparedArmor.IncEVD - diffDodge;
        }
        else if (type == EnumGameItemType.Accessory)
        {
            // TODO:需要确定具体是对比哪个配件

            TKRItemAccessory preparedAccessory = (TKRItemAccessory)preparedEquip;
            if (curCharEquipAccessory1 != null)
            {
                diffAttack = curCharEquipAccessory1.IncATK;
                diffDefense = curCharEquipAccessory1.IncDEF;
                diffHit = curCharEquipAccessory1.IncHIT;
                diffDodge = curCharEquipAccessory1.IncEVD;
                diffSpeed = curCharEquipAccessory1.IncSPD;
                diffUnderstanding = curCharEquipAccessory1.IncUSD;
                diffLuck = curCharEquipAccessory1.IncLuck;
                diffMove = curCharEquipAccessory1.IncMOV;
            }

            diffAttack = preparedAccessory.IncATK - diffAttack;
            diffDefense = preparedAccessory.IncDEF - diffDefense;
            diffHit = preparedAccessory.IncHIT - diffHit;
            diffDodge = preparedAccessory.IncEVD - diffDodge;
            diffSpeed = preparedAccessory.IncSPD - diffSpeed;
            diffUnderstanding = preparedAccessory.IncUSD - diffUnderstanding;
            diffLuck = preparedAccessory.IncLuck - diffLuck;
            diffMove = preparedAccessory.IncMOV - diffMove;
        }

        ShowEquipDiffAttack(diffAttack);
        ShowEquipDiffDefense(diffDefense);
        ShowEquipDiffHit(diffHit);
        ShowEquipDiffDodge(diffDodge);
        ShowEquipDiffSpeed(diffSpeed);
        ShowEquipDiffUnderstanding(diffUnderstanding);
        ShowEquipDiffLuck(diffLuck);
        ShowEquipDiffMove(diffMove);
    }

    private void ReInitEquipmentValue()
    {
        string strZero = "0";
        Attack.text = strZero;
        Defense.text = strZero;
        Hit.text = strZero;
        Dodge.text = strZero;
        Speed.text = strZero;
        Luck.text = strZero;
        Understanding.text = strZero;
        Move.text = strZero;
        UpAttack.gameObject.SetActive(false);
        DownAttack.gameObject.SetActive(false);
        UpDefense.gameObject.SetActive(false);
        DownDefense.gameObject.SetActive(false);
        UpHit.gameObject.SetActive(false);
        DownHit.gameObject.SetActive(false);
        UpDodge.gameObject.SetActive(false);
        DownDodge.gameObject.SetActive(false);
        UpSpeed.gameObject.SetActive(false);
        DownSpeed.gameObject.SetActive(false);
        UpLuck.gameObject.SetActive(false);
        DownLuck.gameObject.SetActive(false);
        UpUnderstanding.gameObject.SetActive(false);
        DownUnderstanding.gameObject.SetActive(false);
        UpMove.gameObject.SetActive(false);
        DownMove.gameObject.SetActive(false);
    }

    private void ShowEquipDiffAttack(int value)
    {
        Attack.text = value.ToString();
        if (value > 0)
        {
            UpAttack.gameObject.SetActive(true);
        }
        else if (value < 0)
        {
            DownAttack.gameObject.SetActive(true);
        }
    }

    private void ShowEquipDiffDefense(int value)
    {
        Defense.text = value.ToString();
        if (value > 0)
        {
            UpDefense.gameObject.SetActive(true);
        }
        else if (value < 0)
        {
            DownDefense.gameObject.SetActive(true);
        }
    }

    private void ShowEquipDiffHit(int value)
    {
        Hit.text = value.ToString();
        if (value > 0)
        {
            UpHit.gameObject.SetActive(true);
        }
        else if (value < 0)
        {
            DownHit.gameObject.SetActive(true);
        }
    }

    private void ShowEquipDiffDodge(int value)
    {
        Dodge.text = value.ToString();
        if (value > 0)
        {
            UpDodge.gameObject.SetActive(true);
        }
        else if (value < 0)
        {
            DownDodge.gameObject.SetActive(true);
        }
    }

    private void ShowEquipDiffSpeed(int value)
    {
        Speed.text = value.ToString();
        if (value > 0)
        {
            UpSpeed.gameObject.SetActive(true);
        }
        else if (value < 0)
        {
            DownSpeed.gameObject.SetActive(true);
        }
    }

    private void ShowEquipDiffLuck(int value)
    {
        Luck.text = value.ToString();
        if (value > 0)
        {
            UpLuck.gameObject.SetActive(true);
        }
        else if (value < 0)
        {
            DownLuck.gameObject.SetActive(true);
        }
    }

    private void ShowEquipDiffUnderstanding(int value)
    {
        Understanding.text = value.ToString();
        if (value > 0)
        {
            UpUnderstanding.gameObject.SetActive(true);
        }
        else if (value < 0)
        {
            DownUnderstanding.gameObject.SetActive(true);
        }
    }

    private void ShowEquipDiffMove(int value)
    {
        Move.text = value.ToString();
        if (value > 0)
        {
            UpMove.gameObject.SetActive(true);
        }
        else if (value < 0)
        {
            DownMove.gameObject.SetActive(true);
        }
    }
}
