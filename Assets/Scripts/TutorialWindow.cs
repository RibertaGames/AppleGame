using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace RibertaGames 
{
    public class TutorialWindow : WindowBase
    {
        [SerializeField] private Button _nextButton;
        [SerializeField] private Button _previousButton;
        [SerializeField] private GameObject _panel1;
        [SerializeField] private GameObject _panel2;
        [SerializeField] private GameObject _panel3;

        private int _currentPageIndex;

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
                if (_currentPageIndex == 0 ||
                    _currentPageIndex == 1)
                {
                    _currentPageIndex++;
                }
            }
            else
            {
                if(_currentPageIndex == 1 ||
                    _currentPageIndex == 2)
                _currentPageIndex--;
            }

            _ActivePage();
        }

        private void _ActivePage()
        {
            _panel1.SetActive(_currentPageIndex == 0);
            _panel2.SetActive(_currentPageIndex == 1);
            _panel3.SetActive(_currentPageIndex == 2);
        }
    }
}