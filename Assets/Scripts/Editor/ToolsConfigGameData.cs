using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ToolsConfigGameDataWindow : EditorWindow
{
    private string charID;
    private string charLevel;
    private string itemID;
    private string itemCount;
    private string goldCount;
    private string mainRoleLv;

    [MenuItem("Tools/TKS/Debug Game Window")]
    public static void ShowConfigCharWindow()
    {
        EditorWindow.GetWindow(typeof(ToolsConfigGameDataWindow));
    }

    ToolsConfigGameDataWindow()
    {
        this.titleContent = new GUIContent("配置数据调试");
    }

    void OnGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.Space(10);
        GUIStyle titleStyle = new GUIStyle();
        titleStyle.fontSize = 24;
        titleStyle.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("配置数据", titleStyle, GUILayout.Width(300));

        GUILayout.Space(10);
        mainRoleLv = EditorGUILayout.TextField("主角Level", mainRoleLv);

        GUILayout.Space(10);
        goldCount = EditorGUILayout.TextField("金钱数", goldCount);

        GUILayout.Space(10);
        charID = EditorGUILayout.TextField("入队角色ID", charID);
        GUILayout.Space(10);
        charLevel = EditorGUILayout.TextField("角色等级", charLevel);

        EditorGUILayout.Space();

        GUILayout.Space(10);
        itemID = EditorGUILayout.TextField("包裹物品ID", itemID);
        GUILayout.Space(10);
        itemCount = EditorGUILayout.TextField("物品数量", itemCount);

        if (GUILayout.Button("Done"))
        {
            if (!string.IsNullOrEmpty(mainRoleLv))
            {
                int lv = int.Parse(mainRoleLv);
                DoneUpdateMainRoleLevel(lv);
            }

            if (!string.IsNullOrEmpty(goldCount))
            {
                int gold = int.Parse(goldCount);
                DoneUpdateGold(gold);
            }

            if (!string.IsNullOrEmpty(charID) && !string.IsNullOrEmpty(charLevel))
            {
                int id = int.Parse(charID);
                int level = int.Parse(charLevel);
                DoneCreateCharToTeam(id, level); 
            }

            if (!string.IsNullOrEmpty(itemID) && !string.IsNullOrEmpty(itemCount))
            {
                int id = int.Parse(itemID);
                int count = int.Parse(itemCount);
                DoneAddItemToPacket(id, count);
            }
        }

        GUILayout.EndVertical();
    }

    private void DoneUpdateMainRoleLevel(int lv)
    {
        if (Application.isPlaying)
        {
            lv = lv > 0 ? lv : 1;
            DocumentDataManager.Instance.GetCurrentDocument().MainRoleInfo.Level = lv;
        }
    }

    private void DoneUpdateGold(int newGold)
    {
        if (Application.isPlaying)
        {
            DocumentDataManager.Instance.GetCurrentDocument().Gold = (uint)newGold;
        }
    }

    private void DoneCreateCharToTeam(int nCharID, int nCharLevel)
    {
        if (Application.isPlaying)
        {
            DocumentDataManager.Instance.CreateCharToTeam(nCharID, nCharLevel);
        }
    }

    private void DoneAddItemToPacket(int nItemID, int nItemCount)
    {
        if (Application.isPlaying)
        {
            DocumentDataManager.Instance.AddTKRItemToPackage(nItemID, nItemCount);
        }
    }
}
