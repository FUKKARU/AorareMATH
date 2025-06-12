using Cysharp.Threading.Tasks;
using General;
using General.Debug;
using General.Extension;
using Main.Data;
using Main.Data.Formula;
using SO;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;
using Ct = System.Threading.CancellationToken;

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

        [SerializeField] private Text previewText;
        [SerializeField] private Text targetText;
        [SerializeField] private Image everythingBlockingImage;
        [SerializeField] private SceneTransitionShaderController sceneTransitionShaderController;
        [SerializeField] private BGMPlayer bgmPlayer;
        [SerializeField] private CountDown countDown;
        [SerializeField] private TimeShower timeShower;
        [SerializeField] private CorrectAmountTextShower correctAmountTextShower;
        [SerializeField] private SkipButtonObserver skipButtonHandler;
        [SerializeField] private ParticleSystem justEffectLeft;
        [SerializeField] private ParticleSystem justEffectRight;
        [SerializeField] private ResultShower resultShower;

        [SerializeField] private AudioSource selectSEAudioSource;
        [SerializeField] private AudioSource attackSEAudioSource;
        [SerializeField] private AudioSource attackFailedSEAudioSource;
        [SerializeField] private AudioSource justAttackedSEAudioSource;
        [SerializeField] private AudioSource resultSEAudioSource;
        [SerializeField, Range(0.01f, 5.0f)] private float mouseHoverSymbolFrameLimitWidth;
        [SerializeField, Range(0.01f, 5.0f)] private float mouseHoverSymbolFrameLimitHeight;

        private Vector2[] _symbolPositions;
        internal Vector2[] SymbolPositions => _symbolPositions;

        private SpriteFollow[] _formulaInstances = new SpriteFollow[12];
        internal SpriteFollow[] FormulaInstances => _formulaInstances;

        internal GameState State { get; private set; } = GameState.Stay; // ゲームの状態
        internal GameData GameData { get; private set; } = new(); // セーブデータ（正解数を格納）
        internal Formula Formula { get; private set; } = new(); // 出題中の問題
        private int target = 0; // 出題中の問題の答え

        internal GameDataHolder GameDataHolder { get; private set; } = new();

        private float _time = 0;
        private float time
        {
            get { return _time; }
            set
            {
                _time = Mathf.Clamp(value, 0, SO_Handler.Entity.InitTimeLimt);
                timeShower.UpdateTimeUI(_time);
            }
        }
        /// <summary>
        /// Rank
        /// </summary>
        internal bool IsHoldingSymbol { get; set; } = false;
        internal bool IsPreviewNumberSameAsTargetThisFrame { get; private set; } = false;

        private bool isFirstOnStay = true;
        private bool isFirstOnOver = true;
        private bool canTimeDecrease = true; // Attackの演出時、時間が減らないようにする
        private bool isDoingAttack = false;
        private bool hasForciblyCleared = false;

        private static readonly float selectSeInterval = 0.1f;
        private bool canPlaySelectSe = false;

        private void OnEnable()
        {
            State = GameState.Stay;

            GameDataHolder?.Init();
            Formula.Init();

            _symbolPositions = symbolFrames.Select(e => e.position.ToVector2()).ToArray();

            SetTargetText(string.Empty);
            SetPreviewText(text: string.Empty);

            time = SO_Handler.Entity.InitTimeLimt;
        }

        private void Update()
        {
            Action action = State switch
            {
                GameState.Stay => OnStay,
                GameState.OnGoing => OnOnGoing,
                GameState.Over => OnOver,
                _ => null
            };
            action?.Invoke();
        }

        private void LateUpdate()
        {
            IsPreviewNumberSameAsTargetThisFrame = false;
        }

        private void OnDestroy()
        {
            GameDataHolder?.Dispose();
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
            CheckSkip();  // スキップボタンが押されたかを監視し、押されたなら次の問題にするよう非同期に発火する
            CheckFormula();  // 入力欄を監視し、ピッタリなら正解演出を非同期に発火する

            if (time > 0)
            {
                if (canTimeDecrease)
                {
                    time -= Time.deltaTime;
                    time = Mathf.Max(0, time);
                }

                ShowPreview();
            }

            if (time <= 0)
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
            bool result = GameDataHolder.CorrectAmount.ToQuestionType().GetNewQuestion(out int[] numbers, out int target, out string answer);
            if (!result) return;
            this.target = target;
#if UNITY_EDITOR
            answer.Show();
#endif

            // インスタンスを作り直す
            DestroyInstances();
            CreateInstances();

            return;



            void DestroyInstances()
            {
                Formula.Init();

                foreach (var e in _formulaInstances) if (e) Destroy(e.gameObject);
                Array.Clear(_formulaInstances, 0, _formulaInstances.Length);

                SetTargetText(string.Empty);
            }

            void CreateInstances()
            {
                InstantiateNumbers(numbers: numbers);
                SetTargetText(target.ToString());

                void InstantiateNumbers(bool doShuffle = true, params int[] numbers)
                {
                    if (numbers == null) return;
                    if (numbers.Length <= 0 || Formula.MaxLength < numbers.Length) return;
                    if (doShuffle) numbers.ShuffleSelf();

                    int brankAmount = Formula.MaxLength - numbers.Length;
                    float brankLength = 1.0f * brankAmount / (numbers.Length + 1);
                    for (int i = 0; i < numbers.Length; i++)
                    {
                        float _x = brankLength * (i + 1) + i + 0.49f; // 左端からの位置（インデックスを小数に拡張した感じ）
                        int x = Mathf.Clamp(Mathf.RoundToInt(_x), 0, Formula.MaxLength - 1); // 丸める（このインデックスに数字を生成）
                        InstantiateNumber(numbers[i], x);
                    }
                }

                void InstantiateNumber(int n, int i)
                {
                    IntStr intStr = new(n);
                    Formula.Data[i] = intStr;

                    Vector2 pos = SymbolPositions[i];
                    var prefabInstance = ToInstance(intStr);
                    var instance = Instantiate(prefabInstance, pos.ToVector3(prefabInstance.Z), Quaternion.identity, transform);
                    _formulaInstances[i] = instance;
                }
            }
        }

        private void ShowPreview()
        {
            IsPreviewNumberSameAsTargetThisFrame = false;

            float? r = Formula.Calcurate();
            if (r.HasValue)
            {
                SetPreviewText(text: $"= {(int)r.Value}");

                float diff = Mathf.Abs(target - r.Value);
                bool isSame = diff < SO_Handler.DiffLimit;

                IsPreviewNumberSameAsTargetThisFrame = isSame;
                Color32 color = isSame ? Color.yellow : Color.red;
                SetPreviewText(color: color);
            }
            else
            {
                SetPreviewText(text: "= <size=120>計算不可</size>");
                SetPreviewText(color: Color.red);
            }
        }

        private void SetTargetText(string text)
        {
            if (targetText != null) targetText.text = text;
        }

        private void SetPreviewText(string text = null, Color? color = null)
        {
            if (previewText != null)
            {
                if (text != null) previewText.text = text;
                if (color.HasValue) previewText.color = color.Value;
            }
        }

        private void CheckSkip()
        {
            if (skipButtonHandler == null || !skipButtonHandler.IsClickedThisFrame) return;
            Skip(destroyCancellationToken).Forget();
        }

        // 式を計算し、ピッタリならアタックする
        private void CheckFormula()
        {
            float? r = Formula?.Calcurate();
            if (!r.HasValue) return;

            if (Mathf.Abs(target - r.Value) <= SO_Handler.DiffLimit)
                Attack(destroyCancellationToken).Forget();
        }

        // 問題数は進まない仕様
        private async UniTaskVoid Skip(Ct ct)
        {
            if (isDoingAttack) return;

            // フラグON
            canTimeDecrease = false;
            isDoingAttack = true;
            if (everythingBlockingImage != null) everythingBlockingImage.enabled = true;

            // この中以外は、Attackと同じ！
            {
                // TODO: 正解を見せたい

                await 1.0f.SecondsWait(ct);
            }

            // フラグOFF
            if (everythingBlockingImage != null) everythingBlockingImage.enabled = false;
            isDoingAttack = false;
            canTimeDecrease = true;

            // 新しく問題を作成
            if (State != GameState.OnGoing) return;
            CreateQuestion();
        }

        private async UniTaskVoid Attack(Ct ct)
        {
            if (isDoingAttack) return;

            // フラグON
            canTimeDecrease = false;
            isDoingAttack = true;
            if (everythingBlockingImage != null) everythingBlockingImage.enabled = true;

            {
                // 演出部
                {
                    if (attackSEAudioSource != null)
                        attackSEAudioSource.Raise(SO_Sound.Entity.AttackSE, SoundType.SE, volume: 0.5f);
                    if (justAttackedSEAudioSource != null)
                        justAttackedSEAudioSource.Raise(SO_Sound.Entity.JustAttackedSE, SoundType.SE, volume: 0.5f);
                    if (justEffectLeft != null)
                        justEffectLeft.Play();
                    if (justEffectRight != null)
                        justEffectRight.Play();
                }

                // スコア更新部
                {
                    time += SO_Handler.Entity.TimeIncreaseAmount;
                    if (++GameDataHolder.CorrectAmount >= SO_Handler.Entity.QuestionAmount)
                    {
                        State = GameState.Over;
                        hasForciblyCleared = true;
                        // フラグOFF
                        {
                            if (everythingBlockingImage != null) everythingBlockingImage.enabled = false;
                            isDoingAttack = false;
                            canTimeDecrease = true;
                        }
                        return;
                    }
                    if (GameDataHolder.CorrectAmount <= 1) correctAmountTextShower.Appear(destroyCancellationToken).Forget();
                }

                await 1.0f.SecondsWait(ct);
            }

            // フラグOFF
            if (everythingBlockingImage != null) everythingBlockingImage.enabled = false;
            isDoingAttack = false;
            canTimeDecrease = true;

            // 新しく問題を作成
            if (State != GameState.OnGoing) return;
            CreateQuestion();
        }

        internal void PlaySelectSE(float pitch = 1.0f)
        {
            if (canPlaySelectSe) return;
            canPlaySelectSe = true;
            selectSeInterval.SecondsWaitAndDo(() => canPlaySelectSe = false, destroyCancellationToken).Forget();
            selectSEAudioSource.Raise(SO_Sound.Entity.SymbolSE, SoundType.SE, pitch: pitch);
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

            throw new Exception("インスタンスが見つかりませんでした");
        }

        internal int GetIndexFromSymbolPosition(Vector2 pos)
        {
            (_, int i, bool isFound) = SymbolPositions.Find(e => e == pos);

            if (isFound) return i;
            else throw new Exception("見つかりませんでした");
        }

        private async UniTask OnLoadFinished(Ct ct)
        {
            if (sceneTransitionShaderController != null)
                await sceneTransitionShaderController.Play(false, ct);
            await UniTask.WaitForSeconds(0.2f, cancellationToken: ct);
            await countDown.Play(ct);
            await UniTask.WaitForSeconds(0.2f, cancellationToken: ct);

            CreateQuestion();
            bgmPlayer.Play();
            State = GameState.OnGoing;
        }

        private async UniTask OnResult(Ct ct)
        {
            GameDataHolder?.SaveRanking();
            int rank = GameDataHolder?.GetRank() ?? 0;

            resultSEAudioSource.Raise(SO_Sound.Entity.ResultSE, SoundType.SE, volume: 0.5f);
            await resultShower.Play(GameDataHolder.CorrectAmount, rank, hasForciblyCleared, ct);
        }

        internal void CheckMouseHoverSymbolFrame(out bool hovering, out int index)
        {
            float lw = mouseHoverSymbolFrameLimitWidth;
            float lh = mouseHoverSymbolFrameLimitHeight;
            Vector2 mousePosition = Extension.MousePositionToWorldPosition(Camera.main, 0).ToVector2();

            for (int i = 0; i < SymbolPositions.Length; i++)
            {
                if (i < SymbolPositions.Length - 1)
                {
                    Vector3 leftPos = SymbolPositions[i];
                    Vector3 rightPos = SymbolPositions[i + 1];
                    bool isMouseHoverLeft = mousePosition.IsIn(-lw, lw, -lh, lh, leftPos);
                    bool isMouseHoverRight = mousePosition.IsIn(-lw, lw, -lh, lh, rightPos);

                    if (isMouseHoverLeft)
                    {
                        if (isMouseHoverRight)
                        {
                            hovering = true;
                            index = i + 1; // 右優先
                            return;
                        }
                        else
                        {
                            hovering = true;
                            index = i;
                            return;
                        }
                    }
                }
                else
                {
                    Vector3 leftPos = SymbolPositions[i];
                    bool isMouseHoverLeft = mousePosition.IsIn(-lw, lw, -lh, lh, leftPos);

                    if (isMouseHoverLeft)
                    {
                        hovering = true;
                        index = i;
                        return;
                    }
                }
            }

            hovering = false;
            index = -1;
        }
    }
}