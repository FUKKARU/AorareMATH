using General.Extension;
using Main.Data;
using Main.Data.Formula;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Main.Handler
{
    internal sealed class NumberSpriteFollow : MonoBehaviour
    {
        [SerializeField] private SymbolType _type;
        internal SymbolType Type => _type;

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private EventTrigger eventTrigger;

        [SerializeField, Header("インスタンスのz座標")] private float _z;
        internal float Z => _z;

        internal Vector3 InitPosition { get; set; }

        private bool isFollowingMouse = false;

        private void OnEnable()
        {
            InitPosition = transform.position;

            spriteRenderer.sprite = _type.GetSprite();

            eventTrigger.AddListener(EventTriggerType.PointerDown, OnPointerDown);
            eventTrigger.AddListener(EventTriggerType.PointerUp, OnPointerUp);
        }

        private void OnDisable()
        {
            spriteRenderer = null;
            eventTrigger = null;
        }

        private void Update()
        {
            if (isFollowingMouse) transform.position = MouseToWorld(Z);
        }

        private void OnPointerDown()
        {
            isFollowingMouse = true;
        }

        private void OnPointerUp()
        {
            isFollowingMouse = false;
            (Vector2 p, _, bool isFound) = GameManager.Instance.SymbolPositions.Find
                (e => transform.position.ToVector2().IsIn(-0.37f, 0.37f, -0.87f, 0.87f, e));
            if (isFound)
            {
                Vector3 fromPos = InitPosition;
                Vector3 toPos = p.ToVector3(Z);
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
                transform.position = InitPosition;
            }
        }

        private Vector3 MouseToWorld(float z)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = z;
            return pos;
        }
    }
}