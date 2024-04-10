using UnityEngine;
using UniRx;

namespace RibertaGames {
    public class GamePresenter : PresenterBase
    {
        private GameModel _model;
        [SerializeField] private GameView _view;

        public void Start()
        {
            Init();
        }

        protected override void _Main()
        {
            _model.GameStart();
        }

        protected override void _SetupModel()
        {
            _model = new GameModel();
        }

        protected override void _SetupView()
        {
            _view.Init();
        }

        protected override void _Subscribe()
        {
            // スコアを設定
            _model.score
                .Subscribe(score => _view.SetScoreText(score.ToString()))
                .AddTo(gameObject);

            // ハイスコアを設定する
            _model.highScore
                .Subscribe(highScore => _view.SetHighScoreText(highScore.ToString()))
                .AddTo(gameObject);

            // 倒した数を設定
            _model.destroyCount
                .Subscribe(count => _view.SetDestroyCountText(count.ToString()))
                .AddTo(gameObject);

            // 現在のターンを設定
            _model.currentTurn
                .Subscribe(currentTurn => _view.SetCurrentTurnText(currentTurn.ToString()))
                .AddTo(gameObject);

            // ゲーム終了通知
            _model.gameEnd
                .Subscribe(_ => {
                    _view.InitializeView();
                    _view.SetActiveBoardFillter(true);
                })
                .AddTo(gameObject);

            // キャラクター生成
            _model.createCharacter
                .Subscribe(info => {
                    _model.nextCharacter = _view.CreateCharacter(info);
                    _ = _model.nextCharacter.moveCharacter
                    .Subscribe(moveInfo => _model.PlayerMoveCharacter(moveInfo.x, moveInfo.y, moveInfo.character))
                    .AddTo(_model.nextCharacter.gameObject);
                })
                .AddTo(gameObject);

            // エネミー生成
            _model.createEnemy
                .Subscribe(info => _model.enemies[info.x, info.y] = _view.CreateEnemy(info))
                .AddTo(gameObject);

            // ゲーム開始ボタン
            _view.gameStartButton
                .Subscribe(_ => {
                    _model.GameStart();
                    _view.SetActiveBoardFillter(false);
                })
                .AddTo(gameObject);

            // ゲーム終了ボタン
            _view.gameEndButton
                .Subscribe(async _ => {
                    _model.GameEnd();
                    await _view.CloseSettingWindow();
                    _view.SetActiveFillter(false);
                })
                .AddTo(gameObject);

            // 設定ウィンドウを開くボタン
            _view.openSettingWindowButton
                .Subscribe(_ => {
                    _view.OpenSettingWindow();
                    _view.SetActiveFillter(true);
                })
                .AddTo(gameObject);

            // 設定ウィンドウを閉じるボタン
            _view.closeSettingWindowButton
                .Subscribe(async _ => {
                    await _view.CloseSettingWindow();
                    _view.SetActiveFillter(false);
                })
                .AddTo(gameObject);

            // 設定ウィンドウを閉じるボタン
            _view.backSettingWindowButton
                .Subscribe(async _ => {
                    await _view.CloseSettingWindow();
                    _view.SetActiveFillter(false);
                })
                .AddTo(gameObject);

            // 遊び方ボタン
            _view.howToPlayButton
                .Subscribe(async _ => {
                    await _view.CloseSettingWindow();
                    _view.OpenTutorialWindow();
                })
                .AddTo(gameObject);

            // チュートリアル閉じる
            _view.backTutorialWindowButton
                .Subscribe(async _ => {
                    await _view.CloseTutorialWindow();
                    _view.SetActiveFillter(false);
                })
                .AddTo(gameObject);
        }
    }
}
