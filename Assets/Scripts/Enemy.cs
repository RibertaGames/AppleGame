using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : CharacterBase
{
    public eGimickType gimickType = eGimickType.None;
    public int hp = 0;

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
        prefab.SetOrigin(GameManager.instance.enemyBoard);
        prefab.Setup(x, y, gimickType, hp);

        return prefab;
    }

    /// <summary>
    /// セットアップ
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="gimickType"></param>
    /// <param name="hp"></param>
    public void Setup(int x, int y, eGimickType gimickType, int hp)
    {
        this.x = x;
        this.y = y;
        this.gimickType = gimickType;
        this.hp = hp;

        name = $"{gimickType} ({x},{y})";
        transform.SetParent(GameManager.instance.enemyBoard.transform, false);
        rect.sizeDelta = new Vector2(size, size);
        transform.localPosition = new Vector3(x * size + originX, y * size + originY, 0);

        UpdateText();
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
        Setup(x, y, gimickType, hp);
    }

    /// <summary>
    /// UIを更新する。
    /// </summary>
    public void UpdateText()
    {
        if (gimickType == eGimickType.Enemy)
        {
            hpText.text = hp.ToString();
        }
        else if(gimickType == eGimickType.Key)
        {
            hpText.text = "Key";
        }
        else if (gimickType == eGimickType.Timer)
        {
            hpText.text = "Timer";
        }
    }
}