using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleEndAnimPanel : MonoBehaviour
{
    public Image WordWin1;
    public Image WordWin2;
    public Image WordLose1;
    public Image WordLose2;

    private readonly float ANIM_DURATION = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        DisplayAllText(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void DisplayAllText(bool bShow)
    {
        WordWin1.gameObject.SetActive(bShow);
        WordWin2.gameObject.SetActive(bShow);
        WordLose1.gameObject.SetActive(bShow);
        WordLose2.gameObject.SetActive(bShow);
    }

    public IEnumerator OpenBattleEndText(bool bWin)
    {
        if (bWin)
            DisplayWinText(true);
        else
            DisplayLoseText(true);

        yield return new WaitForSeconds(ANIM_DURATION);

        // 战斗结束界面需要手动点击触发下一阶段,故不需要自动隐藏 
        //DisplayAllText(false);
    }

    public void CloseBattleEndText()
    {
        DisplayAllText(false);
    }

    private void DisplayWinText(bool bShow)
    {
        WordWin1.gameObject.SetActive(bShow);
        WordWin2.gameObject.SetActive(bShow);
        WordLose1.gameObject.SetActive(false);
        WordLose2.gameObject.SetActive(false);
    }

    private void DisplayLoseText(bool bShow)
    {
        WordWin1.gameObject.SetActive(false);
        WordWin2.gameObject.SetActive(false);
        WordLose1.gameObject.SetActive(bShow);
        WordLose2.gameObject.SetActive(bShow);
    }
}
