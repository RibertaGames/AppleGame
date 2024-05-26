using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour
{
    /// <summary>
    /// アプリ開始時
    /// </summary>
    public void Start()
    {
        _InitializeSetting();
    }

    private void _InitializeSetting()
    {
        // フレームレートを固定
        Time.fixedDeltaTime = 1f / GameDefine.FRAME_COUNT;
        Application.targetFrameRate = GameDefine.FRAME_COUNT;
    }

    public void StoreReview(string review_type)
    {
#if UNITY_IOS
            UnityEngine.iOS.Device.RequestStoreReview();
#elif UNITY_ANDROID
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.Liberta.ShogiFrontier");
#endif
    }
}
