using System;
using System.Collections;
using System.Collections.Generic;
using com.tksr.document;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class UIDocumentSlot : MonoBehaviour
{
    public Image Head1;
    public Image Head2;
    public Image Head3;
    public Image Head4;
    public Image Head5;
    public Image Head6;

    public Text Lv1;
    public Text Lv2;
    public Text Lv3;
    public Text Lv4;
    public Text Lv5;
    public Text Lv6;

    public Text Map;
    public Text TimeBig;
    public Text TimeSmall;
    public Text Gold;

    public Button BtnSaveLoad;
    public Button BtnCancel;

    [HideInInspector]
    public UISaveLoad ParentUI;

    // Start is called before the first frame update
    void Start()
    {
        AddBtnListener();
    }

    [HideInInspector] public int Index;

    [HideInInspector]
    public bool IsSaveMode { get; set; }

    private void AddBtnListener()
    {
        BtnSaveLoad.onClick.AddListener(OnClickSaveLoadButton);
        BtnCancel.onClick.AddListener(OnClickCancel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnClickSaveLoadButton()
    {
        if (IsSaveMode)
        {
            ParentUI.SaveDocumentInSlotByIndex(Index);
        }
        else
        {
            ParentUI.LoadDocumentFromSlotByIndex(Index);
        }
    }

    private void OnClickCancel()
    {
        GameMainManager.Instance.UIRoot.DisplayUIPanelContent(_lastUIPanel);
    }

    private UIGameRootCanvas.UI_PANEL_CONTENT _lastUIPanel = UIGameRootCanvas.UI_PANEL_CONTENT.None;
    public void SetLastUIPanelContent(UIGameRootCanvas.UI_PANEL_CONTENT content)
    {
        _lastUIPanel = content;
    }

    public void SetSaveOrLoad(bool save)
    {
        IsSaveMode = save;
        if (save)
        {
            BtnSaveLoad.GetComponent<Text>().text = ResourceUtils.FORMAT_DOC_SAVE;
        }
        else
        {
            BtnSaveLoad.GetComponent<Text>().text = ResourceUtils.FORMAT_DOC_LOAD;
        }
    }

    public void ClearContent()
    {
        Head1.sprite = null;
        Head2.sprite = null;
        Head3.sprite = null;
        Head4.sprite = null;
        Head5.sprite = null;
        Map.text = string.Empty;
        TimeBig.text = string.Empty;
        TimeSmall.text = string.Empty;
        Gold.text = string.Empty;

        Head1.gameObject.SetActive(false);
        Head2.gameObject.SetActive(false);
        Head3.gameObject.SetActive(false);
        Head4.gameObject.SetActive(false);
        Head5.gameObject.SetActive(false);
        Map.gameObject.SetActive(false);
        TimeBig.gameObject.SetActive(false);
        TimeSmall.gameObject.SetActive(false);
        Gold.gameObject.SetActive(false);
    }

    public void UpdateContent(GameDocument document)
    {
        var mainRoleInfo = document.MainRoleInfo;
        var sceneMap = SceneMapManager.Instance.GetSceneMapItemById(document.SceneId);
        if (sceneMap != null)
        {
            Map.gameObject.SetActive(true);
            Map.text = sceneMap.DisplayName;
        }

        Gold.gameObject.SetActive(true);
        Gold.text = document.Gold.ToString();

        var dateFormat = new DateTime(document.Timestamp);
        TimeBig.gameObject.SetActive(true);
        TimeBig.text = string.Format(ResourceUtils.FORMAT_DATE_YMD, dateFormat.Year, dateFormat.Month, dateFormat.Day);
        TimeSmall.gameObject.SetActive(true);
        TimeSmall.text = string.Format(ResourceUtils.FORMAT_DATE_HMS, dateFormat.Hour, dateFormat.Minute,
            dateFormat.Second);

        Head1.gameObject.SetActive(true);
        Lv1.text = string.Format(ResourceUtils.FORMAT_DOC_CHAR_LV, mainRoleInfo.Level);
        Head1.sprite = CharactersManager.Instance.ReadFullPortraitById(mainRoleInfo.CharId);

        if (document.Team == null || document.Team.Count == 0)
        {
            return;
        }

        if (document.Team.Count > 0)
        {
            Head2.gameObject.SetActive(true);
            GameCharInfo teamCharInfo = document.Candidates.Find(x => x.CharId == document.Team[0]);
            Lv2.text = string.Format(ResourceUtils.FORMAT_DOC_CHAR_LV, teamCharInfo.Level);
            Head2.sprite = CharactersManager.Instance.ReadFullPortraitById(teamCharInfo.CharId);
        }
        if (document.Team.Count > 1)
        {
            Head3.gameObject.SetActive(true);
            GameCharInfo teamCharInfo = document.Candidates.Find(x => x.CharId == document.Team[1]);
            Lv3.text = string.Format(ResourceUtils.FORMAT_DOC_CHAR_LV, teamCharInfo.Level);
            Head3.sprite = CharactersManager.Instance.ReadFullPortraitById(teamCharInfo.CharId);
        }
        if (document.Team.Count > 2)
        {
            Head4.gameObject.SetActive(true);
            GameCharInfo teamCharInfo = document.Candidates.Find(x => x.CharId == document.Team[2]);
            Lv4.text = string.Format(ResourceUtils.FORMAT_DOC_CHAR_LV, teamCharInfo.Level);
            Head4.sprite = CharactersManager.Instance.ReadFullPortraitById(teamCharInfo.CharId);
        }
        if (document.Team.Count > 3)
        {
            Head5.gameObject.SetActive(true);
            GameCharInfo teamCharInfo = document.Candidates.Find(x => x.CharId == document.Team[3]);
            Lv5.text = string.Format(ResourceUtils.FORMAT_DOC_CHAR_LV, teamCharInfo.Level);
            Head5.sprite = CharactersManager.Instance.ReadFullPortraitById(teamCharInfo.CharId);
        }
        if (document.Team.Count > 4)
        {
            Head6.gameObject.SetActive(true);
            GameCharInfo teamCharInfo = document.Candidates.Find(x => x.CharId == document.Team[4]);
            Lv6.text = string.Format(ResourceUtils.FORMAT_DOC_CHAR_LV, teamCharInfo.Level);
            Head6.sprite = CharactersManager.Instance.ReadFullPortraitById(teamCharInfo.CharId);
        }
    }
}
