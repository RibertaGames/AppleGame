using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲームの状態
/// </summary>
public enum eGameState
{
    None,
    Initialize,
    Ready,
    GamePlay,
    GameEnd,
}

/// <summary>
/// ギミック種類
/// </summary>
public enum eGimickType
{
    None,
    Enemy,
    Key,
    Timer,
}

/// <summary>
/// 手持ちのアイテム
/// </summary>
[System.Flags]
public enum eItem
{
    Timer = 1,
    Key = 2,
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public static GameManager instance => _instance;

    private GameManager() { }

    public void Awake()
    {
        DontDestroyOnLoad(this);
        if(_instance == null)
        {
            _instance = this;

            _OnLoad();
        }
    }

    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _totalDamegeText;
    [SerializeField] private TextMeshProUGUI _destroyCountText;
    [SerializeField] private TextMeshProUGUI _currentTurnText;
    [SerializeField] private Button _gamePlayButton;
    [SerializeField] private Button _gamePauseButton;
    [SerializeField] private GameObject _gameEndPanel;

    //プレハブ
    [SerializeField] private Masu _masuPrefab;
    [SerializeField] private Character _characterPrefab;
    [SerializeField] private Enemy _enemyPrefab;
    //ボード
    [SerializeField] private GameObject _enemyBoard;
    [SerializeField] private GameObject _characterBoard;
    //レイ
    [SerializeField] private GraphicRaycaster _graphicRaycaster;

    public GraphicRaycaster graphicRaycaster => _graphicRaycaster;
    public Masu masuPrefab => _masuPrefab;
    public Enemy enemyPrefab => _enemyPrefab;
    public Character characterPrefab => _characterPrefab;
    public GameObject enemyBoard => _enemyBoard;
    public GameObject characterBoard => _characterBoard;

    public static readonly int BLOCK_SIZE = 80;

    public eGameState gameState { get; private set; } = eGameState.None;
    public Game currentGame { get; private set; } = null;

    /// <summary>
    /// 一度だけ処理する
    /// </summary>
    private void _OnLoad()
    {
        _Initialize();
        _gamePlayButton?.onClick.AddListener(_GameStart);
        _gamePauseButton?.onClick.AddListener(_Initialize);

        //ゲーム開始
        _GameStart();
    }

    /// <summary>
    /// ゲームを初期化する。
    /// </summary>
    private void _Initialize()
    {
        //初期化
        gameState = eGameState.Initialize;
        currentGame = null;

        foreach (Transform t in characterBoard.transform)
        {
            Destroy(t.gameObject);
        }
        foreach (Transform t in enemyBoard.transform)
        {
            Destroy(t.gameObject);
        }
        GameResult(new GameProgress());

        //ブラインドオン
        _EnableFillter(true);

        //準備完了
        gameState = eGameState.Ready;
    }

    /// <summary>
    /// ゲーム開始
    /// </summary>
    private void _GameStart()
    {
        if (currentGame != null)
        {
            Debug.LogError("前回のゲームが存在しています。一度初期化してください。");
            return;
        }
        if (gameState != eGameState.Ready)
        {
            Debug.LogError("準備段階でないとゲーム開始できません。");
            return;
        }

        _EnableFillter(false);
        gameState = eGameState.GamePlay;
        currentGame = new Game();
        currentGame.GameStart();
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void GameEnd()
    {
        if (gameState != eGameState.GamePlay)
        {
            Debug.LogError("ゲームをプレイしていません。");
            return;
        }
        //ゲーム終了
        gameState = eGameState.GameEnd;
        var result = currentGame.GetCurrentGameProgress();
        GameResult(result);

        //初期化
        _Initialize();
    }

    /// <summary>
    /// ゲーム結果
    /// </summary>
    /// <param name="result"></param>
    public void GameResult(GameProgress result)
    {
        Debug.Log("スコア: " + result.score);
        Debug.Log("トータルダメージ: " + result.totalDamege);
        Debug.Log("総撃破数: " + result.destroyCount);
        Debug.Log("ターン数: " + result.currentTurn);

        _scoreText.text = result.score.ToString();
        _totalDamegeText.text = result.totalDamege.ToString();
        _destroyCountText.text = result.destroyCount.ToString();
        _currentTurnText.text = result.currentTurn.ToString();
    }

    private void _EnableFillter(bool enable)
    {
        _gameEndPanel.SetActive(enable);
    }
}