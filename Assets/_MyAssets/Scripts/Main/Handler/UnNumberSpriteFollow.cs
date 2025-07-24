using UnityEngine.EventSystems;
using General;
using Main.Data;
using Main.Data.Formula;

namespace Main.Handler
{
    /// <summary>
    /// 車内にあるスプライト（演算子orかっこの前提）
    /// </summary>
    internal sealed class UnNumberSpriteFollow : MonoBehaviour
    {
        [SerializeField] private SymbolType _type;
        internal SymbolType Type => _type;

        [SerializeField] private EventTrigger eventTrigger;
        [SerializeField] private SpriteFollow prefab;
        [SerializeField] private SpriteRenderer thisSpriteRenderer;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite hoverSprite;
        private SpriteRenderer thisInstance = null;

        [SerializeField, Header("インスタンスのz座標")] private float z;
        internal float Z => z;
        [SerializeField, Header("コピーインスタンスのz座標")] private float thisZ;

        [SerializeField, Range(1.0f, 5.0f), Header("モバイル時、ホバー中に何倍に拡大するか")] private float hoverScaleWhenMobile;

        // キャッシュ用
        private new Camera camera = null;

        private bool _isFollowingMouse = false;
        private bool isFollowingMouse
        {
            get => _isFollowingMouse;
            set
            {
                _isFollowingMouse = value;
                GameManager.Instance.IsHoldingSymbol = value; // 掴んでいるものは1つだけのはずなので
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
            eventTrigger.AddListener(EventTriggerType.PointerEnter, OnPointerEnter);
            eventTrigger.AddListener(EventTriggerType.PointerExit, OnPointerExit);
            eventTrigger.AddListener(EventTriggerType.PointerDown, OnPointerDown);
            eventTrigger.AddListener(EventTriggerType.PointerUp, OnPointerUp);
            if (thisSpriteRenderer != null) thisSpriteRenderer.sprite = normalSprite;
        }

        private void Update()
        {
            if (isFollowingMouse)
            {
                if (thisInstance == null) return;

                //TODO: なぜか、Instantiate()の直後はホバースプライトにしても反映されないので、ここで毎フレーム書き換えている.
                thisInstance.sprite = hoverSprite;

                if (GameManager.Instance.State == GameState.Over)
                {
                    isFollowingMouse = false;
                    if (thisSpriteRenderer != null) thisSpriteRenderer.sprite = normalSprite;
                    Destroy(thisInstance.gameObject);
                    thisInstance = null;
                    return;
                }

                thisInstance.transform.position = camera.PointerPositionToWorldPosition(thisZ, trackingPointerId);
            }
        }

        private void OnPointerEnter(PointerEventData data)
        {
            if (GameManager.Instance.State != GameState.OnGoing) return;
            if (isFollowingMouse) return;
            if (GameManager.Instance.IsHoldingSymbol) return;

            // モバイルのみ
            // 他の指からのEnterは無視
            if (trackingPointerId != -1 && trackingPointerId != data.pointerId)
                return;

            if (GameManager.Instance.IsHoverSeAvailable)
                GameManager.Instance.PlaySelectSE(Pitch.Hover);

            if (thisSpriteRenderer != null) thisSpriteRenderer.sprite = hoverSprite;
        }

        private void OnPointerExit(PointerEventData data)
        {
            if (GameManager.Instance.State != GameState.OnGoing) return;
            if (isFollowingMouse) return;
            if (GameManager.Instance.IsHoldingSymbol) return;

            // モバイルのみ
            // 他の指からのExitは無視
            if (trackingPointerId != -1 && trackingPointerId != data.pointerId)
                return;

            if (thisSpriteRenderer != null) thisSpriteRenderer.sprite = normalSprite;
        }

        private void OnPointerDown(PointerEventData data)
        {
            if (GameManager.Instance.State != GameState.OnGoing) return;

            // モバイルのみ
            // IDを追跡開始
            if (trackingPointerId != -1) return;
            trackingPointerId = data.pointerId;

            if (thisInstance != null) return;

            isFollowingMouse = true;
            GameManager.Instance.PlaySelectSE();
            if (thisSpriteRenderer != null) thisSpriteRenderer.sprite = normalSprite;
            thisInstance = Instantiate(thisSpriteRenderer, camera.PointerPositionToWorldPosition(thisZ, trackingPointerId), Quaternion.identity, transform);

            // モバイルでは、指で隠れてしまうので、拡大する
            // SpriteFollow 系統クラスでの共通処理
#if UNITY_IOS || UNITY_ANDROID
            thisInstance.transform.localScale = new(hoverScaleWhenMobile, hoverScaleWhenMobile, 1.0f);
#else
            thisInstance.transform.localScale = Vector3.one;
#endif
        }

        private void OnPointerUp(PointerEventData data)
        {
            if (GameManager.Instance.State != GameState.OnGoing) return;

            // モバイルのみ
            // IDを追跡終了
            int wasTrackingPointerId = trackingPointerId;  // この後使うため、コピーしておく
            if (trackingPointerId != data.pointerId) return;
            trackingPointerId = -1;

            if (thisInstance == null) return;

            isFollowingMouse = false;

            GameManager.Instance.CheckPointerHoverSymbolFrame(out bool hovering, out int index, wasTrackingPointerId);
            if (hovering)
            {
                Vector2 symbolPosition = GameManager.Instance.SymbolPositions[index];

                Vector3 toPos = symbolPosition.ToVector3(z);
                int toIndex = GameManager.Instance.GetIndexFromSymbolPosition(toPos);
                IntStr toSymbol = GameManager.Instance.Formula.Data[toIndex];

                if (Symbol.IsNumber(toSymbol) == false)
                {
                    // はめ込める

                    GameManager.Instance.PlaySelectSE();

                    SpriteFollow instance = Instantiate(prefab, toPos, Quaternion.identity, transform.parent);
                    GameManager.Instance.Formula.Data[toIndex] = Type.GetSymbol();
                    if (toSymbol != Symbol.NONE) Destroy(GameManager.Instance.FormulaInstances[toIndex].gameObject);
                    GameManager.Instance.FormulaInstances[toIndex] = instance;

                    GameManager.Instance.HasFormulaChanged |= true;
                }
                else
                {
                    GameManager.Instance.PlaySelectSE(Pitch.DisposeSymbol);
                }
            }
            else
            {
                GameManager.Instance.PlaySelectSE(Pitch.DisposeSymbol);
            }

            Destroy(thisInstance.gameObject);
            thisInstance = null;
        }
    }
}