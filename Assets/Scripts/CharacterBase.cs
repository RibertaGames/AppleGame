using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    /// <summary>
    /// キャラクターの強さ
    /// </summary>
    public int power { get; private set; }

    /// <summary>
    /// 現在の座標
    /// </summary>
    public int x { get; protected private set; }
    public int y { get; protected private set; }

    [SerializeField] private protected TextMeshProUGUI _hpText;
    [SerializeField] private protected RectTransform _rect;

    //原点座標(0,0) 左下
    protected private float _originX;
    protected private float _originY;
    protected private float _sizeX;
    protected private float _sizeY;

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
    protected private void _Setup(int x, int y , int power)
    {
        this.x = x;
        this.y = y;
        this.power = power;
        transform.localPosition = new Vector3(x * _sizeX + _originX, y * _sizeY + _originY, 0);

        _UpdatePowerText();
    }

    /// <summary>
    /// 強さ表示を更新する
    /// </summary>
    protected private void _UpdatePowerText()
    {
        _hpText.text = power.ToString();
    }

    public void ChangePower(int power)
    {
        this.power = power;
        _UpdatePowerText();
    }
}
