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
            // �X�R�A��ݒ�
            _model.score
                .Subscribe(score => _view.SetScoreText(score.ToString()))
                .AddTo(gameObject);

            // �n�C�X�R�A��ݒ肷��
            _model.highScore
                .Subscribe(highScore => _view.SetHighScoreText(highScore.ToString()))
                .AddTo(gameObject);

            // �|��������ݒ�
            _model.destroyCount
                .Subscribe(count => _view.SetDestroyCountText(count.ToString()))
                .AddTo(gameObject);

            // ���݂̃^�[����ݒ�
            _model.currentTurn
                .Subscribe(currentTurn => _view.SetCurrentTurnText(currentTurn.ToString()))
                .AddTo(gameObject);

            // �Q�[���I���ʒm
            _model.gameEnd
                .Subscribe(_ => {
                    _view.InitializeView();
                    _view.SetActiveBoardFillter(true);
                })
                .AddTo(gameObject);

            // �L�����N�^�[����
            _model.createCharacter
                .Subscribe(info => {
                    _model.nextCharacter = _view.CreateCharacter(info);
                    _ = _model.nextCharacter.moveCharacter
                    .Subscribe(moveInfo => _model.PlayerMoveCharacter(moveInfo.x, moveInfo.y, moveInfo.character))
                    .AddTo(_model.nextCharacter.gameObject);
                })
                .AddTo(gameObject);

            // �G�l�~�[����
            _model.createEnemy
                .Subscribe(info => _model.enemies[info.x, info.y] = _view.CreateEnemy(info))
                .AddTo(gameObject);

            // �Q�[���J�n�{�^��
            _view.gameStartButton
                .Subscribe(_ => {
                    _model.GameStart();
                    _view.SetActiveBoardFillter(false);
                })
                .AddTo(gameObject);

            // �Q�[���I���{�^��
            _view.gameEndButton
                .Subscribe(async _ => {
                    _model.GameEnd();
                    await _view.CloseSettingWindow();
                    _view.SetActiveFillter(false);
                })
                .AddTo(gameObject);

            // �ݒ�E�B���h�E���J���{�^��
            _view.openSettingWindowButton
                .Subscribe(_ => {
                    _view.OpenSettingWindow();
                    _view.SetActiveFillter(true);
                })
                .AddTo(gameObject);

            // �ݒ�E�B���h�E�����{�^��
            _view.closeSettingWindowButton
                .Subscribe(async _ => {
                    await _view.CloseSettingWindow();
                    _view.SetActiveFillter(false);
                })
                .AddTo(gameObject);

            // �ݒ�E�B���h�E�����{�^��
            _view.backSettingWindowButton
                .Subscribe(async _ => {
                    await _view.CloseSettingWindow();
                    _view.SetActiveFillter(false);
                })
                .AddTo(gameObject);

            // �V�ѕ��{�^��
            _view.howToPlayButton
                .Subscribe(_ => { })
                .AddTo(gameObject);
        }
    }
}
