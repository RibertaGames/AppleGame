using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RibertaGames
{
    public class Enemy : CharacterBase
    {
        public eGimickType gimickType = eGimickType.None;

        /// <summary>
        /// セットアップ
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="gimickType"></param>
        /// <param name="hp"></param>
        public async UniTask Setup(int x, int y, eGimickType gimickType, int power)
        {
            _Setup(x, y, power);
            this.gimickType = gimickType;
            name = $"{gimickType} ({x},{y})";

            switch (gimickType)
            {
                case eGimickType.Enemy:
                    _hpText.text = power.ToString();
                    break;
                case eGimickType.Key:
                case eGimickType.Timer:
                    _numObj.SetActive(false);
                    await UniTask.Delay(500);
                    _animator.Play("ItemAnim", 0, 0f);
                    break;
            }
        }

        public void Move()
        {
            y -= 1;
            _Setup(x, y, power);
            name = $"{gimickType} ({x},{y})";
        }
    }
}