using System;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace RibertaGames
{
    public class AppBuilder
    {
        [MenuItem("RibertaGames/Build/iOS & Android")]
        public static void BuildForIOSANDAndroid()
        {
            BuildAddressable();
            BuildForIOS();
            BuildForAndroid();
        }

        [MenuItem("RibertaGames/Build/Android")]
        public static void BuildForAndroid()
        {
            _BumpBuildNumberForAndroid();
            DateTime currentDate = DateTime.Now;
#if UNITY_EDITOR_WIN
            string path = $"/Users/user/Desktop/UnityBuild/android/{ currentDate.ToString("yyyy-MM-dd") }/game.apk";
#elif UNITY_EDITOR_OSX
            string path = $"/Users/yanosyoki/Desktop/UnityBuild/android/{ currentDate.ToString("yyyy-MM-dd") }/game.apk";
#endif
            var report = BuildPipeline.BuildPlayer(_GetAllScenePaths(), path, BuildTarget.Android, BuildOptions.None);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Androidビルド 成功");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.LogError("Androidビルド 失敗");
            }
        }

        [MenuItem("RibertaGames/Build/iOS")]
        public static void BuildForIOS()
        {
            _BumpBuildNumberForIOS();
            // 出力パス。絶対パスで指定すること。また、最後にスラッシュを入れないこと。PostBuildProcess に渡る path が通常ビルドと異なってしまい、思わぬバグを引き起こすことがあります。
            DateTime currentDate = DateTime.Now;
#if UNITY_EDITOR_WIN
            string path = $"/Users/user/Desktop/UnityBuild/ios/{ currentDate.ToString("yyyy-MM-dd") }";
#elif UNITY_EDITOR_OSX
            string path = $"/Users/yanosyoki/Desktop/UnityBuild/android/{ currentDate.ToString("yyyy-MM-dd") }/game.apk";
#endif
            var report = BuildPipeline.BuildPlayer(_GetAllScenePaths(), path, BuildTarget.iOS, BuildOptions.None);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log("IOSビルド 成功: " + summary.totalSize + " bytes");
            }

            if (summary.result == BuildResult.Failed)
            {
                Debug.LogError("IOSビルド 失敗");
            }
        }

        [MenuItem("RibertaGames/Build/Addressable")]
        public static void BuildAddressable()
        {
            AddressableAssetSettings.BuildPlayerContent();
            Debug.Log("[アドレッサブルビルド完了]");
        }

        private static void _BumpBuildNumberForIOS()
        {
            string str = PlayerSettings.iOS.buildNumber;
            int num = int.Parse(str);
            num++;
            PlayerSettings.iOS.buildNumber = num + "";
        }

        private static void _BumpBuildNumberForAndroid()
        {
            PlayerSettings.Android.bundleVersionCode += 1;
        }

        private static string[] _GetAllScenePaths()
        {
            string[] scenes = new string[EditorBuildSettings.scenes.Length];
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                scenes[i] = EditorBuildSettings.scenes[i].path;
            }
            return scenes;
        }
    }
}