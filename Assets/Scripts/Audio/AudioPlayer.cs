using System;
using System.Collections.Generic;
using UnityEngine;

namespace RibertaGames
{
    /// <summary>
    /// オーディオを再生するクラス
    /// </summary>
    public class AudioPlayer
    {
        //再生用のソース
        private readonly AudioSource AUDIO_SOURCE;

        //再生した時間
        public float playedTime => AUDIO_SOURCE.time;

        //再生中のオーディオの名前
        public string currentAudioName => AUDIO_SOURCE.clip == null ? "" : AUDIO_SOURCE.clip.name;

        //再生終了後の処理
        private Action _callback;

        //状態
        public enum eState
        {
            Wait, Delay, Playing, Pause, Fading
        }

        private eState _currentState = eState.Wait;
        public eState currentState => _currentState;

        //ボリュームの基準と倍率
        private float _baseVolume, _volumeRate;
        public float currentVolume => _baseVolume * _volumeRate;

        //再生までの待ち時間
        private float _initialDelay, _currentDelay;
        public float elapsedDelay => _initialDelay - _currentDelay;

        //フェード関係
        private float _fadeProgress, _fadeDuration, _fadeFrom, _fadeTo;
        private Action _fadeCallback;

        //=================================================================================
        //初期化
        //=================================================================================

        public AudioPlayer(AudioSource audioSource)
        {
            AUDIO_SOURCE = audioSource;
            AUDIO_SOURCE.playOnAwake = false;
        }

        //=================================================================================
        //更新
        //=================================================================================

        public void Update()
        {
            //実行中の終了判定
            if (_currentState == eState.Playing && !AUDIO_SOURCE.isPlaying && Mathf.Approximately(AUDIO_SOURCE.time, 0))
            {
                _Finish();
            }
            //再生前の待機
            else if (_currentState == eState.Delay)
            {
                _Delay();
            }
            //フェード
            else if (_currentState == eState.Fading)
            {
                _Fade();
            }
        }

        private void _Delay()
        {
            _currentDelay -= Time.deltaTime;
            if (_currentDelay > 0)
            {
                return;
            }

            AUDIO_SOURCE.Play();

            if (_fadeDuration > 0)
            {
                _currentState = eState.Fading;
                Update();
            }
            else
            {
                _currentState = eState.Playing;
            }
        }

        private void _Fade()
        {
            _fadeProgress += Time.deltaTime;
            float timeRate = Mathf.Min(_fadeProgress / _fadeDuration, 1);

            AUDIO_SOURCE.volume = _GetVolume() * (_fadeFrom * (1 - timeRate) + _fadeTo * timeRate);

            if (timeRate < 1)
            {
                return;
            }

            if (_fadeTo <= 0)
            {
                _Finish();
            }
            else
            {
                _currentState = eState.Playing;
            }
            _fadeCallback?.Invoke();
        }

        //=================================================================================
        //設定、変更
        //=================================================================================

        /// <summary>
        /// ボリュームを変更する(再生中のボリュームも変更する)
        /// </summary>
        public void ChangeVolume(float baseVolume)
        {
            _baseVolume = baseVolume;
            AUDIO_SOURCE.volume = _GetVolume();
        }

        /// <summary>
        /// volumeRate変更,(主にミュート切り替え時に使う)
        /// </summary>
        public void ChangeVolumeRate(float volumeRate)
        {
            _volumeRate = volumeRate;
            AUDIO_SOURCE.volume = _GetVolume();
        }

        //ボリュームを取得
        private float _GetVolume()
        {
            return _baseVolume * _volumeRate;
        }

        //=================================================================================
        //再生開始
        //=================================================================================

        /// <summary>
        /// 再生開始
        /// </summary>
        public void Play(AudioClip audioClip, float baseVolume, float volumeRate, float delay, float pitch, bool isLoop, Action callback = null)
        {
            //停止中でなければ停止させる
            if (_currentState != AudioPlayer.eState.Wait)
            {
                Stop();
            }
            AUDIO_SOURCE.Stop();

            _volumeRate = volumeRate;
            ChangeVolume(baseVolume);

            _initialDelay = delay;
            _currentDelay = _initialDelay;

            AUDIO_SOURCE.pitch = pitch;
            AUDIO_SOURCE.loop = isLoop;
            _callback = callback;

            AUDIO_SOURCE.clip = audioClip;

            _currentState = _currentDelay > 0 ? eState.Delay : eState.Playing;
            if (_currentState == eState.Playing)
            {
                AUDIO_SOURCE.Play();
            }

            //ループ再生でなければ、再生終了のチェックをする
            if (AUDIO_SOURCE.loop)
            {
                return;
            }

            //ポーズされていたらすぐに止める
            if (_currentState == eState.Pause)
            {
                Pause();
            }
        }

        //=================================================================================
        //再生終了
        //=================================================================================

        /// <summary>
        /// 指定された名前のものを再生していたら停止
        /// </summary>
        public void Stop(string audioName)
        {
            if (audioName == currentAudioName)
            {
                Stop();
            }
        }

        /// <summary>
        /// 再生を停止する
        /// </summary>
        public void Stop()
        {
            _callback = null;
            _Finish();
        }

        //再生終了
        private void _Finish()
        {
            _currentState = eState.Wait;

            AUDIO_SOURCE.Stop();
            AUDIO_SOURCE.clip = null;

            _initialDelay = 0;
            _currentDelay = 0;
            _fadeDuration = 0;

            _callback?.Invoke();
        }

        //=================================================================================
        //一時停止、再開
        //=================================================================================

        /// <summary>
        /// 指定された名前のものを再生していたら一時停止
        /// </summary>
        public void Pause(string audioName)
        {
            if (audioName == currentAudioName)
            {
                Pause();
            }
        }

        /// <summary>
        /// 再生していたら一時停止
        /// </summary>
        public void Pause()
        {
            if (_currentState == eState.Playing || _currentState == eState.Fading)
            {
                AUDIO_SOURCE.Pause();
            }

            _currentState = eState.Pause;
        }

        /// <summary>
        /// 指定された名前のものを一時停止していたら再開
        /// </summary>
        public void UnPause(string audioName)
        {
            if (audioName == currentAudioName)
            {
                UnPause();
            }
        }

        /// <summary>
        /// 一時停止していたら再開
        /// </summary>
        public void UnPause()
        {
            if (_currentState != eState.Pause)
            {
                return;
            }

            if (AUDIO_SOURCE.clip == null)
            {
                _currentState = eState.Wait;
            }
            else if (_currentDelay > 0)
            {
                _currentState = eState.Delay;
            }
            else
            {
                AUDIO_SOURCE.UnPause();
                _currentState = _fadeDuration > 0 ? eState.Fading : eState.Playing;
            }
        }

        //=================================================================================
        //フェード
        //=================================================================================

        /// <summary>
        /// 指定された名前のものを再生していたらフェード
        /// </summary>
        public void Fade(string audioName, float duration, float from, float to, Action callback = null)
        {
            if (audioName == currentAudioName)
            {
                Fade(duration, from, to, callback);
            }
        }

        /// <summary>
        /// フェード
        /// </summary>
        public void Fade(float duration, float from, float to, Action callback = null)
        {
            if (_currentState != eState.Playing && _currentState != eState.Delay && _currentState != eState.Fading)
            {
                return;
            }

            _fadeProgress = 0;
            _fadeDuration = duration;
            _fadeFrom = from;
            _fadeTo = to;
            _fadeCallback = callback;

            if (_currentState == eState.Playing)
            {
                _currentState = eState.Fading;
            }
            if (_currentState == eState.Fading)
            {
                Update();
            }
        }

    }
}