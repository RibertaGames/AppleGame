using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RibertaGames
{
    /// <summary>
    /// 拡張メソッド: Texture2D
    /// </summary>
    public static class Texture2DExtensions
    {
        public static Sprite ToSprite(this Texture2D self)
        {
            var rect = new Rect(0, 0, self.width, self.height);
            var pivot = Vector2.one * 0.5f;
            var newSprite = Sprite.Create(self, rect, pivot);
            return newSprite;
        }
    }

    /// <summary>
    /// 拡張メソッド: RectTransform
    /// </summary>
    public static class RectTransformExtensions
    {
        /// <summary>
        /// 縦、横を設定する。
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public static void SetWidthHeightRect(this RectTransform rect, int width, int height)
        {
            var vec2 = rect.sizeDelta;
            vec2.x = width;
            vec2.y = height;
            rect.sizeDelta = vec2;
        }

        /// <summary>
        /// ストレッチの縦、横を設定する。
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        public static void SetStretchedRectOffset(this RectTransform rect, float left = 0, float top = 0, float right = 0, float bottom = 0)
        {
            rect.offsetMin = new Vector2(left, bottom);
            rect.offsetMax = new Vector2(-right, -top);
        }
    }

    public static class Extensions
    {
        //現在のシーン内から特定のコンポーネントをすべて検索する（非アクティブなオブジェクトからも取得する）
        public static T[] GetComponentsInActiveScene<T>(bool includeInactive = true)
        {
            // ActiveなSceneのRootにあるGameObject[]を取得する
            var rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();

            // 空の IEnumerable<T>
            IEnumerable<T> resultComponents = (T[])Enumerable.Empty<T>();
            foreach (var item in rootGameObjects)
            {
                // includeInactive = true を指定するとGameObjectが非活性なものからも取得する
                var components = item.GetComponentsInChildren<T>(includeInactive);
                resultComponents = resultComponents.Concat(components);
            }
            return resultComponents.ToArray();
        }
    }
}