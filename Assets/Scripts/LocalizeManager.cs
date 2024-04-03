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
        /// ���݂̌���
        /// </summary>
        public SystemLanguage language { get; private set; }

        /// <summary>
        /// ���݂̃t�H���g
        /// </summary>
        public TMP_FontAsset font { get; private set; }

        /// <summary>
        /// �t�H���g�A�Z�b�g
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
            //����ݒ�
            SetLanguage(Application.systemLanguage);
        }

        /// <summary>
        /// �����ݒ肷��B
        /// </summary>
        /// <param name="systemLanguage"></param>
        public void SetLanguage(SystemLanguage systemLanguage)
        {
            // ����ݒ�
            _SetLanguage(systemLanguage);

            // ����K��
            _ = _ChangeSelectedLocale(language);

            // �t�H���g�ݒ�
            _SetFont(font);
        }

        /// <summary>
        /// ����؂�ւ�
        /// </summary>
        /// <param name="locale"></param>
        /// <returns></returns>
        private async UniTask _ChangeSelectedLocale(SystemLanguage locale)
        {
            LocalizationSettings.SelectedLocale = Locale.CreateLocale(locale);
            await LocalizationSettings.InitializationOperation.Task;

            Debug.Log("����ύX: " + language);
        }

        /// <summary>
        /// �����ݒ肷��B
        /// </summary>
        private void _SetLanguage(SystemLanguage systemLanguage)
        {
            switch (systemLanguage)
            {
                // ���{��
                case SystemLanguage.Japanese:
                    language = SystemLanguage.Japanese;
                    font = _fontJP;
                    break;

                // ������
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

                // �؍���
                case SystemLanguage.Korean:
                    language = SystemLanguage.Korean;
                    font = _fontKR;
                    break;

                // �t�����X��
                case SystemLanguage.French:
                    language = SystemLanguage.French;
                    font = _fontBase;
                    break;

                // �h�C�c��
                case SystemLanguage.German:
                    language = SystemLanguage.German;
                    font = _fontBase;
                    break;

                // �C���h�l�V�A��
                case SystemLanguage.Indonesian:
                    language = SystemLanguage.Indonesian;
                    font = _fontBase;
                    break;

                // �C�^���A��
                case SystemLanguage.Italian:
                    language = SystemLanguage.Italian;
                    font = _fontBase;
                    break;

                // �|���g�K����
                case SystemLanguage.Portuguese:
                    language = SystemLanguage.Portuguese;
                    font = _fontBase;
                    break;

                // ���V�A��
                case SystemLanguage.Russian:
                    language = SystemLanguage.Russian;
                    font = _fontBase;
                    break;

                // �X�y�C����
                case SystemLanguage.Spanish:
                    language = SystemLanguage.Spanish;
                    font = _fontBase;
                    break;

                // �x�g�i����
                case SystemLanguage.Vietnamese:
                    language = SystemLanguage.Vietnamese;
                    font = _fontBase;
                    break;

                // �p��
                default:
                case SystemLanguage.English:
                    language = SystemLanguage.English;
                    font = _fontBase;
                    break;
            }
        }

        /// <summary>
        /// �t�H���g��ݒ肷��B
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