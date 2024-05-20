using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIBattleOpenAnimPanel : MonoBehaviour
{
    public Image Word1;
    public Image Word2;
    public Image Word3;
    public Image Word4;

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
        Word1.gameObject.SetActive(bShow);
        Word2.gameObject.SetActive(bShow);
        Word3.gameObject.SetActive(bShow);
        Word4.gameObject.SetActive(bShow);
    }

    public IEnumerator OpenBattleText()
    {
        DisplayAllText(true);

        yield return new WaitForSeconds(ANIM_DURATION);

        DisplayAllText(false);
    }
}
