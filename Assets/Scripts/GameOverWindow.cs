using UnityEngine;
using UniRx;
using System;

namespace RibertaGames
{
    public class GameOverWindow : WindowBase
    {
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;

        public IObservable<Unit> yesButton => _yesButton.onClickSubject;

        public IObservable<Unit> noButton => _noButton.onClickSubject;

        /// <summary>
        /// セットアップ
        /// </summary>
        public override void Init()
        {
            base.Init();
        }
    }
}