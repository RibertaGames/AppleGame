using UnityEngine;
using UnityEngine.UI;
using System;
using UniRx;
using Cysharp.Threading.Tasks;
using TMPro;

namespace RibertaGames 
{
    public class SettingWindow : WindowBase
    {
        [SerializeField] private Button _backWindowButton;
        [SerializeField] private Button _gameEndButton;
        [SerializeField] private Button _howToPlayButton;
        [SerializeField] private Slider _seSlider;
        [SerializeField] private Slider _bgmSlider;
        [SerializeField] private TMP_Dropdown _languageDropdown;

        public IObservable<Unit> backWindowButton => _backWindowButton.onClickSubject;
        public IObservable<Unit> gameEndButton => _gameEndButton.onClickSubject;
        public IObservable<Unit> howToPlayButton => _howToPlayButton.onClickSubject;
    }
}