using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RibertaGames
{
	/// <summary>
	/// オーディオファイルへのパスを定数で管理するクラスを自動で作成するスクリプト
	/// </summary>
	public class AudioPathCreator : AssetPostprocessor
	{

		//オーディオファイルが入ってるディレクトリへのパス
		public static readonly string BGM_DIRECTORY_PATH = "Resources/Audio/" + BGMManager.AUDIO_DIRECTORY_PATH;
		public static readonly string SE_DIRECTORY_PATH = "Resources/Audio/" + SEManager.AUDIO_DIRECTORY_PATH;
		public static readonly string BGM_DIRECTORY_PAT = "Audio/" + BGMManager.AUDIO_DIRECTORY_PATH;
		public static readonly string SE_DIRECTORY_PAT = "Audio/" + SEManager.AUDIO_DIRECTORY_PATH;
		private static readonly string BEFOR_PATH = "Editor\\Audio";
		private static readonly string AFTER_PATH = "Scripts\\Common\\Audio";

		//=================================================================================
		//変更の監視
		//=================================================================================

#if !UNITY_CLOUD_BUILD

		//オーディオファイルが入ってるディレクトリが変更されたら、自動で各スクリプトを作成
		private static void _OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			//UnityPackageで最初にインポートした時はEntityがまだnull
			if (AudioManagerSetting.entity == null || !AudioManagerSetting.entity.isAutoUpdateAudioPath)
			{
				return;
			}

			List<string[]> assetsList = new List<string[]>() {
			importedAssets, deletedAssets, movedAssets, movedFromAssetPaths
		};

			//すぐにResources.LoadAllで取得出来ない場合もあるので間を開けて実行
			EditorApplication.delayCall += () =>
			{
				if (_ExistsPathInAssets(assetsList, BGM_DIRECTORY_PATH))
				{
					_CreateBGMPath();
				}
				if (_ExistsPathInAssets(assetsList, SE_DIRECTORY_PATH))
				{
					_CreateSEPath();
				}
			};
		}

		//入力されたassetsのパスの中に、指定したパスが含まれるものが一つでもあるか
		private static bool _ExistsPathInAssets(List<string[]> assetPathsList, string targetPath)
		{
			return assetPathsList
				.Any(assetPaths => assetPaths
					.Any(assetPath => assetPath
						.Contains(targetPath)));
		}

#endif

		//=================================================================================
		//スクリプト作成
		//=================================================================================

		//BGMとSEファイルへのパスを定数で管理するクラスを作成
		[MenuItem("RibertaGames/Create BGM&SE Path")]
		private static void _CreateAudionPath()
		{
			_CreateBGMPath();
			_CreateSEPath();
		}
		private static void _CreateBGMPath()
		{
			_Create(BGM_DIRECTORY_PAT);
		}
		private static void _CreateSEPath()
		{
			_Create(SE_DIRECTORY_PAT);
		}

		//オーディオファイルへのパスを定数で管理するクラスを作成
		private static void _Create(string directoryPath)
		{
			//オーディオファイルへのパスを抽出
			string directoryName = Path.GetFileName(directoryPath);
			var audioPathDict = new Dictionary<string, string>();

			foreach (var audioClip in Resources.LoadAll<AudioClip>(directoryPath))
			{
				//アセットへのパスを取得
				var assetPath = AssetDatabase.GetAssetPath(audioClip);

				//Resources以下のパス(拡張子なし)に変換
				var targetIndex = assetPath.LastIndexOf("Resources", StringComparison.Ordinal) + "Resources".Length + 1;
				var resourcesPath = assetPath.Substring(targetIndex);
				resourcesPath = resourcesPath.Replace(Path.GetExtension(resourcesPath), "");

				//オーディオ名の重複チェック
				var audioName = audioClip.name;
				if (audioPathDict.ContainsKey(audioName))
				{
					Debug.LogError(audioName + " is duplicate!\n1 : " + resourcesPath + "\n2 : " + audioPathDict[audioName]);
				}
				audioPathDict[audioName] = resourcesPath;
			}

			//このスクリプトがある所へのパス取得し、定数クラスを書き出す場所を決定
			string selfFileName = "AudioPathCreator.cs";
			string selfPath = Directory.GetFiles("Assets", "*", System.IO.SearchOption.AllDirectories)
				.FirstOrDefault(path => System.IO.Path.GetFileName(path) == selfFileName);

			string exportPath = selfPath.Replace(selfFileName, "").Replace(BEFOR_PATH, AFTER_PATH);

			//定数クラス作成
			ConstantsClassCreator.Create(directoryName + "Path", directoryName + "ファイルへのパスを定数で管理するクラス", audioPathDict, exportPath);
		}

	}
}