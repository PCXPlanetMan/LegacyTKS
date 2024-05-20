using com.tksr.schema;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextsManager : Singleton<TextsManager>
{
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private SchemaTexts schemaTexts;

    public void LoadTextsSchema(string jsonItems)
    {
        schemaTexts = JsonConvert.DeserializeObject<SchemaTexts>(jsonItems);
    }

    public TextsDialogItem GetDialogItemById(int Id)
    {
        if (schemaTexts != null && schemaTexts.dialogs != null)
        {
            if (schemaTexts.dialogs.ContainsKey(Id.ToString()))
            {
                return schemaTexts.dialogs[Id.ToString()];
            }
        }
        return null;
    }

    public TextsInformationItem GetInformationItemById(int Id)
    {
        if (schemaTexts != null)
        {
            if (schemaTexts.informations.ContainsKey(Id.ToString()))
            {
                return schemaTexts.informations[Id.ToString()];
            }
        }
        return null;
    }

    public TextsHistoryItem GetHistoryItemById(int Id)
    {
        if (schemaTexts != null)
        {
            if (schemaTexts.histories.ContainsKey(Id.ToString()))
            {
                return schemaTexts.histories[Id.ToString()];
            }
        }

        return null;
    }

    public TextsNoteItem GetStoryNoteItemById(int Id)
    {
        if (schemaTexts != null)
        {
            if (schemaTexts.notes.ContainsKey(Id.ToString()))
            {
                return schemaTexts.notes[Id.ToString()];
            }
        }

        return null;
    }
}
