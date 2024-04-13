using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RibertaGames
{
    [RequireComponent(typeof(RectTransform))]
    public class Character : CharacterBase
    {
        /// <summary>
        /// マウス動かす前の座標
        /// </summary>
        private Vector3 _defaultPosition;

        /// <summary>
        /// キャラクタームーブ
        /// </summary>
        private Subject<(int, int, Character)> _moveCharacter = new Subject<(int, int, Character)>();

        /// <summary>
        /// 外部公開: キャラクタームーブ
        /// </summary>
        public IObservable<(int x, int y, Character character)> moveCharacter => _moveCharacter;

        /// <summary>
        /// UIのレイキャスタ
        /// </summary>
        private GraphicRaycaster _graphicRaycaster;

        /// <summary>
        /// 移動可能か
        /// </summary>
        private bool _isEnableMove = true;

        /// <summary>
        /// セットアップ
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="power"></param>
        public void Setup(int x, int y, int power)
        {
            _Setup(x, y, power);
            name = $"Character ({x},{y}) power:{power}";
            _defaultPosition = transform.localPosition;
        }

        /// <summary>
        /// レイキャスタを設定
        /// </summary>
        /// <param name="graphicRaycaster"></param>
        public void SetGraphicRaycaster(GraphicRaycaster graphicRaycaster)
        {
            _graphicRaycaster = graphicRaycaster;
        }

        /// <summary>
        /// 移動可能
        /// </summary>
        public void SetEnableMove(bool active)
        {
            _isEnableMove = active;
        }

        /// <summary>
        /// ドラッグアンドドロップ
        /// </summary>
        public void OnMouseDrag()
        {
            if (!_isEnableMove) return;

            var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 1;
            _rect.position = worldPosition;
        }

        /// <summary>
        /// マウスを離したら
        /// </summary>
        public void OnMouseUp()
        {
            if (!_isEnableMove) return;

            //現在のマウスの位置からレイキャストを撃ってヒットしたものを取得します。
            PointerEventData poiner = new PointerEventData(EventSystem.current);
            poiner.position = Input.mousePosition;
            List<RaycastResult> result = new List<RaycastResult>();
            _graphicRaycaster.Raycast(poiner, result);

            if (result != null && result.Count != 0)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    if (result[i].gameObject.CompareTag("CharacterMasu"))
                    {
                        var masu = result[i].gameObject.GetComponent<Masu>();
                        _moveCharacter.OnNext((masu.x, masu.y, this));
                    }
                }
            }

            //失敗したので、元の位置に戻す。
            transform.localPosition = _defaultPosition;
        }

        /// <summary>
        /// マージ可能か？
        /// </summary>
        /// <param name="targetCharacter"></param>
        /// <returns></returns>
        public bool IsEnableMarge(Character targetCharacter)
        {
            //マージ可能: ターゲットが自分自身ではない、かつパワーが等しい。
            if (!(x == targetCharacter.x && y == targetCharacter.y) &&
                power == targetCharacter.power)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// マージする。
        /// </summary>
        /// <param name="targetCharacter"></param>
        public void Marge(Character targetCharacter)
        {
            if (IsEnableMarge(targetCharacter))
            {
                ChangePower(power + targetCharacter.power);
                targetCharacter.Dead().Forget();
            }
        }

        /// <summary>
        /// 次のキャラクターのポジションを設定する。
        /// </summary>
        public void SetNextCharacterPosition(float x, float y)
        {
            transform.localPosition = new Vector3(x * _sizeX + _originX, y * _sizeY + _originY, 0);
            _defaultPosition = transform.localPosition;
        }
    }
}