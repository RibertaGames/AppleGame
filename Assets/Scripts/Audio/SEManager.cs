using System;
using UnityEngine;

namespace RibertaGames
{
    /// <summary>
    /// SE関連の管理をするクラス
    /// </summary>
    public class SEManager : AudioManager<SEManager>
    {

        //AudioPlayerの数(同時再生可能数)
        protected override int _audioPlayerNum => AudioManagerSetting.entity.sEAudioPlayerNum;

        //オーディオファイルが入ってるディレクトリへのパス
        public static readonly string AUDIO_DIRECTORY_PATH = "Assets/Resources/Audio/SE";

        //ボリューム倍率調整をするか
        [SerializeField]
        private bool _shouldAdjustVolumeRate = true;

        //=================================================================================
        //初期化
        //=================================================================================

        //起動時に実行される
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void _Initialize()
        {
            if (AudioManagerSetting.entity.isAutoGenerateSEManager)
            {
                new GameObject("SEManager", typeof(SEManager));
            }
        }

        protected override void _Init()
        {
            base._Init();
            var setting = AudioManagerSetting.entity;

            _LoadAudioClip(AUDIO_DIRECTORY_PATH, setting.seCacheType, setting.isReleaseSECache);

            _shouldAdjustVolumeRate = setting.shouldAdjustSeVolumeRate;
            ChangeBaseVolume(setting.seBaseVolume);
            if (!setting.isDestroySEManager)
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
        public void Play(AudioClip audioClip, float volumeRate = 1, float delay = 0, float pitch = 1, bool isLoop = false, Action callback = null)
        {
            volumeRate = _AdjustVolumeRate(volumeRate, audioClip.name);
            if (volumeRate > 0)
            {
                _RunPlayer(audioClip, volumeRate, delay, pitch, isLoop, callback);
            }
        }

        /// <summary>
        /// 再生
        /// </summary>
        public void Play(string audioPath, float volumeRate = 1, float delay = 0, float pitch = 1, bool isLoop = false, Action callback = null)
        {
            volumeRate = _AdjustVolumeRate(volumeRate, audioPath);
            if (volumeRate > 0)
            {
                _RunPlayer(audioPath, volumeRate, delay, pitch, isLoop, callback);
            }
        }

        //=================================================================================
        //取得
        //=================================================================================

        //ボリュームの倍率を調整(同じものが再生されていたらボリュームを下げて、音が爆発しないように)
        private float _AdjustVolumeRate(float volumeRate, string audioPathOrName)
        {
            if (!_shouldAdjustVolumeRate)
            {
                return volumeRate;
            }

            var audioName = _PathToName(audioPathOrName);

            //指定したものと同じものを再生しているプレイヤーを取得、なければそのままのボリュームを返す
            var targetAudioPlayers = AUDIO_PLAYER_LIST.FindAll(player => player.currentAudioName == audioName);
            if (targetAudioPlayers.Count == 0)
            {
                return volumeRate;
            }

            //同じSEが鳴ってすぐならボリュームを下げる
            foreach (var targetAudioPlayer in AUDIO_PLAYER_LIST.FindAll(player => player.currentAudioName == audioName))
            {
                if (targetAudioPlayer.currentVolume <= 0)
                {
                    continue;
                }

                float playedTime = targetAudioPlayer.playedTime;
                if (targetAudioPlayer.currentState == AudioPlayer.eState.Delay)
                {
                    playedTime += targetAudioPlayer.elapsedDelay;
                }

                if (playedTime <= 0.025f)
                {
                    return 0;
                }
                else if (playedTime <= 0.05f)
                {
                    volumeRate *= 0.8f;
                }
                else if (playedTime <= 0.1f)
                {
                    volumeRate *= 0.9f;
                }
            }

            return volumeRate;
        }

    }
}