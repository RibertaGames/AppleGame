using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RibertaGames
{
    /// <summary>
    /// ÉQÅ[ÉÄÇÃèÛë‘
    /// </summary>
    public enum eGameState
    {
        None,
        Initialize,
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
            }
        }
        public void Start()
        {
            BGMManager.instance.Play(BGMPath.MUS_MUS_BGM, isLoop: true);
        }
    }
}