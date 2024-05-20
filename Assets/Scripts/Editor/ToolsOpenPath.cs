using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class ToolsOpenPath : EditorWindow
{
    [MenuItem("Tools/TKS/PersistentDataPath")]
    private static void Apply()
    {
        string path = Application.persistentDataPath;


        if (string.IsNullOrEmpty(path)) 
            return;

        if (!Directory.Exists(path))
        {
            UnityEngine.Debug.LogError("No Directory: " + path);
            return;
        }

        // 新开线程防止锁死
        Thread newThread = new Thread(new ParameterizedThreadStart(CmdOpenDirectory));
        newThread.Start(path);
    }

    private static void CmdOpenDirectory(object obj)
    {
        Process p = new Process();
#if UNITY_EDITOR_WIN
        p.StartInfo.FileName = "cmd.exe";
        p.StartInfo.Arguments = "/c stßart " + obj.ToString();
#elif UNITY_EDITOR_OSX
	    p.StartInfo.FileName = "bash";
        // TODO: Set shellPath
        string shellPath = "";

	    string shPath = shellPath + "openDir.sh";
	    p.StartInfo.Arguments = shPath + " " + obj.ToString();
#endif
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.RedirectStandardError = true;
        p.StartInfo.CreateNoWindow = true;
        p.Start();

        p.WaitForExit();
        p.Close();
    }

    /*
    openDir.sh

    #!/bin/bash
    tempDirectory=$*
     
    echo "${tempDirectory}"
    open "${tempDirectory}"
     */
}
