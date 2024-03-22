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
        public IObservable<Unit> gameEndButton => _gameEndButton.onClickSubject;
        public IObservable<Unit> howToPlayButton => _howToPlayButton.onClickSubject;

        public void Setup(Action closeWindowAction)
        {
            // ����
            _closeWindowButton.onClickSubject
                .Subscribe(async _ => {
                    await _Close();
                    closeWindowAction?.Invoke();
                })
                .AddTo(this);

            // ����
            _backWindowButton.onClickSubject
                .Subscribe(async _ => {
                    await _Close();
                    closeWindowAction?.Invoke();
                })
                .AddTo(this);

            gameObject.SetActive(false);
        }

        /// <summary>
        /// �J��
        /// </summary>
        public void Open()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <returns></returns>
        private async UniTask _Close()
        {
            _animator.Play(eSettingWindowAnim.WindowClose.ToString());
            await UniTask.Delay(500);
            gameObject.SetActive(false);
        }
    }
}