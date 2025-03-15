using General.Extension;
using Main.Data;
using Main.Data.Formula;
using UnityEngine;
using UnityEngine.EventSystems;

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

        private void OnEnable()
        {
            eventTrigger.AddListener(EventTriggerType.PointerEnter, OnPointerEnter);
            eventTrigger.AddListener(EventTriggerType.PointerExit, OnPointerExit);
            eventTrigger.AddListener(EventTriggerType.PointerDown, OnPointerDown);
            eventTrigger.AddListener(EventTriggerType.PointerUp, OnPointerUp);
            if (thisSpriteRenderer != null) thisSpriteRenderer.sprite = normalSprite;
        }

        private void OnDisable()
        {
            eventTrigger = null;
            prefab = null;
            thisSpriteRenderer = null;
            thisInstance = null;
        }

        private void Update()
        {
            if (isFollowingMouse)
            {
                if (thisInstance == null) return;

                if (GameManager.Instance.State == GameState.Over)
                {
                    isFollowingMouse = false;
                    if (thisSpriteRenderer != null) thisSpriteRenderer.sprite = normalSprite;
                    Destroy(thisInstance.gameObject);
                    thisInstance = null;
                    return;
                }

                thisInstance.transform.position = Camera.main.MousePositionToWorldPosition(thisZ);
            }
        }

        private void OnPointerEnter()
        {
            if (GameManager.Instance.State != GameState.OnGoing) return;
            if (isFollowingMouse) return;
            if (GameManager.Instance.IsHoldingSymbol) return;

            if (thisSpriteRenderer != null) thisSpriteRenderer.sprite = hoverSprite;
        }

        private void OnPointerExit()
        {
            if (GameManager.Instance.State != GameState.OnGoing) return;
            if (isFollowingMouse) return;
            if (GameManager.Instance.IsHoldingSymbol) return;

            if (thisSpriteRenderer != null) thisSpriteRenderer.sprite = normalSprite;
        }

        private void OnPointerDown()
        {
            if (GameManager.Instance.State != GameState.OnGoing) return;

            if (thisInstance != null) return;

            isFollowingMouse = true;
            GameManager.Instance.PlaySelectSE();
            if (thisSpriteRenderer != null) thisSpriteRenderer.sprite = normalSprite;
            thisInstance = Instantiate(thisSpriteRenderer, Camera.main.MousePositionToWorldPosition(thisZ), Quaternion.identity, transform);
            thisInstance.transform.localScale = Vector3.one;
        }

        private void OnPointerUp()
        {
            if (GameManager.Instance.State != GameState.OnGoing) return;

            if (thisInstance == null) return;

            isFollowingMouse = false;

            GameManager.Instance.CheckMouseHoverSymbolFrame(out bool hovering, out int index);
            if (hovering)
            {
                Vector2 symbolPosition = GameManager.Instance.SymbolPositions[index];

                Vector3 toPos = symbolPosition.ToVector3(z);
                int toIndex = GameManager.Instance.GetIndexFromSymbolPosition(toPos);
                IntStr toSymbol = GameManager.Instance.Formula.Data[toIndex];

                if (Symbol.IsNumber(toSymbol) == false)
                {
                    GameManager.Instance.PlaySelectSE();

                    SpriteFollow instance = Instantiate(prefab, toPos, Quaternion.identity, transform.parent);
                    instance.transform.localScale =
                        Symbol.IsOperator(Type.GetSymbol()) == true ? new(0.4f, 0.4f, 1) : Vector3.one;

                    GameManager.Instance.Formula.Data[toIndex] = Type.GetSymbol();

                    if (toSymbol != Symbol.NONE) Destroy(GameManager.Instance.FormulaInstances[toIndex].gameObject);
                    GameManager.Instance.FormulaInstances[toIndex] = instance;
                }
                else
                {
                    GameManager.Instance.PlaySelectSE(SpriteFollow.UnSelectSePitch);
                }
            }
            else
            {
                GameManager.Instance.PlaySelectSE(SpriteFollow.UnSelectSePitch);
            }

            Destroy(thisInstance.gameObject);
            thisInstance = null;
        }
    }
}