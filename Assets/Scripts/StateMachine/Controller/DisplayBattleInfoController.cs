using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayBattleInfoController : MonoBehaviour
{
    public UIBattleOpenAnimPanel OpenAnim;
    public UIBattleEndAnimPanel EndAnim;
    public UIBattleResultPanel ResultPanel;

    // Start is called before the first frame update
    void Start()
    {
        ResultPanel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator DisplayBattleOpenAnimation()
    {
        if (OpenAnim != null)
        {
            yield return OpenAnim.OpenBattleText();
        }
    }

    public IEnumerator DisplayBattleEndAnimation(bool bWin)
    {
        if (EndAnim != null)
        {
            yield return EndAnim.OpenBattleEndText(bWin);
        }
    }

    public void CloseBattleEndAnimation()
    {
        EndAnim.CloseBattleEndText();
    }

    public void ShowBattleResultPanel(bool bShow)
    {
        ResultPanel.gameObject.SetActive(bShow);
        if (bShow)
        {

        }
    }
}
