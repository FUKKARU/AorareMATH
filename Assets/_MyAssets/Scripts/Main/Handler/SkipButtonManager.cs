using UnityEngine;
using Cysharp.Threading.Tasks;
using General.Button;
using General.Extension;
using Text = TMPro.TextMeshProUGUI;

namespace Main.Handler
{
    internal sealed class SkipButtonManager : ATextButtonManager
    {
        [SerializeField] private Text skipLeftAmountText;
        [SerializeField, Range(0.01f, 5.0f)] private float clickInterval;

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

        internal bool IsClickedThisFrame { get; private set; } = false;

        private void Start()
        {
            skipLeftAmount = SO.SO_Handler.Entity.SkipAmount;
        }

        private void LateUpdate()
        {
            IsClickedThisFrame = false;
        }

        protected sealed override bool CanEnter => CanFirePointerEvent();
        protected sealed override bool CanExit => CanFirePointerEvent();
        protected sealed override bool CanDown => CanFirePointerEvent();
        protected sealed override bool CanUp => CanFirePointerEvent();

        protected sealed override bool CanPlaySeOnEnter => CanFirePointerEvent() && GameManager.Instance.IsHoverSeAvailable;

        private bool CanFirePointerEvent()
        {
            if (GameManager.Instance.State != GameState.OnGoing) return false;
            if (GameManager.Instance.IsHoldingSymbol) return false;

            return true;
        }

        protected sealed override void OnClickSucceeded()
        {
            GameManager.Instance.PlaySelectSE();

            if (onInterval) return;

            IsClickedThisFrame = true;
            GameManager.Instance.HasFormulaChanged |= true;

            if ((--skipLeftAmount) <= 0)
            {
                IsClickedThisFrame = false; // フラグをリセット
                this.gameObject.SetActive(false); // このスクリプトが全てのルートに付いている想定
                return;
            }

            onInterval = true;
            clickInterval.SecAwaitThenDo(() => onInterval = false, ct: destroyCancellationToken).Forget();
        }
    }
}