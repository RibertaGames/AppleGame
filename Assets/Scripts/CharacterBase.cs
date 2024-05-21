using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace RibertaGames
{
    public class CharacterBase : MonoBehaviour
    {
        [SerializeField] protected private RectTransform _rect;
        [SerializeField] protected private Animator _animator;

        [SerializeField] protected private Image _image;
        [SerializeField] protected private Image _additiveImage;
        [SerializeField] protected private GameObject _numObj;
        [SerializeField] protected private TextMeshProUGUI _hpText;

        public int x { get; protected private set; }
        public int y { get; protected private set; }
        public int power { get; private set; }

        //原点座標(0,0) 左下
        protected private float _originX;
        protected private float _originY;
        protected private float _sizeX;
        protected private float _sizeY;

        private Subject<Unit> _changePower = new Subject<Unit>();

        public IObservable<Unit> changePower => _changePower;

        /// <summary>
        /// 原点を設定する。
        /// </summary>
        /// <param name="setBoard"></param>
        public void SetOrigin(RectTransform boardRect, float boardX, float boardY)
        {
            _sizeX = (int)(boardRect.sizeDelta.x / boardX);
            _sizeY = (int)(boardRect.sizeDelta.y / boardY);
            _originX = -(boardRect.sizeDelta.x / 2) + _sizeX / 2;
            _originY = -(boardRect.sizeDelta.y / 2) + _sizeY / 2;
        }

        /// <summary>
        /// セットアップ
        /// </summary>
        protected private void _Setup(int x, int y, int power)
        {
            this.x = x;
            this.y = y;
            this.power = power;
            transform.localPosition = new Vector3(x * _sizeX + _originX, y * _sizeY + _originY, 0);

            _hpText.SetText(power.ToString());
        }

        /// <summary>
        /// 強さを変更する。
        /// </summary>
        /// <param name="power"></param>
        public void ChangePower(int power)
        {
            this.power = power;
            _hpText.SetText(power.ToString());
            _changePower.OnNext(Unit.Default);

            if(this as Character)
            {
                _animator.Play("PopAnim", 0, 0f);
            }
            else if (this as Enemy)
            {
                _animator.Play("DamegeAnim", 0, 0f);
            }
        }

        /// <summary>
        /// 画像を設定
        /// </summary>
        /// <param name="s"></param>
        public void SetupImage(Sprite s)
        {
            _image.sprite = s;
            _additiveImage.sprite = s;
            //_additiveImage.color = new Color(1, 1, 1, 0.05f);
            //_additiveImage
            //    .DOFade(0.3f, 2.0f) // 1.0秒かけてアルファ値を0.1fにする
            //    .SetLoops(-1, LoopType.Yoyo) // 無限ループさせてYoyo（行ったり来たり）する
            //    .SetEase(Ease.InOutSine);
        }

        /// <summary>
        /// 死亡
        /// </summary>
        public async UniTask Dead()
        {
            if (this is Enemy enemy && enemy.gimickType == eGimickType.Enemy)
            {
                _animator.Play("DeadAnim", 0, 0f);
                await UniTask.Delay(700);
            }
            Destroy(gameObject);
        }

        /// <summary>
        /// アイテム獲得
        /// </summary>
        /// <returns></returns>
        public async UniTask GetItem()
        {
            if (this is Enemy enemy)
            {
                switch (enemy.gimickType)
                {
                    case eGimickType.Key:
                        SEManager.instance.Play(SEPath.GET_ITEM);
                        _animator.Play("GetItemAnim", 0, 0f);
                        break;
                    case eGimickType.Timer:
                        SEManager.instance.Play(SEPath.GET_ITEM);
                        _animator.Play("GetItemAnim", 0, 0f);
                        break;
                }
                await UniTask.Delay(700);
            }
            Destroy(gameObject);
        }
    }
}