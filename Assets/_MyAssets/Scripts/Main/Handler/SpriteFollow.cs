using General.Extension;
using Main.Data;
using Main.Data.Formula;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Main.Handler
{
    /// <summary>
    /// パネル上のスプライト
    /// </summary>
    internal sealed class SpriteFollow : MonoBehaviour
    {
        [SerializeField] private SymbolType _type;
        internal SymbolType Type => _type;

        [SerializeField] private EventTrigger eventTrigger;

        [SerializeField, Header("インスタンスのz座標")] private float _z;
        internal float Z => _z;
        [SerializeField, Header("持ち上げた際のインスタンスのz座標")] private float followZ;

        [SerializeField, Range(1.0f, 5.0f), Header("モバイル時、ホバー中に何倍に拡大するか")] private float hoverScaleWhenMobile;

        // キャッシュ用
        private new Camera camera = null;

        internal Vector3 InitPosition { get; set; }

        private bool _isFollowingMouse = false;
        private bool isFollowingMouse
        {
            get => _isFollowingMouse;
            set
            {
                _isFollowingMouse = value;
                GameManager.Instance.IsHoldingSymbol = value; // 掴んでいるものは1つだけのはずなので

                // モバイルでは、指で隠れてしまうので、拡大する
                // SpriteFollow 系統クラスでの共通処理
#if UNITY_IOS || UNITY_ANDROID
                float hoverScale = value ? hoverScaleWhenMobile : 1.0f / hoverScaleWhenMobile;
                transform.SetScaleXY(transform.localScale.x * hoverScale, transform.localScale.y * hoverScale);
#endif
            }
        }

        // Down/Upの所では、最初にDownされたポインターのみを追跡するようにする
        // DownされてからUpされたら、追跡状態はリセット(-1)される
        private int trackingPointerId = -1;

        private void Awake()
        {
            camera = Camera.main;
        }

        private void Start()
        {
            InitPosition = transform.position;

            eventTrigger.AddListener(EventTriggerType.PointerDown, OnPointerDown);
            eventTrigger.AddListener(EventTriggerType.PointerUp, OnPointerUp);
        }

        private void Update()
        {
            if (isFollowingMouse)
            {
                if (GameManager.Instance.State == GameState.Over)
                {
                    isFollowingMouse = false;
                    transform.position = InitPosition;
                    return;
                }

                if (camera != null)
                    transform.position = camera.PointerPositionToWorldPosition(followZ, trackingPointerId);
            }
        }

        // 範囲内でボタンを押す(タップ)した時
        private void OnPointerDown(PointerEventData data)
        {
            if (GameManager.Instance.State != GameState.OnGoing) return;

            // モバイルのみ
            // IDを追跡開始
            if (trackingPointerId != -1) return;
            trackingPointerId = data.pointerId;

            GameManager.Instance.PlaySelectSE();

            isFollowingMouse = true;
        }

        // PointerDown後にボタン(指)を放した時
        private void OnPointerUp(PointerEventData data)
        {
            if (GameManager.Instance.State != GameState.OnGoing) return;

            // モバイルのみ
            // IDを追跡終了
            int wasTrackingPointerId = trackingPointerId;  // この後使うため、コピーしておく
            if (trackingPointerId != data.pointerId) return;
            trackingPointerId = -1;

            isFollowingMouse = false;

            GameManager.Instance.CheckPointerHoverSymbolFrame(out bool hovering, out int index, wasTrackingPointerId);
            if (hovering)
            {
                Vector2 symbolPosition = GameManager.Instance.SymbolPositions[index];

                GameManager.Instance.PlaySelectSE();

                Vector3 fromPos = InitPosition;
                Vector3 toPos = symbolPosition.ToVector3(Z);
                int fromIndex = GameManager.Instance.GetIndexFromSymbolPosition(fromPos);
                int toIndex = GameManager.Instance.GetIndexFromSymbolPosition(toPos);

                if (GameManager.Instance.Formula.Data[toIndex] != Symbol.NONE)
                {
                    // 入れ替え

                    var otherInstance = GameManager.Instance.FormulaInstances[toIndex];

                    GameManager.Instance.Formula.Data[fromIndex] = otherInstance.Type.GetSymbol();
                    GameManager.Instance.Formula.Data[toIndex] = Type.GetSymbol();

                    GameManager.Instance.FormulaInstances[fromIndex] = otherInstance;
                    GameManager.Instance.FormulaInstances[toIndex] = this;

                    transform.position = toPos; InitPosition = toPos;
                    otherInstance.transform.position = fromPos; otherInstance.InitPosition = fromPos;

                    GameManager.Instance.HasFormulaChanged |= true;
                }
                else
                {
                    // はめ込める

                    GameManager.Instance.Formula.Data[fromIndex] = Symbol.NONE;
                    GameManager.Instance.Formula.Data[toIndex] = Type.GetSymbol();

                    GameManager.Instance.FormulaInstances[fromIndex] = null;
                    GameManager.Instance.FormulaInstances[toIndex] = this;

                    transform.position = toPos; InitPosition = toPos;

                    GameManager.Instance.HasFormulaChanged |= true;
                }
            }
            else
            {
                GameManager.Instance.PlaySelectSE(Pitch.DisposeSymbol);

                if (Symbol.IsNumber(Type.GetSymbol()) == true)
                {
                    // 元の位置に戻す
                    transform.position = InitPosition;
                }
                else
                {
                    // 消す
                    int i = GameManager.Instance.GetIndexFromSymbolPosition(InitPosition);
                    GameManager.Instance.Formula.Data[i] = Symbol.NONE;
                    GameManager.Instance.FormulaInstances[i] = null;
                    Destroy(gameObject);
                }
            }
        }
    }
}