using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;
using Cysharp.Threading.Tasks;

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
        [SerializeField] private Dropdown _languageDropdown;
        [SerializeField] private Animator _animator;

        private enum eSettingWindowAnim
        {
            WindowOpen,
            WindowClose,
        }

        public IObservable<Unit> gameEndButton => _gameEndButton.onClickSubject;
        public IObservable<Unit> howToPlayButton => _howToPlayButton.onClickSubject;

        public void Setup()
        {
            _closeWindowButton.onClickSubject
                .Subscribe(async _ => await _Close())
                .AddTo(this);

            _backWindowButton.onClickSubject
                .Subscribe(async _ => await _Close())
                .AddTo(this);

            _animator.Play(eSettingWindowAnim.WindowOpen.ToString());
        }

        private async UniTask _Close()
        {
            _animator.Play(eSettingWindowAnim.WindowClose.ToString());
            await UniTask.Delay(1000);
            Destroy(gameObject);
        }
    }
}