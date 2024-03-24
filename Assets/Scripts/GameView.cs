using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;
using Cysharp.Threading.Tasks;

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

        [SerializeField] private Sprite[]_animal;  // �����̉摜
        [SerializeField] private Sprite[] _fruits; // �t���[�c�̉摜

        private readonly float NEXT_CHARACTER_POSITION_Y = -1.35f;

        /// <summary>
        /// �Q�[���J�n�{�^��
        /// </summary>
        public IObservable<Unit> gameStartButton => _gamePauseButton.onClickSubject;

        /// <summary>
        /// �Q�[���I���{�^��
        /// </summary>
        public IObservable<Unit> gameEndButton => _settingWindow.gameEndButton;

        /// <summary>
        /// �V�ѕ��{�^��
        /// </summary>
        public IObservable<Unit> howToPlayButton => _settingWindow.howToPlayButton;

        /// <summary>
        /// �ݒ�E�B���h�E���J���{�^��
        /// </summary>
        public IObservable<Unit> openSettingWindowButton => _settingButton.onClickSubject;

        /// <summary>
        /// �ݒ�E�B���h�E�̕���{�^��
        /// </summary>
        public IObservable<Unit> closeSettingWindowButton => _settingWindow.closeWindowButton;

        /// <summary>
        /// �ݒ�E�B���h�E�̖߂�{�^��
        /// </summary>
        public IObservable<Unit> backSettingWindowButton => _settingWindow.backWindowButton;

        /// <summary>
        /// ������
        /// </summary>
        public override void Init()
        {
            // ������
            InitializeView();

            // �ݒ�E�B���h�E��������
            _settingWindow.Init();

            // �t�B���^�[�I�t
            SetActiveFillter(false);
            SetActiveBoardFillter(false);
        }

        /// <summary>
        /// ��ʂ�������
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
        /// �ݒ�E�B���h�E���J��
        /// </summary>
        public void OpenSettingWindow() => _settingWindow.Open();

        /// <summary>
        /// �ݒ�E�B���h�E�����
        /// </summary>
        public async UniTask CloseSettingWindow() => await _settingWindow.Close();

        /// <summary>
        /// �{�[�h�t�B���^�[�I���I�t
        /// </summary>
        /// <param name="active"></param>
        public void SetActiveBoardFillter(bool active) => _boardFillter.SetActive(active);

        /// <summary>
        /// �t�B���^�[�I���I�t
        /// </summary>
        /// <param name="active"></param>
        public void SetActiveFillter(bool active) => _fillter.SetActive(active);

        /// <summary>
        /// �X�R�A��\��
        /// </summary>
        /// <param name="score"></param>
        public void SetScoreText(string score) => _scoreText.SetText(score);

        /// <summary>
        /// �n�C�X�R�A��\��
        /// </summary>
        /// <param name="highScore"></param>
        public void SetHighScoreText(string highScore) => _highScoreText.SetText(highScore);

        /// <summary>
        /// �|��������\��
        /// </summary>
        /// <param name="destroyCount"></param>
        public void SetDestroyCountText(string destroyCount) => _destroyCountText.SetText(destroyCount);

        /// <summary>
        /// ���݂̃^�[����\��
        /// </summary>
        /// <param name="currentTurn"></param>
        public void SetCurrentTurnText(string currentTurn) => _currentTurnText.SetText(currentTurn);

        /// <summary>
        /// �L�����N�^�[�𐶐�����B
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
        /// �G�l�~�[�𐶐�����B
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