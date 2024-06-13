using System;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace RibertaGames
{
    public class AppBuilder
    {
        [MenuItem("RibertaGames/Build/IOSAndroid")]
        public static void BuildForIOSAndroid()
        {
            BuildForIOS();
            BuildForAndroidAPK();
        }

        [MenuItem("RibertaGames/Build/Android/apk")]
        public static void BuildForAndroidAPK()
        {
            _BumpBuildNumberForAndroid();
#if UNITY_EDITOR_WIN
            string path = $"/Users/user/Desktop/UnityBuild/android/game.apk";
#elif UNITY_EDITOR_OSX
            string path = $"/Users/riberta/Desktop/UnityBuild/android/game.apk";
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

        [MenuItem("RibertaGames/Build/Android/aab")]
        public static void BuildForAndroidAAB()
        {
            _BumpBuildNumberForAndroid();
#if UNITY_EDITOR_WIN
            string path = $"/Users/user/Desktop/UnityBuild/android/game.aab";
#elif UNITY_EDITOR_OSX
            string path = $"/Users/riberta/Desktop/UnityBuild/android/game.aab";
#endif
            //--- ※Google Play の 64 ビット要件に準拠していません
            // PlayerSettingsを変更してスクリプティングバックエンドをIL2CPPに設定
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);

            // ターゲットアーキテクチャをARM64に設定
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            //---

            //--- ※API レベル 33 以上を対象にする必要があります。
            // ターゲットAPIレベルを設定
            PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)33;
            //---

            //--- ※この App Bundle に関連付けられている難読化解除ファイルはありません。
            // Player => UseR8にチェック

            // ※この App Bundle にはネイティブ コードが含まれ、デバッグ シンボルがアップロードされていません。
            EditorUserBuildSettings.androidCreateSymbols = AndroidCreateSymbols.Debugging;

            // AABをビルドするように設定
            EditorUserBuildSettings.buildAppBundle = true;

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

            //チームIDの自動認識
            PlayerSettings.iOS.appleEnableAutomaticSigning = true;
            PlayerSettings.iOS.appleDeveloperTeamID = "L5896L5C58";

            // 出力パス。絶対パスで指定すること。また、最後にスラッシュを入れないこと。PostBuildProcess に渡る path が通常ビルドと異なってしまい、思わぬバグを引き起こすことがあります。
#if UNITY_EDITOR_WIN
            string path = $"/Users/user/Desktop/UnityBuild/ios";
#elif UNITY_EDITOR_OSX
            string path = $"/Users/riberta/Desktop/UnityBuild/ios";
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