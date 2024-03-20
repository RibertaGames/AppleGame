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
        /// エネミーを生成する。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="hp"></param>
        /// <param name="gimickType"></param>
        public static Enemy CreateEnemy(int x, int y, int hp, eGimickType gimickType)
        {
            var prefab = Instantiate(GameManager.instance.enemyPrefab);
            prefab.SetOrigin(GameManager.instance.enemyBoard, Game.ENEMY_MASU_X, Game.ENEMY_MASU_Y);
            prefab.Setup(x, y, gimickType, hp);
            prefab.transform.SetParent(GameManager.instance.enemyBoard.transform, false);

            return prefab;
        }

        /// <summary>
        /// セットアップ
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="gimickType"></param>
        /// <param name="hp"></param>
        public void Setup(int x, int y, eGimickType gimickType, int power)
        {
            _Setup(x, y, power);
            this.gimickType = gimickType;
            name = $"{gimickType} ({x},{y})";

            if (gimickType == eGimickType.Enemy)
            {
                _hpText.text = power.ToString();
            }
            else if (gimickType == eGimickType.Key)
            {
                _hpText.text = "Key";
            }
            else if (gimickType == eGimickType.Timer)
            {
                _hpText.text = "Timer";
            }
        }

        /// <summary>
        /// 破壊する。
        /// </summary>
        public void Destroy()
        {
            Debug.Log(gameObject.name + "を倒した");
            Destroy(gameObject);
        }

        public void Move()
        {
            y -= 1;
            Setup(x, y, gimickType, power);
        }
    }
}