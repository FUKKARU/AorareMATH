using UnityEngine;
using UnityEngine.EventSystems;
using General.Extension;
using Main.Data;
using Cysharp.Threading.Tasks;

namespace Main.Handler
{
    internal sealed class SkipButtonObserver : MonoBehaviour
    {
        [SerializeField] private new Transform transform;
        [SerializeField] private EventTrigger eventTrigger;
        [SerializeField, Range(1.0f, 2.0f)] private float hoverScale;
        [SerializeField, Range(0.01f, 5.0f)] private float clickInterval;

        private Vector3 initScale = Vector3.one;
        private bool onInterval = false;  // クリックのクールタイム中かどうか

        // 「問題をとばす」「次にすすむ」を交互に出すので、両方でこのフラグを共通で使用できる
        internal bool IsClickedThisFrame { get; private set; } = false;

        private void Start()
        {
            initScale = transform.localScale;

            if (eventTrigger != null)
            {
                eventTrigger.AddListener(EventTriggerType.PointerEnter, OnPointerEnter);
                eventTrigger.AddListener(EventTriggerType.PointerExit, OnPointerExit);
                eventTrigger.AddListener(EventTriggerType.PointerClick, OnPointerClick);
            }
        }

        private void LateUpdate()
        {
            IsClickedThisFrame = false;
        }

        private void OnPointerEnter()
        {
            if (GameManager.Instance.IsHoldingSymbol) return;

            GameManager.Instance.PlaySelectSE(Pitch.Hover);
            transform.localScale = initScale * hoverScale;
        }

        private void OnPointerExit()
        {
            if (GameManager.Instance.IsHoldingSymbol) return;

            transform.localScale = initScale;
        }

        private void OnPointerClick()
        {
            if (onInterval) return;
            if (GameManager.Instance.IsHoldingSymbol) return;

            GameManager.Instance.PlaySelectSE();
            IsClickedThisFrame = true;

            onInterval = true;
            clickInterval.SecondsWaitAndDo(() => onInterval = false, destroyCancellationToken).Forget();
        }
    }
}
