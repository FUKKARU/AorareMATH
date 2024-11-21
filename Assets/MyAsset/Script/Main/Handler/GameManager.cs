using Cysharp.Threading.Tasks;
using DG.Tweening;
using General;
using General.Extension;
using Main.Data;
using Main.Data.Formula;
using SO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using unityroom.Api;
using Random = UnityEngine.Random;

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
        [SerializeField, Header("OA, OS, OM, OD, PL, PR の順番\nアシストありの時だけ使用")] private UnNumberSpriteFollow[] assistSymbolSprites;
        [SerializeField, Header("E_1 - E_12 の順番")] private Transform[] symbolFrames;

        [SerializeField] private EndValue endValue;
        [SerializeField] private Duration duration;

        [SerializeField] private Transform loadImageTf;
        [SerializeField] private GameObject sceneReloader;
        [SerializeField] private TMPro.TextMeshProUGUI previewText;
        [SerializeField] private CountDown countDown;
        [SerializeField] private ShowTimerDiff showTimerDiff;
        [SerializeField] private ParticleSystem justEffectLeft;
        [SerializeField] private ParticleSystem justEffectRight;
        [SerializeField] private ResultShower resultShower;

        [SerializeField] private AudioSource selectSEAudioSource;
        [SerializeField] private AudioSource attackSEAudioSource;
        [SerializeField] private AudioSource attackFailedSEAudioSource;
        [SerializeField] private AudioSource justAttackedSEAudioSource;
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

        private Vector3 timerDiffTextInitPosition;

        private bool isFirstOnStay = true;
        private bool isFirstOnOver = true;

        private bool _isAttackable = false;
        internal bool IsAttackable => _isAttackable;

        private void OnEnable()
        {
            ct = this.GetCancellationTokenOnDestroy();

            State = GameState.Stay;

            Formula.Init();

            _symbolPositions = symbolFrames.Select(e => e.position.ToVector2()).ToArray();

            SetPositionX(loadImageTf, 0);
            sceneReloader.SetActive(false);
            SetText(previewText);

            Time = SO_Handler.Entity.InitTimeLimt;
        }

        private void OnDisable()
        {
            symbolSprites = null;
            symbolFrames = null;
            loadImageTf = null;
            sceneReloader = null;
            previewText = null;
            countDown = null;
            showTimerDiff = null;
            justEffectLeft = null;
            justEffectRight = null;
            resultShower = null;
            selectSEAudioSource = null;
            attackSEAudioSource = null;
            attackFailedSEAudioSource = null;
            justAttackedSEAudioSource = null;
            resultSEAudioSource = null;
            _formulaInstances = null;
            GameData = null;
            Question = null;
            Formula = null;
        }

        private void Update()
        {
            Do(State switch
            {
                GameState.Stay => OnStay,
                GameState.OnGoing => OnOnGoing,
                GameState.Over => OnOver,
                _ => null
            });
        }

        private void OnStay()
        {
            if (!isFirstOnStay) return;
            else isFirstOnStay = false;

            // 以降は1回だけ実行される

            OnLoadFinished(ct).Forget();
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

            OnResult(ct).Forget();
        }

        private void CreateQuestion()
        {
            (Question.N, Question.Target) = QuestionGenerater.GetNew(out Formula answer);

            // 前の問題のインスタンスを消す
            Formula.Init();
            foreach (var e in _formulaInstances) if (e) Destroy(e.gameObject);
            Array.Clear(_formulaInstances, 0, _formulaInstances.Length);

            // 新しくインスタンスを作る
            InstantiateNumbers(Question.N, answer);
        }

        private void InstantiateNumbers((int N1, int N2, int N3, int N4) n, Formula answer)
        {
            switch (Difficulty.Value)
            {
                // nは無視
                case Difficulty.Type.EasyWithAssist:
                    {
                        // 数字の位置一覧
                        int[] indices = new int[4];
                        int _i = 0;

                        // 数字の位置を探して生成(引数が正しい前提)
                        foreach ((int i, IntStr e) in answer.Data.Enumerate())
                        {
                            if (Symbol.IsNumber(e) == true)
                            {
                                InstantiateNumber(e.Int, i);
                                indices[_i++] = i;
                            }
                        }

                        // ランダムに取得する演算子のインデックス(演算子が1つ以上存在する前提)
                        int j = 0;
                        do
                        {
                            j = Random.Range(0, answer.Data.Count);
                        }
                        while (Symbol.IsOperator(answer.Data[j]) != true);
                        InstantiateAssistUnNumber(answer.Data[j], j);
                    }
                    break;
                // answerは無視
                case Difficulty.Type.EasyWithoutAssist:
                    {
                        InstantiateNumber(n.N1, 2);
                        InstantiateNumber(n.N2, 4);
                        InstantiateNumber(n.N3, 7);
                        InstantiateNumber(n.N4, 9);
                    }
                    break;
                // answerは無視
                case Difficulty.Type.Normal:
                    {
                        InstantiateNumber(n.N1, 2);
                        InstantiateNumber(n.N2, 4);
                        InstantiateNumber(n.N3, 7);
                        InstantiateNumber(n.N4, 9);
                    }
                    break;
                default: return;
            }
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

        private void InstantiateAssistUnNumber(IntStr symbol, int i)
        {
            Formula.Data[i] = symbol;

            Vector2 pos = SymbolPositions[i];
            var assistInstance = ToAssistInstance(symbol);
            assistInstance.ForciblyInstantiateSpriteFollowHere(symbol, i);
        }

        private void ShowPreview()
        {
            float? r = Formula.Calcurate();
            if (r.HasValue)
            {
                SetText(previewText, $"= {(int)r.Value}");

                float diff = Mathf.Abs(Question.Target - r.Value);
                Color32 color;
                if (SO_Handler.Entity.DiffLimit < diff) color = Colors.ValueFar;
                else if (SO_Handler.Entity.JustDiffLimit < diff) color = Colors.ValueClose;
                else color = Colors.ValueJust;
                SetColor(previewText, color);
            }
            else
            {
                SetText(previewText);
            }
        }

        private void SetText(TMPro.TextMeshProUGUI tmpro, string text = "")
        {
            if (tmpro == null) return;
            if (text == null) text = "";

            tmpro.text = text;
        }

        private void SetColor(TMPro.TextMeshProUGUI tmpro, Color32 color)
        {
            if (tmpro == null) return;

            tmpro.color = color;
        }

        internal void Attack()
        {
            float? r = Formula.Calcurate();

            if (r.HasValue)
            {
                float diff = Mathf.Abs(Question.Target - r.Value);
                Do((diff <= SO_Handler.Entity.DiffLimit) ? () => OnAttackSucceeded(diff) : OnAttackFailed);
            }
            else
            {
                OnAttackFailed();
            }
        }

        private void OnAttackSucceeded(float diff)
        {
            bool isJust = (diff == 0);

            if (attackSEAudioSource) attackSEAudioSource.Raise(SO_Sound.Entity.AttackSE, SoundType.SE);
            if (isJust)
            {
                if (justAttackedSEAudioSource)
                    justAttackedSEAudioSource.Raise(SO_Sound.Entity.JustAttackedSE, SoundType.SE);
                if (justEffectLeft) justEffectLeft.Play();
                if (justEffectRight) justEffectRight.Play();
            }

            var list = Array.AsReadOnly(SO_Handler.Entity.TimeIncreaseAmountList);
            for (int i = 0; i < list.Count; i++)
            {
                if (i < diff) continue;

                float timeDiff = list[i];
                Time += timeDiff;
                showTimerDiff.PlayAnimation
                    (duration.TimerDiffText, endValue.TimerDiffText,
                    $"+ {(int)timeDiff}",
                    isJust ? Colors.ValueJust : Colors.ValueFar);
                break;
            }

            GameData.DefeatedEnemyNum++;
            if (isJust) GameData.PerfectlyDefeatedEnemyNum++;

            CreateQuestion();
        }

        private void OnAttackFailed()
        {
            attackFailedSEAudioSource.Raise(SO_Sound.Entity.AttackFailedSE, SoundType.SE);
        }

        internal void PlaySelectSE() => selectSEAudioSource.Raise(SO_Sound.Entity.SymbolSE, SoundType.SE);

        private void SendScore()
        {
            SendScoreImpl(1, GameData.DefeatedEnemyNum);
            SendScoreImpl(2, GameData.PerfectlyDefeatedEnemyNum);
        }

        private void SendScoreImpl(int boardNo, int score)
           => UnityroomApiClient.Instance.SendScore(boardNo, score, ScoreboardWriteMode.HighScoreDesc);

        private void Do(Action action) => action?.Invoke();

        private SpriteFollow ToInstance(IntStr symbol)
        {
            foreach (var e in symbolSprites)
            {
                if (e.Type.GetSymbol() == symbol)
                {
                    return e;
                }
            }

            throw new Exception("インスタンスが見つかりませんでした");
        }

        private UnNumberSpriteFollow ToAssistInstance(IntStr symbol)
        {
            foreach (var e in assistSymbolSprites)
            {
                if (e.Type.GetSymbol() == symbol)
                {
                    return e;
                }
            }

            throw new Exception("インスタンスが見つかりませんでした");
        }

        internal int GetIndexFromSymbolPosition(Vector2 pos)
        {
            (_, int i, bool isFound) = SymbolPositions.Find(e => e == pos);

            if (isFound) return i;
            else throw new Exception("見つかりませんでした");
        }

        private void SetPositionX(Transform tf, float x)
        {
            Vector3 pos = tf.position;
            pos.x = x;
            tf.position = pos;
        }

        private async UniTask OnLoadFinished(CancellationToken ct)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(duration.BeforeLoadImage), cancellationToken: ct);
            await loadImageTf.DOLocalMoveX(endValue.LoadImage, duration.LoadImageMove).ToUniTask(cancellationToken: ct);
            await UniTask.Delay(TimeSpan.FromSeconds(duration.LoadImageToCountDown), cancellationToken: ct);
            sceneReloader.SetActive(true);
            await countDown.Play(ct);
            await UniTask.Delay(TimeSpan.FromSeconds(duration.CountDownToGameStart), cancellationToken: ct);

            _isAttackable = true;
            CreateQuestion();
            State = GameState.OnGoing;
        }

        private async UniTask OnResult(CancellationToken ct)
        {
            sceneReloader.SetActive(false);

            resultSEAudioSource.Raise(SO_Sound.Entity.ResultSE, SoundType.SE);

            SendScore();

            await resultShower.Play(GameData.DefeatedEnemyNum, GameData.PerfectlyDefeatedEnemyNum, ct);

            string sceneName;
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.R)) { sceneName = SO_SceneName.Entity.Main; break; }
                else if (Input.GetKeyDown(KeyCode.Space)) { sceneName = SO_SceneName.Entity.Title; break; }

                await UniTask.Yield(ct);
            }
            SceneManager.LoadScene(sceneName);
        }

        [Serializable]
        internal struct EndValue
        {
            [SerializeField] internal float LoadImage;
            [SerializeField] internal float TimerDiffText;
        }

        [Serializable]
        internal struct Duration
        {
            [SerializeField] internal float BeforeLoadImage;
            [SerializeField] internal float LoadImageMove;
            [SerializeField] internal float LoadImageToCountDown;
            [SerializeField] internal float CountDownToGameStart;
            [SerializeField] internal float TimerDiffText;
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
                _ => throw new Exception("不正な種類です")
            };
        }
    }

    internal static class QuestionGenerater
    {
        internal static ((int N1, int N2, int N3, int N4) N, int Target) GetNew(out Formula answer)
        {
            switch (Difficulty.Value)
            {
                case Difficulty.Type.EasyWithAssist: return GetFixed(out answer);
                case Difficulty.Type.EasyWithoutAssist: return GetFixed(out answer);
                case Difficulty.Type.Normal: answer = null; return GetRandom();
                default: answer = null; return default;
            }
        }

        // ランダムに生成
        private static ((int N1, int N2, int N3, int N4) N, int Target) GetRandom()
        {
            return (CreateLicensePlateNumbers(), CreateTargetNumber());
        }

        // 事前生成
        private static ((int N1, int N2, int N3, int N4) N, int Target) GetFixed(out Formula answer)
        {
            if (FixedQuestions == null) { answer = null; return default; }
            int len = FixedQuestions.Count;
            if (len == 0) { answer = null; return default; }
            int index = Random.Range(0, len);
            ((int n1, int n2, int n3, int n4) n, int target, Formula _answer) = FixedQuestions[index];
            answer = _answer;
            return (n, target);
        }

        // 事前生成のデータ(両端の間隔も意識して、データを格納すること！)
        // ↓は適当なデータ
        internal static ReadOnlyCollection<((int N1, int N2, int N3, int N4) N, int Target, Formula Answer)> FixedQuestions =
        new List<((int N1, int N2, int N3, int N4) N, int Target, Formula Answer)>()
        {
            ((1, 2, 3, 4), 10, new Formula()
            {
                Data = new()
                {
                   Symbol.NONE,Symbol.NONE, Symbol.N1, Symbol.OA, Symbol.N2, Symbol.OA,
                    Symbol.NONE, Symbol.N3, Symbol.OA, Symbol.N4,Symbol.NONE,Symbol.NONE
                }
            }),
             ((1, 9, 1, 9), 10, new Formula()
            {
                Data = new()
                {
                    Symbol.NONE, Symbol.PL, Symbol.N1, Symbol.OD, Symbol.N9, Symbol.OA,
                    Symbol.N1, Symbol.PR,Symbol.OM,Symbol.N9,Symbol.NONE,Symbol.NONE
                }
            }),
             ((1, 2, 5, 1), 10, new Formula()
            {
                Data = new()
                {
                    Symbol.NONE, Symbol.PL, Symbol.N1, Symbol.OM, Symbol.PL, Symbol.N2,
                    Symbol.OA,Symbol.N5,Symbol.PR,Symbol.PR,Symbol.OM,Symbol.N1
                }
            })
        }.AsReadOnly();

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
            (params Func<(int N1, int N2, int N3, int N4), bool>[] functions)
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

    internal static class Colors
    {
        internal static readonly Color32 ValueJust = new(225, 225, 0, 255);
        internal static readonly Color32 ValueClose = new(225, 0, 225, 255);
        internal static readonly Color32 ValueFar = new(225, 0, 0, 255);
    }
}