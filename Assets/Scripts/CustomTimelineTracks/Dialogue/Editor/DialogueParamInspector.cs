using System.Collections;
using System.Collections.Generic;
using com.tksr.schema;
using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueClip))]
public class DialogueParamInspector : Editor
{
	private SerializedProperty commandProp;
	private int typeIndex;

    private SchemaTexts schemaTexts = null;
    private string GetDialogContentsById(int Id)
    {
        if (schemaTexts != null && schemaTexts.dialogs != null)
        {
            if (schemaTexts.dialogs.ContainsKey(Id.ToString()))
            {
                TextsDialogItem item = schemaTexts.dialogs[Id.ToString()];
                if (item != null)
                {
                    return item.Contents;
                }
            }
        }

        return null;
    }


    private void OnEnable()
	{
		SceneView.onSceneGUIDelegate += OnSceneGUI;
		commandProp = serializedObject.FindProperty("template");

        if (schemaTexts == null)
        {
            string assetBundleName = ResourceUtils.AB_CFG_DATA;
            string assetName = ResourceUtils.ASSET_SCHEMA_TEXTS;
            string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
            if (assetPaths.Length != 0)
            {
                UnityEngine.Object target = AssetDatabase.LoadMainAssetAtPath(assetPaths[0]);
                TextAsset taEvents = target as TextAsset;
                string jsonItems = taEvents.text;
                schemaTexts = JsonConvert.DeserializeObject<SchemaTexts>(jsonItems);
            }
        }
    }

	public override void OnInspectorGUI()
	{
		EditorGUILayout.PropertyField(commandProp);
        SerializedProperty dialogsParamProp = commandProp.FindPropertyRelative("DialogsParam");
		//Debug.LogFormat("Dialog Count = {0}", dialogsParamProp.arraySize);
        for (int x = 0; x < dialogsParamProp.arraySize; x++)
        {
            SerializedProperty dialogParamProp = dialogsParamProp.GetArrayElementAtIndex(x);
			SerializedProperty dialogIDProp = dialogParamProp.FindPropertyRelative("DialogID");
            SerializedProperty showSideProp = dialogParamProp.FindPropertyRelative("ShowSide");
            SerializedProperty textProp = dialogParamProp.FindPropertyRelative("Text");

			int dialogID = dialogIDProp.intValue;
            int showSide = showSideProp.intValue;
            //if (string.IsNullOrEmpty(textProp.stringValue))
            {
                textProp.stringValue = GetDialogContentsById(dialogID);
            }
        }

        serializedObject.ApplyModifiedProperties();
	}

	private void OnDisable()
	{
		SceneView.onSceneGUIDelegate -= OnSceneGUI;
	}

    //Draws a position handle on the position associated with the AICommand
	//the handle can be moved to reposition the targetPosition property
	private void OnSceneGUI(SceneView v)
	{
		
	}
}
