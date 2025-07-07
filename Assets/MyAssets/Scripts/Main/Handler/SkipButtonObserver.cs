using UnityEngine;
using General.Extension;
using Cysharp.Threading.Tasks;
using Text = TMPro.TextMeshProUGUI;
using General.Button;

namespace Main.Handler
{

    internal sealed class SkipButtonObserver : ASimpleButtonManager
    {
        [SerializeField] private Sprite skipSprite;
        [SerializeField] private Sprite continueSprite;
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

        // 「問題をとばす」「次にすすむ」を交互に出すので、両方でこのフラグを共通で使用できる
        internal bool IsClickedThisFrame { get; private set; } = false;
        // trueなら「問題をとばす」状態、falseなら「次にすすむ」状態
        private bool canDecreaseSkipAmount = true;

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

        protected sealed override bool CanPlaySeOnEnter => GameManager.Instance.IsHoverSeAvailable;

        private bool CanFirePointerEvent()
        {
            if (GameManager.Instance.State != GameState.OnGoing) return false;
            if (GameManager.Instance.IsHoldingSymbol) return false;
            if (onInterval) return false;

            return true;
        }

        protected sealed override void OnClickSucceeded()
        {
            GameManager.Instance.PlaySelectSE();
            IsClickedThisFrame = true;

            if (canDecreaseSkipAmount)
                --skipLeftAmount;
            else if (skipLeftAmount <= 0)
            {
                if (Image != null) Image.gameObject.SetActive(false);
                if (skipLeftAmountText != null) skipLeftAmountText.gameObject.SetActive(false);
                return;
            }
            canDecreaseSkipAmount = !canDecreaseSkipAmount;
            if (Image != null)
                Image.sprite = canDecreaseSkipAmount ? skipSprite : continueSprite;

            onInterval = true;
            clickInterval.SecondsWaitAndDo(() => onInterval = false, destroyCancellationToken).Forget();
        }
    }
}
