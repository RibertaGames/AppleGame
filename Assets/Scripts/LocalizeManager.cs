using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace RibertaGames
{
    public class LocalizeManager : SingletonMonoBehaviour<LocalizeManager>
    {
        /// <summary>
        /// 現在の言語
        /// </summary>
        public SystemLanguage language { get; private set; }

        /// <summary>
        /// 現在のフォント
        /// </summary>
        public TMP_FontAsset font { get; private set; }

        /// <summary>
        /// フォントアセット
        /// </summary>
        [SerializeField] private TMP_FontAsset _fontBase;
        [SerializeField] private TMP_FontAsset _fontJP;
        [SerializeField] private TMP_FontAsset _fontCN;
        [SerializeField] private TMP_FontAsset _fontKR;

        protected override void _Init()
        {
            base._Init();
        }
        
        public void Awake()
        {
            //言語設定
            SetLanguage(Application.systemLanguage);
        }

        /// <summary>
        /// 言語を設定する。
        /// </summary>
        /// <param name="systemLanguage"></param>
        public void SetLanguage(SystemLanguage systemLanguage = SystemLanguage.English)
        {
            // 言語設定
            _SetLanguage(systemLanguage);

            // 言語適応
            _ = _ChangeSelectedLocale(language);

            // フォント設定
            //_SetFont(font);
        }

        /// <summary>
        /// 言語切り替え
        /// </summary>
        /// <param name="locale"></param>
        /// <returns></returns>
        private async UniTask _ChangeSelectedLocale(SystemLanguage locale)
        {
            LocalizationSettings.SelectedLocale = Locale.CreateLocale(locale);
            await LocalizationSettings.InitializationOperation.Task;

            Debug.Log("言語変更: " + language);
        }

        /// <summary>
        /// 言語を設定する。
        /// </summary>
        private void _SetLanguage(SystemLanguage systemLanguage)
        {
            switch (systemLanguage)
            {
                // 日本語
                case SystemLanguage.Japanese:
                    language = SystemLanguage.Japanese;
                    font = _fontJP;
                    break;

                // 中国語
                case SystemLanguage.Chinese:
                    language = SystemLanguage.ChineseSimplified;
                    font = _fontCN;
                    break;
                case SystemLanguage.ChineseSimplified:
                    language = SystemLanguage.ChineseSimplified;
                    font = _fontCN;
                    break;
                case SystemLanguage.ChineseTraditional:
                    language = SystemLanguage.ChineseTraditional;
                    font = _fontCN;
                    break;

                // 韓国語
                case SystemLanguage.Korean:
                    language = SystemLanguage.Korean;
                    font = _fontKR;
                    break;

                // フランス語
                case SystemLanguage.French:
                    language = SystemLanguage.French;
                    font = _fontBase;
                    break;

                // ドイツ語
                case SystemLanguage.German:
                    language = SystemLanguage.German;
                    font = _fontBase;
                    break;

                // インドネシア語
                case SystemLanguage.Indonesian:
                    language = SystemLanguage.Indonesian;
                    font = _fontBase;
                    break;

                // イタリア語
                case SystemLanguage.Italian:
                    language = SystemLanguage.Italian;
                    font = _fontBase;
                    break;

                // ポルトガル語
                case SystemLanguage.Portuguese:
                    language = SystemLanguage.Portuguese;
                    font = _fontBase;
                    break;

                // ロシア語
                case SystemLanguage.Russian:
                    language = SystemLanguage.Russian;
                    font = _fontBase;
                    break;

                // スペイン語
                case SystemLanguage.Spanish:
                    language = SystemLanguage.Spanish;
                    font = _fontBase;
                    break;

                // ベトナム語
                case SystemLanguage.Vietnamese:
                    language = SystemLanguage.Vietnamese;
                    font = _fontBase;
                    break;

                // 英語
                default:
                case SystemLanguage.English:
                    language = SystemLanguage.English;
                    font = _fontBase;
                    break;
            }
        }

        /// <summary>
        /// フォントを設定する。
        /// </summary>
        private void _SetFont(TMP_FontAsset font)
        {
            var list = Extensions.GetComponentsInActiveScene<TextWrapper>();
            foreach (var textWrapper in list)
            {
                textWrapper.SetupText(font);
            }
        }
    }
}