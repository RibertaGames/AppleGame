using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace RibertaGames 
{
    public class TutorialWindow : WindowBase
    {
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _previousButton;
        [SerializeField] private GameObject _panel1;
        [SerializeField] private GameObject _panel2;
        [SerializeField] private GameObject _panel3;

        [SerializeField] private Image[] _selectIcon;

        private int _currentPageIndex;
        private readonly int MAX_PAGE_INDEX = 3;

        /// <summary>
        /// セットアップ
        /// </summary>
        public override void Init()
        {
            base.Init();

            _nextButton.onClick.AddListener(() => 
            {
                _ChangePage(true);
            });
            _previousButton.onClick.AddListener(() => {
                _ChangePage(false);
            });

            _currentPageIndex = 0;
            _ActivePage();
        }

        public override void Open()
        {
            base.Open();

            _currentPageIndex = 0;
            _ActivePage();
        }

        /// <summary>
        /// ページを変更
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        private void _ChangePage(bool nextPage)
        {
            if (nextPage)
            {
                _currentPageIndex = Math.Min(MAX_PAGE_INDEX, _currentPageIndex + 1);
            }
            else
            {
                _currentPageIndex = Math.Max(0, _currentPageIndex - 1);
            }

            _ActivePage();
        }

        private void _ActivePage()
        {
            _panel1.SetActive(_currentPageIndex == 0);
            _panel2.SetActive(_currentPageIndex == 1);
            _panel3.SetActive(_currentPageIndex == 2);

            for(int i = 0; i < _selectIcon.Length; i++)
            {
                _selectIcon[i].color = Color.white;
            }

            _selectIcon[_currentPageIndex].color = new Color(1f, 120f /255f , 0f);
        }
    }
}