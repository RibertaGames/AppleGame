using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RibertaGames
{
    /// <summary>
    /// オーディオを管理するマネージャクラスの設定ファイル
    /// </summary>
    public class AudioManagerSetting : ScriptableObject
    {

        //外部からアクセスするようの実体、初アクセス時にLoadする
        private static AudioManagerSetting _entity = null;
        public static AudioManagerSetting entity
        {
            get
            {
                if (_entity == null)
                {
                    _entity = Resources.Load<AudioManagerSetting>(AUDIO_MANAGER_SETTING);
                }
                return _entity;
            }
        }

        //パス
        public static readonly string AUDIO_MANAGER_SETTING = "Audio/AudioManagerSetting";

        //オーディオファイルへのパスを定数で管理するクラスを自動更新するか
        [SerializeField]
        private bool _isAutoUpdateAudioPath = true;
        public bool isAutoUpdateAudioPath => _isAutoUpdateAudioPath;

        //同時再生可能数
        [SerializeField]
        private int _bgmAudioPlayerNum = 3, _seAudioPlayerNum = 10;
        public int bGMAudioPlayerNum => _bgmAudioPlayerNum;
        public int sEAudioPlayerNum => _seAudioPlayerNum;

        //基準ボリューム
        [SerializeField]
        private float _bgmBaseVolume = 1f, _seBaseVolume = 1f;
        public float bGMBaseVolume => _bgmBaseVolume;
        public float sEBaseVolume => _seBaseVolume;

        //SEのボリューム倍率調整をするか
        [SerializeField]
        private bool _shouldAdjustSEVolumeRate = true;
        public bool shouldAdjustSeVolumeRate => _shouldAdjustSEVolumeRate;

        //BGMManager、SEManagerを自動生成するか
        [SerializeField]
        private bool _isAutoGenerateBGMManager = true, _isAutoGenerateSEManager = true;
        public bool isAutoGenerateBGMManager => _isAutoGenerateBGMManager;
        public bool isAutoGenerateSEManager => _isAutoGenerateSEManager;

        //BGMManager、SEManagerを破棄するか
        [SerializeField]
        private bool _isDestroyBGMManager = false, _isDestroySEManager = false;
        public bool isDestroyBGMManager => _isDestroyBGMManager;
        public bool isDestroySEManager => _isDestroySEManager;

        //AudioClipのキャッシュ設定
        [SerializeField]
        private eAudioCacheType _bgmCacheType = eAudioCacheType.All, _seCacheType = eAudioCacheType.All;
        public eAudioCacheType bgmCacheType => _bgmCacheType;
        public eAudioCacheType seCacheType => _seCacheType;

        [SerializeField]
        private bool _isReleaseBGMCache = false, _isReleaseSECache = false;
        public bool isReleaseBGMCache => _isReleaseBGMCache;
        public bool isReleaseSECache => _isReleaseSECache;

        //オーディオファイルの自動設定
        [SerializeField]
        private bool _isAutoUpdateBGMSetting = true, _isAutoUpdateSESetting = true;
        public bool isAutoUpdateBGMSetting => _isAutoUpdateBGMSetting;
        public bool isAutoUpdateSESetting => _isAutoUpdateSESetting;

        [SerializeField]
        private bool _forceToMonoForBGM = true, _forceToMonoForSE = true;
        public bool forceToMonoForBGM => _forceToMonoForBGM;
        public bool forceToMonoForSE => _forceToMonoForSE;

        [SerializeField]
        private bool _normalizeForBGM = true, _normalizeForSE = true;
        public bool normalizeForBGM => _normalizeForBGM;
        public bool normalizeForSE => _normalizeForSE;

        [SerializeField]
        private bool _ambisonicForBGM = false, _ambisonicForSE = false;
        public bool ambisonicForBGM => _ambisonicForBGM;
        public bool ambisonicForSE => _ambisonicForSE;

        [SerializeField]
        private bool _loadInBackgroundForBGM = false, _loadInBackgroundForSE = false;
        public bool loadInBackgroundForBGM => _loadInBackgroundForBGM;
        public bool loadInBackgroundForSE => _loadInBackgroundForSE;

        [SerializeField]
        private AudioClipLoadType _loadTypeForBGM = AudioClipLoadType.Streaming, _loadTypeForSE = AudioClipLoadType.CompressedInMemory;
        public AudioClipLoadType loadTypeForBGM => _loadTypeForBGM;
        public AudioClipLoadType loadTypeForSE => _loadTypeForSE;

        [SerializeField]
        private float _qualityForBGM = 0.3f, _qualityForSE = 0.3f;
        public float qualityForBGM => _qualityForBGM;
        public float qualityForSE => _qualityForSE;

        [SerializeField]
        private AudioCompressionFormat _compressionFormatForBGM = AudioCompressionFormat.Vorbis, _compressionFormatForSE = AudioCompressionFormat.Vorbis;
        public AudioCompressionFormat compressionFormatForBGM => _compressionFormatForBGM;
        public AudioCompressionFormat compressionFormatForSE => _compressionFormatForSE;

#if UNITY_EDITOR
        [SerializeField]
        private AudioSampleRateSetting _sampleRateSettingForBGM = AudioSampleRateSetting.OptimizeSampleRate, _sampleRateSettingForSE = AudioSampleRateSetting.OptimizeSampleRate;
        public AudioSampleRateSetting sampleRateSettingForBGM => _sampleRateSettingForBGM;
        public AudioSampleRateSetting sampleRateSettingForSE => _sampleRateSettingForSE;
#endif

    }
}