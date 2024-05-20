using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ToolsSetSceneTags : EditorWindow
{
    [MenuItem("Tools/TKS/SetSceneTags")]
    private static void Apply()
    {
        AssetDatabase.RemoveUnusedAssetBundleNames();

        string[] guids = AssetDatabase.FindAssets("t:scene", null);

        foreach (string guid in guids)
        {
            string strScenePath = AssetDatabase.GUIDToAssetPath(guid);
            if (strScenePath.StartsWith("Assets/Scenes/"))
            {
                int nLast = strScenePath.LastIndexOf("/");
                string strSceneName = strScenePath.Substring(nLast + 1);
                string strFinalScene = strSceneName.Substring(strSceneName.LastIndexOf("_") + 1);
                strFinalScene = strFinalScene.Remove(strFinalScene.LastIndexOf("."));
                //Debug.Log(strFinalScene);
                string strTag = string.Empty;
                if (strScenePath.StartsWith("Assets/Scenes/World"))
                {
                    strTag = "scene/world/" + strFinalScene.ToLower();
                }
                else if (strScenePath.StartsWith("Assets/Scenes/Battle"))
                {
                    strTag = "scene/battle/" + strFinalScene.ToLower();
                }

                if (!string.IsNullOrEmpty(strTag))
                {
                    AssetImporter assetImporter = AssetImporter.GetAtPath(strScenePath);
                    assetImporter.assetBundleName = strTag;
                    assetImporter.SaveAndReimport();
                }
            }
        }
    }
}
