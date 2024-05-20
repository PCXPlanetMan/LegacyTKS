using System.Collections;
using System.Collections.Generic;
using com.tksr.document;
using UnityEngine;
using UnityEngine.UI;

public class UIListTeamMember : MonoBehaviour
{
    public Image Portrait;
    public Text Name;
    public Text HP;
    public Text MP;
    public Image LinkLine;
    public Text LabelHP;
    public Text LabelMP;
    public Image Background;

    private GameCharInfo curCharInfo;
    private DataCharStatsInfo curCharActiveStats;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameCharInfo GetCurrentCharInfo()
    {
        return curCharInfo;
    }

    public DataCharStatsInfo GetCurrentCharActiveStats()
    {
        return curCharActiveStats;
    }

    public void LoadCharInformation(GameCharInfo charInfo)
    {
        curCharInfo = charInfo;

        if (charInfo.CharId == ResourceUtils.MAINROLE_ID)
        {
            Name.text = string.Format("{0}{1}", DocumentDataManager.Instance.GetMainRoleLastName(), DocumentDataManager.Instance.GetMainRoleFirstName());
        }
        else
        {
            Name.text = CharactersManager.Instance.ReadCharNameById(charInfo.CharId);
        }

        Portrait.sprite = CharactersManager.Instance.ReadFullPortraitById(charInfo.CharId);
        HP.text = charInfo.HP.ToString();
        MP.text = charInfo.MP.ToString();

        curCharActiveStats = CharactersManager.Instance.ParseDataCharStatsInfo(charInfo);
    }

    public void SetSelected(bool bSelected)
    {
        float fAlpha = 0.4f;
        if (bSelected)
        {
            Background.color = Color.white;
            Portrait.color = Color.white;
            LabelHP.color = Color.green;
            LabelMP.color = Color.cyan;
            Name.color = Color.yellow;
            HP.color = Color.white;
            MP.color = Color.white;
            LinkLine.gameObject.SetActive(true);
        }
        else
        {
            Color color = Portrait.color;
            color.a = fAlpha;
            Portrait.color = color;

            LabelHP.color = Color.white;
            color = LabelHP.color;
            color.a = fAlpha;
            LabelHP.color = color;

            LabelMP.color = Color.white;
            color = LabelMP.color;
            color.a = fAlpha;
            LabelMP.color = color;

            Name.color = Color.white;
            color = Name.color;
            color.a = fAlpha;
            Name.color = color;

            HP.color = Color.white;
            color = HP.color;
            color.a = fAlpha;
            HP.color = color;

            MP.color = Color.white;
            color = MP.color;
            color.a = fAlpha;
            MP.color = color;

            Background.color = Color.white;
            color = Background.color;
            color.a = fAlpha;
            Background.color = color;

            LinkLine.gameObject.SetActive(false);
        }
    }
}
