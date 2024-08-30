using Cysharp.Threading.Tasks;
using General.Debug;
using General.Extension;
using Main.Data;
using Main.Data.Formula;
using SO;
using System.Threading;
using UnityEngine;

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

        [SerializeField, Header("N0-N9,OA-OD,PL-PR の順番")] private SymbolSprite[] symbolSprites;

        private SymbolSprite[] questionInstances;

        internal GameState State { get; set; }
        internal GameData GameData { get; set; }
        internal Question Question { get; set; }

        private float _time = 0;
        internal float Time
        {
            get { return _time; }
            set { _time = Mathf.Clamp(value, byte.MinValue, byte.MaxValue); }
        }

        private CancellationToken ct;

        private bool isFirstOnStay = true;

        private bool _isAttackable = false;
        internal bool IsAttackable => _isAttackable;

        private void Start()
        {
            ct = this.GetCancellationTokenOnDestroy();

            State = GameState.Stay;

            GameData = new();
            GameData.Reset();

            Question = new();

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
            "Over".Show();
        }

        private void CreateQuestion()
        {
            (Question.N, Question.Target) = RandomGeneration.RandomGenerate();

            // 前の問題のインスタンスを消す
            if (questionInstances != null)
            {
                foreach (var e in questionInstances)
                {
                    Destroy(e.gameObject);
                }
                questionInstances = null;
            }

            // 新しくインスタンスを作る
            questionInstances = new SymbolSprite[4]
            {
                Instantiate(ToInstance(Question.N.N1.ToIntStr()), new(-5, 0), Quaternion.identity, transform),
                Instantiate(ToInstance(Question.N.N2.ToIntStr()), new(-2.5f, 0), Quaternion.identity, transform),
                Instantiate(ToInstance(Question.N.N3.ToIntStr()), new(2.5f, 0), Quaternion.identity, transform),
                Instantiate(ToInstance(Question.N.N4.ToIntStr()), new(5, 0), Quaternion.identity, transform)
            };
        }

        internal void Attack()
        {
            // 要修正
            Formula formula = new();

            float? r = formula.Calcurate();

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

        private void Do(System.Action action)
        {
            action();
        }

        private SymbolSprite ToInstance(IntStr symbol)
        {
            foreach (var e in symbolSprites)
            {
                if (e.GetSymbol() == symbol)
                {
                    return e;
                }
            }

            throw new System.Exception("インスタンスが見つかりませんでした");
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