using com.tksr.data;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        Input.multiTouchEnabled = true;
        Input.simulateMouseWithTouches = true;
    }

    // Update is called once per frame
    void Update()
    {
        bool bLeftMouseClicked = Input.GetMouseButtonDown(0);
        bool bRightMouseClicked = Input.GetMouseButtonDown(1);

        var gameMode = GameMainManager.Instance.CurGameMode;
        switch (gameMode)
        {
            case EnumGameMode.ScenarioDialogMoment:
            case EnumGameMode.ScenarioStaticNote:
                {
                    if (bLeftMouseClicked)
                    {
                        ScenarioManager.Instance.ResumeCurrentTimeline();
                    }
                }
                break;
            case EnumGameMode.ScenarioDialogHasSelection:
            {
                    // 在Timeline中如果有Selection的对话,则应该由选项点击来决定接下来的流程
            }
                break;
            case EnumGameMode.Gameplay:
            {
                if (bLeftMouseClicked)
                {
                    CharactersManager.Instance.DriveMainRoleByInputPos(Input.mousePosition);
                }
                else if (bRightMouseClicked)
                {
                    // TODO: 打开Settings界面应该显示上一次停留的界面,目前简单处理为始终是"系统"界面
                    GameMainManager.Instance.UIRoot.DisplayUIPanelContent(UIGameRootCanvas.UI_PANEL_CONTENT.InfoSettings_System);
                    GameMainManager.Instance.CurGameMode = EnumGameMode.MainContentUI;
                }
            }
                break;
            case EnumGameMode.SceneTaskDialog:
            {
                if (bLeftMouseClicked)
                {
                    Debug.Log("SceneTaskDialog Left Clicked");
                    bool bHasAnyDialog = ScenarioManager.Instance.PopScenarioDialogs();
                    if (!bHasAnyDialog)
                    {
                        // 检测物品所有时,由于有后续对话,所以不应该退出
                        if (ScenarioManager.Instance.IsCheckingItemsOwner)
                        {
                            ScenarioManager.Instance.FinishPostTasks();
                            return;
                        }

                        ScenarioManager.Instance.EndDoingTaskDialogs();
                        CharactersManager.Instance.DriveMainRoleByInputPos(Input.mousePosition);
                    }
                }
            }
                break;
            case EnumGameMode.BlackMaskForDialog:
            {
                    // Do Nothing
            }
                break;
            case EnumGameMode.WaitingInputName:
                {

                }
                break;
            case EnumGameMode.InLittleGameUI:
            {

            }
                break;
            case EnumGameMode.MainContentUI:
            {
                if (bRightMouseClicked)
                {
                    var bExited = GameMainManager.Instance.UIRoot.RightClickToExitPanel();
                    if (bExited)
                    {
                        GameMainManager.Instance.CurGameMode = EnumGameMode.Gameplay;
                    }
                }
            }
                break;
            case EnumGameMode.Battle:
            {

            }
                break;
        }
    }
}
