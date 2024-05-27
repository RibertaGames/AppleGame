#if UNITY_IOS

using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
//20221206ATTのFrameとinfo.plistを自動で追加するスクリプト

public static class ATTPostProcessBuild
{
    [PostProcessBuild]//ビルド時に実行する
    private static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath)
    {
        string infoPlistPath = buildPath + "/Info.plist";
        PlistDocument infoPlist = new PlistDocument();
        infoPlist.ReadFromFile(infoPlistPath);
        PlistElementDict root = infoPlist.root;
        root.SetString("NSUserTrackingUsageDescription", "不適切な広告の表示を避けるために'トラッキングを許可'に設定してください。");
        infoPlist.WriteToFile(infoPlistPath);

        string pbxProjectPath = PBXProject.GetPBXProjectPath(buildPath);
        PBXProject pbxProject = new PBXProject();
        pbxProject.ReadFromFile(pbxProjectPath);
        string targetGuid = pbxProject.GetUnityFrameworkTargetGuid();
        pbxProject.AddFrameworkToProject(targetGuid, "AppTrackingTransparency.framework", true);
        pbxProject.WriteToFile(pbxProjectPath);
    }
}

#endif