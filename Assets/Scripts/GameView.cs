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
        /// �Q�[���|�[�Y�C�x���g
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

            //�ݒ�E�B���h�E��������
            _settingWindow.Setup(() => 
            {
                _SetActiveFillter(false);
            });

            // �ݒ�{�^������������ݒ�E�B���h�E���J��
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
        /// �X�R�A��\��
        /// </summary>
        /// <param name="score"></param>
        public void SetScoreText(string score)
        {
            _scoreText.SetText(score);
        }

        /// <summary>
        /// �n�C�X�R�A��\��
        /// </summary>
        /// <param name="highScore"></param>
        public void SetHighScoreText(string highScore)
        {
            _highScoreText.SetText(highScore);
        }

        /// <summary>
        /// �|��������\��
        /// </summary>
        /// <param name="destroyCount"></param>
        public void SetDestroyCountText(string destroyCount)
        {
            _destroyCountText.SetText(destroyCount);
        }

        /// <summary>
        /// ���݂̃^�[����\��
        /// </summary>
        /// <param name="currentTurn"></param>
        public void SetCurrentTurnText(string currentTurn)
        {
            _currentTurnText.SetText(currentTurn);
        }

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