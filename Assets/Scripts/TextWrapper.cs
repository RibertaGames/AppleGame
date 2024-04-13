using UnityEngine;
using TMPro;

namespace RibertaGames
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextWrapper : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        public void Reset()
        {
            _text = gameObject.GetComponent<TextMeshProUGUI>();
        }

        public void Start()
        {
            SetupText();
        }

        public void SetupText(TMP_FontAsset font = null)
        {
            if (_text == null)
            {
                _text = gameObject.GetComponent<TextMeshProUGUI>();
            }

            //フォントを設定する。
            if (_text != null && font != null)
            {
                _text.font = font;
            }
        }
    }
}