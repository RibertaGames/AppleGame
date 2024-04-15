using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using UniRx;

namespace RibertaGames
{
    [RequireComponent(typeof(Image))]             //アタッチ時Imageコンポーネントがなかったら追加する。
    [AddComponentMenu("RibertaGames/Button", 0)]  //メニューから選択できる様にする。
    [DisallowMultipleComponent] // 重複不可
    public class Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [SerializeField] private Image _image;
        [Header("ボタンの連打回数"), SerializeField] private eButtonHitsType _hitsType = eButtonHitsType.Duplication;
        [Header("ボタンの種類"),SerializeField] private eButtonType _buttonType = eButtonType.DecideButton;
        [Header("ボタンを押した時の透明度"), SerializeField] private float _buttonClickAlpha = 0.8f;

        /// <summary>
        /// クリックイベント([NonSerialized]を外すとエディターから設定可能になる。)
        /// </summary>
        [NonSerialized] public UnityEvent onClick = new UnityEvent();

        /// <summary>
        /// ボタン画像
        /// </summary>
        public Image image => _image;

        /// <summary>
        /// 外部公開; クリックイベント
        /// </summary>
        public IObservable<Unit> onClickSubject => _onClickSubject;

        private Subject<Unit> _onClickSubject = new Subject<Unit>();

        /// <summary>
        /// ボタン連打種類
        /// </summary>
        private enum eButtonHitsType
        {
            Single,      //重複なし
            Duplication, //連打重複あり
        }

        /// <summary>
        /// ボタン種別
        /// </summary>
        private enum eButtonType
        {
            DecideButton, //決定ボタン
            BackButton,   //戻るボタン
        }

        /// <summary>
        /// 同じボタン内で複数押したか
        /// </summary>
        private bool _block = false;

        /// <summary>
        /// 同じ設定の全てのボタンを押せなくする用
        /// </summary>
        //private static bool _blockAll = false;

        /// <summary>
        /// ボタンアタッチ時適応
        /// </summary>
        public void Reset()
        {
            _image = gameObject.GetComponent<Image>();
        }

        public void Awake()
        {
            _Release();
        }

        /// <summary>
        /// ボタンが下がった時
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerDown(PointerEventData eventData)
        {
            if (_image != null)
            {
                //押した瞬間、画像がグレーぽくなる。
                var c = _image.color;
                c.a = _buttonClickAlpha;
                _image.color = c;
            }
        }

        /// <summary>
        /// ボタンが元に戻った時
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerUp(PointerEventData eventData)
        {
            if (_image != null)
            {
                var c = _image.color;
                c.a = 1f;
                _image.color = c;
            }
        }

        /// <summary>
        /// ボタンをクリックした時
        /// </summary>
        /// <param name="eventData"></param>
        public async void OnPointerClick(PointerEventData eventData)
        {
            //連打対策
            if (_hitsType == eButtonHitsType.Single)
            {
                if (_block)
                {
                    Debug.LogWarning("連打対策: 同じボタンを無視");
                    return;
                }

                //if (_blockAll)
                //{
                //    Debug.LogWarning("連打対策: 全てのボタンを無視");
                //    return;
                //}
                _block = true;
                //_blockAll = true;
            }

            //ClickFunc
            var canceled = await _OnButtonClick(this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();
            //例外だったら
            if (canceled)
            {
                //解放
                _Release();
            }
        }

        /// <summary>
        /// クリックイベント
        /// </summary>
        private async UniTask _OnButtonClick(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            //決定ボタンのSE
            if (_buttonType == eButtonType.DecideButton)
            {
                //決定音
            }
            //戻るボタンのSE
            else if (_buttonType == eButtonType.BackButton)
            {
                //戻る音
            }

            // 決定ボタン
            SEManager.instance.Play(SEPath.DECIDE2);

            //クリックイベント発動
            onClick?.Invoke();
            _onClickSubject?.OnNext(Unit.Default);

            if (_hitsType == eButtonHitsType.Single)
            {
                //待機
                await UniTask.Delay(1000, cancellationToken: cancellationToken);
                //解放
                _Release();
            }
        }

        /// <summary>
        /// ボタン解放
        /// </summary>
        void _Release()
        {
            _block = false;
            //_blockAll = false;
        }
    }
}