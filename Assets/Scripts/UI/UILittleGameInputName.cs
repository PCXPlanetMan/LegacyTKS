using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILittleGameInputName : MonoBehaviour
{
    public Button BtnCancel;
    public Button BtnOK;

    public InputField InputFirstName;
    public InputField InputLastName;

    public Image Portrait;

    // Start is called before the first frame update
    void Start()
    {
        BtnCancel.onClick.AddListener(OnClickButtonCancel);
        BtnOK.onClick.AddListener(OnClickButtonOK);
        InputFirstName.onValueChanged.AddListener(delegate
        {
            OnInputValueChanged(InputFirstName);
        });
        InputFirstName.ActivateInputField();
        InputFirstName.Select();

        var head = CharactersManager.Instance.ReadFullPortraitById(ResourceUtils.MAINROLE_ID);
        if (head != null)
        {
            Portrait.sprite = head;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnClickButtonCancel()
    {
        InputFirstName.text = "";
        InputLastName.text = "";
        InputFirstName.ActivateInputField();
        InputFirstName.Select();
    }

    private void OnClickButtonOK()
    {
        if (string.IsNullOrWhiteSpace(InputFirstName.text) || string.IsNullOrWhiteSpace(InputLastName.text))
        {
            return;
        }

        DocumentDataManager.Instance.UpdateMainRoleName(InputFirstName.text, InputLastName.text);
        ScenarioManager.Instance.ResumeCurrentTimeline();
        GameMainManager.Instance.UIRoot.ExitUIPanelContent(UIGameRootCanvas.UI_PANEL_CONTENT.None);
    }

    private void OnInputValueChanged(InputField input)
    {
        if (input == InputFirstName)
        {
            if (input.text.Length == input.characterLimit)
            {
                InputLastName.ActivateInputField();
                InputLastName.Select();
            }
        }
    }
}
