using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Linq;

namespace RibertaGames
{
    public class GameView : ViewBase
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _highScoreText;
        [SerializeField] private TextMeshProUGUI _destroyCountText;
        [SerializeField] private TextMeshProUGUI _currentTurnText;
        [SerializeField] private Button _settingButton;
        [SerializeField] private Button _gamePauseButton;
        [SerializeField] private Character _characterPrefab;
        [SerializeField] private Enemy _enemyPrefab;
        [SerializeField] private RectTransform _enemyBoard;
        [SerializeField] private RectTransform _characterBoard;
        [SerializeField] private GraphicRaycaster _graphicRaycaster;

        [SerializeField] private GameObject _fillter;
        [SerializeField] private GameObject _boardFillter;
        [SerializeField] private SettingWindow _settingWindow;
        [SerializeField] private TutorialWindow _tutorialWindow;

        [SerializeField] private Sprite[]_animal;  // 動物の画像
        [SerializeField] private Sprite[] _fruits; // フルーツの画像
        [SerializeField] private Sprite _key;      // キー画像
        [SerializeField] private Sprite _timer;  // タイマー画像

        private readonly float NEXT_CHARACTER_POSITION_Y = -1.35f;

        /// <summary>
        /// ゲーム開始ボタン
        /// </summary>
        public IObservable<Unit> gameStartButton => _gamePauseButton.onClickSubject;

        /// <summary>
        /// ゲーム終了ボタン
        /// </summary>
        public IObservable<Unit> gameEndButton => _settingWindow.gameEndButton;

        /// <summary>
        /// 遊び方ボタン
        /// </summary>
        public IObservable<Unit> howToPlayButton => _settingWindow.howToPlayButton;

        /// <summary>
        /// 設定ウィンドウを開くボタン
        /// </summary>
        public IObservable<Unit> openSettingWindowButton => _settingButton.onClickSubject;

        /// <summary>
        /// 設定ウィンドウの閉じるボタン
        /// </summary>
        public IObservable<Unit> closeSettingWindowButton => _settingWindow.closeWindowButton;

        /// <summary>
        /// 設定ウィンドウの戻るボタン
        /// </summary>
        public IObservable<Unit> backSettingWindowButton => _settingWindow.backWindowButton;

        /// <summary>
        /// チュートリアルウィンドウの戻るボタン
        /// </summary>
        public IObservable<Unit> backTutorialWindowButton => _tutorialWindow.closeWindowButton;

        /// <summary>
        /// 初期化
        /// </summary>
        public override void Init()
        {
            // 初期化
            InitializeView();

            // 設定ウィンドウを初期化
            _settingWindow.Init();

            // チュートリアルウィンドウを初期化
            _tutorialWindow.Init();

            // フィルターオフ
            SetActiveFillter(false);
            SetActiveBoardFillter(false);
        }

        /// <summary>
        /// 画面を初期化
        /// </summary>
        public void InitializeView()
        {
            foreach (Transform t in _characterBoard.transform)
            {
                Destroy(t.gameObject);
            }
            foreach (Transform t in _enemyBoard.transform)
            {
                Destroy(t.gameObject);
            }
        }

        /// <summary>
        /// 設定ウィンドウを開く
        /// </summary>
        public void OpenSettingWindow() => _settingWindow.Open();

        /// <summary>
        /// 設定ウィンドウを閉じる
        /// </summary>
        public async UniTask CloseSettingWindow() => await _settingWindow.Close();

        /// <summary>
        /// チュートリアルウィンドウを開く
        /// </summary>
        public void OpenTutorialWindow() => _tutorialWindow.Open();

        /// <summary>
        /// チュートリアルウィンドウを閉じる
        /// </summary>
        public async UniTask CloseTutorialWindow() => await _tutorialWindow.Close();

        /// <summary>
        /// ボードフィルターオンオフ
        /// </summary>
        /// <param name="active"></param>
        public void SetActiveBoardFillter(bool active) => _boardFillter.SetActive(active);

        /// <summary>
        /// フィルターオンオフ
        /// </summary>
        /// <param name="active"></param>
        public void SetActiveFillter(bool active) => _fillter.SetActive(active);

        /// <summary>
        /// スコアを表示
        /// </summary>
        /// <param name="score"></param>
        public void SetScoreText(string score) => _scoreText.SetText(score);

        /// <summary>
        /// ハイスコアを表示
        /// </summary>
        /// <param name="highScore"></param>
        public void SetHighScoreText(string highScore) => _highScoreText.SetText(highScore);

        /// <summary>
        /// 倒した数を表示
        /// </summary>
        /// <param name="destroyCount"></param>
        public void SetDestroyCountText(string destroyCount) => _destroyCountText.SetText(destroyCount);

        /// <summary>
        /// 現在のターンを表示
        /// </summary>
        /// <param name="currentTurn"></param>
        public void SetCurrentTurnText(string currentTurn) => _currentTurnText.SetText(currentTurn);

        /// <summary>
        /// キャラクターが移動できるか設定
        /// </summary>
        /// <param name="active"></param>
        public void SetEnableMove(bool active)
        {
            var charaList = _characterBoard.GetComponentsInChildren<Character>();
            charaList.ToList().ForEach(chara => chara.SetEnableMove(active));
        }

        /// <summary>
        /// キャラクターを生成する。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="power"></param>
        public Character CreateCharacter(EntityInfo info, bool nextCharacter = false)
        {
            var prefab = Instantiate(_characterPrefab, _characterBoard, false);
            prefab.SetOrigin(_characterBoard, info.boardX, info.boardY);
            prefab.Setup(info.x, info.y, info.power);
            prefab.SetGraphicRaycaster(_graphicRaycaster);
            prefab.SetupImage(_CharacterImg(info.power));

            if (nextCharacter)
            {
                prefab.SetNextCharacterPosition(info.x, NEXT_CHARACTER_POSITION_Y);
            }

            // 画像更新
            prefab.changePower
                .Subscribe(_ => prefab.SetupImage(_CharacterImg(prefab.power)))
                .AddTo(gameObject);

            return prefab;
        }

        /// <summary>
        /// エネミーを生成する。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="hp"></param>
        /// <param name="gimickType"></param>
        public Enemy CreateEnemy(EntityInfo info)
        {
            var prefab = Instantiate(_enemyPrefab, _enemyBoard, false);
            prefab.SetOrigin(_enemyBoard, info.boardX, info.boardY);
            prefab.Setup(info.x, info.y, info.gimickType, info.power).Forget();
            prefab.SetupImage(_EnemyImg(info.power, info.gimickType));

            // 画像更新
            prefab.changePower
                .Subscribe(_ => prefab.SetupImage(_EnemyImg(prefab.power, prefab.gimickType)))
                .AddTo(gameObject);

            return prefab;
        }

        private int[] _powerList = new int[] { 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048 };

        /// <summary>
        /// キャラクター画像を設定
        /// </summary>
        private Sprite _CharacterImg(int power)
        {
            for(int i = 0; i < _powerList.Length; i++)
            {
                if (power <= _powerList[i])
                {
                    return _animal[i];
                }
            }
            return _animal[_animal.Length - 1];
        }

        /// <summary>
        /// エネミー画像を設定
        /// </summary>
        /// <param name="power"></param>
        /// <returns></returns>
        private Sprite _EnemyImg(int power, eGimickType gimickType)
        {
            if (gimickType == eGimickType.Enemy) 
            {
                for (int i = 0; i < _powerList.Length; i++)
                {
                    if (power <= _powerList[i])
                    {
                        return _fruits[i];
                    }
                }
                return _fruits[_fruits.Length - 1];
            }
            else if(gimickType == eGimickType.Key)
            {
                return _key;
            }
            else if (gimickType == eGimickType.Timer)
            {
                return _timer;
            }
            return _fruits[0];
        }
    }
}