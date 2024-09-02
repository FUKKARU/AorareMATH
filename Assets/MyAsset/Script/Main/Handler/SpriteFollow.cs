using General.Extension;
using Main.Data;
using Main.Data.Formula;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Main.Handler
{
    internal sealed class SpriteFollow : MonoBehaviour
    {
        [SerializeField] private SymbolType _type;
        internal SymbolType Type => _type;

        [SerializeField] private EventTrigger eventTrigger;

        [SerializeField, Header("インスタンスのz座標")] private float _z;
        internal float Z => _z;

        internal Vector3 InitPosition { get; set; }

        private bool isFollowingMouse = false;

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
            if (isFollowingMouse) transform.position = MouseToWorld(Z);
        }

        private void OnPointerDown()
        {
            isFollowingMouse = true;
        }

        private void OnPointerUp()
        {
            isFollowingMouse = false;

            transform.position.ToVector2().JudgeAttachable
                (p =>
                {
                    GameManager.Instance.PlaySelectSE();

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
                },
                p =>
                {
                    if (Symbol.IsNumber(Type.GetSymbol()) == true)
                    {
                        // 元の位置に戻す
                        transform.position = InitPosition;
                    }
                    else
                    {
                        // 消す
                        int index = GameManager.Instance.GetIndexFromSymbolPosition(InitPosition);
                        GameManager.Instance.Formula.Data[index] = Symbol.NONE;
                        GameManager.Instance.FormulaInstances[index] = null;
                        Destroy(gameObject);
                    }
                });
        }

        private Vector3 MouseToWorld(float z)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = z;
            return pos;
        }
    }

    internal static class SpriteFollowEx
    {
        internal static void JudgeAttachable(this Vector2 position,
            System.Action<Vector2> actionIfAttachable, System.Action<Vector2> actionIfNotAttachable)
        {
            (Vector2 p, _, bool isFound) = GameManager.Instance.SymbolPositions.Find
                (e => position.IsIn(-0.37f, 0.37f, -0.87f, 0.87f, e));

            if (isFound) actionIfAttachable(p);
            else actionIfNotAttachable(p);
        }
    }
}