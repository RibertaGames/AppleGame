using System.IO;
using UnityEditor;
using UnityEngine;

// 起動時に実行.
[InitializeOnLoad]
public class ScriptableObjectCreator : EditorWindow
{
    // スクリプトの入力パス.
    private static string inputPath = "Assets/Scripts/Master/AutoCompile";
    // アセットの出力パス.
    private static string outputPath = "Assets/Resources/MasterAsset/AutoCompile";

    // スクリプトの拡張子.
    public static readonly string scriptExtension = ".cs";
    // アセットの拡張子.
    public static readonly string assetExtension = ".asset";

    // コンストラクタ（起動時に呼び出される）.
    //static ScriptableObjectCreator()
    //{
    //    // 処理を呼び出す.
    //    UpdateAssets();
    //}

    //// アセット更新時に実行.
    //public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetsPath)
    //{
    //    // 処理を呼び出す.
    //    UpdateAssets();
    //}

    // メニューにアイテムを追加.
    [MenuItem("Tools/ScriptableObject更新")]
    // 呼び出す関数.
    public static void UpdateAssets()
    {
        // アセットを削除する.
        DeleteAll(outputPath);

        // アセットを生成する.
        CreateAll(inputPath);

        // データベースをリフレッシュする.
        AssetDatabase.Refresh();
    }

    // 出力先をチェック.
    public static void DeleteAll(string targetPath)
    {
        // ディレクトリパスの配列.
        string[] directories = Directory.GetDirectories(targetPath);
        // ファイルパスの配列.
        string[] files = Directory.GetFiles(targetPath);

        // ファイルを削除.
        foreach (var file in files)
        {
            DeleteAsset(file);
        }

        // さらに深い階層を探索.
        foreach (var directory in directories)
        {
            DeleteAll(directory);
        }
    }

    // 入力元をチェック.
    public static void CreateAll(string targetPath)
    {
        // ディレクトリパスの配列.
        string[] directories = Directory.GetDirectories(targetPath);
        // ファイルパスの配列.
        string[] files = Directory.GetFiles(targetPath);

        // ファイルを生成.
        foreach (var file in files)
        {
            CreateAsset(file);
        }

        // さらに深い階層を探索.
        foreach (var directory in directories)
        {
            CreateAll(directory);
        }
    }

    // アセットを削除.
    public static void DeleteAsset(string targetPath)
    {
        // ファイル情報.
        string directoryName = Path.GetDirectoryName(targetPath);
        string fileName = Path.GetFileName(targetPath);
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(targetPath);
        string extension = Path.GetExtension(targetPath);

        // スクリプトのパス.
        string scriptPath = inputPath.Replace("/", "\\");
        // アセットのパス.
        string assetPath = outputPath.Replace("/", "\\");

        // アセットファイルである.
        if (extension == assetExtension)
        {
            // アセットのパス.
            string filePath = targetPath.Replace("/", "\\");
            // スクリプトのパス.
            string checkPath = Path.Combine(directoryName.Replace(assetPath, scriptPath), fileNameWithoutExtension + scriptExtension);

            // 対応するファイルが存在しない.
            if (!File.Exists(checkPath))
            {
                // 削除.
                AssetDatabase.DeleteAsset(filePath);

                Debug.Log("ScriptableObject Deleteed : " + filePath);
            }
        }
    }

    // アセットを生成.
    public static void CreateAsset(string targetPath)
    {
        // ファイル情報.
        string directoryName = Path.GetDirectoryName(targetPath);
        string fileName = Path.GetFileName(targetPath);
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(targetPath);
        string extension = Path.GetExtension(targetPath);

        // スクリプトのパス.
        string scriptPath = inputPath.Replace("/", "\\");
        // アセットのパス.
        string assetPath = outputPath.Replace("/", "\\");

        // スクリプトファイルである.
        if (extension == scriptExtension)
        {
            // 生成.
            var obj = CreateInstance(fileNameWithoutExtension);
            string filePath = Path.Combine(directoryName.Replace(scriptPath, assetPath), fileNameWithoutExtension + assetExtension);
            CreateAssetWithOverwrite(obj, filePath);

            Debug.Log("ScriptableObject Created : " + filePath);
        }
    }

    // アセットを上書きで作成する(metaデータはそのまま).
    public static void CreateAssetWithOverwrite(UnityEngine.Object asset, string exportPath)
    {
        // アセットが存在しない場合はそのまま作成(metaファイルも新規作成).
        if (!File.Exists(exportPath))
        {
            AssetDatabase.CreateAsset(asset, exportPath);
            return;
        }

        // 仮ファイルを作るためのディレクトリを作成.
        var fileName = Path.GetFileName(exportPath);
        var tmpDirectoryPath = Path.Combine(exportPath.Replace(fileName, ""), "tmp");
        Directory.CreateDirectory(tmpDirectoryPath);

        // 仮ファイルを保存.
        var tmpFilePath = Path.Combine(tmpDirectoryPath, fileName);
        AssetDatabase.CreateAsset(asset, tmpFilePath);

        // 仮ファイルを既存のファイルに上書き(metaデータはそのまま).
        FileUtil.ReplaceFile(tmpFilePath, exportPath);

        // 仮ディレクトリとファイルを削除.
        AssetDatabase.DeleteAsset(tmpDirectoryPath);

        // データ変更をUnityに伝えるためインポートしなおし.
        AssetDatabase.ImportAsset(exportPath);
    }
}

