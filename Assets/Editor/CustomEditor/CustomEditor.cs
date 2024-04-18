using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RibertaGames
{
    public class CustomEditor : MonoBehaviour
    {
        private const string HOME = "RibertaGames";

        /// <summary>
        /// 独自ボタンを生成する。
        /// </summary>
        [MenuItem("GameObject/" + HOME + "/Button", false, priority = 0)]
        [MenuItem("GameObject/UI/Custom Button", false, 0)]
        public static void CreateCustomButton()
        {
            GameObject[] objs = Selection.gameObjects;

            //オブジェクト選択してたらその下の階層に追加する。
            if(objs != null && objs.Length != 0)
            {
                _CreateCustomButton(objs[0].transform);
            }
            else
            {
                _CreateCustomButton();
            }

            //独自ボタンを展開
            void _CreateCustomButton(Transform t = null)
            {
                //ボタンを作成
                var parent= new GameObject("Button");
                parent.transform.SetParent(t, false);
                var rt = parent.AddComponent<RectTransform>();
                var img = parent.AddComponent<Image>();
                parent.AddComponent<Button>();

                //子を作成
                var child = new GameObject("TextMesh");
                child.transform.SetParent(parent.transform,false);
                var text = child.AddComponent<TextMeshProUGUI>();
                var setting = child.AddComponent<TextWrapper>();
                text.text = "Button";
                text.fontSize = 24f;
                text.color = new Color(50f / 255f, 50f / 255f, 50f / 255f);
                text.alignment = TextAlignmentOptions.Center;

                //大きさ調整
                var vec2 = rt.sizeDelta;
                vec2.x = 160;
                vec2.y = 30;
                rt.sizeDelta = vec2;

                //デフォルト画像設定
                img.sprite = Resources.Load<Sprite>("Resources/unity_builtin_extra/UISprite");
            }
        }

        [MenuItem("RibertaGames/セーブデータ削除")]
        public static void ResetSaveData()
        {
            var savedata = new SaveData();
            savedata.Clear();
            savedata.Save();
        }
    }
}