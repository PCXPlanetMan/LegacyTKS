using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIInfoPanelSystem : MonoBehaviour
{
    public Button BtnSave;
    public Button BtnLoad;
    public Button BtnExit;
    public Button BtnMusicOn;
    public Button BtnMusicOff;
    public Button BtnBGMOn;
    public Button BtnBGMOff;

    void Awake()
    {
        AddButtonOnClickListener();
    }

    void OnEnable()
    {
        UpdatePanelData();
    }

    private void UpdatePanelData()
    {
        int nOnOff = PlayerPrefs.GetInt(ResourceUtils.PREFS_SYSTEM_MUSIC_ONOFF);
        bool bMusic = false;
        if (nOnOff > 0)
        {
            bMusic = true;
        }
        nOnOff = PlayerPrefs.GetInt(ResourceUtils.PREFS_SYSTEM_BGM_ONOFF);
        bool bBGM = false;
        if (nOnOff > 0)
        {
            bBGM = true;
        }
        int nMusicVolume = PlayerPrefs.GetInt(ResourceUtils.PREFS_SYSTEM_MUSIC_VOLUME);
        int nBGMVolume = PlayerPrefs.GetInt(ResourceUtils.PREFS_SYSTEM_BGM_VOLUME);
        ShowSystemData(bMusic, bBGM, nMusicVolume, nBGMVolume);
    }

    private bool _bMusicOn;
    private bool _bBGMOn;
    private int _nMusic;
    private int _nBGM;
    private void ShowSystemData(bool bMusic, bool bBGM, int nMusic, int nBGM)
    {
        _bMusicOn = bMusic;
        _bBGMOn = bBGM;
        _nMusic = nMusic;
        _nBGM = nBGM;

        BtnMusicOn.GetComponent<UIBtnSimpleHighlightAnim>().ShowHighlighted(bMusic);
        BtnMusicOff.GetComponent<UIBtnSimpleHighlightAnim>().ShowHighlighted(!bMusic);
        BtnBGMOn.GetComponent<UIBtnSimpleHighlightAnim>().ShowHighlighted(bBGM);
        BtnBGMOff.GetComponent<UIBtnSimpleHighlightAnim>().ShowHighlighted(!bBGM);
    }

    private void AddButtonOnClickListener()
    {
        BtnSave.onClick.AddListener(OnClickSave);
        BtnLoad.onClick.AddListener(OnClickLoad);
        BtnExit.onClick.AddListener(OnClickExit);
        BtnMusicOn.onClick.AddListener(OnClickMusicOn);
        BtnMusicOff.onClick.AddListener(OnClickMusicOff);
        BtnBGMOn.onClick.AddListener(OnClickBGMOn);
        BtnBGMOff.onClick.AddListener(OnClickBGMOff);
    }

    private void OnClickSave()
    {
        GameMainManager.Instance.UIRoot.DisplayUIPanelContent(UIGameRootCanvas.UI_PANEL_CONTENT.Save);
    }

    private void OnClickLoad()
    {
        GameMainManager.Instance.UIRoot.DisplayUIPanelContent(UIGameRootCanvas.UI_PANEL_CONTENT.Load);
    }

    private void OnClickExit()
    {
        GameMainManager.Instance.UIRoot.DisplayUIPanelContent(UIGameRootCanvas.UI_PANEL_CONTENT.MainTitle);
        GameMainManager.Instance.EnableGameContent(false);
    }

    private void OnClickMusicOn()
    {
        if (!_bMusicOn)
        {
            BtnMusicOn.GetComponent<UIBtnSimpleHighlightAnim>().ShowHighlighted(true);
            BtnMusicOff.GetComponent<UIBtnSimpleHighlightAnim>().ShowHighlighted(false);
        }
        _bMusicOn = true;
        PlayerPrefs.SetInt(ResourceUtils.PREFS_SYSTEM_MUSIC_ONOFF, 1);
    }

    private void OnClickMusicOff()
    {
        if (_bMusicOn)
        {
            BtnMusicOn.GetComponent<UIBtnSimpleHighlightAnim>().ShowHighlighted(false);
            BtnMusicOff.GetComponent<UIBtnSimpleHighlightAnim>().ShowHighlighted(true);
        }
        _bMusicOn = false;
        PlayerPrefs.SetInt(ResourceUtils.PREFS_SYSTEM_MUSIC_ONOFF, 0);
    }

    private void OnClickBGMOn()
    {
        if (!_bBGMOn)
        {
            BtnBGMOn.GetComponent<UIBtnSimpleHighlightAnim>().ShowHighlighted(true);
            BtnBGMOff.GetComponent<UIBtnSimpleHighlightAnim>().ShowHighlighted(false);
        }
        _bBGMOn = true;
        PlayerPrefs.SetInt(ResourceUtils.PREFS_SYSTEM_BGM_ONOFF, 1);
    }

    private void OnClickBGMOff()
    {
        if (_bBGMOn)
        {
            BtnBGMOn.GetComponent<UIBtnSimpleHighlightAnim>().ShowHighlighted(false);
            BtnBGMOff.GetComponent<UIBtnSimpleHighlightAnim>().ShowHighlighted(true);
        }
        _bBGMOn = false;
        PlayerPrefs.SetInt(ResourceUtils.PREFS_SYSTEM_BGM_ONOFF, 0);
    }
}
