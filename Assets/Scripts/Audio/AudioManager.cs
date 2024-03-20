using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RibertaGames
{
	//オーディオのキャッシュの種類
	public enum eAudioCacheType
	{
		None, All, Used
	}

	/// <summary>
	/// オーディオを管理するマネージャクラスの親クラス
	/// </summary>
	public abstract class AudioManager<T> : SingletonMonoBehaviour<T> where T : MonoBehaviourWithInit
	{
		//キャッシュの種類
		private eAudioCacheType _cacheType;

		//キャッシュしているAudioClip
		private Dictionary<string, AudioClip> _audioClipDict = new Dictionary<string, AudioClip>();

		//実際にオーディオを再生するクラス
		protected readonly List<AudioPlayer> AUDIO_PLAYER_LIST = new List<AudioPlayer>();

		//次に再生するプレイヤーの番号
		private int _nextAudioPlayerNo = 0;

		//AudioPlayerの数(同時再生可能数)
		protected abstract int _audioPlayerNum { get; }
		public int audioPlayerNum => _audioPlayerNum;

		//ボリュームの基準と倍率
		private float _baseVolume = 1f;

		//=================================================================================
		//初期化、破棄
		//=================================================================================

		protected override void _Init()
		{
			base._Init();

			//AudioSourceとAudioPlayer生成
			for (int i = 0; i < _audioPlayerNum; i++)
			{
				AUDIO_PLAYER_LIST.Add(new AudioPlayer(gameObject.AddComponent<AudioSource>()));
			}
		}

		//指定したディレクトリにあるAudioClipをロードし、キャッシュ
		protected void _LoadAudioClip(string directoryPath, eAudioCacheType cacheType, bool isReleaseBGMCache)
		{
			_cacheType = cacheType;
			if (_cacheType == eAudioCacheType.All)
			{
				_audioClipDict = Resources.LoadAll<AudioClip>(directoryPath).ToDictionary(clip => clip.name, clip => clip);
			}
			if (_cacheType == eAudioCacheType.Used && isReleaseBGMCache)
			{
				SceneManager.sceneUnloaded += (scene) =>
				{
					_audioClipDict.Clear();
				};
			}
		}


		//=================================================================================
		//更新
		//=================================================================================

		public void Update()
		{
			foreach (var audioPlayer in AUDIO_PLAYER_LIST)
			{
				if (audioPlayer.currentState != AudioPlayer.eState.Wait)
				{
					audioPlayer.Update();
				}
			}
		}

		//=================================================================================
		//設定、変更
		//=================================================================================

		/// <summary>
		/// ボリュームの基準を変更する(再生中のボリュームも変更する)
		/// </summary>
		public void ChangeBaseVolume(float baseVolume)
		{
			_baseVolume = baseVolume;
			AUDIO_PLAYER_LIST.Where(player => player.currentState != AudioPlayer.eState.Wait).ToList()
				.ForEach(player => player.ChangeVolume(_baseVolume));
		}

		//=================================================================================
		//取得、判定
		//=================================================================================

		/// <summary>
		/// 再生中のオーディオの名前を全て取得
		/// </summary>
		public List<string> GetCurrentAudioNames()
		{
			return AUDIO_PLAYER_LIST.Where(player => player.currentState != AudioPlayer.eState.Wait).Select(player => player.currentAudioName).ToList();
		}

		/// <summary>
		/// 再生しているものがあるか
		/// </summary>
		public bool IsPlaying()
		{
			return GetCurrentAudioNames().Count > 0;
		}

		//=================================================================================
		//再生開始
		//=================================================================================

		/// <summary>
		/// 再生開始
		/// </summary>
		protected void _RunPlayer(AudioClip audioClip, float volumeRate, float delay, float pitch, bool isLoop, Action callback = null)
		{
			_GetNextAudioPlayer().Play(audioClip, _baseVolume, volumeRate, delay, pitch, isLoop, callback);
		}

		/// <summary>
		/// 再生開始
		/// </summary>
		protected void _RunPlayer(string audioPath, float volumeRate, float delay, float pitch, bool isLoop, Action callback = null)
		{
			_RunPlayer(_GetAudioClip(audioPath), volumeRate, delay, pitch, isLoop, callback);
		}

		//オーディオのパスを名前に変換
		protected string _PathToName(string audioPath)
		{
			return Path.GetFileNameWithoutExtension(audioPath);
		}

		//指定したパスのAudioClipを取得
		private AudioClip _GetAudioClip(string audioPath)
		{
			string audioName = _PathToName(audioPath);

			if (_audioClipDict.ContainsKey(audioName))
			{
				return _audioClipDict[audioName];
			}

			var audioClip = Resources.Load<AudioClip>(audioPath);
			if (audioClip == null)
			{
				Debug.LogError(audioPath + " not found");
			}

			if (_cacheType == eAudioCacheType.Used)
			{
				_audioClipDict[audioName] = audioClip;
			}

			return audioClip;
		}

		//次に再生するAudioPlayerを取得
		private AudioPlayer _GetNextAudioPlayer()
		{
			var audioPlayer = AUDIO_PLAYER_LIST[_nextAudioPlayerNo];

			_nextAudioPlayerNo++;
			if (_nextAudioPlayerNo >= AUDIO_PLAYER_LIST.Count)
			{
				_nextAudioPlayerNo = 0;
			}

			return audioPlayer;
		}

		//=================================================================================
		//再生終了
		//=================================================================================

		/// <summary>
		/// 指定されたパスまたは名前のものが再生されていたら停止
		/// </summary>
		public void Stop(string audioPathOrName)
		{
			var audioName = _PathToName(audioPathOrName);
			AUDIO_PLAYER_LIST.ForEach(player => player.Stop(audioName));
		}

		/// <summary>
		/// 全ての再生を停止する
		/// </summary>
		public void Stop()
		{
			AUDIO_PLAYER_LIST.ForEach(player => player.Stop());
		}

		//=================================================================================
		//フェード
		//=================================================================================

		/// <summary>
		/// 指定されたパスまたは名前のものが再生されていたらフェードする
		/// </summary>
		public void Fade(string audioPathOrName, float duration, float from, float to, Action callback)
		{
			var audioName = _PathToName(audioPathOrName);
			AUDIO_PLAYER_LIST.ForEach(player => player.Fade(audioName, duration, from, to, callback));
		}

		/// <summary>
		/// 指定されたパスまたは名前のものが再生されていたらフェードアウトする
		/// </summary>
		public void FadeOut(string audioPathOrName, float duration = 1f, Action callback = null)
		{
			var audioName = _PathToName(audioPathOrName);
			Fade(audioName, duration, 1, 0, callback);
		}

		/// <summary>
		/// 指定されたパスまたは名前のものが再生されていたらフェードインする
		/// </summary>
		public void FadeIn(string audioPathOrName, float duration = 1f, Action callback = null)
		{
			var audioName = _PathToName(audioPathOrName);
			Fade(audioName, duration, 0, 1, callback);
		}

		/// <summary>
		/// 再生しているものをフェードする
		/// </summary>
		public void Fade(float duration, float from, float to, Action callback)
		{
			AUDIO_PLAYER_LIST.ForEach(player => player.Fade(duration, from, to, callback));
		}

		/// <summary>
		/// 再生しているものをフェードアウトする
		/// </summary>
		public void FadeOut(float duration = 1f, Action callback = null)
		{
			Fade(duration, 1, 0, callback);
		}

		/// <summary>
		/// 再生しているものをフェードインする
		/// </summary>
		public void FadeIn(float duration = 1f, Action callback = null)
		{
			Fade(duration, 0, 1, callback);
		}

		//=================================================================================
		//一時停止、再開
		//=================================================================================

		/// <summary>
		/// 指定されたパスまたは名前のものが再生されていたら一時停止
		/// </summary>
		public void Pause(string audioPathOrName)
		{
			var audioName = _PathToName(audioPathOrName);
			AUDIO_PLAYER_LIST.ForEach(player => player.Pause(audioName));
		}

		/// <summary>
		/// 全ての再生を一時停止
		/// </summary>
		public void Pause()
		{
			AUDIO_PLAYER_LIST.ForEach(player => player.Pause());
		}

		/// <summary>
		/// 指定されたパスまたは名前のものが一時停止されていたら再開
		/// </summary>
		public void UnPause(string audioPathOrName)
		{
			var audioName = _PathToName(audioPathOrName);
			AUDIO_PLAYER_LIST.ForEach(player => player.UnPause(audioName));
		}

		/// <summary>
		/// 一時停止しているものを全て再開
		/// </summary>
		public void UnPause()
		{
			AUDIO_PLAYER_LIST.ForEach(player => player.UnPause());
		}

	}
}