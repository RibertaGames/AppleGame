using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class Character : CharacterBase
{
    public int power = 0;

    //キャラクター側の可能な数字
    public static readonly int[] CHARACTER_ENABLE_NUM = new int[] { 2, 4, 8, 16, 32, 64};
    //次のキャラクターの固定X,Y
    public static readonly int NEXT_CHARACTER_X = 3;
    public static readonly int NEXT_CHARACTER_Y = -1;

    /// <summary>
    /// キャラクターを生成する。
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="power"></param>
    public static Character CreateCharacter(int x, int y, int power)
    {
        var prefab = Instantiate(GameManager.instance.characterPrefab);
        prefab.SetOrigin(GameManager.instance.characterBoard);
        prefab.Setup(x, y, power);
        
        return prefab;
    }

    /// <summary>
    /// セットアップ
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="power"></param>
    public void Setup(int x, int y, int power)
    {
        this.x = x;
        this.y = y;
        this.power = power;

        name = $"Character ({x},{y}) power:{power}";
        transform.SetParent(GameManager.instance.characterBoard.transform, false);
        rect.sizeDelta = new Vector2(size, size);
        transform.localPosition = new Vector3(x * size + originX, y * size + originY, 0);
        UpdateText();
    }

    /// <summary>
    /// ドラッグアンドドロップ
    /// </summary>
    public void OnMouseDrag()
    {
        var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPosition.z = 1;
        rect.position = worldPosition;
    }
    /// <summary>
    /// マウスを離したら
    /// </summary>
    public void OnMouseUp()
    {
        //現在のマウスの位置からレイキャストを撃ってヒットしたものを取得します。
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
                    Debug.Log("マスを選択: " + masu.x + " - " + masu.y);
                    var move = GameManager.instance.currentGame.PlayerMoveCharacter(masu.x, masu.y, this);
                    if (move)
                    {
                        Debug.Log("移動成功: " + result[i].gameObject.name + " 次のターンへ");
                        //再度セットアップ
                        Setup(masu.x, masu.y, power);
                        return;
                    }
                }
            }
        }

        //失敗したので、元の位置に戻す。
        transform.localPosition = new Vector3(x * GameManager.BLOCK_SIZE + originX, y * GameManager.BLOCK_SIZE + originY, 0);
    }

    /// <summary>
    /// マージ可能か？
    /// </summary>
    /// <param name="targetCharacter"></param>
    /// <returns></returns>
    public bool IsEnableMarge(Character targetCharacter)
    {
        //マージ可能: ターゲットが自分自身ではない、かつパワーが等しい。
        if(!(x == targetCharacter.x && y == targetCharacter.y) && 
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
            power += targetCharacter.power;
            UpdateText();
            Destroy(targetCharacter.gameObject);
        }
    }

    /// <summary>
    /// UIを更新する。
    /// </summary>
    public void UpdateText()
    {
        hpText.text = power.ToString();
    }
}
