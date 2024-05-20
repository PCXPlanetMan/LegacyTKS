using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILittleGamePuzzleHuaRongDao : MonoBehaviour
{
    public Button BtnExit;

    public Button BtnDevSucc;

    private Action<bool> callback;

    // Start is called before the first frame update
    void Start()
    {
        BtnExit.onClick.AddListener(OnClickButtonExit);
        BtnDevSucc.onClick.AddListener(OnClickButtonDevSucc);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCallBackFunction(Action<bool> cb)
    {
        callback = cb;
    }

    private void OnClickButtonExit()
    {
        GameMainManager.Instance.UIRoot.ExitUIPanelContent(UIGameRootCanvas.UI_PANEL_CONTENT.None);
        if (callback != null)
        {
            callback(false);
        }
        callback = null;
    }

    private void OnClickButtonDevSucc()
    {
        GameMainManager.Instance.UIRoot.ExitUIPanelContent(UIGameRootCanvas.UI_PANEL_CONTENT.None);

        if (callback != null)
        {
            callback(true);
        }

        callback = null;
    }
}
