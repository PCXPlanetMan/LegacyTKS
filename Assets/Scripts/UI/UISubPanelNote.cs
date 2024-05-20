using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISubPanelNote : MonoBehaviour
{
    public Button BtnPageDown;
    public Button BtnPageUp;
    public Text Page;
    public List<Text> Lines;

    private int maxPages = 1;
    private int curPage = 1;
    private List<int> storyNotesList;

    private readonly int MAX_LINES = 3;

    // Start is called before the first frame update
    void Start()
    {
        AddButtonListeners();
    }

    void OnEnable()
    {
        LoadStoryNotes();
        UpdateNotesIntoLines();
    }

    private void AddButtonListeners()
    {
        BtnPageDown.onClick.AddListener(OnClickBtnPageDown);
        BtnPageUp.onClick.AddListener(OnClickBtnPageUp);
    }

    private void OnClickBtnPageDown()
    {
        if (curPage < maxPages)
        {
            curPage++;
            UpdateNotesIntoLines();
        }
    }

    private void OnClickBtnPageUp()
    {
        if (curPage > 1)
        {
            curPage--;
            UpdateNotesIntoLines();
        }
    }

    private void UpdateButtonUI()
    {
        if (maxPages == 1)
        {
            BtnPageDown.gameObject.SetActive(false);
            BtnPageUp.gameObject.SetActive(false);
        }
        else
        {
            if (curPage == 1)
            {
                BtnPageDown.gameObject.SetActive(true);
                BtnPageUp.gameObject.SetActive(false);
            }
            else if (curPage == maxPages)
            {
                BtnPageDown.gameObject.SetActive(false);
                BtnPageUp.gameObject.SetActive(true);
            }
            else
            {
                BtnPageDown.gameObject.SetActive(true);
                BtnPageUp.gameObject.SetActive(true);
            }
        }
    }

    private void LoadStoryNotes()
    {
        var curDocument = DocumentDataManager.Instance.GetCurrentDocument();
        storyNotesList = curDocument.StoryNotes;
        if (storyNotesList.Count > 0)
        {
            maxPages = storyNotesList.Count / MAX_LINES + 1;
        }
    }

    private void UpdateNotesIntoLines()
    {
        int page = curPage;
        for (int i = 0; i < MAX_LINES && i < Lines.Count; i++)
        {
            int index = (page - 1) * MAX_LINES + i;
            if (index < storyNotesList.Count)
            {
                int noteId = storyNotesList[index];
                var noteItem = TextsManager.Instance.GetStoryNoteItemById(noteId);
                if (noteItem != null)
                {
                    Lines[i].text = noteItem.Contents;
                }
                else
                {
                    Lines[i].text = "";
                }
            }
            else
            {
                Lines[i].text = "";
            }
        }

        string strPage = string.Format(ResourceUtils.FORMAT_UI_PAGE, curPage, maxPages);
        Page.text = strPage;

        UpdateButtonUI();
    }
}
