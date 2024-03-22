using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;

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
        [SerializeField] private SettingWindow _settingWindow;

        private readonly float NEXT_CHARACTER_POSITION_Y = -1.35f;

        /// <summary>
        /// ゲームポーズイベント
        /// </summary>
        public IObservable<Unit> gamePauseButton => _gamePauseButton.onClickSubject;

        public override void Init()
        {
            foreach (Transform t in _characterBoard.transform)
            {
                Destroy(t.gameObject);
            }
            foreach (Transform t in _enemyBoard.transform)
            {
                Destroy(t.gameObject);
            }

            //設定ウィンドウを初期化
            _settingWindow.Setup(() => 
            {
                _SetActiveFillter(false);
            });

            // 設定ボタンを押したら設定ウィンドウを開く
            _settingButton.onClickSubject
                .Subscribe(_ => 
                {
                    _SetActiveFillter(true);
                    _settingWindow.Open();
                })
                .AddTo(gameObject);

            _SetActiveFillter(false);
        }

        private void _SetActiveFillter(bool active)
        {
            _fillter.SetActive(active);
        }

        /// <summary>
        /// スコアを表示
        /// </summary>
        /// <param name="score"></param>
        public void SetScoreText(string score)
        {
            _scoreText.SetText(score);
        }

        /// <summary>
        /// ハイスコアを表示
        /// </summary>
        /// <param name="highScore"></param>
        public void SetHighScoreText(string highScore)
        {
            _highScoreText.SetText(highScore);
        }

        /// <summary>
        /// 倒した数を表示
        /// </summary>
        /// <param name="destroyCount"></param>
        public void SetDestroyCountText(string destroyCount)
        {
            _destroyCountText.SetText(destroyCount);
        }

        /// <summary>
        /// 現在のターンを表示
        /// </summary>
        /// <param name="currentTurn"></param>
        public void SetCurrentTurnText(string currentTurn)
        {
            _currentTurnText.SetText(currentTurn);
        }

        /// <summary>
        /// キャラクターを生成する。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="power"></param>
        public Character CreateCharacter(EntityInfo info)
        {
            var prefab = Instantiate(_characterPrefab, _characterBoard, false);
            prefab.SetOrigin(_characterBoard, info.boardX, info.boardY);
            prefab.Setup(info.x, info.y, info.power);
            prefab.SetGraphicRaycaster(_graphicRaycaster);
            prefab.SetNextCharacterPosition(info.x, NEXT_CHARACTER_POSITION_Y);
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
            var prefab = Instantiate(_enemyPrefab);
            prefab.SetOrigin(_enemyBoard, info.boardX, info.boardY);
            prefab.Setup(info.x, info.y, info.gimickType, info.power);
            prefab.transform.SetParent(_enemyBoard, false);
            return prefab;
        }
    }
}