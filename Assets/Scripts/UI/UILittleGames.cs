using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILittleGames : MonoBehaviour
{
    public UILittleGameInputName InputName;
    public UILittleGamePuzzleBaGua PuzzleBaGua;
    public UILittleGamePuzzleHuaRongDao PuzzleHuaRongDao;

    public enum UI_LITTLE_GAME_TYPE
    {
        Invalid,
        InputName,
        PuzzleBaGua,
        PuzzleHuaRongDao,
    }

    private UI_LITTLE_GAME_TYPE currentGameType = UI_LITTLE_GAME_TYPE.Invalid;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowLittleGame(UI_LITTLE_GAME_TYPE type, Action<bool> cb = null)
    {
        if (currentGameType == type)
        {
            return;
        }

        currentGameType = type;
        HideAllContents();
        switch(currentGameType)
        {
            case UI_LITTLE_GAME_TYPE.InputName:
                {
                    InputName.gameObject.SetActive(true);
                }
                break;
            case UI_LITTLE_GAME_TYPE.PuzzleBaGua:
            {
                    PuzzleBaGua.gameObject.SetActive(true);
                    PuzzleBaGua.SetCallBackFunction(cb);
            }
                break;
            case UI_LITTLE_GAME_TYPE.PuzzleHuaRongDao:
            {
                PuzzleHuaRongDao.gameObject.SetActive(true);
                PuzzleHuaRongDao.SetCallBackFunction(cb);
            }
                break;
        }
    }

    private void HideAllContents()
    {
        InputName.gameObject.SetActive(false);
        PuzzleBaGua.gameObject.SetActive(false);
    }
}
