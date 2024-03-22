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
        /// �}�E�X�������O�̍��W
        /// </summary>
        private Vector3 _defaultPosition;

        /// <summary>
        /// �L�����N�^�[���[�u
        /// </summary>
        private Subject<(int, int, Character)> _moveCharacter = new Subject<(int, int, Character)>();

        /// <summary>
        /// �O�����J: �L�����N�^�[���[�u
        /// </summary>
        public IObservable<(int x, int y, Character character)> moveCharacter => _moveCharacter;

        /// <summary>
        /// UI�̃��C�L���X�^
        /// </summary>
        private GraphicRaycaster _graphicRaycaster;

        /// <summary>
        /// �Z�b�g�A�b�v
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
        /// ���C�L���X�^��ݒ�
        /// </summary>
        /// <param name="graphicRaycaster"></param>
        public void SetGraphicRaycaster(GraphicRaycaster graphicRaycaster)
        {
            _graphicRaycaster = graphicRaycaster;
        }

        /// <summary>
        /// �h���b�O�A���h�h���b�v
        /// </summary>
        public void OnMouseDrag()
        {
            var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 1;
            _rect.position = worldPosition;
        }

        /// <summary>
        /// �}�E�X�𗣂�����
        /// </summary>
        public void OnMouseUp()
        {
            //���݂̃}�E�X�̈ʒu���烌�C�L���X�g�������ăq�b�g�������̂��擾���܂��B
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

            //���s�����̂ŁA���̈ʒu�ɖ߂��B
            transform.localPosition = _defaultPosition;
        }

        /// <summary>
        /// �}�[�W�\���H
        /// </summary>
        /// <param name="targetCharacter"></param>
        /// <returns></returns>
        public bool IsEnableMarge(Character targetCharacter)
        {
            //�}�[�W�\: �^�[�Q�b�g���������g�ł͂Ȃ��A���p���[���������B
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
        /// �}�[�W����B
        /// </summary>
        /// <param name="targetCharacter"></param>
        public void Marge(Character targetCharacter)
        {
            if (IsEnableMarge(targetCharacter))
            {
                ChangePower(power + targetCharacter.power);
                Destroy(targetCharacter.gameObject);
            }
        }

        /// <summary>
        /// ���̃L�����N�^�[�̃|�W�V������ݒ肷��B
        /// </summary>
        public void SetNextCharacterPosition(float x, float y)
        {
            transform.localPosition = new Vector3(x * _sizeX + _originX, y * _sizeY + _originY, 0);
            _defaultPosition = transform.localPosition;
        }
    }
}