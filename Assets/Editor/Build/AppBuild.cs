using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace RibertaGames
{
    public class AppBuilder
    {
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
            //--- keystoreの設定
            // TODO: 
            //---

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

            //--- ※この App Bundle に関連付けられている難読化解除ファイルはありません。ProGuard/R8
            // コードの縮小 (minification) の有効化/難読化
            // => **mapping.txtが生成されるのでアップロードする。
            PlayerSettings.Android.minifyRelease = true;
            PlayerSettings.Android.minifyDebug = false; // デバッグビルドでは縮小を無効にする
            PlayerSettings.Android.minifyWithR8 = true;
            // プロガード設定ファイルのパスを設定(不要？)
            //「BuildSetting」=>「PlayerSettng」=>「Android」=> 「PublishingSettings」=>
            //「Build」=>「Custom Proguard File」をオン =>「Assets/Plugins/Android/proguard-user.txtが生成される」
            //---

            // Android 13 以上をターゲットとするアプリで広告 ID を使用する場合は、
            // マニフェストに com.google.android.gms.permission.AD_ID 権限を含める必要があります。
            //「Custom Main Manifest」をオン => 「AndroidManifest.xml」が生成される。↓追加
            // <manifest><uses-permission android:name="com.google.android.gms.permission.AD_ID"/></manifest>

            // ※この App Bundle にはネイティブ コードが含まれ、デバッグ シンボルがアップロードされていません。
            // クラッシュやANRの解析を容易にするために、デバッグシンボルの生成を有効
            // zipのデバッグシンボルが出力されるのでこれもアップロードする。
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