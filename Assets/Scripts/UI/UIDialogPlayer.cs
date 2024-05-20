using System.Collections.Generic;
using com.tksr.property;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogPlayer : MonoBehaviour
{
    private IGameCharacterRenderer _attachedChar;
    private RectTransform _rectT;

    public RectTransform RectLeftArrow;
    public RectTransform RectRightArrow;
    public Text LeftContent;
    public Text RightContent;
    public RectTransform RectLeftMain;
    public RectTransform RectRightMain;
    public Text OSContent;
    public RectTransform RectOSMain;
    public RectTransform RectSelectionMain;

    public List<Text> SelectionsList;

    private RectTransform currentDlgRect;
    private RectTransform mainPanelRT;
    private Vector2 mainUISize;

    // Start is called before the first frame update
    void Start()
    {
        _rectT = this.gameObject.GetComponent<RectTransform>();
 
        mainPanelRT = GameMainManager.Instance.UIRoot.GetComponent<RectTransform>();
        CanvasScaler cas = mainPanelRT.GetComponent<CanvasScaler>();
        if (cas != null)
        {
            mainUISize = new Vector2(cas.referenceResolution.x, cas.referenceResolution.y);
        }
    }

    private void ResetAllContents()
    {
        RectLeftMain.gameObject.SetActive(false);
        RectLeftArrow.gameObject.SetActive(false);
        RectRightMain.gameObject.SetActive(false);
        RectRightArrow.gameObject.SetActive(false);
        RectOSMain.gameObject.SetActive(false);
        RectSelectionMain.gameObject.SetActive(false);
        RectOSMain.gameObject.GetComponent<Image>().enabled = true;
        currentDlgRect = null;
    }

    private void ShowLeftArrowOrRight(bool bLeft, string strContent)
    {
        ResetAllContents();

        strContent = ChangeEnterFlag(strContent);

        RectOSMain.gameObject.SetActive(false);
        if (bLeft)
        {
            RectLeftMain.gameObject.SetActive(true);
            RectLeftArrow.gameObject.SetActive(true);
            RectRightMain.gameObject.SetActive(false);
            RectRightArrow.gameObject.SetActive(false);
            LeftContent.text = strContent;
            currentDlgRect = RectLeftMain;
        }
        else
        {
            RectLeftMain.gameObject.SetActive(false);
            RectLeftArrow.gameObject.SetActive(false);
            RectRightMain.gameObject.SetActive(true);
            RectRightArrow.gameObject.SetActive(true);
            RightContent.text = strContent;
            currentDlgRect = RectRightMain;
        }
    }

    private void ShowOSContent(string strContent)
    {
        ResetAllContents();

        strContent = ChangeEnterFlag(strContent);
        RectLeftMain.gameObject.SetActive(false);
        RectLeftArrow.gameObject.SetActive(false);
        RectRightMain.gameObject.SetActive(false);
        RectRightArrow.gameObject.SetActive(false);
        RectOSMain.gameObject.SetActive(true);
        OSContent.text = strContent;
        currentDlgRect = RectOSMain;
    }

    private void ShowSighContent(string strContent)
    {
        ShowOSContent(strContent);
        RectOSMain.gameObject.GetComponent<Image>().enabled = false;
    }

    private void LateUpdate()
    {
        if (_attachedChar != null)
        {
            Vector3 vecCharPos = _attachedChar.CalcUIDialogWorldPos();
            Vector3 screenPos = Camera.main.WorldToScreenPoint(vecCharPos);
            _rectT.position = screenPos;

            // 限制对话框始终在屏幕之内(暂时没有考虑父子之间的缩放关系)
            if (currentDlgRect != null)
            {
                Vector2 dlgScreenPos = Camera.main.WorldToScreenPoint(currentDlgRect.transform.position);
                Vector2 dlgAnchoredPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(mainPanelRT, dlgScreenPos, Camera.main, out dlgAnchoredPos);

                float leftPosX = dlgAnchoredPos.x - currentDlgRect.sizeDelta.x * currentDlgRect.pivot.x;
                float rightPosX = dlgAnchoredPos.x + currentDlgRect.sizeDelta.x * (1f - currentDlgRect.pivot.x);

                float offsetX = 0f;
                if (leftPosX < -mainUISize.x / 2)
                {
                    offsetX = Mathf.Abs(-mainUISize.x / 2 - leftPosX);
                }
                else if (rightPosX > mainUISize.x / 2)
                {
                    offsetX = mainUISize.x / 2 - rightPosX;
                }

                float topPosY = dlgAnchoredPos.y + currentDlgRect.sizeDelta.y * (1f - currentDlgRect.pivot.y);
                float bottomPosY = dlgAnchoredPos.y - currentDlgRect.sizeDelta.y * currentDlgRect.pivot.y;

                float offsetY = 0f;
                if (bottomPosY < -mainUISize.y / 2)
                {
                    offsetY = Mathf.Abs(-mainUISize.y / 2 - bottomPosY);
                }
                else if (topPosY > mainUISize.y / 2)
                {
                    offsetY = mainUISize.y / 2 - topPosY;
                }

                screenPos += new Vector3(offsetX, offsetY, 0f);

                Camera.main.WorldToScreenPoint(vecCharPos);
                // 当对话框超出屏幕被强制规范的时候,让箭头显示在合适的位置
                if (RectLeftArrow.gameObject.activeInHierarchy)
                {
                    Vector3 oldLeft = RectLeftArrow.localPosition;
                    oldLeft.x = -offsetX;
                    RectLeftArrow.transform.localPosition = oldLeft;
                }
                if (RectRightArrow.gameObject.activeInHierarchy)
                {
                    Vector3 oldRight = RectRightArrow.localPosition;
                    oldRight.x = -offsetX;
                    RectRightArrow.transform.localPosition = oldRight;
                }
                _rectT.position = screenPos;
            }

        }
    }

    public enum EnumUIDialogShowMode
    {
        NormalByOrientation = -1,
        NormalLeftArrow,
        NormalRightArrow,
        ShowOS,
        ShowSelections,
        ShowSigh,
    }

    public IGameCharacterRenderer CurrentAttachedChar()
    {
        return _attachedChar;
    }

    public void AttachChar(IGameCharacterRenderer igcr, string strContent, EnumUIDialogShowMode showSideMode = EnumUIDialogShowMode.NormalByOrientation)
    {
        if (_attachedChar != igcr)
            _attachedChar = igcr;

        if (showSideMode == EnumUIDialogShowMode.NormalLeftArrow) // Left Arrow in Story
        {
            ShowLeftArrowOrRight(true, strContent);
            _attachedChar.ParseDialogPosParamNormalDisplay(true);
        }
        else if (showSideMode == EnumUIDialogShowMode.NormalRightArrow) // Right Arrow in Story
        {
  
            ShowLeftArrowOrRight(false, strContent);
            _attachedChar.ParseDialogPosParamNormalDisplay(false);
        }
        else if (showSideMode == EnumUIDialogShowMode.ShowOS)
        {
            ShowOSContent(strContent);
            _attachedChar.ParseDialogPosParamOSDisplay();
        }
        else if (showSideMode == EnumUIDialogShowMode.ShowSigh)
        {
            ShowSighContent(strContent);
            _attachedChar.ParseDialogPosParamSighDisplay();
        }
        else if (showSideMode == EnumUIDialogShowMode.NormalByOrientation)
        {
            // By Char current orientation

            bool bShowLeftArrow = false;
            EnumOrientation orient = igcr.CharAnimRender.GetCharOrientation();
            if (orient == EnumOrientation.Left)
            {
                bShowLeftArrow = true;
            }
            else if (orient == EnumOrientation.Right)
            {
                bShowLeftArrow = false;
            }
            ShowLeftArrowOrRight(bShowLeftArrow, strContent);
            _attachedChar.ParseDialogPosParamNormalDisplay(bShowLeftArrow);
        }
        else if (showSideMode == EnumUIDialogShowMode.ShowSelections)
        {
            var listSelections = SchemaParser.ParseUISelectionToList(strContent);
            if (listSelections.Count > SelectionsList.Count)
            {
                Debug.LogError("Actual selection strings count is larger than UI text count.");
            }
            else
            {
                ShowSelectionsUI(listSelections);
            }
            _attachedChar.ParseDialogPosParamSelectionDisplay();
        }
    }

    private void ShowSelectionsUI(List<string> selections)
    {
        ResetAllContents();

        foreach (var sel in SelectionsList)
        {
            sel.gameObject.SetActive(false);
        }

        for (int i = 0; i < selections.Count; i++)
        {
            var strSelection = selections[i];
            SelectionsList[i].gameObject.SetActive(true);
            strSelection = ChangeEnterFlag(strSelection);
            SelectionsList[i].text = strSelection;
        }

        RectSelectionMain.gameObject.SetActive(true);
        currentDlgRect = RectSelectionMain;
    }

    private string ChangeEnterFlag(string strOri)
    {
        strOri = strOri.Replace("\\n", "\n");
        return strOri;
    }

    public void DetachChar()
    {
        _attachedChar = null;
        LeftContent.text = string.Empty;
        RightContent.text = string.Empty;
        OSContent.text = string.Empty;
    }

    /// <summary>
    /// 当选择对话框某个选项点击时,应该驱动接下来的对话流程(继续或者终止)
    /// 此回调晚于InputManager中LeftClicked
    /// </summary>
    /// <param name="index"></param>
    public void OnBtnSelectionClicked(int index)
    {
        Debug.Log("Selection index = " + index);
        //ScenarioManager.Instance.GoFlowAfterDialogSelectionItemClicked(index);
        ScenarioManager.Instance.OnDialogItemSelectedDoTask(index);
    }
}
