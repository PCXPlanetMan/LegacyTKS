using com.tksr.document;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISaveLoad : MonoBehaviour
{
    public Button BtnPageUp;
    public Button BtnPageDown;
    public Text TxtPageInfo;

    private int _curPageIndex = 0;
    private readonly int maxPages = 3;

    private readonly string pageStringFormat = "第{0}页 共{1}页";

    public UIDocumentSlot Slot1;
    public UIDocumentSlot Slot2;
    public UIDocumentSlot Slot3;

    public enum DocumentStatus
    {
        Save,
        Load
    }

    private DocumentStatus _currentDocStatus = DocumentStatus.Load;

    [HideInInspector]
    public UIGameRootCanvas.UI_PANEL_CONTENT FromPanelContent = UIGameRootCanvas.UI_PANEL_CONTENT.None;

    // Start is called before the first frame update
    void Start()
    {
        BtnPageUp.onClick.AddListener(OnClickPageUp);
        BtnPageDown.onClick.AddListener(OnClickPageDown);
        Slot1.Index = 1;
        Slot2.Index = 2;
        Slot3.Index = 3;
        Slot1.ParentUI = this;
        Slot2.ParentUI = this;
        Slot3.ParentUI = this;
        UpdatePageStatus();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnClickPageUp()
    {
        DoPageUp();
    }

    private void OnClickPageDown()
    {
        DoPageDown();
    }

    private void UpdatePageStatus()
    {
        TxtPageInfo.text = string.Format(ResourceUtils.FORMAT_UI_ALL_PAGES, _curPageIndex + 1, maxPages);
        if (_curPageIndex == 0)
        {
            BtnPageUp.interactable = false;
            BtnPageDown.interactable = true;
        }
        else if (_curPageIndex == maxPages - 1)
        {
            BtnPageUp.interactable = true;
            BtnPageDown.interactable = false;
        }
        else
        {
            BtnPageUp.interactable = true;
            BtnPageDown.interactable = true;
        }

        Slot1.SetLastUIPanelContent(FromPanelContent);
        Slot2.SetLastUIPanelContent(FromPanelContent);
        Slot3.SetLastUIPanelContent(FromPanelContent);

        UpdateDocumentStatus(_currentDocStatus);
    }

    private void DoPageUp()
    {
        if (_curPageIndex == 0)
        {
            return;
        }

        _curPageIndex--;

        UpdatePageStatus();
    }

    private void DoPageDown()
    {
        if (_curPageIndex == maxPages - 1)
        {
            return;
        }

        _curPageIndex++;

        UpdatePageStatus();
    }

    public void UpdateDocumentStatus(DocumentStatus status)
    {
        _currentDocStatus = status;
        var archives = DocumentDataManager.Instance.LoadTKSArchives();
        if (archives != null)
        {
            int itemIndex = _curPageIndex * 3 + 1;
            if (itemIndex < archives.Documents.Count)
            {
                var document = archives.Documents[itemIndex];
                UpdateSlot(Slot1, document);
            }
            else
            {
                UpdateSlot(Slot1, null);
            }
            itemIndex = _curPageIndex * 3 + 2;
            if (itemIndex < archives.Documents.Count)
            {
                var document = archives.Documents[itemIndex];
                UpdateSlot(Slot2, document);
            }
            else
            {
                UpdateSlot(Slot2, null);
            }
            itemIndex = _curPageIndex * 3 + 3;
            if (itemIndex < archives.Documents.Count)
            {
                var document = archives.Documents[itemIndex];
                UpdateSlot(Slot3, document);
            }
            else
            {
                UpdateSlot(Slot3, null);
            }
        }

        if (_currentDocStatus == DocumentStatus.Save)
        {
            Slot1.SetSaveOrLoad(true);
            Slot2.SetSaveOrLoad(true);
            Slot3.SetSaveOrLoad(true);
        }
        else
        {
            Slot1.SetSaveOrLoad(false);
            Slot2.SetSaveOrLoad(false);
            Slot3.SetSaveOrLoad(false);
        }
    }

    private void UpdateSlot(UIDocumentSlot slot, GameDocument doc)
    {
        if (doc == null || doc.DocumentId == -1)
        {
            slot.ClearContent();
        }
        else
        {
            slot.UpdateContent(doc);
        }
    }

    public void SaveDocumentInSlotByIndex(int index)
    {
        var documentIndex = _curPageIndex * 3 + index;
        DocumentDataManager.Instance.SaveGameDocument(documentIndex);
        UpdateDocumentStatus(_currentDocStatus);
    }

    public void LoadDocumentFromSlotByIndex(int index)
    {
        var documentIndex = _curPageIndex * 3 + index;
        bool bSucc = DocumentDataManager.Instance.LoadGameDocument(documentIndex);
        // TODO: 根据加载文档进入正确的场景
        if (bSucc)
        {
            GameMainManager.Instance.UIRoot.ExitUIPanelContent();
            var document = DocumentDataManager.Instance.GetCurrentDocument();
            GameMainManager.Instance.NextSceneId = document.SceneId;
            GameMainManager.Instance.NextEntryId = 0;
            GameMainManager.Instance.SwitchToNextScene(true);
        }
    }
}
