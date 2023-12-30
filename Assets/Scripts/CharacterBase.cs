using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    //���݂̍��W
    public int x;
    public int y;

    public TextMeshProUGUI hpText;
    public RectTransform rect;

    //���_���W(0,0) ����
    public float originX = 0;
    public float originY = 0;
    public int size = 0;

    /// <summary>
    /// ���_��ݒ肷��B
    /// </summary>
    /// <param name="setBoard"></param>
    public void SetOrigin(GameObject setBoard)
    {
        var boardRect = setBoard.GetComponent<RectTransform>();
        size = GameManager.BLOCK_SIZE;
        originX = -(boardRect.sizeDelta.x / 2) + size / 2;
        originY = -(boardRect.sizeDelta.y / 2) + size / 2;
    }
}
