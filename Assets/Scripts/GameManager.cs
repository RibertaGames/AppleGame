using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RibertaGames
{
    /// <summary>
    /// �Q�[���̏��
    /// </summary>
    public enum eGameState
    {
        None,
        Initialize,
        Ready,
        GamePlay,
        GameEnd,
    }

    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        public static GameManager instance => _instance;

        private GameManager() { }

        public void Awake()
        {
            DontDestroyOnLoad(this);
            if (_instance == null)
            {
                _instance = this;

                _OnLoad();
            }
        }

        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _totalDamegeText;
        [SerializeField] private TextMeshProUGUI _destroyCountText;
        [SerializeField] private TextMeshProUGUI _currentTurnText;
        [SerializeField] private Button _settingButton;
        [SerializeField] private Button _gamePauseButton;
        [SerializeField] private GameObject _gameEndPanel;

        //�v���n�u
        [SerializeField] private Masu _masuPrefab;
        [SerializeField] private Character _characterPrefab;
        [SerializeField] private Enemy _enemyPrefab;
        //�{�[�h
        [SerializeField] private RectTransform _enemyBoard;
        [SerializeField] private RectTransform _characterBoard;
        //���C
        [SerializeField] private GraphicRaycaster _graphicRaycaster;

        public GraphicRaycaster graphicRaycaster => _graphicRaycaster;
        public Masu masuPrefab => _masuPrefab;
        public Enemy enemyPrefab => _enemyPrefab;
        public Character characterPrefab => _characterPrefab;
        public RectTransform enemyBoard => _enemyBoard;
        public RectTransform characterBoard => _characterBoard;

        public eGameState gameState { get; private set; } = eGameState.None;
        public Game currentGame { get; private set; } = null;

        /// <summary>
        /// ��x������������
        /// </summary>
        private void _OnLoad()
        {
            _Initialize();
            _settingButton?.onClick.AddListener(_GameStart);
            _gamePauseButton?.onClick.AddListener(_GameStart);

            //�Q�[���J�n
            _GameStart();
        }

        /// <summary>
        /// �Q�[��������������B
        /// </summary>
        private void _Initialize()
        {
            //������
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

            //�u���C���h�I��
            _EnableFillter(true);

            //��������
            gameState = eGameState.Ready;
        }

        /// <summary>
        /// �Q�[���J�n
        /// </summary>
        private void _GameStart()
        {
            if (currentGame != null)
            {
                Debug.LogError("�O��̃Q�[�������݂��Ă��܂��B��x���������Ă��������B");
                return;
            }
            if (gameState != eGameState.Ready)
            {
                Debug.LogError("�����i�K�łȂ��ƃQ�[���J�n�ł��܂���B");
                return;
            }

            _EnableFillter(false);
            gameState = eGameState.GamePlay;
            currentGame = new Game();
            currentGame.GameStart();
        }

        /// <summary>
        /// �Q�[���I��
        /// </summary>
        public void GameEnd()
        {
            if (gameState != eGameState.GamePlay)
            {
                Debug.LogError("�Q�[�����v���C���Ă��܂���B");
                return;
            }
            //�Q�[���I��
            gameState = eGameState.GameEnd;
            var result = currentGame.GetCurrentGameProgress();
            GameResult(result);

            //������
            _Initialize();
        }

        /// <summary>
        /// �Q�[������
        /// </summary>
        /// <param name="result"></param>
        public void GameResult(GameProgress result)
        {
            Debug.Log("�X�R�A: " + result.score);
            Debug.Log("�g�[�^���_���[�W: " + result.totalDamege);
            Debug.Log("�����j��: " + result.destroyCount);
            Debug.Log("�^�[����: " + result.currentTurn);

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
}