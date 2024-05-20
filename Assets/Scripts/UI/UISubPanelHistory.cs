using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISubPanelHistory : MonoBehaviour
{
    public Text History;

    private readonly int HISTORY_ID = 12001;

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnEnable()
    {
        LoadHistoryContent();
    }

    private void LoadHistoryContent()
    {
        var currentDocument = DocumentDataManager.Instance.GetCurrentDocument();
        string strFullName;
        if (currentDocument != null && History != null)
        {
            strFullName = string.Format("{0}{1}", DocumentDataManager.Instance.GetMainRoleLastName(), DocumentDataManager.Instance.GetMainRoleFirstName());

            var historyItem = TextsManager.Instance.GetHistoryItemById(HISTORY_ID);
            if (historyItem != null && !string.IsNullOrEmpty(historyItem.Contents))
            {
                string strContent = string.Format(historyItem.Contents, strFullName, strFullName);
                History.text = strContent;
            }
        }
    }
}
