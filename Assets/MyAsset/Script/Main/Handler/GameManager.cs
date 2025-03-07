using Cysharp.Threading.Tasks;
using DG.Tweening;
using General;
using General.Extension;
using Main.Data;
using Main.Data.Formula;
using SO;
using System;
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

    internal sealed class GameManager : ASingletonMonoBehaviour<GameManager>
    {
        [SerializeField, Header("N0 - N9 の順番")] private SpriteFollow[] symbolSprites;
        [SerializeField, Header("OA, OS, OM, OD, PL, PR の順番\nアシストありの時だけ使用")] private UnNumberSpriteFollow[] assistSymbolSprites;
        [SerializeField, Header("E_1 - E_12 の順番")] private Transform[] symbolFrames;

        [SerializeField] private Transform loadImageTf;
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

        internal GameState State { get; set; } = GameState.Stay;
        internal GameData GameData { get; set; } = new();
        internal Question Question { get; set; } = new();
        internal Formula Formula { get; set; } = new();

        // 事前生成の問題データから、一定個数をシャッフルして抽出
        private ((int N1, int N2, int N3, int N4) N, int Target, Formula Answer)[] questions = null;

        private float _time = 0;
        internal float Time
        {
            get { return _time; }
            set { _time = Mathf.Clamp(value, byte.MinValue, byte.MaxValue); }
        }

        private bool isFirstOnStay = true;
        private bool isFirstOnOver = true;
        private bool isAttackable = false;
        private bool hasForciblyCleared = false;

        private void OnEnable()
        {
            State = GameState.Stay;

            Formula.Init();
            PickupQuestions();

            _symbolPositions = symbolFrames.Select(e => e.position.ToVector2()).ToArray();

            SetPositionX(loadImageTf, 0);
            SetPreviewText(text: string.Empty);

            Time = SO_Handler.Entity.InitTimeLimt;
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

            OnLoadFinished(destroyCancellationToken).Forget();
        }

        private void OnOnGoing()
        {
            if (isAttackable && Input.GetKeyDown(KeyCode.Space)) Attack();

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

            OnResult(destroyCancellationToken).Forget();
        }

        private void CreateQuestion()
        {
            (Question.N, Question.Target, _) = questions[GameData.CorrectAmount];

            // 前の問題のインスタンスを消す
            Formula.Init();
            foreach (var e in _formulaInstances) if (e) Destroy(e.gameObject);
            Array.Clear(_formulaInstances, 0, _formulaInstances.Length);

            // 新しくインスタンスを作る
            InstantiateNumbers(Question.N);
        }

        private void InstantiateNumbers((int N1, int N2, int N3, int N4) n)
        {
            int[] numbers = new int[4] { n.N1, n.N2, n.N3, n.N4 };
            Shuffle(numbers);

            InstantiateNumber(numbers[0], 2);
            InstantiateNumber(numbers[1], 4);
            InstantiateNumber(numbers[2], 7);
            InstantiateNumber(numbers[3], 9);
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

        private void PickupQuestions()
        {
            var fixedQuestions = FixedQuestions.Data.ToArray();
            Shuffle(fixedQuestions);
            this.questions = fixedQuestions.Take(SO_Handler.Entity.QuestionAmount).ToArray();
        }

        private void ShowPreview()
        {
            float? r = Formula.Calcurate();
            if (r.HasValue)
            {
                SetPreviewText(text: $"= {(int)r.Value}");

                float diff = Mathf.Abs(Question.Target - r.Value);
                Color32 color = diff < SO_Handler.DiffLimit ? Color.yellow : Color.red;
                SetPreviewText(color: color);
            }
            else
            {
                SetPreviewText(text: string.Empty);
            }
        }

        private void SetPreviewText(string text = null, Color? color = null)
        {
            if (previewText != null)
            {
                if (text != null) previewText.text = text;
                if (color.HasValue) previewText.color = color.Value;
            }
        }

        private void Attack()
        {
            float? r = Formula.Calcurate();

            if (r.HasValue)
            {
                float diff = Mathf.Abs(Question.Target - r.Value);
                Do((diff <= SO_Handler.DiffLimit) ? () => OnAttackSucceeded(diff) : OnAttackFailed);
            }
            else
            {
                OnAttackFailed();
            }
        }

        private void OnAttackSucceeded(float diff)
        {
            if (attackSEAudioSource != null) attackSEAudioSource.Raise(SO_Sound.Entity.AttackSE, SoundType.SE);
            if (justAttackedSEAudioSource != null) justAttackedSEAudioSource.Raise(SO_Sound.Entity.JustAttackedSE, SoundType.SE);
            if (justEffectLeft != null) justEffectLeft.Play();
            if (justEffectRight != null) justEffectRight.Play();

            float timeDiff = SO_Handler.Entity.TimeIncreaseAmount;
            Time += timeDiff;
            showTimerDiff.PlayAnimation(1.0f, 260, $"+ {(int)timeDiff}", Color.yellow);

            if (++GameData.CorrectAmount >= SO_Handler.Entity.QuestionAmount)
            {
                State = GameState.Over;
                hasForciblyCleared = true;
                return;
            }

            CreateQuestion();
        }

        private void OnAttackFailed()
        {
            attackFailedSEAudioSource.Raise(SO_Sound.Entity.AttackFailedSE, SoundType.SE);
        }

        internal void PlaySelectSE() => selectSEAudioSource.Raise(SO_Sound.Entity.SymbolSE, SoundType.SE);

#if false
        private void SendScore()
        {
            SendScoreImpl(1, GameData.CorrectAmount);
        }

        private void SendScoreImpl(int boardNo, int score)
           => UnityroomApiClient.Instance.SendScore(boardNo, score, ScoreboardWriteMode.HighScoreDesc);
#endif

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
            await UniTask.WaitForSeconds(0.2f, cancellationToken: ct);
            await loadImageTf.DOLocalMoveX(18.5f, 0.5f).ConvertToUniTask(loadImageTf, ct);
            await UniTask.WaitForSeconds(0.2f, cancellationToken: ct);
            await countDown.Play(ct);
            await UniTask.WaitForSeconds(0.2f, cancellationToken: ct);

            isAttackable = true;
            CreateQuestion();
            State = GameState.OnGoing;
        }

        private async UniTask OnResult(CancellationToken ct)
        {
            resultSEAudioSource.Raise(SO_Sound.Entity.ResultSE, SoundType.SE);

#if false
            SendScore();
#endif

            await resultShower.Play(GameData.CorrectAmount, hasForciblyCleared, ct);

            string sceneName;
            while (true)
            {
                if (Input.GetKeyDown(KeyCode.R)) { sceneName = SO_SceneName.Entity.Main; break; }
                else if (Input.GetKeyDown(KeyCode.Space)) { sceneName = SO_SceneName.Entity.Title; break; }

                await UniTask.NextFrame(ct);
            }
            SceneManager.LoadScene(sceneName);
        }

        private void Shuffle<T>(T[] array)
        {
            if (array == null) return;
            int n = array.Length;
            if (n <= 0) return;
            for (int i = n - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                var tmp = array[i];
                array[i] = array[j];
                array[j] = tmp;
            }
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
}