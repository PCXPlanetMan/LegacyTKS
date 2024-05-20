using AssetBundles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ToolsCopyAssetBundlesToStreaming : EditorWindow
{
    private static int progress = 0;
    private static int total = 0;

    [MenuItem("Tools/TKS/CopyAssetBundles")]
    private static void Apply()
    {
        string strProjectPath = System.Environment.CurrentDirectory;
        string strAssetBundlesPath = Path.Combine(strProjectPath, Utility.AssetBundlesOutputPath);
        string strPlatform = Utility.GetPlatformName();
        strAssetBundlesPath = Path.Combine(strAssetBundlesPath, strPlatform);
        string strStreaming = Application.streamingAssetsPath;
        strStreaming = Path.Combine(strStreaming, strPlatform);

        total = 0;
        CalcTotalFiles(strAssetBundlesPath);

        CopyFolder(strAssetBundlesPath, strStreaming);
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    private static void CalcTotalFiles(string sourcePath)
    {
        if (Directory.Exists(sourcePath))
        {
            List<string> files = new List<string>(Directory.GetFiles(sourcePath));
            files.ForEach(c =>
            {
                string fileName = Path.GetFileName(c);
                if (!fileName.EndsWith(".manifest"))
                {
                    total++;
                }
            });
            //获得源文件下所有目录文件
            List<string> folders = new List<string>(Directory.GetDirectories(sourcePath));
            folders.ForEach(c =>
            {
                //采用递归的方法实现
                CalcTotalFiles(c);
            });
        }
    }

    private static void CopyFolder(string sourcePath, string destPath)
    {
        if (Directory.Exists(sourcePath))
        {
            if (Directory.Exists(destPath))
            {
                Directory.Delete(destPath, true);
            }

            //目标目录不存在则创建
            try
            {
                Directory.CreateDirectory(destPath);
            }
            catch (Exception ex)
            {
                throw new Exception("创建目标目录失败：" + ex.Message);
            }

            //获得源文件下所有文件
            List<string> files = new List<string>(Directory.GetFiles(sourcePath));
            files.ForEach(c =>
            {
                string fileName = Path.GetFileName(c);
                if (!fileName.EndsWith(".manifest"))
                {
                    string destFile = Path.Combine(new string[] { destPath, Path.GetFileName(c) });
                    progress++;
                    EditorUtility.DisplayProgressBar("Copy AssetBundles", "Copy AssetBundles from Project Root to StreamingAssetsPath", progress * 1f / total);

                    File.Copy(c, destFile, true);//覆盖模式
                }
            });
            //获得源文件下所有目录文件
            List<string> folders = new List<string>(Directory.GetDirectories(sourcePath));
            folders.ForEach(c =>
            {
                string destDir = Path.Combine(new string[] { destPath, Path.GetFileName(c) });
                //采用递归的方法实现
                CopyFolder(c, destDir);
            });
        }
    }
}
