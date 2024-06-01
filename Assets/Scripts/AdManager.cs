using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RibertaGames
{

    public class AdManager : SingletonMonoBehaviour<AdManager>
    {
        private AdManager() { }

        /// <summary>
        /// テストモード
        /// </summary>
        private readonly bool AD_TEST_MODE = false;

#if UNITY_ANDROID

        //本番ID
        private readonly string ANDROID_BANNER_ID = "ca-app-pub-8184775472875165/3722688788";
        private readonly string ANDROID_INTERSTITIAL_ID = "ca-app-pub-8184775472875165/9615132889";
        private readonly string ANDROID_REWARD_ID = "";

        //テストID
        private readonly string TEST_ANDROID_BANNER_ID = "ca-app-pub-3940256099942544/6300978111";
        private readonly string TEST_ANDROID_INTERSTITIAL_ID = "ca-app-pub-3940256099942544/1033173712";
        private readonly string TEST_ANDROID_REWARD_ID = "ca-app-pub-3940256099942544/5224354917";

#elif UNITY_IPHONE

        //本番ID
        private readonly string IPONE_BANNER_ID = "ca-app-pub-8184775472875165/9886008890";
        private readonly string IPONE_INTERSTITIAL_ID = "ca-app-pub-8184775472875165/6506587965";
        private readonly string IPONE_REWARD_ID = "";

        //テストID
        private readonly string TEST_IPONE_BANNER_ID = "ca-app-pub-3940256099942544/2934735716";
        private readonly string TEST_IPONE_INTERSTITIAL_ID = "ca-app-pub-3940256099942544/4411468910";
        private readonly string TEST_IPONE_REWARD_ID = "ca-app-pub-3940256099942544/1712485313";

#endif

        private string _GetID(eAdMob eAdMob)
        {
            string adUnitId = "";
#if UNITY_ANDROID
            switch (eAdMob)
            {
                case eAdMob.Banner:
                    if (AD_TEST_MODE) adUnitId = TEST_ANDROID_BANNER_ID;
                    else adUnitId = ANDROID_BANNER_ID;
                    break;
                case eAdMob.Interstitial:
                    if (AD_TEST_MODE) adUnitId = TEST_ANDROID_INTERSTITIAL_ID;
                    else adUnitId = ANDROID_INTERSTITIAL_ID;
                    break;
                case eAdMob.Reward:
                    if (AD_TEST_MODE) adUnitId = TEST_ANDROID_REWARD_ID;
                    else adUnitId = ANDROID_REWARD_ID;
                    break;
            }
#elif UNITY_IPHONE
      switch (eAdMob)
        {
            case eAdMob.Banner:
                if (AD_TEST_MODE) adUnitId = TEST_IPONE_BANNER_ID;
                else adUnitId = IPONE_BANNER_ID;
                break;
            case eAdMob.Interstitial:
                if (AD_TEST_MODE) adUnitId = TEST_IPONE_INTERSTITIAL_ID;
                else adUnitId = IPONE_INTERSTITIAL_ID;
                break;
            case eAdMob.Reward:
                if (AD_TEST_MODE) adUnitId = TEST_IPONE_REWARD_ID;
                else adUnitId = IPONE_REWARD_ID;
                break;
        }             
#endif
            return adUnitId;
        }

        /// <summary>
        /// AdMobの広告種類
        /// </summary>
        public enum eAdMob
        {
            Banner = 0,
            Interstitial = 1,
            Reward = 2,
        }

        /// <summary>
        /// AdMob: バナー広告
        /// </summary>
        private BannerView _bannerView;

        /// <summary>
        /// AdMob: インタースティシャル広告
        /// </summary>
        private InterstitialAd _interstitial;

        /// <summary>
        /// AdMob: リワード広告
        /// </summary>
        private RewardedAd _rewardedAd;

        public void Start()
        {
            MobileAds.Initialize(initStatus => { });

            // バナー表示
            LoadAndShowAdmobBanner();

            // インタースティシャル広告をロード
            LoadAdmobInterstitial();
        }

        /// <summary>
        /// バナー広告表示
        /// </summary>
        public void LoadAndShowAdmobBanner()
        {
            string adUnitId = _GetID(eAdMob.Banner);
            AdSize adaptiveSize = AdSize.GetPortraitAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            _bannerView = new BannerView(adUnitId, adaptiveSize, AdPosition.Bottom);
            AdRequest request = new AdRequest.Builder().Build();
            _bannerView.LoadAd(request);
        }

        private int _retryCount = 0;
        private int _interstitialCount = 2; // 最初は表示させる。

        /// <summary>
        /// インタースティシャル広告をロード
        /// </summary>
        public void LoadAdmobInterstitial()
        {
            string adUnitId = _GetID(eAdMob.Interstitial);
            _interstitial = new InterstitialAd(adUnitId);
            _interstitial.OnAdLoaded += _HandleOnAdLoaded;
            _interstitial.OnAdFailedToLoad += _HandleOnAdFailedToLoad;
            AdRequest request = new AdRequest.Builder().Build();
            _interstitial.LoadAd(request);

            void _HandleOnAdLoaded(object sender, EventArgs args)
            {
                _retryCount = 0;
            }

            // 広告の読み込みが失敗したときの処理
            void _HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
            {
                if (_retryCount < 10)
                {
                    _retryCount++;
                    StartCoroutine(_RetryLoadAd());
                }
                IEnumerator _RetryLoadAd()
                {
                    _interstitial.OnAdLoaded -= _HandleOnAdLoaded;
                    _interstitial.OnAdFailedToLoad -= _HandleOnAdFailedToLoad;
                    _interstitial.Destroy();
                    yield return new WaitForSeconds(2);
                    LoadAdmobInterstitial();
                }
            }
        }

        /// <summary>
        /// インタースティシャル広告を表示する
        /// </summary>
        public void ShowAdmobInterstitial(Action getReward)
        {
            // 3回に一回インタースティシャル広告を表示させる
            _interstitialCount++;
            if(_interstitialCount % 3 != 0)
            {
                getReward();
                return;
            }

            if (_interstitial != null && _interstitial.IsLoaded())
            {
                _interstitial.OnAdClosed += _HandleOnAdClosed;
                _interstitial.OnAdFailedToShow += _HandleOnAdFailedToShow;
                _interstitial.Show();
            }
            else
            {
                getReward();
            }

            // 広告の表示が失敗したときの処理
            void _HandleOnAdFailedToShow(object sender, AdErrorEventArgs args)
            {
                _interstitial.OnAdClosed -= _HandleOnAdClosed;
                _interstitial.OnAdFailedToShow -= _HandleOnAdFailedToShow;
                _interstitial.Destroy();
                LoadAdmobInterstitial();
                getReward();
            }

            // 閉じた時
            void _HandleOnAdClosed(object sender, EventArgs args)
            {
                _interstitial.OnAdClosed -= _HandleOnAdClosed;
                _interstitial.OnAdFailedToShow -= _HandleOnAdFailedToShow;
                _interstitial.Destroy();
                LoadAdmobInterstitial();
                getReward();
            }
        }

        /// <summary>
        /// リワード広告
        /// </summary>
        private void _AdmobReward()
        {
            string adUnitId = _GetID(eAdMob.Reward);

            _rewardedAd = new RewardedAd(adUnitId);
            _rewardedAd.OnAdLoaded += _HandleRewardedAdLoaded;
            _rewardedAd.OnUserEarnedReward += _HandleUserEarnedReward;
            _rewardedAd.OnAdClosed += _HandleRewardedAdClosed;
            AdRequest request = new AdRequest.Builder().Build();
            _rewardedAd.LoadAd(request);

            // リワード閉じる
            void _HandleRewardedAdClosed(object sender, EventArgs args)
            {
                //AdmobReward();
            }

            // リワードゲット
            void _HandleUserEarnedReward(object sender, Reward args)
            {
            }

            // リワードロード後に表示
            void _HandleRewardedAdLoaded(object sender, EventArgs args)
            {
                if (_rewardedAd.IsLoaded())
                {
                    _rewardedAd.Show();
                }
            }
        }
    }
}