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
        private SpriteRenderer thisInstance = null;

        [SerializeField, Header("インスタンスのz座標")] private float z;
        internal float Z => z;
        [SerializeField, Header("コピーインスタンスのz座標")] private float thisZ;

        private bool isFollowingMouse = false;

        private void OnEnable()
        {
            eventTrigger.AddListener(EventTriggerType.PointerDown, OnPointerDown);
            eventTrigger.AddListener(EventTriggerType.PointerUp, OnPointerUp);
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
                    Destroy(thisInstance.gameObject);
                    thisInstance = null;
                    return;
                }

                thisInstance.transform.position = Camera.main.MousePositionToWorldPosition(thisZ);
            }
        }

        private void OnPointerDown()
        {
            if (GameManager.Instance.State != GameState.OnGoing) return;

            if (thisInstance != null) return;

            isFollowingMouse = true;
            GameManager.Instance.PlaySelectSE();
            thisInstance = Instantiate(thisSpriteRenderer, Camera.main.MousePositionToWorldPosition(thisZ), Quaternion.identity, transform);
            thisInstance.transform.localScale = Vector3.one;
        }

        private void OnPointerUp()
        {
            if (GameManager.Instance.State != GameState.OnGoing) return;

            if (thisInstance == null) return;

            isFollowingMouse = false;

            thisInstance.transform.position.ToVector2().JudgeAttachable
                (p =>
                {
                    GameManager.Instance.PlaySelectSE();

                    Vector3 toPos = p.ToVector3(z);
                    int toIndex = GameManager.Instance.GetIndexFromSymbolPosition(toPos);
                    IntStr toSymbol = GameManager.Instance.Formula.Data[toIndex];

                    bool? isNumber = Symbol.IsNumber(toSymbol);
                    if (!isNumber.HasValue || isNumber.Value) return;

                    SpriteFollow instance = Instantiate(prefab, toPos, Quaternion.identity, transform.parent);
                    instance.transform.localScale =
                        Symbol.IsOperator(Type.GetSymbol()) == true ? new(0.4f, 0.4f, 1) : Vector3.one;

                    GameManager.Instance.Formula.Data[toIndex] = Type.GetSymbol();

                    if (toSymbol != Symbol.NONE) Destroy(GameManager.Instance.FormulaInstances[toIndex].gameObject);
                    GameManager.Instance.FormulaInstances[toIndex] = instance;
                },
                p =>
                {
                    Extension.Pass();
                });

            Destroy(thisInstance.gameObject);
            thisInstance = null;
        }

        internal void ForciblyInstantiateSpriteFollowHere(IntStr symbol, int index)
        {
            if (Symbol.IsNumber(symbol) != false) return;

            Vector3 toPos = GameManager.Instance.SymbolPositions[index].ToVector3(z);
            SpriteFollow instance = Instantiate(prefab, toPos, Quaternion.identity, transform.parent);
            // 演算子orかっこの前提
            instance.transform.localScale =
            Symbol.IsOperator(Type.GetSymbol()) == true ? new(0.4f, 0.4f, 1) : Vector3.one;
            GameManager.Instance.FormulaInstances[index] = instance;
        }
    }
}