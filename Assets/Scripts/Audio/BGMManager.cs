using System;
using UnityEngine;

namespace RibertaGames
{
    /// <summary>
    /// BGM関連の管理をするクラス
    /// </summary>
    public class BGMManager : AudioManager<BGMManager>
    {

        //AudioPlayerの数(同時再生可能数)
        protected override int _audioPlayerNum => AudioManagerSetting.entity.bGMAudioPlayerNum;

        //再生に使ってるプレイヤークラス
        private AudioPlayer audioPlayer => AUDIO_PLAYER_LIST[0];

        //オーディオファイルが入ってるディレクトリへのパス
        public static readonly string AUDIO_DIRECTORY_PATH = "Assets/Resources/Audio/BGM";

        //=================================================================================
        //初期化
        //=================================================================================

        //起動時に実行される
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void _Initialize()
        {
            if (AudioManagerSetting.entity.isAutoGenerateBGMManager)
            {
                new GameObject("BGMManager", typeof(BGMManager));
            }
        }

        protected override void _Init()
        {
            base._Init();
            var setting = AudioManagerSetting.entity;

            _LoadAudioClip(AUDIO_DIRECTORY_PATH, setting.bgmCacheType, setting.isReleaseBGMCache);

            ChangeBaseVolume(setting.bgmBaseVolume / 4f);
            if (!setting.isDestroyBGMManager)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        //=================================================================================
        //再生
        //=================================================================================

        /// <summary>
        /// 再生
        /// </summary>
        public void Play(AudioClip audioClip, float volumeRate = 1, float delay = 0, float pitch = 1, bool isLoop = true, bool allowsDuplicate = false)
        {
            //重複が許可されてない場合は、既に再生しているものを止める
            if (!allowsDuplicate)
            {
                Stop();
            }
            _RunPlayer(audioClip, volumeRate, delay, pitch, isLoop);
        }

        /// <summary>
        /// 再生
        /// </summary>
        public void Play(string audioPath, float volumeRate = 1, float delay = 0, float pitch = 1, bool isLoop = true, bool allowsDuplicate = false)
        {
            //重複が許可されてない場合は、既に再生しているものを止める
            if (!allowsDuplicate)
            {
                Stop();
            }
            _RunPlayer(audioPath, volumeRate, delay, pitch, isLoop);
        }

        /// <summary>
        /// 再生中のものをフェードアウトさせて、次のを再生開始する
        /// </summary>
        public void FadeOut(string audioPath, float fadeOutDuration = 1f, float volumeRate = 1, float delay = 0, float pitch = 1, bool isLoop = true, Action callback = null)
        {
            if (!IsPlaying())
            {
                Play(audioPath, volumeRate, delay, pitch, isLoop);
                return;
            }

            FadeOut(fadeOutDuration, () =>
            {
                Play(audioPath, volumeRate, delay, pitch, isLoop);
                callback?.Invoke();
            });
        }

        /// <summary>
        /// 再生中のものを即停止させて、次のをフェードインで開始する
        /// </summary>
        public void FadeIn(string audioPath, float fadeInDuration = 1f, float volumeRate = 1, float delay = 0, float pitch = 1, bool isLoop = true, Action callback = null)
        {
            Stop();
            Play(audioPath, volumeRate, delay, pitch, isLoop);
            FadeIn(audioPath, fadeInDuration, callback);
        }

        /// <summary>
        /// 再生中のものをフェードアウトさせて、次のをフェードインで開始する
        /// </summary>
        public void FadeOutAndFadeIn(string audioPath, float fadeOutDuration = 1f, float fadeInDuration = 1f, float volumeRate = 1, float delay = 0, float pitch = 1, bool isLoop = true, Action callback = null)
        {
            if (!IsPlaying())
            {
                FadeIn(audioPath, fadeInDuration, volumeRate, delay, pitch, isLoop, callback);
                return;
            }

            FadeOut(fadeOutDuration, () =>
            {
                FadeIn(audioPath, fadeInDuration, volumeRate, delay, pitch, isLoop, callback);
            });
        }

        /// <summary>
        /// 再生中のものをフェードアウトさせて、同時に次のをフェードインで開始する
        /// </summary>
        public void CrossFade(string audioPath, float fadeDuration = 1f, float volumeRate = 1, float delay = 0, float pitch = 1, bool isLoop = true, Action callback = null)
        {
            if (GetCurrentAudioNames().Count >= audioPlayerNum)
            {
                Debug.LogWarning("クロスフェードするにはAudio Player Numが足りません");
            }

            foreach (var currentAudioName in GetCurrentAudioNames())
            {
                FadeOut(currentAudioName, fadeDuration);
            }

            Play(audioPath, volumeRate, delay, pitch, isLoop, allowsDuplicate: true);
            FadeIn(audioPath, fadeDuration, callback);
        }
    }
}