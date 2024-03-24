using RibertaGames;
using System;
using System.Collections.Generic;
using UniRx;
using Cysharp.Threading.Tasks;

namespace RibertaGames
{
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

        #region �Q�[���萔

        /// <summary>
        /// �Ֆʂ̃}�X
        /// </summary>
        private readonly int ENEMY_MASU_X = 7;
        private readonly int ENEMY_MASU_Y = 7;
        private readonly int CHARACTER_MASU_X = 7;
        private readonly int CHARACTER_MASU_Y = 2;

        /// <summary>
        /// �G�l�~�[�̃X�|�[������Y���W
        /// </summary>
        public readonly int SPAWN_ENEMY_MASU_Y = 6;

        /// <summary>
        /// �L�����N�^�[���̐i���\�Ȑ���
        /// </summary>
        private readonly int[] CHARACTER_ENABLE_NUM = new int[] { 2, 4, 8, 16, 32, 64 };

        /// <summary>
        /// ���̐����L�����N�^�[�̌Œ�X,Y
        /// </summary>
        private readonly int NEXT_CHARACTER_X = 3;
        private readonly int NEXT_CHARACTER_Y = -1;

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
        private readonly int KEY_POP_PERCENT = 18;

        /// <summary>
        /// �^�C�}�[�̏o���m��
        /// </summary>
        private readonly int TIMER_POP_PERCENT = 2;

        /// <summary>
        /// �o�����閡������ɍŒ჌�x���œo��
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
        /// �R���X�g���N�^
        /// </summary>
        public GameModel()
        {
            _random = new Random();
            _saveData = new SaveData();

            _Initialize();
        }

        /// <summary>
        /// �Q�[���J�n
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
        /// ����������B
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
        /// �Q�[���I��
        /// </summary>
        public void GameEnd()
        {
            _gameState = eGameState.GameEnd;
            _SetHighScore();
            _Initialize();

            _gameEnd.OnNext(Unit.Default);
        }

        /// <summary>
        /// ���̃^�[����
        /// </summary>
        private async UniTask _NextTurn()
        {
            //���̃^�[����
            _currentTurn.Value++;

            //����Ⴆ�Ȃ������^�[���v��
            _noKeyTurn++;

            //�����̍U��
            await _CharacterAttack();

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
        }

        /// <summary>
        /// �n�C�X�R�A�擾
        /// </summary>
        private void _GetHighScore()
        {
            _highScore.Value = _saveData.GetInt(eSaveDataType.HighScore.ToString(), 0);
        }

        /// <summary>
        /// �n�C�X�R�A�X�V
        /// </summary>
        private void _SetHighScore()
        {
            if (_score.Value > _highScore.Value)
            {
                _saveData.SetInt(eSaveDataType.HighScore.ToString(), _score.Value);
            }
        }

        /// <summary>
        /// �L�����N�^�[�̍U��
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
                        //�U�������瑍�_���[�W�����v�Z���Ă����B
                        _totalDamege += Attack(x, characters[x, y].power);
                    }
                };
            };

            // �U���A�j���[�V������
            if (isAttack)
            {
                await UniTask.Delay(700);
            }

            int Attack(int x, int characterPower)
            {
                //�����邲�ƂɈЗ͌�������
                int totalDamege = 0;

                for (int y = 0; y < enemies.GetLength(1); y++)
                {
                    var enemy = enemies[x, y];
                    //�������W
                    if (enemy != null)
                    {
                        // �U���J�n
                        isAttack = true;

                        //�G�l�~�[�̏ꍇ
                        if (enemy.gimickType == eGimickType.Enemy)
                        {
                            int enemyHp = enemy.power;
                            enemy.ChangePower(Math.Max(0, enemy.power - characterPower));
                            //�G��|����(�З͌������Ċђʂ���)
                            if (enemy.power <= 0)
                            {
                                _destroyCount.Value++; //���j�����Z
                                enemy.Dead().Forget();
                                enemies[x, y] = null;
                                totalDamege += enemyHp;
                            }
                            //�G���󂯎~�߂�(�ђʎ~�܂�)
                            else if (enemy.power > 0)
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
                            enemy.Dead().Forget();
                            enemies[x, y] = null;
                        }
                        //�^�C�}�[�̏ꍇ
                        else if (enemy.gimickType == eGimickType.Timer)
                        {
                            //�G�l�~�[��1�^�[���ҋ@
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
            for (int x = 0; x < enemies.GetLength(0); x++)
            {
                for (int y = 0; y < enemies.GetLength(1); y++)
                {
                    var enemy = enemies[x, y];
                    if (enemy != null)
                    {
                        //�G�l�~�[�������Ō�܂ōs������I��
                        var nextY = y - 1;
                        if (nextY < 0)
                        {
                            if (enemy.gimickType == eGimickType.Enemy)
                            {
                                //�Q�[���I���I
                                GameEnd();
                                return false;
                            }
                            else
                            {
                                //�{�[�i�X�A�C�e���͍폜
                                enemy.Dead().Forget();
                                enemies[x, y] = null;
                                continue;
                            }
                        }
                        //���ֈړ�
                        enemy.Move();
                        enemies[x, nextY] = enemies[x, y];
                        enemies[x, y] = null;
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
                _createEnemy.OnNext(new EntityInfo(x, SPAWN_ENEMY_MASU_Y, hp, ENEMY_MASU_X, ENEMY_MASU_Y, gimick));
            }
        }

        /// <summary>
        /// �G�l�~�[�̐����v�Z
        /// </summary>
        /// <returns></returns>
        private int _GetEnemyCount()
        {
            //�{�[�i�X�^�C��: ���S���������v
            if (_currentTurn.Value % BONUS_STAGE_TURN == 0)
            {
                return enemies.GetLength(0);
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
        /// �G�l�~�[�̎�ނ𒊑I
        /// </summary>
        /// <returns></returns>
        private eGimickType _GetRandomGimickType()
        {
            int rand = _random.Next(0, 101);

            //30�^�[�����Ƀ{�[�i�X�X�e�[�W: �������v
            if (_currentTurn.Value % BONUS_STAGE_TURN == 0)
            {
                rand = _random.Next(81, 101);
            }

            //5�^�[���ȏ㌮���Ⴆ�Ă��Ȃ�
            if (_noKeyTurn >= NO_KEY_TURN)
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
        /// �v���C���[���L�����N�^�[���ړ������鏈���B
        /// </summary>
        public void PlayerMoveCharacter(int newX, int newY, Character selectCharacter)
        {
            //�ړ���
            var oldX = selectCharacter.x;
            var oldY = selectCharacter.y;
            //�ړ���
            var chara = characters[newX, newY];

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
                    return;
                }
            }
            //�ړ���ɃL�����N�^�[�����Ȃ�: �ړ�
            else if (chara == null)
            {
                //�ēx�Z�b�g�A�b�v
                selectCharacter.Setup(newX, newY, selectCharacter.power);
                characters[newX, newY] = selectCharacter;
            }

            //�ړ���������
            if (oldX == NEXT_CHARACTER_X && oldY == NEXT_CHARACTER_Y)
            {
                nextCharacter = null;
            }
            else
            {
                characters[oldX, oldY] = null;
            }

            //����!���̃^�[����
            _NextTurn().Forget();
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
            int destroyBonus = _destroyCount.Value * DESTROY_BONUS;

            // �^�[�����ɂ��{�[�i�X => ��^�[��:30�|�C���g
            int turnBonus = (_currentTurn.Value - 1) * TURN_BONUS;

            // �_���[�W���ɂ��{�[�i�X => 100�_���[�W:1�|�C���g
            int damageBonus = _totalDamege / DAMEGE_BONUS;

            // �X�R�A���v�Z
            _score.Value = destroyBonus + turnBonus + damageBonus; ;
        }

        /// <summary>
        /// ���̃L�����N�^�[�𐶐�����B
        /// </summary>
        private void _CreateNextCharacter()
        {
            if (nextCharacter != null)
            {
                UnityEngine.Debug.Log("���Ɏ��̃L�����N�^�[���ҋ@���ł��B");
                return;
            }

            //�w��^�[�����Ƃɐ��������2�̗ݏ�̐������傫���Ȃ��Ă���
            int generateCount = (int)(_currentTurn.Value / 30f);
            int[] generateList = _GetElementsArray(CHARACTER_ENABLE_NUM, generateCount);
            var nextCharacterPower = _GetRandomNumber(generateList);

            // ���2�Ŏ����Ă݂�B
            if (ALWAYS_MIN_NUMBER)
            {
                nextCharacterPower = 2;
            }

            //�T���ɐ���������B
            _createCharacter.OnNext(new EntityInfo(NEXT_CHARACTER_X, NEXT_CHARACTER_Y, nextCharacterPower, CHARACTER_MASU_X, CHARACTER_MASU_Y));

            int[] _GetElementsArray(int[] array, int maxCount)
            {
                // �w�肳�ꂽ�l�܂ł̗v�f���擾
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
        /// �^�[�����ɉ����ăG�l�~�[�̋������v�Z����
        /// </summary>
        /// <param name="currentTurn"></param>
        /// <returns></returns>
        private int _GetEnemyStrength()
        {
            //�^�[�����~�{��: �{����1�^�[����:0.5�{,100�^�[����:1�{,200�^�[����:1.5�{,300�^�[����:2�{
            //var multiple = 0.2f + ((float)_currentTurn.Value / 100) * 0.3f; // �ŏ��ȒP�A100�^�[�����炢����ʔ����A150����͂���
            var multiple = 0.5f;

            int strongMax = 1 + (int)(_currentTurn.Value * multiple); //�����̏��
            int strongMin = 1;                                        //�����̉���
            //TODO: ���܂Ɏア����o�Ă��Ăق���
            int result = _random.Next(strongMin, strongMax);
            UnityEngine.Debug.Log($"Random({strongMin}�`{strongMax}) => " + result);
            return result;
        }
    }
}