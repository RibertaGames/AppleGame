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
        //[SerializeField] private TMP_Dropdown _languageDropdown;

        public IObservable<Unit> backWindowButton => _backWindowButton.onClickSubject;
        public IObservable<Unit> gameEndButton => _gameEndButton.onClickSubject;
        public IObservable<Unit> howToPlayButton => _howToPlayButton.onClickSubject;

        public void Start()
        {
            var setting = AudioManagerSetting.entity;

            _bgmSlider.value = setting.bgmBaseVolume;
            _seSlider.value = setting.seBaseVolume;

            //BGM変更
            _bgmSlider.OnValueChangedAsObservable()
                .Subscribe(value => {
                    BGMManager.instance.ChangeBaseVolume(value / 4f);
                    setting.bgmBaseVolume = value;
                })
                .AddTo(gameObject);

            //SE変更
            _seSlider.OnValueChangedAsObservable()
                .Subscribe(value => {
                    SEManager.instance.ChangeBaseVolume(value);
                    setting.seBaseVolume = value;
                })
                .AddTo(gameObject);
        }
    }
}