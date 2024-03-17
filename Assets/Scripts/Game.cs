using System;
using System.Collections.Generic;

/// <summary>
/// �M�~�b�N���
/// </summary>
public enum eGimickType
{
    None,
    Enemy,
    Key,
    Timer,
}

/// <summary>
/// �莝���̃A�C�e��
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
    /// �Ֆʂ̃}�X
    /// </summary>
    public static readonly int ENEMY_MASU_X = 7;
    public static readonly int ENEMY_MASU_Y = 7;
    public static readonly int CHARACTER_MASU_X = 7;
    public static readonly int CHARACTER_MASU_Y = 2;

    /// <summary>
    /// �G�l�~�[�̃X�|�[������Y���W
    /// </summary>
    private readonly int SPAWN_ENEMY_MASU_Y = ENEMY_MASU_Y - 1;

    /// <summary>
    /// �L�����N�^�[���̐i���\�Ȑ���
    /// </summary>
    public static readonly int[] CHARACTER_ENABLE_NUM = new int[] { 2, 4, 8, 16, 32, 64 };

    /// <summary>
    /// ���̐����L�����N�^�[�̌Œ�X,Y
    /// </summary>
    public static readonly int NEXT_CHARACTER_X = 3;
    public static readonly int NEXT_CHARACTER_Y = -1;
    public static readonly float NEXT_CHARACTER_POSITION_Y = -1.35f;

    /// <summary>
    /// ��̌��j: 10�|�C���g
    /// </summary>
    private readonly int DESTROY_BONUS = 10;

    /// <summary>
    /// ��^�[��: 5�|�C���g
    /// </summary>
    private readonly int TURN_BONUS = 5;

    /// <summary>
    /// 100�_���[�W��: 1�|�C���g
    /// </summary>
    private readonly int DAMEGE_BONUS = 100;

    /// <summary>
    /// 30�^�[�����Ƀ{�[�i�X�X�e�[�W: �������v
    /// </summary>
    private readonly int BONUS_STAGE_TURN = 30;

    /// <summary>
    /// �~�Ϗ��u: 5�^�[�������o�Ȃ������献���o���B
    /// </summary>
    private readonly int NO_KEY_TURN = 5;

    /// <summary>
    /// �G�l�~�[�̏o���m��
    /// </summary>
    private readonly int ENEMY_POP_PERCENT = 80;

    /// <summary>
    /// ���̏o���m��
    /// </summary>
    private readonly int KEY_POP_PERCENT = 15;

    /// <summary>
    /// �^�C�}�[�̏o���m��
    /// </summary>
    private readonly int TIMER_POP_PERCENT = 5;

    /// <summary>
    /// �R���X�g���N�^
    /// </summary>
    public Game()
    {
        _Initialize();
    }

    /// <summary>
    /// �Q�[���J�n
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
    /// �L�����N�^�[���ړ������鏈���B
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
    /// ���݂̃Q�[���̏󋵂��擾
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
    /// ����������B
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
    /// �Q�[���I��
    /// </summary>
    private void _GameEnd()
    {
        GameManager.instance.GameEnd();
        _Initialize();
        return;
    }

    /// <summary>
    /// ���̃^�[����
    /// </summary>
    private void _NextTurn()
    {
        //���̃^�[����
        _currentTurn++;

        //����Ⴆ�Ȃ������^�[���v��
        _noKeyTurn++;

        //�����̍U��
        _CharacterAttack();

        //���̃L�����N�^�[�����p�\�ɂȂ�����
        _IsEnableKey();

        //�^�C�}�[����: �������Ȃ��ꍇ�̓G�l�~�[�i�s�B
        if (!_IsEnableTimer())
        {
            //�G�l�~�[�s�i
            if (_EnemyMove())
            {
                //�i�s��́A���̃G�l�~�[����
                _GenerateEnemiesWave();
            }
            //1�b�ҋ@
            //Task.Delay(1000).Wait();
        }
        //�X�R�A���v�Z
        _CalcScore();

        //UI�X�V
        GameManager.instance.GameResult(GetCurrentGameProgress());
    }

    /// <summary>
    /// �L�����N�^�[�̍U��
    /// </summary>
    private void _CharacterAttack()
    {
        for (int x = 0; x < _characters.GetLength(0); x++)
        {
            for (int y = 0; y < _characters.GetLength(1); y++)
            {
                if (_characters[x, y] != null)
                {
                    //�U�������瑍�_���[�W�����v�Z���Ă����B
                    _totalDamege += Attack(x, _characters[x, y].power);
                }
            };
        };

        int Attack(int x, int characterPower)
        {
            //�����邲�ƂɈЗ͌�������
            int totalDamege = 0;
            
            for (int y = 0; y < _enemies.GetLength(1); y++)
            {
                var enemy = _enemies[x, y];
                //�������W
                if (enemy != null)
                {
                    //�G�l�~�[�̏ꍇ
                    if (enemy.gimickType == eGimickType.Enemy)
                    {
                        int enemyHp = enemy.power;
                        enemy.ChangePower(enemy.power - characterPower);
                        //�G��|����(�З͌������Ċђʂ���)
                        if (enemy.power <= 0)
                        {
                            _destroyCount++; //���j�����Z
                            enemy.Destroy();
                            _enemies[x, y] = null;
                            totalDamege += enemyHp;
                        }
                        //�G���󂯎~�߂�(�ђʎ~�܂�)
                        else if(enemy.power > 0)
                        {
                            totalDamege += characterPower;
                        }

                        //�З͌���
                        characterPower -= enemyHp;

                        //�З͂������Ȃ��̂ŋA��B
                        if (characterPower <= 0)
                        {
                            return totalDamege;
                        }
                    }
                    //���̏ꍇ
                    else if (enemy.gimickType == eGimickType.Key)
                    {
                        //���p�\
                        _currentItem |= eItem.Key;
                        enemy.Destroy();
                        _enemies[x, y] = null;
                    }
                    //�^�C�}�[�̏ꍇ
                    else if (enemy.gimickType == eGimickType.Timer)
                    {
                        //�G�l�~�[��1�^�[���ҋ@
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
    /// ���������Ă��邩�H
    /// </summary>
    private void _IsEnableKey()
    {
        if (_currentItem.HasFlag(eItem.Key))
        {
            _currentItem &= ~eItem.Key;
            //���̃L�����N�^�[�𐶐�����B
            _CreateNextCharacter();
        }
    }

    /// <summary>
    /// �^�C�}�[�͎����Ă��邩�H
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
    /// �G�l�~�[�̍s�i
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
                    //�G�l�~�[�������Ō�܂ōs������I��
                    var nextY = y - 1;
                    if (nextY < 0)
                    {
                        if (enemy.gimickType == eGimickType.Enemy)
                        {
                            //�Q�[���I���I
                            _GameEnd();
                            return false;
                        }
                        else
                        {
                            //�{�[�i�X�A�C�e���͍폜
                            enemy.Destroy();
                            _enemies[x, y] = null;
                            continue;
                        }
                    }
                    //���ֈړ�
                    enemy.Move();
                    _enemies[x, nextY] = _enemies[x, y];
                    _enemies[x, y] = null;
                }
            }
        };
        //�s�i����
        return true;
    }

    /// <summary>
    /// �G�l�~�[�E�F�[�u�𐶐�
    /// </summary>
    private void _GenerateEnemiesWave()
    {
        //���̃^�[����������G�̐�
        for (int i = 0; i < _GetEnemyCount(); i++)
        {
            //�����ꏊ
            int[] canMove = _CheckGenerateMasu();
            var x = _GetRandomNumber(canMove);
            //�G�̋���
            var hp = _GetEnemyStrength();
            //�G�������^�C�}�[��
            var gimick = _GetRandomGimickType();
            //�G����
            var enemy = Enemy.CreateEnemy(x, SPAWN_ENEMY_MASU_Y, hp, gimick);
            //�L�����Ă���
            _enemies[x, SPAWN_ENEMY_MASU_Y] = enemy;
        }
    }

    /// <summary>
    /// �G�l�~�[�̐����v�Z
    /// </summary>
    /// <returns></returns>
    private int _GetEnemyCount()
    {
        //�{�[�i�X�^�C��: ���S���������v
        if (_currentTurn % BONUS_STAGE_TURN == 0)
        {
            return _enemies.GetLength(0);
        }

        return _random.Next(2, 4);
    }

    /// <summary>
    /// �G�l�~�[�𐶐�����ۂɐ����ł���ꏊ�����ׂ�B
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
    /// �w�肵���z��̒��Ń����_���Ȓl��Ԃ��B
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    private int _GetRandomNumber(int[] values)
    {
        // �z��̒����烉���_���Ɉ��I��
        //int randomIndex = random.Next(0, values.Length);

        // �z��̒�����Ⴂ�ԍ������I�΂�₷��
        double adjustedRandom = Math.Pow(_random.NextDouble(), 2);
        int randomIndex = (int)(adjustedRandom * values.Length);

        int randomValue = values[randomIndex];

        return randomValue;
    }

    /// <summary>
    /// �w�肵���l�܂ł̗v�f�̔z��̕Ԃ�
    /// </summary>
    /// <param name="array"></param>
    /// <param name="maxCount"></param>
    /// <returns></returns>
    private int[] _GetElementsArray(int[] array, int maxCount)
    {
        // �w�肳�ꂽ�l�܂ł̗v�f���擾
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
    /// �G�l�~�[�̎�ނ𒊑I
    /// </summary>
    /// <returns></returns>
    private eGimickType _GetRandomGimickType()
    {
        int rand = _random.Next(0, 101);

        //30�^�[�����Ƀ{�[�i�X�X�e�[�W: �������v
        if (_currentTurn % BONUS_STAGE_TURN == 0)
        {
            rand = _random.Next(81, 101);
        }

        //5�^�[���ȏ㌮���Ⴆ�Ă��Ȃ�
        if(_noKeyTurn >= NO_KEY_TURN)
        {
            UnityEngine.Debug.Log("�~�Ϗ��u����");
            _noKeyTurn = 0;
            return eGimickType.Key;
        }

        // �G
        if (rand <= ENEMY_POP_PERCENT)
        {
            return eGimickType.Enemy;
        }
        // ��
        else if (rand <= ENEMY_POP_PERCENT + KEY_POP_PERCENT)
        {
            _noKeyTurn = 0;
            return eGimickType.Key;
        }
        // ���v
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
    /// �v���C���[���L�����N�^�[���ړ������鏈���B
    /// </summary>
    private bool _PlayerMoveCharacter(int newX, int newY, Character selectCharacter)
    {
        //�ړ���
        var oldX = selectCharacter.x;
        var oldY = selectCharacter.y;
        //�ړ���
        var chara = _characters[newX, newY];

        //�ړ���ɃL�����N�^�[������: �}�[�W
        if (chara != null)
        {
            if (chara.IsEnableMarge(selectCharacter))
            {
                //�������}�[�W����
                chara.Marge(selectCharacter);
            }
            //���ɃL�����N�^�[������ ���� �}�[�W�o���Ȃ��B
            else
            {
                return false; //���p�ł��Ȃ�
            }
        }
        //�ړ���ɃL�����N�^�[�����Ȃ�: �ړ�
        else if(chara == null)
        {
            //�ēx�Z�b�g�A�b�v
            selectCharacter.Setup(newX, newY, selectCharacter.power);
            _characters[newX, newY] = selectCharacter;
        }

        //�ړ���������
        if (oldX == NEXT_CHARACTER_X && oldY == NEXT_CHARACTER_Y)
        {
            _nextCharacter = null;
        }
        else
        {
            _characters[oldX, oldY] = null;
        }

        //����!���̃^�[����
        _NextTurn();

        return true; //����
    }

    /// <summary>
    /// �X�R�A���v�Z����B
    /// </summary>
    /// <param name="defeatedEnemies"></param>
    /// <param name="turnsElapsed"></param>
    /// <param name="totalDamage"></param>
    /// <returns></returns>
    private void _CalcScore()
    {
        // ���j���ɂ��{�[�i�X => ��̌��j:10�|�C���g
        int destroyBonus = _destroyCount * DESTROY_BONUS;

        // �^�[�����ɂ��{�[�i�X => ��^�[��:30�|�C���g
        int turnBonus = (_currentTurn - 1) * TURN_BONUS;

        // �_���[�W���ɂ��{�[�i�X => 100�_���[�W:1�|�C���g
        int damageBonus = _totalDamege / DAMEGE_BONUS;

        // �e�{�[�i�X�����Z
        int totalBonus = destroyBonus + turnBonus + damageBonus;

        //�X�R�A���L������B
        _score = totalBonus;
    }

    /// <summary>
    /// ���̃L�����N�^�[�𐶐�����B
    /// </summary>
    private void _CreateNextCharacter()
    {
        if (_nextCharacter != null)
        {
            UnityEngine.Debug.Log("���Ɏ��̃L�����N�^�[���ҋ@���ł��B");
            return;
        }

        //�w��^�[�����Ƃɐ��������2�̗ݏ�̐������傫���Ȃ��Ă���
        int generateCount = (int)(_currentTurn / 30f);
        int[] generateList = _GetElementsArray(CHARACTER_ENABLE_NUM, generateCount);
        var nextCharacterPower = _GetRandomNumber(generateList);

        //�T���ɐ���������B
        _nextCharacter = Character.CreateCharacter(NEXT_CHARACTER_X, NEXT_CHARACTER_Y, nextCharacterPower);
        _nextCharacter.SetNextCharacterPosition(NEXT_CHARACTER_X, NEXT_CHARACTER_POSITION_Y);
    }

    /// <summary>
    /// �^�[�����ɉ����ăG�l�~�[�̋������v�Z����
    /// </summary>
    /// <param name="currentTurn"></param>
    /// <returns></returns>
    private int _GetEnemyStrength()
    {
        //�^�[�����~�{��: �{����1�^�[����:0.5�{,100�^�[����:1�{,200�^�[����:1.5�{,300�^�[����:2�{
        //var multiple = Mathf.Min(0.5f + (_currentTurn / 100) * 0.5f, 1f);
        var multiple = Math.Min(0.5f + (_currentTurn / 150) * 0.5f, 1f);
        if (multiple == 1f)
        {
            multiple = 1f + (_currentTurn / 150) * 0.1f;
        }

        int strongMax = 1 + (int)(_currentTurn * multiple); //�����̏��
        int strongMin = 1 + strongMax / 3;              //�����̉���
        int result = _random.Next(strongMin, strongMax);
        UnityEngine.Debug.Log($"Random({strongMin}�`{strongMax}) => " + result);
        return result;
    }
}