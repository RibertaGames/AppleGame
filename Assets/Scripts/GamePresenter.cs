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

            _view.gamePauseButton
                .Subscribe()
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
        }
    }
}
