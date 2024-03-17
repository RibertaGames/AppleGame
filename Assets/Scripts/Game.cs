using System;
using System.Collections.Generic;

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
[System.Flags]
public enum eItem
{
    Timer = 1,
    Key = 2,
}

public class GameProgress
{
    public int score = 0;
    public int totalDamege = 0;
    public int destroyCount = 0;
    public int currentTurn = 0;
}

public class Game
{
    private int _score = 0;
    private int _totalDamege = 0;
    private int _destroyCount = 0;
    private int _currentTurn = 0;
    private eItem _currentItem = new eItem();

    private Enemy[,] _enemies = null;
    private Character[,] _characters = null;
    private Character _nextCharacter = null;

    private int _noKeyTurn = 0;

    private Random _random = new Random();

    /// <summary>
    /// 盤面のマス
    /// </summary>
    public static readonly int ENEMY_MASU_X = 7;
    public static readonly int ENEMY_MASU_Y = 7;
    public static readonly int CHARACTER_MASU_X = 7;
    public static readonly int CHARACTER_MASU_Y = 2;

    /// <summary>
    /// エネミーのスポーンするY座標
    /// </summary>
    private readonly int SPAWN_ENEMY_MASU_Y = ENEMY_MASU_Y - 1;

    /// <summary>
    /// キャラクター側の進化可能な数字
    /// </summary>
    public static readonly int[] CHARACTER_ENABLE_NUM = new int[] { 2, 4, 8, 16, 32, 64 };

    /// <summary>
    /// 次の生成キャラクターの固定X,Y
    /// </summary>
    public static readonly int NEXT_CHARACTER_X = 3;
    public static readonly int NEXT_CHARACTER_Y = -1;
    public static readonly float NEXT_CHARACTER_POSITION_Y = -1.35f;

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
    private readonly int KEY_POP_PERCENT = 15;

    /// <summary>
    /// タイマーの出現確率
    /// </summary>
    private readonly int TIMER_POP_PERCENT = 5;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Game()
    {
        _Initialize();
    }

    /// <summary>
    /// ゲーム開始
    /// </summary>
    public void GameStart()
    {
        if (_currentTurn == 0)
        {
            _NextTurn();
            _CreateNextCharacter();
        }
    }

    /// <summary>
    /// キャラクターを移動させる処理。
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="selectCharacter"></param>
    /// <returns></returns>
    public bool PlayerMoveCharacter(int x, int y, Character selectCharacter)
    {
        return _PlayerMoveCharacter(x, y, selectCharacter);
    }

    /// <summary>
    /// 現在のゲームの状況を取得
    /// </summary>
    /// <returns></returns>
    public GameProgress GetCurrentGameProgress()
    {
        GameProgress result = new GameProgress()
        {
            score = _score,
            totalDamege = _totalDamege,
            destroyCount = _destroyCount,
            currentTurn = _currentTurn,
        };

        return result;
    }

    /// <summary>
    /// 初期化する。
    /// </summary>
    private void _Initialize()
    {
        _score = 0;
        _totalDamege = 0;
        _destroyCount = 0;
        _currentTurn = 0;
        _currentItem = new eItem();
        _enemies = new Enemy[ENEMY_MASU_X, ENEMY_MASU_Y];
        _characters = new Character[CHARACTER_MASU_X, CHARACTER_MASU_Y];
        _nextCharacter = null;
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    private void _GameEnd()
    {
        GameManager.instance.GameEnd();
        _Initialize();
        return;
    }

    /// <summary>
    /// 次のターンへ
    /// </summary>
    private void _NextTurn()
    {
        //次のターンへ
        _currentTurn++;

        //鍵を貰えなかったターン計測
        _noKeyTurn++;

        //味方の攻撃
        _CharacterAttack();

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

        //UI更新
        GameManager.instance.GameResult(GetCurrentGameProgress());
    }

    /// <summary>
    /// キャラクターの攻撃
    /// </summary>
    private void _CharacterAttack()
    {
        for (int x = 0; x < _characters.GetLength(0); x++)
        {
            for (int y = 0; y < _characters.GetLength(1); y++)
            {
                if (_characters[x, y] != null)
                {
                    //攻撃したら総ダメージ数を計算しておく。
                    _totalDamege += Attack(x, _characters[x, y].power);
                }
            };
        };

        int Attack(int x, int characterPower)
        {
            //当たるごとに威力減衰する
            int totalDamege = 0;
            
            for (int y = 0; y < _enemies.GetLength(1); y++)
            {
                var enemy = _enemies[x, y];
                //同じ座標
                if (enemy != null)
                {
                    //エネミーの場合
                    if (enemy.gimickType == eGimickType.Enemy)
                    {
                        int enemyHp = enemy.power;
                        enemy.ChangePower(enemy.power - characterPower);
                        //敵を倒した(威力減衰して貫通する)
                        if (enemy.power <= 0)
                        {
                            _destroyCount++; //撃破数加算
                            enemy.Destroy();
                            _enemies[x, y] = null;
                            totalDamege += enemyHp;
                        }
                        //敵が受け止めた(貫通止まる)
                        else if(enemy.power > 0)
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
                        enemy.Destroy();
                        _enemies[x, y] = null;
                    }
                    //タイマーの場合
                    else if (enemy.gimickType == eGimickType.Timer)
                    {
                        //エネミーが1ターン待機
                        _currentItem |= eItem.Timer;
                        enemy.Destroy();
                        _enemies[x, y] = null;
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
        for (int x = 0; x < _enemies.GetLength(0); x++)
        {
            for (int y = 0; y < _enemies.GetLength(1); y++)
            {
                var enemy = _enemies[x, y];
                if (enemy != null)
                {
                    //エネミー属性が最後まで行ったら終了
                    var nextY = y - 1;
                    if (nextY < 0)
                    {
                        if (enemy.gimickType == eGimickType.Enemy)
                        {
                            //ゲーム終了！
                            _GameEnd();
                            return false;
                        }
                        else
                        {
                            //ボーナスアイテムは削除
                            enemy.Destroy();
                            _enemies[x, y] = null;
                            continue;
                        }
                    }
                    //次へ移動
                    enemy.Move();
                    _enemies[x, nextY] = _enemies[x, y];
                    _enemies[x, y] = null;
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
            var enemy = Enemy.CreateEnemy(x, SPAWN_ENEMY_MASU_Y, hp, gimick);
            //記憶しておく
            _enemies[x, SPAWN_ENEMY_MASU_Y] = enemy;
        }
    }

    /// <summary>
    /// エネミーの数を計算
    /// </summary>
    /// <returns></returns>
    private int _GetEnemyCount()
    {
        //ボーナスタイム: 一列全部鍵か時計
        if (_currentTurn % BONUS_STAGE_TURN == 0)
        {
            return _enemies.GetLength(0);
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
        for (int x = 0; x < _enemies.GetLength(0); x++)
        {
            if (_enemies[x, SPAWN_ENEMY_MASU_Y] == null)
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
    /// 指定した値までの要素の配列の返す
    /// </summary>
    /// <param name="array"></param>
    /// <param name="maxCount"></param>
    /// <returns></returns>
    private int[] _GetElementsArray(int[] array, int maxCount)
    {
        // 指定された値までの要素を取得
        List<int> list = new List<int>();
        for(int i = 0; i < array.Length; i++)
        {
            if(i > maxCount)
            {
                break;
            }
            list.Add(array[i]);
        }

        return list.ToArray();
    }

    /// <summary>
    /// エネミーの種類を抽選
    /// </summary>
    /// <returns></returns>
    private eGimickType _GetRandomGimickType()
    {
        int rand = _random.Next(0, 101);

        //30ターン毎にボーナスステージ: 鍵か時計
        if (_currentTurn % BONUS_STAGE_TURN == 0)
        {
            rand = _random.Next(81, 101);
        }

        //5ターン以上鍵が貰えていない
        if(_noKeyTurn >= NO_KEY_TURN)
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
        else if(rand <= ENEMY_POP_PERCENT + KEY_POP_PERCENT + TIMER_POP_PERCENT)
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
    private bool _PlayerMoveCharacter(int newX, int newY, Character selectCharacter)
    {
        //移動元
        var oldX = selectCharacter.x;
        var oldY = selectCharacter.y;
        //移動先
        var chara = _characters[newX, newY];

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
                return false; //利用できない
            }
        }
        //移動先にキャラクターがいない: 移動
        else if(chara == null)
        {
            //再度セットアップ
            selectCharacter.Setup(newX, newY, selectCharacter.power);
            _characters[newX, newY] = selectCharacter;
        }

        //移動元を消す
        if (oldX == NEXT_CHARACTER_X && oldY == NEXT_CHARACTER_Y)
        {
            _nextCharacter = null;
        }
        else
        {
            _characters[oldX, oldY] = null;
        }

        //成功!次のターンへ
        _NextTurn();

        return true; //成功
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
        int destroyBonus = _destroyCount * DESTROY_BONUS;

        // ターン数によるボーナス => 一ターン:30ポイント
        int turnBonus = (_currentTurn - 1) * TURN_BONUS;

        // ダメージ数によるボーナス => 100ダメージ:1ポイント
        int damageBonus = _totalDamege / DAMEGE_BONUS;

        // 各ボーナスを合算
        int totalBonus = destroyBonus + turnBonus + damageBonus;

        //スコアを記憶する。
        _score = totalBonus;
    }

    /// <summary>
    /// 次のキャラクターを生成する。
    /// </summary>
    private void _CreateNextCharacter()
    {
        if (_nextCharacter != null)
        {
            UnityEngine.Debug.Log("既に次のキャラクターが待機中です。");
            return;
        }

        //指定ターンごとに生成される2の累乗の数字が大きくなっていく
        int generateCount = (int)(_currentTurn / 30f);
        int[] generateList = _GetElementsArray(CHARACTER_ENABLE_NUM, generateCount);
        var nextCharacterPower = _GetRandomNumber(generateList);

        //控室に生成させる。
        _nextCharacter = Character.CreateCharacter(NEXT_CHARACTER_X, NEXT_CHARACTER_Y, nextCharacterPower);
        _nextCharacter.SetNextCharacterPosition(NEXT_CHARACTER_X, NEXT_CHARACTER_POSITION_Y);
    }

    /// <summary>
    /// ターン数に応じてエネミーの強さを計算する
    /// </summary>
    /// <param name="currentTurn"></param>
    /// <returns></returns>
    private int _GetEnemyStrength()
    {
        //ターン数×倍数: 倍数は1ターン目:0.5倍,100ターン目:1倍,200ターン目:1.5倍,300ターン目:2倍
        //var multiple = Mathf.Min(0.5f + (_currentTurn / 100) * 0.5f, 1f);
        var multiple = Math.Min(0.5f + (_currentTurn / 150) * 0.5f, 1f);
        if (multiple == 1f)
        {
            multiple = 1f + (_currentTurn / 150) * 0.1f;
        }

        int strongMax = 1 + (int)(_currentTurn * multiple); //強さの上限
        int strongMin = 1 + strongMax / 3;              //強さの下限
        int result = _random.Next(strongMin, strongMax);
        UnityEngine.Debug.Log($"Random({strongMin}～{strongMax}) => " + result);
        return result;
    }
}