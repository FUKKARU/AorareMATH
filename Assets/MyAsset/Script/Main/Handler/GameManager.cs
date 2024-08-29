using Cysharp.Threading.Tasks;
using General.Debug;
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
                _ => throw new System.Exception("•s³‚ÈŽí—Þ‚Å‚·")
            });
        }

        private void OnStay()
        {
            if (!isFirstOnStay) return;
            else isFirstOnStay = false;

            SO_Handler.Entity.WaitDurOnGameStarted.SecondsWaitAndDo(() =>
            {
                CreateQuestion();
                State = GameState.OnGoing;
            },
            ct).Forget();
        }

        private void OnOnGoing()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Attack(new(Symbol.N1, Symbol.OA, Symbol.N3));
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                Attack(new(Symbol.N1, Symbol.OA));
            }

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
        }

        internal void Attack(Formula formula)
        {
            float? r = formula.Calcurate();

            if (!r.HasValue)
            {
                // UŒ‚Ž¸”s

                return;
            }
            else
            {
                // UŒ‚¬Œ÷

                float diff = Mathf.Abs(Question.Target - r.Value);
                if (diff != 0) Time -= 2 * diff;
                else Time += 2f;

                GameData.DefeatedEnemyNum++;

                CreateQuestion();
            }
        }

        private void Do(System.Action action)
        {
            action();
        }
    }

    internal static class Ex
    {
        internal static async UniTask SecondsWaitAndDo(this float waitSeconds, System.Action act, CancellationToken ct)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(waitSeconds));
            act();
        }
    }
}