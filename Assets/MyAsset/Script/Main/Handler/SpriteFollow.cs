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

        internal static readonly float UnSelectSePitch = 1.5f;

        internal Vector3 InitPosition { get; set; }

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
            InitPosition = transform.position;

            eventTrigger.AddListener(EventTriggerType.PointerDown, OnPointerDown);
            eventTrigger.AddListener(EventTriggerType.PointerUp, OnPointerUp);
        }

        private void OnDisable()
        {
            eventTrigger = null;
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

                transform.position = Camera.main.MousePositionToWorldPosition(followZ);
            }
        }

        private void OnPointerDown()
        {
            if (GameManager.Instance.State != GameState.OnGoing) return;

            GameManager.Instance.PlaySelectSE();

            isFollowingMouse = true;
        }

        private void OnPointerUp()
        {
            if (GameManager.Instance.State != GameState.OnGoing) return;

            isFollowingMouse = false;

            GameManager.Instance.CheckMouseHoverSymbolFrame(out bool hovering, out int index);
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
                }
                else
                {
                    GameManager.Instance.Formula.Data[fromIndex] = Symbol.NONE;
                    GameManager.Instance.Formula.Data[toIndex] = Type.GetSymbol();

                    GameManager.Instance.FormulaInstances[fromIndex] = null;
                    GameManager.Instance.FormulaInstances[toIndex] = this;

                    transform.position = toPos; InitPosition = toPos;
                }
            }
            else
            {
                GameManager.Instance.PlaySelectSE(SpriteFollow.UnSelectSePitch);

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