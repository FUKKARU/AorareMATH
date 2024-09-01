using Cysharp.Threading.Tasks;
using General.Debug;
using General.Extension;
using Main.Data;
using Main.Data.Formula;
using SO;
using System.Threading;
using UnityEngine;
using unityroom.Api;

namespace Main.Handler
{
    internal enum GameState
    {
        Stay,
        OnGoing,
        Over
    }

    internal sealed class GameManager : MonoBehaviour
    {
        #region
        internal static GameManager Instance { get; set; } = null;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        #endregion

        [SerializeField, Header("N0 - N9 の順番")] private NumberSpriteFollow[] symbolSprites;
        [SerializeField, Header("E_1 - E_12 の順番")] private Transform[] symbolFrames;
        private Vector2[] _symbolPositions;
        internal Vector2[] SymbolPositions => _symbolPositions;

        private NumberSpriteFollow[] questionInstances;
        private NumberSpriteFollow[] _formulaInstances;
        internal NumberSpriteFollow[] FormulaInstances => _formulaInstances;

        internal GameState State { get; set; }
        internal GameData GameData { get; set; }
        internal Question Question { get; set; }
        internal Formula Formula { get; set; }

        private float _time = 0;
        internal float Time
        {
            get { return _time; }
            set { _time = Mathf.Clamp(value, byte.MinValue, byte.MaxValue); }
        }

        private CancellationToken ct;

        private bool isFirstOnStay = true;
        private bool isFirstOnOver = true;

        private bool _isAttackable = false;
        internal bool IsAttackable => _isAttackable;

        private void Start()
        {
            ct = this.GetCancellationTokenOnDestroy();

            State = GameState.Stay;

            GameData = new();
            GameData.Reset();

            Question = new();

            Formula = new
                (Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE,
                Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE, Symbol.NONE);

            _symbolPositions = symbolFrames.Map(e => e.position.ToVector2()).ToArray();

            _formulaInstances = new NumberSpriteFollow[12];

            Time = SO_Handler.Entity.InitTimeLimt;
        }

        private void Update()
        {
            Do(State switch
            {
                GameState.Stay => OnStay,
                GameState.OnGoing => OnOnGoing,
                GameState.Over => OnOver,
                _ => throw new System.Exception("不正な種類です")
            });
        }

        private void OnStay()
        {
            if (!isFirstOnStay) return;
            else isFirstOnStay = false;

            // 以降は1回だけ実行される

            SO_Handler.Entity.WaitDurOnGameStarted.SecondsWaitAndDo(() =>
            {
                _isAttackable = true;
                CreateQuestion();
                State = GameState.OnGoing;
            },
            ct).Forget();
        }

        private void OnOnGoing()
        {
            if (Time > 0)
            {
                Time -= UnityEngine.Time.deltaTime;
                Time = Mathf.Max(0, Time);
            }

            if (Time <= 0)
            {
                State = GameState.Over;
            }
        }

        private void OnOver()
        {
            if (!isFirstOnOver) return;
            else isFirstOnOver = false;

            // 以降は1回だけ実行される

            SendScore(GameData.DefeatedEnemyNum);
        }

        private void CreateQuestion()
        {
            (Question.N, Question.Target) = RandomGeneration.RandomGenerate();

            // 前の問題のインスタンスを消す
            if (questionInstances != null)
            {
                foreach (var e in questionInstances) Destroy(e.gameObject);
                questionInstances = null;
                _formulaInstances = new NumberSpriteFollow[12];
            }

            // 新しくインスタンスを作る
            questionInstances = new NumberSpriteFollow[4]
            {
                InstantiateNumber(Question.N.N1, 2),
                InstantiateNumber(Question.N.N2, 4),
                InstantiateNumber(Question.N.N3, 7),
                InstantiateNumber(Question.N.N4, 9),
            };
        }

        private NumberSpriteFollow InstantiateNumber(int n, int i)
        {

            Formula.Data[i] = n.ToIntStr();

            Vector2 pos = SymbolPositions[i];
            var prefabInstance = ToInstance(n.ToIntStr());
            var instance =
                Instantiate(prefabInstance, new(pos.x, pos.y, prefabInstance.Z), Quaternion.identity, transform);
            _formulaInstances[i] = instance;
            return instance;
        }

        internal void Attack()
        {
            float? r = Formula.Calcurate();

            if (!r.HasValue)
            {
                // 攻撃失敗

                return;
            }
            else
            {
                // 攻撃成功

                _isAttackable = false;

                float diff = Mathf.Abs(Question.Target - r.Value);
                if (diff != 0) Time -= 2 * diff;
                else Time += 2f;

                GameData.DefeatedEnemyNum++;

                CreateQuestion();

                _isAttackable = true;
            }
        }

        private void SendScore(int score)
        {
            UnityroomApiClient.Instance.SendScore(1, score, ScoreboardWriteMode.HighScoreDesc);
        }

        private void Do(System.Action action)
        {
            action();
        }

        private NumberSpriteFollow ToInstance(IntStr symbol)
        {
            foreach (var e in symbolSprites)
            {
                if (e.Type.GetSymbol() == symbol)
                {
                    return e;
                }
            }

            throw new System.Exception("インスタンスが見つかりませんでした");
        }

        internal int GetIndexFromSymbolPosition(Vector2 pos)
        {
            (_, int i, bool isFound) = SymbolPositions.Find(e => e == pos);

            if (isFound) return i;
            else throw new System.Exception("見つかりませんでした");
        }
    }

    internal static class GameManagerEx
    {
        internal static IntStr ToIntStr(this int val)
        {
            return val switch
            {
                0 => Symbol.N0,
                1 => Symbol.N1,
                2 => Symbol.N2,
                3 => Symbol.N3,
                4 => Symbol.N4,
                5 => Symbol.N5,
                6 => Symbol.N6,
                7 => Symbol.N7,
                8 => Symbol.N8,
                9 => Symbol.N9,
                _ => throw new System.Exception("不正な種類です")
            };
        }
    }
}