using System.Linq;
using General;
using General.Shaders;
using Main.Data;
using Main.Data.Formula;
using SO;

namespace Main.Handler
{
    internal enum GameState : byte
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
        [SerializeField] private UnNumberSpritesAnimator[] unNumberSpritesAnimators;
        [SerializeField] private TimeShower timeShower;
        [SerializeField] private CorrectAmountTextShower correctAmountTextShower;
        [SerializeField] private SkipButtonManager skipButtonManager;
        [SerializeField] private UntilResultCountDown untilResultCountDown;
        [SerializeField] private ParticleSystem justEffectLeft;
        [SerializeField] private ParticleSystem justEffectRight;
        [SerializeField] private ResultShower resultShower;

        [SerializeField, Range(0.01f, 5.0f)] private float mouseHoverSymbolFrameLimitWidth;
        [SerializeField, Range(0.01f, 5.0f)] private float mouseHoverSymbolFrameLimitHeight;

        private Vector2[] _symbolPositions;
        internal Vector2[] SymbolPositions => _symbolPositions;

        private SpriteFollow[] _formulaInstances = new SpriteFollow[12];
        internal SpriteFollow[] FormulaInstances => _formulaInstances;

        internal GameState State { get; private set; } = GameState.Stay; // ゲームの状態
        internal RankDataHolder rankDataHolder { get; private set; } = null; // セーブデータに対して、ランキングの読み書きを行うラッパー
        internal int CorrectAmount => rankDataHolder.CorrectAmount;
        internal Formula Formula { get; private set; } = new(); // 出題中の問題
        private int target = 0; // 出題中の問題のターゲット数
        private string answer = string.Empty; // 出題中の問題の答え

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

        // 操作されて、式の状態が変化したかどうかを監視する (Followスクリプト、スキップボタンから書き換えて合図を送る)
        // true なら、PreviewTextを更新する必要がある
        internal bool HasFormulaChanged { get; set; } = false;

        // IsHoldingSymbolがfalseになってから、ホバー音が再生不可になっている時間
        private static readonly float hoverSeInterval = 0.1f;
        // IsHoldingSymbolがfalseになってから少しの間だけ、ホバー音を再生不可にするためのフラグ
        internal bool IsHoverSeAvailable { get; private set; } = true;

        private bool isFirstOnStay = true;
        private bool isFirstOnOver = true;
        private bool canTimeDecrease = true; // Attackの演出時、時間が減らないようにする
        private bool isDoingAttack = false;
        private bool isPreviewTextOverriding = false; // PreviewTextが上書きされているかどうか (問題の答えを見せるときなどに使う)
        private bool hasForciblyCleared = false;

        private void Start()
        {
            State = GameState.Stay;

            rankDataHolder = RankDataHolder.Create();

            _symbolPositions = symbolFrames.Select(e => e.position.ToVector2()).ToArray();

            SetTargetText(string.Empty);
            SetPreviewText(text: "= ???", color: Color.red);  // 操作されていない最初の時は、プレビューを見せないようにする (混乱させないため)

            time = SO_Handler.Entity.InitTimeLimt;

            // ずっと実行させとくので十分だと思う
            UpdateHoverSeCooltime(destroyCancellationToken).Forget();

            // デリゲート
            if (skipButtonManager != null)
                skipButtonManager.OnClicked += Skip;
        }

        private void Update() => (State switch
        {
            GameState.Stay => OnStay,
            GameState.OnGoing => OnOnGoing,
            GameState.Over => OnOver,
            _ => null as Action
        })?.Invoke();

        private void LateUpdate()
        {
            IsPreviewNumberSameAsTargetThisFrame = false;
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

        private void CreateQuestion(bool isFirstCall = false)
        {
            bool result = rankDataHolder.CorrectAmount.ToQuestionType().GetNewQuestion(out int[] numbers, out int target, out string answer);
            if (!result) return;
            this.target = target;
            this.answer = answer;

            answer.Log();

            // インスタンスを作り直す
            DestroyInstances();
            CreateInstances();

            return;



            void DestroyInstances()
            {
                Formula?.ClearData();

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
                    Element element = new(n);
                    Formula.SetData(i, element);

                    Vector2 pos = SymbolPositions[i];
                    var prefabInstance = ToInstance(element);
                    var instance = Instantiate(prefabInstance, pos.ToVector3(prefabInstance.Z), Quaternion.identity, transform);
                    _formulaInstances[i] = instance;

                    // 有効化する
                    if (instance.TryGetComponent(out SpriteAnimator animator))
                        animator.Enable(isFirstQuestion: isFirstCall);
                }
            }
        }

        private void ShowPreview()
        {
            if (isPreviewTextOverriding) return;
            if (!HasFormulaChanged) return;

            IsPreviewNumberSameAsTargetThisFrame = false;

            double r = Formula.Calcurate();
            if (!double.IsNaN(r))
            {
                SetPreviewText(text: $"= {(int)r}");

                double diff = Math.Abs(target - r);
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

        // PreviewText の上書きフラグがONの間に、呼ばれる想定
        private void SetAnswerToPreviewText(string answer)
          => SetPreviewText(text: $"<size=90>答え:</size> {answer}", color: Color.black);

        // 式を計算し、ピッタリならアタックする
        private void CheckFormula()
        {
            double r = Formula.Calcurate();
            if (double.IsNaN(r)) return;

            if (Math.Abs(target - r) <= SO_Handler.DiffLimit)
                Attack(destroyCancellationToken).Forget();
        }

        // 問題数は進まない仕様
        private bool Skip()
        {
            if (State != GameState.OnGoing) return false;
            if (isDoingAttack) return false;

            // 新しく問題を作成
            CreateQuestion();

            return true;
        }

        // everythingBlockingImage : サウンドスライダーはクリック不可 (演出時間が短いので、許容する)
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
                    AudioSourceManager.Instance.Play(SO_Sound.Entity.AttackSE, SoundType.SE, volume: 0.5f);
                    AudioSourceManager.Instance.Play(SO_Sound.Entity.JustAttackedSE, SoundType.SE, volume: 0.5f);
                    if (justEffectLeft != null)
                        justEffectLeft.Play();
                    if (justEffectRight != null)
                        justEffectRight.Play();
                }

                // スコア更新部
                {
                    time += SO_Handler.Entity.TimeIncreaseAmount;
                    if (++rankDataHolder.CorrectAmount >= SO_Handler.Entity.QuestionAmount)
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
                    if (rankDataHolder.CorrectAmount <= 1) correctAmountTextShower.Appear(destroyCancellationToken).Forget();
                }

                await 1.0f.SecAwait(ct: ct);
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
            => AudioSourceManager.Instance.Play(SO_Sound.Entity.SymbolSE, SoundType.SE, pitch: pitch);

        private async UniTaskVoid UpdateHoverSeCooltime(Ct ct)
        {
            while (true)
            {
                await UniTask.WaitUntil(() => IsHoldingSymbol == true, cancellationToken: ct);
                await UniTask.WaitUntil(() => IsHoldingSymbol == false, cancellationToken: ct);
                IsHoverSeAvailable = false;
                await UniTask.WhenAny(
                    UniTask.WaitForSeconds(hoverSeInterval, cancellationToken: ct),
                    UniTask.WaitUntil(() => IsHoldingSymbol == true, cancellationToken: ct)
                );
                IsHoverSeAvailable = true;
            }
        }

        private SpriteFollow ToInstance(Element element)
        {
            foreach (var e in symbolSprites)
            {
                if (e.Type.GetElement() == element)
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

            CreateQuestion(isFirstCall: true);
            if (unNumberSpritesAnimators != null)
            {
                foreach (var animator in unNumberSpritesAnimators)
                {
                    if (animator == null) continue;
                    animator.BeginAnimation(ct).Forget();
                }
            }
            bgmPlayer.Play();
            State = GameState.OnGoing;
        }

        private async UniTask OnResult(Ct ct)
        {
            int rank = rankDataHolder?.GetRank() ?? 0;

            // 最後の問題の答えを見せる
            if (!hasForciblyCleared)
            {
                SetAnswerToPreviewText(answer);
                if (untilResultCountDown != null)
                    await untilResultCountDown.BeginCountDown(ct);
            }

            AudioSourceManager.Instance.Play(SO_Sound.Entity.ResultSE, SoundType.SE, volume: 0.5f);
            await resultShower.Play(rankDataHolder.CorrectAmount, rank, hasForciblyCleared, ct);
        }

#if UNITY_IOS || UNITY_ANDROID
        // アプリがバックグラウンドに行って戻った時、不正を防止するために、その分残り時間を減らす
        private DateTime bgPauseTime;  // バックグラウンドに行った瞬間の時間を保存

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                bgPauseTime = DateTime.Now;
            }
            else
            {
                TimeSpan delta = DateTime.Now - bgPauseTime;

                if (State == GameState.OnGoing && canTimeDecrease)
                {
                    time -= (float)delta.TotalSeconds;
                    if (time <= 0)
                    {
                        State = GameState.Over;
                    }
                }
            }
        }
#endif

        // PCの場合、pointerIdは-1のままでOK(無視される). モバイルの場合、対象にしたい指のIDを指定する.
        internal void CheckPointerHoverSymbolFrame(out bool hovering, out int index, int pointerId = -1)
        {
            float lw = mouseHoverSymbolFrameLimitWidth;
            float lh = mouseHoverSymbolFrameLimitHeight;
            Vector2 mousePosition = Extension.PointerPositionToWorldPosition(Camera.main, 0, pointerId).ToVector2();

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