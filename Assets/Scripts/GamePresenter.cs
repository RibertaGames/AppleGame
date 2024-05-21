using UnityEngine;
using UniRx;

namespace RibertaGames {
    public class GamePresenter : PresenterBase
    {
        private GameModel _model;
        [SerializeField] private GameView _view;

        private SaveData _saveData = new SaveData();

        public void Start()
        {
            Init();

            //途中のセーブデータが存在した時
            var game = _saveData.GetClass(eSaveDataType.GameCached.ToString(), new GameCached());
            if (game != null && game.currentTurn > 0)
            {
                _model.SetGameModel(game);
            }
            else
            {
                _ = _model.GameStart();
            }
        }

        protected override void _SetupModel()
        {
            _model = new GameModel();
        }

        protected override void _SetupView()
        {
            _view.Init();

            // チュートリアルを表示
            if (!_model.AlreadyTutorialClear())
            {
                _view.SetActiveFillter(true);
                _view.OpenTutorialWindow();
                _model.SetTutorialClear();
            }
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

            // 直接キャラクター生成
            _model.createCharacter
                .Subscribe(info => {
                     var character = _view.CreateCharacter(info);
                    _model.characters[info.x, info.y] = character;
                    _ = character.moveCharacter
                    .Subscribe(moveInfo => _model.PlayerMoveCharacter(moveInfo.x, moveInfo.y, moveInfo.character))
                    .AddTo(character.gameObject);
                })
                .AddTo(gameObject);

            // 次のキャラクター生成
            _model.createNextCharacter
                .Subscribe(info => {
                    _model.nextCharacter = _view.CreateCharacter(info, true);
                    _ = _model.nextCharacter.moveCharacter
                    .Subscribe(moveInfo => _model.PlayerMoveCharacter(moveInfo.x, moveInfo.y, moveInfo.character))
                    .AddTo(_model.nextCharacter.gameObject);
                })
                .AddTo(gameObject);

            // エネミー生成
            _model.createEnemy
                .Subscribe(info => _model.enemies[info.x, info.y] = _view.CreateEnemy(info))
                .AddTo(gameObject);

            // 移動可能か？
            _model.isEnableMove
                .Subscribe(active => _view.SetEnableMove(active))
                .AddTo(gameObject);


            // ゲーム開始ボタン
            _view.gameStartButton
                .Subscribe(async _ => {
                    if (_model.GetCurrentGameState() != eGameState.GamePlay)
                    {
                        _view.InitializeView();
                        await _model.GameStart();
                        _view.SetActiveBoardFillter(false);
                    }
                    else
                    {
                        _view.SetActiveFillter(true);
                        _view.OpenGameOverWindow();
                    }
                })
                .AddTo(gameObject);

            // ゲーム終了ボタン
            _view.gameEndButton
                .Subscribe(async _ => {
                    await _view.CloseSettingWindow();
                    _view.OpenGameOverWindow();
                })
                .AddTo(gameObject);

            // ゲーム終了通知
            _model.gameEnd
                .Subscribe(_ => {
                    _view.SetActiveBoardFillter(true);
                    _view.SetEnableMove(false);
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

            // ゲームオーバー: YESボタン登録
            _view.yesGameOverWindowButton
                .Subscribe(async _ => {
                    _model.GameEnd();
                    await _view.CloseGameOverWindow();

                    _view.SetActiveFillter(false);
                    _view.SetActiveBoardFillter(false);
                    if (_model.GetCurrentGameState() != eGameState.GamePlay)
                    {
                        _view.InitializeView();
                        await _model.GameStart();
                        _view.SetActiveBoardFillter(false);
                    }
                })
                .AddTo(gameObject);

            // ゲームオーバー: NOボタン登録
            _view.noGameOverWindowButton
                .Subscribe(async _ => {
                    await _view.CloseGameOverWindow();
                    _view.SetActiveFillter(false);
                    _view.SetActiveBoardFillter(false);
                })
                .AddTo(gameObject);

            // ゲームオーバー閉じる
            _view.backGameOverWindowButton
                .Subscribe(async _ => {
                    await _view.CloseGameOverWindow();
                    _view.SetActiveFillter(false);
                    _view.SetActiveBoardFillter(false);
                })
                .AddTo(gameObject);
        }
    }
}
