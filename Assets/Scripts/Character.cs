using System.Collections;
using System.Collections.Generic;
using TMPro;
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
        /// �L�����N�^�[�𐶐�����B
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="power"></param>
        public static Character CreateCharacter(int x, int y, int power)
        {
            var prefab = Instantiate(GameManager.instance.characterPrefab);
            prefab.SetOrigin(GameManager.instance.characterBoard, Game.CHARACTER_MASU_X, Game.CHARACTER_MASU_Y);
            prefab.Setup(x, y, power);
            prefab.transform.SetParent(GameManager.instance.characterBoard, false);
            return prefab;
        }

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
            GameManager.instance.graphicRaycaster.Raycast(poiner, result);

            if (result != null && result.Count != 0)
            {
                for (int i = 0; i < result.Count; i++)
                {
                    if (result[i].gameObject.CompareTag("CharacterMasu"))
                    {
                        var masu = result[i].gameObject.GetComponent<Masu>();
                        var move = GameManager.instance.currentGame.PlayerMoveCharacter(masu.x, masu.y, this);
                        if (move)
                        {
                            //�ēx�Z�b�g�A�b�v
                            Setup(masu.x, masu.y, power);
                            return;
                        }
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