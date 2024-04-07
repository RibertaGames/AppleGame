using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;
public class AutoBuildScript : MonoBehaviour
{
    [MenuItem("Tools/Build", false)]
    public static void Build()
    {
        var output = "../Build/Build.exe";
        bool isDevelopment = false;
        var args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-outputPath":
                    output = args[i + 1];   //出力先の設定
                    break;
                case "-development":
                    isDevelopment = true;   //Developmentビルドにする
                    break;
                default:
                    break;
            }
        }

        var option = new BuildPlayerOptions();
        option.locationPathName = output;
        if (isDevelopment)
        {
            //optionsはビットフラグなので、|で追加していくことができる
            //option.options = BuildOptions.Development /*| BuildOptions.AllowDebugging*/;
        }
        option.target = BuildTarget.StandaloneWindows64; //ビルドターゲットを設定. 今回はWin64
        var result = BuildPipeline.BuildPlayer(option);
        if (result.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log("BUILD SUCCESS");
        }
        else
        {
            Debug.LogError("BUILD FAILED");
        }
    }
}