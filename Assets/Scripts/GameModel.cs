using RibertaGames;
using System;
using System.Collections.Generic;
using UniRx;
using Cysharp.Threading.Tasks;

namespace RibertaGames
{
    /// <summary>
    /// ギミック種類
    /// </summary>
    public enum eGimickType
    {
        None,
        Enemy,
        Key,
        Timer,
    }

    /// <summary>
    /// 手持ちのアイテム
    /// </summary>
    [Flags]
    public enum eItem
    {
        Timer = 1,
        Key = 2,
    }

    public struct EntityInfo
    {
        public EntityInfo(int x, int y, int power, int boardX, int boardY, eGimickType gimickType = eGimickType.None)
        {
            this.x = x;
            this.y = y;
            this.power = power;
            this.boardX = boardX;
            this.boardY = boardY;
            this.gimickType = gimickType;
        }

        public int x;
        public int y;
        public int power;
        public int boardX;
        public int boardY;
        public eGimickType gimickType;
    }

    public class GameModel
    {
        private ReactiveProperty<int> _score = new ReactiveProperty<int>();
        private ReactiveProperty<int> _highScore = new ReactiveProperty<int>();
        private ReactiveProperty<int> _destroyCount = new ReactiveProperty<int>();
        private ReactiveProperty<int> _currentTurn = new ReactiveProperty<int>();
        private int _totalDamege;

        private Subject<Unit> _gameEnd = new Subject<Unit>();
        private Subject<EntityInfo> _createCharacter = new Subject<EntityInfo>();
        private Subject<EntityInfo> _createEnemy = new Subject<EntityInfo>();

        public Enemy[,] enemies;
        public Character[,] characters;
        public Character nextCharacter;

        private eItem _currentItem = new eItem();
        private eGameState _gameState = eGameState.Initialize;
        private int _noKeyTurn;
        private Random _random;
        private SaveData _saveData;

        #region ゲーム定数

        /// <summary>
        /// 盤面のマス
        /// </summary>
        private readonly int ENEMY_MASU_X = 7;
        private readonly int ENEMY_MASU_Y = 7;
        private readonly int CHARACTER_MASU_X = 7;
        private readonly int CHARACTER_MASU_Y = 2;

        /// <summary>
        /// エネミーのスポーンするY座標
        /// </summary>
        public readonly int SPAWN_ENEMY_MASU_Y = 6;

        /// <summary>
        /// キャラクター側の進化可能な数字
        /// </summary>
        private readonly int[] CHARACTER_ENABLE_NUM = new int[] { 2, 4, 8, 16, 32, 64 };

        /// <summary>
        /// 次の生成キャラクターの固定X,Y
        /// </summary>
        private readonly int NEXT_CHARACTER_X = 3;
        private readonly int NEXT_CHARACTER_Y = -1;

        /// <summary>
        /// 一体撃破: 10ポイント
        /// </summary>
        private readonly int DESTROY_BONUS = 10;

        /// <summary>
        /// 一ターン: 5ポイント
        /// </summary>
        private readonly int TURN_BONUS = 5;

        /// <summary>
        /// 100ダメージ毎: 1ポイント
        /// </summary>
        private readonly int DAMEGE_BONUS = 100;

        /// <summary>
        /// 30ターン毎にボーナスステージ: 鍵か時計
        /// </summary>
        private readonly int BONUS_STAGE_TURN = 30;

        /// <summary>
        /// 救済処置: 5ターン鍵が出なかったら鍵を出す。
        /// </summary>
        private readonly int NO_KEY_TURN = 5;

        /// <summary>
        /// エネミーの出現確率
        /// </summary>
        private readonly int ENEMY_POP_PERCENT = 80;

        /// <summary>
        /// 鍵の出現確率
        /// </summary>
        private readonly int KEY_POP_PERCENT = 18;

        /// <summary>
        /// タイマーの出現確率
        /// </summary>
        private readonly int TIMER_POP_PERCENT = 2;

        /// <summary>
        /// 出現する味方が常に最低レベルで登場
        /// </summary>
        private readonly bool ALWAYS_MIN_NUMBER = true;

        #endregion

        public IObservable<int> score => _score;
        public IObservable<int> highScore => _highScore;
        public IObservable<int> destroyCount => _destroyCount;
        public IObservable<int> currentTurn => _currentTurn;

        public IObservable<EntityInfo> createCharacter => _createCharacter;
        public IObservable<EntityInfo> createEnemy => _createEnemy;
        public IObservable<Unit> gameEnd => _gameEnd;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GameModel()
        {
            _random = new Random();
            _saveData = new SaveData();

            _Initialize();
        }

        /// <summary>
        /// ゲーム開始
        /// </summary>
        public async UniTask GameStart()
        {
            if (_gameState == eGameState.Initialize)
            {
                _gameState = eGameState.GamePlay;
                await _NextTurn();
                _CreateNextCharacter();
            }
        }

        /// <summary>
        /// 初期化する。
        /// </summary>
        private void _Initialize()
        {
            _score.Value = 0;
            _highScore.Value = 0;
            _destroyCount.Value = 0;
            _currentTurn.Value = 0;
            _totalDamege = 0;

            _gameState = eGameState.Initialize;
            _currentItem = new eItem();
            enemies = new Enemy[ENEMY_MASU_X, ENEMY_MASU_Y];
            characters = new Character[CHARACTER_MASU_X, CHARACTER_MASU_Y];
            nextCharacter = null;
            _GetHighScore();
        }

        /// <summary>
        /// ゲーム終了
        /// </summary>
        public void GameEnd()
        {
            _gameState = eGameState.GameEnd;
            _SetHighScore();
            _Initialize();

            _gameEnd.OnNext(Unit.Default);
        }

        /// <summary>
        /// 次のターンへ
        /// </summary>
        private async UniTask _NextTurn()
        {
            //次のターンへ
            _currentTurn.Value++;

            //鍵を貰えなかったターン計測
            _noKeyTurn++;

            //味方の攻撃
            await _CharacterAttack();

            //次のキャラクターが利用可能になったか
            _IsEnableKey();

            //タイマー発動: 発動しない場合はエネミー進行。
            if (!_IsEnableTimer())
            {
                //エネミー行進
                if (_EnemyMove())
                {
                    //進行後は、次のエネミー生成
                    _GenerateEnemiesWave();
                }
                //1秒待機
                //Task.Delay(1000).Wait();
            }
            //スコアを計算
            _CalcScore();
        }

        /// <summary>
        /// ハイスコア取得
        /// </summary>
        private void _GetHighScore()
        {
            _highScore.Value = _saveData.GetInt(eSaveDataType.HighScore.ToString(), 0);
        }

        /// <summary>
        /// ハイスコア更新
        /// </summary>
        private void _SetHighScore()
        {
            if (_score.Value > _highScore.Value)
            {
                _saveData.SetInt(eSaveDataType.HighScore.ToString(), _score.Value);
            }
        }

        /// <summary>
        /// キャラクターの攻撃
        /// </summary>
        private async UniTask _CharacterAttack()
        {
            bool isAttack = false;
            for (int x = 0; x < characters.GetLength(0); x++)
            {
                for (int y = 0; y < characters.GetLength(1); y++)
                {
                    if (characters[x, y] != null)
                    {
                        //攻撃したら総ダメージ数を計算しておく。
                        _totalDamege += Attack(x, characters[x, y].power);
                    }
                };
            };

            // 攻撃アニメーション中
            if (isAttack)
            {
                await UniTask.Delay(700);
            }

            int Attack(int x, int characterPower)
            {
                //当たるごとに威力減衰する
                int totalDamege = 0;

                for (int y = 0; y < enemies.GetLength(1); y++)
                {
                    var enemy = enemies[x, y];
                    //同じ座標
                    if (enemy != null)
                    {
                        // 攻撃開始
                        isAttack = true;

                        //エネミーの場合
                        if (enemy.gimickType == eGimickType.Enemy)
                        {
                            int enemyHp = enemy.power;
                            enemy.ChangePower(Math.Max(0, enemy.power - characterPower));
                            //敵を倒した(威力減衰して貫通する)
                            if (enemy.power <= 0)
                            {
                                _destroyCount.Value++; //撃破数加算
                                enemy.Dead().Forget();
                                enemies[x, y] = null;
                                totalDamege += enemyHp;
                            }
                            //敵が受け止めた(貫通止まる)
                            else if (enemy.power > 0)
                            {
                                totalDamege += characterPower;
                            }

                            //威力減衰
                            characterPower -= enemyHp;

                            //威力がもうないので帰る。
                            if (characterPower <= 0)
                            {
                                return totalDamege;
                            }
                        }
                        //鍵の場合
                        else if (enemy.gimickType == eGimickType.Key)
                        {
                            //利用可能
                            _currentItem |= eItem.Key;
                            enemy.Dead().Forget();
                            enemies[x, y] = null;
                        }
                        //タイマーの場合
                        else if (enemy.gimickType == eGimickType.Timer)
                        {
                            //エネミーが1ターン待機
                            _currentItem |= eItem.Timer;
                            enemy.Dead().Forget();
                            enemies[x, y] = null;
                        }
                    }
                }
                return totalDamege;
            }
        }

        /// <summary>
        /// 鍵を持っているか？
        /// </summary>
        private void _IsEnableKey()
        {
            if (_currentItem.HasFlag(eItem.Key))
            {
                _currentItem &= ~eItem.Key;
                //次のキャラクターを生成する。
                _CreateNextCharacter();
            }
        }

        /// <summary>
        /// タイマーは持っているか？
        /// </summary>
        /// <returns></returns>
        private bool _IsEnableTimer()
        {
            if (_currentItem.HasFlag(eItem.Timer))
            {
                _currentItem &= ~eItem.Timer;
                return true;
            }
            return false;
        }

        /// <summary>
        /// エネミーの行進
        /// </summary>
        private bool _EnemyMove()
        {
            for (int x = 0; x < enemies.GetLength(0); x++)
            {
                for (int y = 0; y < enemies.GetLength(1); y++)
                {
                    var enemy = enemies[x, y];
                    if (enemy != null)
                    {
                        //エネミー属性が最後まで行ったら終了
                        var nextY = y - 1;
                        if (nextY < 0)
                        {
                            if (enemy.gimickType == eGimickType.Enemy)
                            {
                                //ゲーム終了！
                                GameEnd();
                                return false;
                            }
                            else
                            {
                                //ボーナスアイテムは削除
                                enemy.Dead().Forget();
                                enemies[x, y] = null;
                                continue;
                            }
                        }
                        //次へ移動
                        enemy.Move();
                        enemies[x, nextY] = enemies[x, y];
                        enemies[x, y] = null;
                    }
                }
            };
            //行進成功
            return true;
        }

        /// <summary>
        /// エネミーウェーブを生成
        /// </summary>
        private void _GenerateEnemiesWave()
        {
            //このターン生成する敵の数
            for (int i = 0; i < _GetEnemyCount(); i++)
            {
                //発生場所
                int[] canMove = _CheckGenerateMasu();
                var x = _GetRandomNumber(canMove);
                //敵の強さ
                var hp = _GetEnemyStrength();
                //敵か鍵かタイマーか
                var gimick = _GetRandomGimickType();
                //敵生成
                _createEnemy.OnNext(new EntityInfo(x, SPAWN_ENEMY_MASU_Y, hp, ENEMY_MASU_X, ENEMY_MASU_Y, gimick));
            }
        }

        /// <summary>
        /// エネミーの数を計算
        /// </summary>
        /// <returns></returns>
        private int _GetEnemyCount()
        {
            //ボーナスタイム: 一列全部鍵か時計
            if (_currentTurn.Value % BONUS_STAGE_TURN == 0)
            {
                return enemies.GetLength(0);
            }

            return _random.Next(2, 4);
        }

        /// <summary>
        /// エネミーを生成する際に生成できる場所か調べる。
        /// </summary>
        /// <returns></returns>
        private int[] _CheckGenerateMasu()
        {
            List<int> canMove = new List<int>();
            for (int x = 0; x < enemies.GetLength(0); x++)
            {
                if (enemies[x, SPAWN_ENEMY_MASU_Y] == null)
                {
                    canMove.Add(x);
                }
            }
            return canMove.ToArray();
        }

        /// <summary>
        /// 指定した配列の中でランダムな値を返す。
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        private int _GetRandomNumber(int[] values)
        {
            // 配列の中からランダムに一つを選ぶ
            //int randomIndex = random.Next(0, values.Length);

            // 配列の中から若い番号がより選ばれやすい
            double adjustedRandom = Math.Pow(_random.NextDouble(), 2);
            int randomIndex = (int)(adjustedRandom * values.Length);

            int randomValue = values[randomIndex];

            return randomValue;
        }

        /// <summary>
        /// エネミーの種類を抽選
        /// </summary>
        /// <returns></returns>
        private eGimickType _GetRandomGimickType()
        {
            int rand = _random.Next(0, 101);

            //30ターン毎にボーナスステージ: 鍵か時計
            if (_currentTurn.Value % BONUS_STAGE_TURN == 0)
            {
                rand = _random.Next(81, 101);
            }

            //5ターン以上鍵が貰えていない
            if (_noKeyTurn >= NO_KEY_TURN)
            {
                UnityEngine.Debug.Log("救済処置発動");
                _noKeyTurn = 0;
                return eGimickType.Key;
            }

            // 敵
            if (rand <= ENEMY_POP_PERCENT)
            {
                return eGimickType.Enemy;
            }
            // 鍵
            else if (rand <= ENEMY_POP_PERCENT + KEY_POP_PERCENT)
            {
                _noKeyTurn = 0;
                return eGimickType.Key;
            }
            // 時計
            else if (rand <= ENEMY_POP_PERCENT + KEY_POP_PERCENT + TIMER_POP_PERCENT)
            {
                return eGimickType.Timer;
            }
            else
            {
                return eGimickType.Enemy;
            }
        }

        /// <summary>
        /// プレイヤーがキャラクターを移動させる処理。
        /// </summary>
        public void PlayerMoveCharacter(int newX, int newY, Character selectCharacter)
        {
            //移動元
            var oldX = selectCharacter.x;
            var oldY = selectCharacter.y;
            //移動先
            var chara = characters[newX, newY];

            //移動先にキャラクターがいる: マージ
            if (chara != null)
            {
                if (chara.IsEnableMarge(selectCharacter))
                {
                    //数字をマージする
                    chara.Marge(selectCharacter);
                }
                //既にキャラクターがいる かつ マージ出来ない。
                else
                {
                    return;
                }
            }
            //移動先にキャラクターがいない: 移動
            else if (chara == null)
            {
                //再度セットアップ
                selectCharacter.Setup(newX, newY, selectCharacter.power);
                characters[newX, newY] = selectCharacter;
            }

            //移動元を消す
            if (oldX == NEXT_CHARACTER_X && oldY == NEXT_CHARACTER_Y)
            {
                nextCharacter = null;
            }
            else
            {
                characters[oldX, oldY] = null;
            }

            //成功!次のターンへ
            _NextTurn().Forget();
        }

        /// <summary>
        /// スコアを計算する。
        /// </summary>
        /// <param name="defeatedEnemies"></param>
        /// <param name="turnsElapsed"></param>
        /// <param name="totalDamage"></param>
        /// <returns></returns>
        private void _CalcScore()
        {
            // 撃破数によるボーナス => 一体撃破:10ポイント
            int destroyBonus = _destroyCount.Value * DESTROY_BONUS;

            // ターン数によるボーナス => 一ターン:30ポイント
            int turnBonus = (_currentTurn.Value - 1) * TURN_BONUS;

            // ダメージ数によるボーナス => 100ダメージ:1ポイント
            int damageBonus = _totalDamege / DAMEGE_BONUS;

            // スコアを計算
            _score.Value = destroyBonus + turnBonus + damageBonus; ;
        }

        /// <summary>
        /// 次のキャラクターを生成する。
        /// </summary>
        private void _CreateNextCharacter()
        {
            if (nextCharacter != null)
            {
                UnityEngine.Debug.Log("既に次のキャラクターが待機中です。");
                return;
            }

            //指定ターンごとに生成される2の累乗の数字が大きくなっていく
            int generateCount = (int)(_currentTurn.Value / 30f);
            int[] generateList = _GetElementsArray(CHARACTER_ENABLE_NUM, generateCount);
            var nextCharacterPower = _GetRandomNumber(generateList);

            // 常に2で試してみる。
            if (ALWAYS_MIN_NUMBER)
            {
                nextCharacterPower = 2;
            }

            //控室に生成させる。
            _createCharacter.OnNext(new EntityInfo(NEXT_CHARACTER_X, NEXT_CHARACTER_Y, nextCharacterPower, CHARACTER_MASU_X, CHARACTER_MASU_Y));

            int[] _GetElementsArray(int[] array, int maxCount)
            {
                // 指定された値までの要素を取得
                List<int> list = new List<int>();
                for (int i = 0; i < array.Length; i++)
                {
                    if (i > maxCount)
                    {
                        break;
                    }
                    list.Add(array[i]);
                }

                return list.ToArray();
            }
        }

        /// <summary>
        /// ターン数に応じてエネミーの強さを計算する
        /// </summary>
        /// <param name="currentTurn"></param>
        /// <returns></returns>
        private int _GetEnemyStrength()
        {
            //ターン数×倍数: 倍数は1ターン目:0.5倍,100ターン目:1倍,200ターン目:1.5倍,300ターン目:2倍
            //var multiple = 0.2f + ((float)_currentTurn.Value / 100) * 0.3f; // 最初簡単、100ターンぐらいから面白い、150からはきつい
            var multiple = 0.5f;

            int strongMax = 1 + (int)(_currentTurn.Value * multiple); //強さの上限
            int strongMin = 1;                                        //強さの下限
            //TODO: たまに弱いやつも出てきてほしい
            int result = _random.Next(strongMin, strongMax);
            UnityEngine.Debug.Log($"Random({strongMin}～{strongMax}) => " + result);
            return result;
        }
    }
}