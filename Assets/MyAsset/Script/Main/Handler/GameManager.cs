using Cysharp.Threading.Tasks;
using General;
using General.Debug;
using General.Extension;
using Main.Data;
using Main.Data.Formula;
using SO;
using System.Collections.Generic;
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

        [SerializeField, Header("N0 - N9 の順番")] private SpriteFollow[] symbolSprites;
        [SerializeField, Header("E_1 - E_12 の順番")] private Transform[] symbolFrames;

        [SerializeField] private TMPro.TextMeshProUGUI previewText;

        [SerializeField] private AudioSource attackSEAudioSource;
        [SerializeField] private AudioSource attackFailedSEAudioSource;
        [SerializeField] private AudioSource resultSEAudioSource;

        private Vector2[] _symbolPositions;
        internal Vector2[] SymbolPositions => _symbolPositions;

        private SpriteFollow[] _formulaInstances = new SpriteFollow[12];
        internal SpriteFollow[] FormulaInstances => _formulaInstances;

        internal GameState State { get; set; }
        internal GameData GameData { get; set; } = new();
        internal Question Question { get; set; } = new();
        internal Formula Formula { get; set; } = new();

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

        private void OnEnable()
        {
            ct = this.GetCancellationTokenOnDestroy();

            State = GameState.Stay;

            Formula.Init();

            _symbolPositions = symbolFrames.Map(e => e.position.ToVector2()).ToArray();

            previewText.text = "";

            Time = SO_Handler.Entity.InitTimeLimt;
        }

        private void OnDisable()
        {
            System.Array.Clear(symbolSprites, 0, symbolSprites.Length);
            System.Array.Clear(symbolFrames, 0, symbolFrames.Length);
            symbolSprites = null;
            symbolFrames = null;
            previewText = null;
            attackSEAudioSource = null;
            attackFailedSEAudioSource = null;
            resultSEAudioSource = null;

            System.Array.Clear(_formulaInstances, 0, _formulaInstances.Length);
            _formulaInstances = null;

            GameData.Reset();
            GameData = null;

            Question = null;

            Formula.Reset();
            Formula = null;
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

                ShowPreview();
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

            if (resultSEAudioSource != null)
                resultSEAudioSource.Raise(SO_Sound.Entity.ResultSE, SoundType.SE);

            SendScore(GameData.DefeatedEnemyNum);
        }

        private void CreateQuestion()
        {
            (Question.N, Question.Target) = QuestionGenerater.GetRandom();

            // 前の問題のインスタンスを消す
            Formula.Init();
            foreach (var e in _formulaInstances) if (e) Destroy(e.gameObject);
            System.Array.Clear(_formulaInstances, 0, _formulaInstances.Length);

            // 新しくインスタンスを作る
            InstantiateNumber(Question.N.N1, 2);
            InstantiateNumber(Question.N.N2, 4);
            InstantiateNumber(Question.N.N3, 7);
            InstantiateNumber(Question.N.N4, 9);
        }

        private void InstantiateNumber(int n, int i)
        {
            Formula.Data[i] = n.ToIntStr();

            Vector2 pos = SymbolPositions[i];
            var prefabInstance = ToInstance(n.ToIntStr());
            var instance =
                Instantiate(prefabInstance, pos.ToVector3(prefabInstance.Z), Quaternion.identity, transform);
            _formulaInstances[i] = instance;
        }

        private void ShowPreview()
        {
            if (previewText == null) return;

            float? r = Formula.Calcurate();
            if (r.HasValue)
            {
                previewText.text = $"= {(int)r.Value}";
                previewText.color = (r.Value == Question.Target) ?
                    new Color32(225, 225, 0, 255) : new Color32(225, 0, 225, 255);
            }
            else
            {
                previewText.text = "";
            }
        }

        internal void Attack()
        {
            float? r = Formula.Calcurate();

            if (r.HasValue)  // 攻撃成功
            {
                attackSEAudioSource.Raise(SO_Sound.Entity.AttackSE, SoundType.SE);

                float diff = Mathf.Abs(Question.Target - r.Value);
                var list = System.Array.AsReadOnly(SO_Handler.Entity.TimeIncreaseAmountList);
                float errorOfst = 0.01f;
                for (int i = 0; i < list.Count; i++)
                {
                    if (i + errorOfst < diff) continue;
                    Time += list[i];
                    break;
                }

                GameData.DefeatedEnemyNum++;

                CreateQuestion();
            }
            else  // 攻撃失敗
            {
                attackFailedSEAudioSource.Raise(SO_Sound.Entity.AttackFailedSE, SoundType.SE);
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

        private SpriteFollow ToInstance(IntStr symbol)
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

    internal static class QuestionGenerater
    {
        internal static ((int N1, int N2, int N3, int N4) N, int Target) GetRandom()
        {
            return (CreateLicensePlateNumbers(), CreateTargetNumber());
        }

        private static (int N1, int N2, int N3, int N4) CreateLicensePlateNumbers()
        {
            return CreateRandomNumbers
                    (
                    e => e.Count(0) <= 1,  // 0の数が1個以下
                    e => e.Count().Max() <= 2,  // 重複している数が2個以下
                    e => e.Count(0, 1, 7).Sum() <= 2  // 使いにくい数の合計が2個以下
                    );
        }

        private static int CreateTargetNumber()
        {
            return Random.Range(5, 16);
        }

        /// <summary>
        /// 条件を全て満たす組を生成
        /// </summary>
        private static (int N1, int N2, int N3, int N4) CreateRandomNumbers
            (params System.Func<(int N1, int N2, int N3, int N4), bool>[] functions)
        {
            int cnt = 0;
            while (true)
            {
                var n = CreateRandomNumbers();
                if (n.All(functions)) return n;
                if (++cnt > byte.MaxValue) return (0, 0, 0, 0);
            }
        }

        /// <summary>
        /// ランダムに組を生成
        /// </summary>
        private static (int N1, int N2, int N3, int N4) CreateRandomNumbers()
        {
            return (Random.Range(0, 10), Random.Range(0, 10), Random.Range(0, 10), Random.Range(0, 10));
        }

        /// <summary>
        /// 組の中で、targetの個数を数える
        /// </summary>
        private static int Count(this (int N1, int N2, int N3, int N4) n, int target)
        {
            int ret = 0;
            if (n.N1 == target) ret++;
            if (n.N2 == target) ret++;
            if (n.N3 == target) ret++;
            if (n.N4 == target) ret++;
            return ret;
        }

        /// <summary>
        /// 組の中で、targetsの個数を順に数える
        /// </summary>
        private static IEnumerable<int> Count(this (int N1, int N2, int N3, int N4) n, params int[] targets)
        {
            foreach (int e in targets)
            {
                yield return n.Count(e);
            }
        }

        /// <summary>
        /// 組の中で、0~9の個数を順に数える
        /// </summary>
        private static IEnumerable<int> Count(this (int N1, int N2, int N3, int N4) n)
        {
            for (int i = 0; i < 10; i++)
            {
                yield return n.Count(i);
            }
        }

        /// <summary>
        /// 最小を求める
        /// </summary>
        private static int Min(this IEnumerable<int> itr)
        {
            int ret = int.MaxValue;
            foreach (int e in itr) ret = Mathf.Min(ret, e);
            return ret;
        }

        /// <summary>
        /// 最大を求める
        /// </summary>
        private static int Max(this IEnumerable<int> itr)
        {
            int ret = int.MinValue;
            foreach (int e in itr) ret = Mathf.Max(ret, e);
            return ret;
        }

        /// <summary>
        /// 合計を求める
        /// </summary>
        private static int Sum(this IEnumerable<int> itr)
        {
            int ret = 0;
            foreach (int e in itr) ret += e;
            return ret;
        }
    }
}