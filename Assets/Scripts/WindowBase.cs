using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


namespace RibertaGames
{
    public class WindowBase : MonoBehaviour
    {
        [SerializeField] private Button _closeWindowButton;
        [SerializeField] private Animator _animator;

        private enum eSettingWindowAnim
        {
            WindowOpen,
            WindowClose,
        }

        public IObservable<Unit> closeWindowButton => _closeWindowButton.onClickSubject;

        public virtual void Init()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 開く
        /// </summary>
        public virtual void Open()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 閉じる
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