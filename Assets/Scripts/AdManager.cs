using GoogleMobileAds.Api;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    /// <summary>
    /// �e�X�g���[�h
    /// </summary>
    public static readonly bool AD_TEST_MODE = true;

#if UNITY_ANDROID

    //�{��ID
    private static readonly string ANDROID_BANNER_ID = "";
    private static readonly string ANDROID_INTERSTITIAL_ID = "";
    private static readonly string ANDROID_REWARD_ID = "";

    //�e�X�gID
    private static readonly string TEST_ANDROID_BANNER_ID = "ca-app-pub-3940256099942544/6300978111";
    private static readonly string TEST_ANDROID_INTERSTITIAL_ID = "ca-app-pub-3940256099942544/1033173712";
    private static readonly string TEST_ANDROID_REWARD_ID = "ca-app-pub-3940256099942544/5224354917";

#elif UNITY_IPHONE

    //�{��ID
    private static readonly string IPONE_BANNER_ID = "";
    private static readonly string IPONE_INTERSTITIAL_ID = "";
    private static readonly string IPONE_REWARD_ID = "";

    //�e�X�gID
    private static readonly string TEST_IPONE_BANNER_ID = "ca-app-pub-3940256099942544/2934735716";
    private static readonly string TEST_IPONE_INTERSTITIAL_ID = "ca-app-pub-3940256099942544/4411468910";
    private static readonly string TEST_IPONE_REWARD_ID = "ca-app-pub-3940256099942544/1712485313";

#endif

    /// <summary>
    /// AdMob�̍L�����
    /// </summary>
    public enum eAdMob
    {
        Banner = 0,
        Interstitial = 1,
        Reward = 2,
    }

    /// <summary>
    /// AdMob: �o�i�[�L��
    /// </summary>
    private BannerView _bannerView;

    /// <summary>
    /// AdMob: �C���^�[�X�e�B�V�����L��
    /// </summary>
    private InterstitialAd _interstitial;

    /// <summary>
    /// AdMob: �����[�h�L��
    /// </summary>
    private RewardedAd _rewardedAd;

    /// <summary>
    /// �����[�h��
    /// </summary>
    private int _rewardCount = 0;

    public void Start()
    {
        MobileAds.Initialize(initStatus => { });
    }

    /// <summary>
    /// AdMob�̍L�����[�h
    /// </summary>
    /// <param name="adType"></param>
    public void LoadAdMob(eAdMob adType)
    {
        switch (adType)
        {
            case eAdMob.Banner:
                _LoadBanner();
                break;
            case eAdMob.Interstitial:
                _LoadInterstitial();
                break;
            case eAdMob.Reward:
                _LoadReward();
                break;
        }
    }

    /// <summary>
    /// �L���̕\��
    /// </summary>
    /// <param name="adType"></param>
    public void ShowAdMob(eAdMob adType)
    {
        switch (adType)
        {
            case eAdMob.Banner:
                _ShowBanner();
                break;
            case eAdMob.Interstitial:
                _ShowInterstitialAd();
                break;
            case eAdMob.Reward:
                _ShowReward();
                break;
        }
    }

    #region �L�����[�h
    private void _LoadBanner()
    {
        string adUnitId;
        #if UNITY_ANDROID
            if(AD_TEST_MODE) adUnitId = TEST_ANDROID_BANNER_ID;
            else adUnitId = ANDROID_BANNER_ID;
        #elif UNITY_IPHONE
            if (AD_TEST_MODE) adUnitId = TEST_IPONE_BANNER_ID;
            else adUnitId = IPONE_BANNER_ID;
        #else
            adUnitId = "unexpected_platform";
        #endif

        StartCoroutine(_HandleOnBannerAdClose());

        _bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

        _bannerView.OnBannerAdLoaded += () => { };
        _bannerView.OnAdClicked += () => { };
        _bannerView.OnAdFullScreenContentOpened += () => { };
        _bannerView.OnAdFullScreenContentClosed += () => StartCoroutine(_HandleOnBannerAdClose()); ;
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) => StartCoroutine(_HandleOnBannerAdClose());
        _bannerView.OnAdImpressionRecorded += () => { };
        _bannerView.OnAdPaid += (AdValue value) => { };
    }

    private void _LoadInterstitial()
    {
        string adUnitId;
        #if UNITY_ANDROID
            if (AD_TEST_MODE) adUnitId = TEST_ANDROID_INTERSTITIAL_ID;
            else adUnitId = ANDROID_INTERSTITIAL_ID;
        #elif UNITY_IPHONE
            if (AD_TEST_MODE) adUnitId = TEST_IPONE_INTERSTITIAL_ID;
            else adUnitId = IPONE_INTERSTITIAL_ID;
        #else
            adUnitId = "unexpected_platform";
        #endif

        InterstitialAd.Load(
            adUnitId,
            new AdRequest(),
          (InterstitialAd ad, LoadAdError loadAdError) =>
          {
              StartCoroutine(_HandleOnInterstitialAdClose());
              if (loadAdError != null || ad == null) return;

              ad.OnAdClicked += () => { };
              ad.OnAdFullScreenContentOpened += () => { };
              ad.OnAdFullScreenContentClosed += () => StartCoroutine(_HandleOnInterstitialAdClose()); ;
              ad.OnAdFullScreenContentFailed += (AdError error) => StartCoroutine(_HandleOnInterstitialAdClose());
              ad.OnAdImpressionRecorded += () => { };
              ad.OnAdPaid += (AdValue value) => { };

              _interstitial = ad;
          });
    }

    private void _LoadReward()
    {
        string adUnitId;
        #if UNITY_ANDROID
            if(AD_TEST_MODE) adUnitId = TEST_ANDROID_REWARD_ID;
            else adUnitId = ANDROID_REWARD_ID;
        #elif UNITY_IPHONE
            if(AD_TEST_MODE) adUnitId = TEST_IPONE_REWARD_ID;
            else adUnitId = IPONE_REWARD_ID;
        #else
            adUnitId = "unexpected_platform";
        #endif

        RewardedAd.Load(adUnitId, new AdRequest(),
        (RewardedAd ad, LoadAdError loadError) =>
        {
            StartCoroutine(_HandleOnRewardAdClose());
            if (loadError != null || ad == null) return;

            ad.OnAdClicked += () => { };
            ad.OnAdFullScreenContentOpened += () => { };
            ad.OnAdFullScreenContentClosed += () => StartCoroutine(_HandleOnRewardAdClose());
            ad.OnAdFullScreenContentFailed += (AdError error) => StartCoroutine(_HandleOnRewardAdClose());
            ad.OnAdImpressionRecorded += () => { };
            ad.OnAdPaid += (AdValue value) => { };

            _rewardedAd = ad;
        });
    }
    #endregion

    #region �L���\��

    /// <summary>
    /// �o�i�[�L����\��
    /// </summary>
    private void _ShowBanner()
    {
        if (_bannerView != null)
        {
            AdRequest request = new AdRequest();
            _bannerView.LoadAd(request);
        }
    }

    /// <summary>
    /// �C���^�[�X�e�B�V�����L����\��
    /// </summary>
    private void _ShowInterstitialAd()
    {
        if (_interstitial != null &&
            _interstitial.CanShowAd())
        {
            _interstitial.Show();
        }
    }

    /// <summary>
    /// �����[�h�L���\��
    /// </summary>
    private void _ShowReward()
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) => _HandleUserEarnedReward(reward));
        }
    }

    #endregion

    #region �L���I����

    private IEnumerator _HandleOnBannerAdClose()
    {
        if(_bannerView != null)
        {
            _bannerView.Destroy();
            yield return null;
            _bannerView = null;
        }
    }
    private IEnumerator _HandleOnInterstitialAdClose()
    {
        if (_interstitial != null)
        {
            _interstitial.Destroy();
            yield return null;
            _interstitial = null;
        }
    }
    private IEnumerator _HandleOnRewardAdClose()
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            //��t���[���ҋ@
            yield return null;
            _rewardedAd = null;
        }
    }

    #endregion

    /// <summary>
    /// �����[�h�L��������̕�V
    /// </summary>
    /// <param name="args"></param>
    private void _HandleUserEarnedReward(Reward args)
    {
        Debug.Log(args.Type + " - " + args.Amount);
        _rewardCount++;
    }
}
