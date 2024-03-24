using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;
using Cysharp.Threading.Tasks;
using TMPro;

namespace RibertaGames 
{
    public class SettingWindow : MonoBehaviour
    {
        [SerializeField] private Button _closeWindowButton;
        [SerializeField] private Button _backWindowButton;
        [SerializeField] private Button _gameEndButton;
        [SerializeField] private Button _howToPlayButton;
        [SerializeField] private Slider _seSlider;
        [SerializeField] private Slider _bgmSlider;
        [SerializeField] private TMP_Dropdown _languageDropdown;
        [SerializeField] private Animator _animator;

        private enum eSettingWindowAnim
        {
            WindowOpen,
            WindowClose,
        }

        public IObservable<Unit> closeWindowButton => _closeWindowButton.onClickSubject;
        public IObservable<Unit> backWindowButton => _backWindowButton.onClickSubject;
        public IObservable<Unit> gameEndButton => _gameEndButton.onClickSubject;
        public IObservable<Unit> howToPlayButton => _howToPlayButton.onClickSubject;

        public void Init()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// ŠJ‚­
        /// </summary>
        public void Open()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// •Â‚¶‚é
        /// </summary>
        /// <returns></returns>
        public async UniTask Close()
        {
            _animator.Play(eSettingWindowAnim.WindowClose.ToString());
            await UniTask.Delay(500);
            gameObject.SetActive(false);
        }
    }
}