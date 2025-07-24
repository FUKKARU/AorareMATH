using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using General;
using General.Button;
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

        // 内部ロジックを実行できたか、戻り値で返す
        internal Func<bool> OnClicked { get; set; } = null;

        private void Start()
        {
            skipLeftAmount = SO.SO_Handler.Entity.SkipAmount;
        }

        private void OnDestroy()
        {
            OnClicked = null;
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

            if (OnClicked?.Invoke() != true) return; // クリックした時の処理を実行できなかったら、残り回数を消費しない
            GameManager.Instance.HasFormulaChanged |= true;

            if ((--skipLeftAmount) <= 0)
            {
                this.gameObject.SetActive(false); // このスクリプトが全てのルートに付いている想定
                return;
            }

            onInterval = true;
            clickInterval.SecAwaitThenDo(() => onInterval = false, ct: destroyCancellationToken).Forget();
        }
    }
}