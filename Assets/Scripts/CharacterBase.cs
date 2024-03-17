using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    /// <summary>
    /// �L�����N�^�[�̋���
    /// </summary>
    public int power { get; private set; }

    /// <summary>
    /// ���݂̍��W
    /// </summary>
    public int x { get; protected private set; }
    public int y { get; protected private set; }

    [SerializeField] private protected TextMeshProUGUI _hpText;
    [SerializeField] private protected RectTransform _rect;

    //���_���W(0,0) ����
    protected private float _originX;
    protected private float _originY;
    protected private float _sizeX;
    protected private float _sizeY;

    /// <summary>
    /// ���_��ݒ肷��B
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
    /// �Z�b�g�A�b�v
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
    /// �����\�����X�V����
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
