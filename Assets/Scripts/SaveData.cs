using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace RibertaGames
{
	/// <summary>
	/// セーブデータの種類
	/// </summary>
	public enum eSaveDataType
	{
		HighScore //ハイスコア
	}

	/// <summary>
	/// Json形式でセーブできるクラスを提供します。
	/// </summary>
	/// <remarks>
	/// 最初に値を設定、取得するタイミングでファイル読み出します。
	/// </remarks>
	public class SaveData
	{
		/// <summary>
		/// SingletonなSaveDatabaseクラス
		/// </summary>
		private SaveDataBase _savedatabase = null;

		private SaveDataBase savedatabase
		{
			get
			{
				if (_savedatabase == null)
				{
					//Application TODOだね
					string path = Application.persistentDataPath + "/";
					string fileName = Application.companyName + "." + Application.productName + ".Data.json";
					Debug.Log(path + fileName);
					_savedatabase = new SaveDataBase(path, fileName);
				}
				return _savedatabase;
			}
		}

		private void _Log(string key, bool getOrSet)
		{
			string s = getOrSet ? "を取得しました。" : "にセットしました。";
			Debug.Log($"セーブデータ【{key}】{s}");
		}

		#region Public Static Methods

		/// <summary>
		/// 指定したキーとT型のクラスコレクションをセーブデータに追加します。
		/// </summary>
		/// <typeparam name="T">ジェネリッククラス</typeparam>
		/// <param name="key">キー</param>
		/// <param name="list">T型のList</param>
		/// <exception cref="System.ArgumentException"></exception>
		/// <remarks>指定したキーとT型のクラスコレクションをセーブデータに追加します。</remarks>
		public void SetList<T>(string key, List<T> list)
		{
			_Log(key, false);
			savedatabase.SetList<T>(key, list);
		}

		/// <summary>
		///  指定したキーとT型のクラスコレクションをセーブデータから取得します。
		/// </summary>
		/// <typeparam name="T">ジェネリッククラス</typeparam>
		/// <param name="key">キー</param>
		/// <param name="_default">デフォルトの値</param>
		/// <exception cref="System.ArgumentException"></exception>
		/// <returns></returns>
		public List<T> GetList<T>(string key, List<T> _default)
		{
			_Log(key, true);
			return savedatabase.GetList<T>(key, _default);
		}

		/// <summary>
		///  指定したキーとT型のクラスをセーブデータに追加します。
		/// </summary>
		/// <typeparam name="T">ジェネリッククラス</typeparam>
		/// <param name="key">キー</param>
		/// <param name="_default">デフォルトの値</param>
		/// <exception cref="System.ArgumentException"></exception>
		/// <returns></returns>
		public T GetClass<T>(string key, T _default) where T : class, new()
		{
			_Log(key, true);
			return savedatabase.GetClass(key, _default);

		}

		/// <summary>
		///  指定したキーとT型のクラスコレクションをセーブデータから取得します。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="obj"></param>
		/// <exception cref="System.ArgumentException"></exception>
		public void SetClass<T>(string key, T obj) where T : class, new()
		{
			_Log(key, false);
			savedatabase.SetClass<T>(key, obj);
		}

		/// <summary>
		/// 指定されたキーに関連付けられている値を取得します。
		/// </summary>
		/// <param name="key">キー</param>
		/// <param name="value">値</param>
		/// <exception cref="System.ArgumentException"></exception>
		public void SetString(string key, string value)
		{
			_Log(key, false);
			savedatabase.SetString(key, value);
		}

		/// <summary>
		/// 指定されたキーに関連付けられているString型の値を取得します。
		/// 値がない場合、_defaultの値を返します。省略した場合、空の文字列を返します。
		/// </summary>
		/// <param name="key">キー</param>
		/// <param name="_default">デフォルトの値</param>
		/// <exception cref="System.ArgumentException"></exception>
		/// <returns></returns>
		public string GetString(string key, string _default = "")
		{
			_Log(key, true);
			return savedatabase.GetString(key, _default);
		}

		/// <summary>
		/// 指定されたキーに関連付けられているInt型の値を取得します。
		/// </summary>
		/// <param name="key">キー</param>
		/// <param name="value">デフォルトの値</param>
		/// <exception cref="System.ArgumentException"></exception>
		public void SetInt(string key, int value)
		{
			_Log(key, false);
			savedatabase.SetInt(key, value);
		}

		/// <summary>
		/// 指定されたキーに関連付けられているInt型の値を取得します。
		/// 値がない場合、_defaultの値を返します。省略した場合、0を返します。
		/// </summary>
		/// <param name="key">キー</param>
		/// <param name="_default">デフォルトの値</param>
		/// <exception cref="System.ArgumentException"></exception>
		/// <returns></returns>
		public int GetInt(string key, int _default = 0)
		{
			_Log(key, true);
			return savedatabase.GetInt(key, _default);
		}
		
		/// <summary>
		/// 指定されたキーに関連付けられているfloat型の値を取得します。
		/// </summary>
		/// <param name="key">キー</param>
		/// <param name="value">デフォルトの値</param>
		/// <exception cref="System.ArgumentException"></exception>
		public void SetFloat(string key, float value)
		{
			_Log(key, false);
			savedatabase.SetFloat(key, value);
		}

		/// <summary>
		/// 指定されたキーに関連付けられているfloat型の値を取得します。
		/// 値がない場合、_defaultの値を返します。省略した場合、0.0fを返します。
		/// </summary>
		/// <param name="key">キー</param>
		/// <param name="_default">デフォルトの値</param>
		/// <exception cref="System.ArgumentException"></exception>
		/// <returns></returns>
		public float GetFloat(string key, float _default = 0.0f)
		{
			_Log(key, true);
			return savedatabase.GetFloat(key, _default);
		}

		/// <summary>
		/// セーブデータからすべてのキーと値を削除します。
		/// </summary>
		public void Clear()
		{
			savedatabase.Clear();
		}

		/// <summary>
		/// 指定したキーを持つ値を セーブデータから削除します。
		/// </summary>
		/// <param name="key">キー</param>
		/// <exception cref="System.ArgumentException"></exception>
		public void Remove(string key)
		{
			savedatabase.Remove(key);
		}

		/// <summary>
		/// セーブデータ内にキーが存在するかを取得します。
		/// </summary>
		/// <param name="_key">キー</param>
		/// <exception cref="System.ArgumentException"></exception>
		/// <returns></returns>
		public bool ContainsKey(string _key)
		{
			return savedatabase.ContainsKey(_key);
		}

		/// <summary>
		/// セーブデータに格納されたキーの一覧を取得します。
		/// </summary>
		/// <exception cref="System.ArgumentException"></exception>
		/// <returns></returns>
		public List<string> Keys()
		{
			return savedatabase.Keys();
		}

		/// <summary>
		/// 明示的にファイルに書き込みます。
		/// </summary>
		public void Save()
		{
			savedatabase.Save();
		}

		#endregion

		#region SaveDatabase Class

		[Serializable]
		private class SaveDataBase
		{
			#region Fields

			private string _path;
			//保存先
			public string path
			{
				get { return _path; }
				set { _path = value; }
			}

			private string _fileName;
			//ファイル名
			public string fileName
			{
				get { return _fileName; }
				set { _fileName = value; }
			}

			private Dictionary<string, string> _saveDictionary;
			//keyとjson文字列を格納

			#endregion

			#region Constructor&Destructor

			public SaveDataBase(string _path, string _fileName)
			{
				this._path = _path;
				this._fileName = _fileName;
				_saveDictionary = new Dictionary<string, string>();
				Load();

			}

			#endregion

			#region Public Methods

			public void SetList<T>(string key, List<T> list)
			{
				_KeyCheck(key);
				var serializableList = new Serialization<T>(list);
				string json = JsonUtility.ToJson(serializableList);
				_saveDictionary[key] = json;
				_AfterSetFunc();
			}

			public List<T> GetList<T>(string key, List<T> _default)
			{
				_KeyCheck(key);
				if (!_saveDictionary.ContainsKey(key))
				{
					return _default;
				}
				string json = _saveDictionary[key];
				Serialization<T> deserializeList = JsonUtility.FromJson<Serialization<T>>(json);

				return deserializeList.ToList();
			}

			public T GetClass<T>(string key, T _default) where T : class, new()
			{
				_KeyCheck(key);
				if (!_saveDictionary.ContainsKey(key))
					return _default;

				string json = _saveDictionary[key];
				T obj = JsonUtility.FromJson<T>(json);
				return obj;

			}

			public void SetClass<T>(string key, T obj) where T : class, new()
			{
				_KeyCheck(key);
				string json = JsonUtility.ToJson(obj);
				_saveDictionary[key] = json;
				_AfterSetFunc();
			}

			public void SetString(string key, string value)
			{
				_KeyCheck(key);
				_saveDictionary[key] = value;
				_AfterSetFunc();
			}

			public string GetString(string key, string _default)
			{
				_KeyCheck(key);

				if (!_saveDictionary.ContainsKey(key))
					return _default;
				return _saveDictionary[key];
			}

			public void SetInt(string key, int value)
			{
				_KeyCheck(key);
				_saveDictionary[key] = value.ToString();
				_AfterSetFunc();
			}

			public int GetInt(string key, int _default)
			{
				_KeyCheck(key);
				if (!_saveDictionary.ContainsKey(key))
					return _default;
				int ret;
				if (!int.TryParse(_saveDictionary[key], out ret))
				{
					ret = 0;
				}
				return ret;
			}

			public void SetFloat(string key, float value)
			{
				_KeyCheck(key);
				_saveDictionary[key] = value.ToString();
				_AfterSetFunc();
			}

			public float GetFloat(string key, float _default)
			{
				float ret;
				_KeyCheck(key);
				if (!_saveDictionary.ContainsKey(key))
					ret = _default;

				if (!float.TryParse(_saveDictionary[key], out ret))
				{
					ret = 0.0f;
				}
				return ret;
			}

			public void Clear()
			{
				_saveDictionary.Clear();

			}

			public void Remove(string key)
			{
				_KeyCheck(key);
				if (_saveDictionary.ContainsKey(key))
				{
					_saveDictionary.Remove(key);
				}

			}

			public bool ContainsKey(string _key)
			{

				return _saveDictionary.ContainsKey(_key);
			}

			public List<string> Keys()
			{
				return _saveDictionary.Keys.ToList<string>();
			}

			public void Save()
			{
				using (StreamWriter writer = new StreamWriter(_path + _fileName, false, Encoding.GetEncoding("utf-8")))
				{
					var serialDict = new Serialization<string, string>(_saveDictionary);
					serialDict.OnBeforeSerialize();
					string dictJsonString = JsonUtility.ToJson(serialDict);
					writer.WriteLine(dictJsonString);
				}
			}

			private void _AfterSetFunc()
			{
				Save();
			}

			public void Load()
			{
				if (File.Exists(_path + _fileName))
				{
					using (StreamReader sr = new StreamReader(_path + _fileName, Encoding.GetEncoding("utf-8")))
					{
						if (_saveDictionary != null)
						{
							var sDict = JsonUtility.FromJson<Serialization<string, string>>(sr.ReadToEnd());
							sDict.OnAfterDeserialize();
							_saveDictionary = sDict.ToDictionary();
						}
					}
				}
				else { _saveDictionary = new Dictionary<string, string>(); }
			}

			public string GetJsonString(string key)
			{
				_KeyCheck(key);
				if (_saveDictionary.ContainsKey(key))
				{
					return _saveDictionary[key];
				}
				else
				{
					return null;
				}
			}

			#endregion

			#region Private Methods

			/// <summary>
			/// キーに不正がないかチェックします。
			/// </summary>
			private void _KeyCheck(string _key)
			{
				if (string.IsNullOrEmpty(_key))
				{
					throw new ArgumentException("invalid key!!");
				}
			}

			#endregion
		}

		#endregion

		#region Serialization Class

		// List<T>
		[Serializable]
		private class Serialization<T>
		{
			public List<T> target;

			public List<T> ToList()
			{
				return target;
			}

			public Serialization()
			{
			}

			public Serialization(List<T> target)
			{
				this.target = target;
			}
		}
		// Dictionary<TKey, TValue>
		[Serializable]
		private class Serialization<TKey, TValue>
		{
			public List<TKey> keys;
			public List<TValue> values;
			private Dictionary<TKey, TValue> _target;

			public Dictionary<TKey, TValue> ToDictionary()
			{
				return _target;
			}

			public Serialization()
			{
			}

			public Serialization(Dictionary<TKey, TValue> target)
			{
				this._target = target;
			}

			public void OnBeforeSerialize()
			{
				keys = new List<TKey>(_target.Keys);
				values = new List<TValue>(_target.Values);
			}

			public void OnAfterDeserialize()
			{
				int count = Math.Min(keys.Count, values.Count);
				_target = new Dictionary<TKey, TValue>(count);
				Enumerable.Range(0, count).ToList().ForEach(i => _target.Add(keys[i], values[i]));
			}
		}

		#endregion
	}
}