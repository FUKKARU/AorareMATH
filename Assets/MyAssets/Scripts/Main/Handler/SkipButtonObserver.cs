using UnityEngine;
using UnityEngine.EventSystems;
using General.Extension;
using Main.Data;
using Cysharp.Threading.Tasks;
using Text = TMPro.TextMeshProUGUI;

namespace Main.Handler
{
    internal sealed class SkipButtonObserver : MonoBehaviour
    {
        [SerializeField] private new Transform transform;
        [SerializeField] private EventTrigger eventTrigger;
        [SerializeField] private Text skipLeftAmountText;
        [SerializeField, Range(1.0f, 2.0f)] private float hoverScale;
        [SerializeField, Range(0.01f, 5.0f)] private float clickInterval;

        private Vector3 initScale = Vector3.one;
        private bool onInterval = false;  // クリックのクールタイム中かどうか

        private int _skipLeftAmount = 0;
        private int skipLeftAmount
        {
            get => _skipLeftAmount;
            set
            {
                _skipLeftAmount = Mathf.Clamp(value, 0, 9);
                if (skipLeftAmountText != null)
                    skipLeftAmountText.text = $"あと<size=90><color=#dd2222> {_skipLeftAmount} </color></size>回";
            }
        }

        // 「問題をとばす」「次にすすむ」を交互に出すので、両方でこのフラグを共通で使用できる
        internal bool IsClickedThisFrame { get; private set; } = false;
        // trueなら「問題をとばす」状態、falseなら「次にすすむ」状態
        private bool canDecreaseSkipAmount = true;

        private void Start()
        {
            skipLeftAmount = SO.SO_Handler.Entity.SkipAmount;
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

            if (canDecreaseSkipAmount)
                --skipLeftAmount;
            else if (skipLeftAmount <= 0)
            {
                if (transform != null) transform.gameObject.SetActive(false);
                if (skipLeftAmountText != null) skipLeftAmountText.gameObject.SetActive(false);
                return;
            }
            canDecreaseSkipAmount = !canDecreaseSkipAmount;

            onInterval = true;
            clickInterval.SecondsWaitAndDo(() => onInterval = false, destroyCancellationToken).Forget();
        }
    }
}
