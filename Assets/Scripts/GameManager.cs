using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RibertaGames
{
    /// <summary>
    /// ゲームの状態
    /// </summary>
    public enum eGameState
    {
        None,
        Initialize,
        GamePlay,
        GameEnd,
    }

    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        private GameManager() { }

        public void Awake()
        {
            DontDestroyOnLoad(this);
        }

        public void Start()
        {
            BGMManager.instance.Play(BGMPath.MUS_MUS_BGM, isLoop: true);
            AdManager.instance.LoadAndShowAdMob(AdManager.eAdMob.Banner);
        }
    }
}