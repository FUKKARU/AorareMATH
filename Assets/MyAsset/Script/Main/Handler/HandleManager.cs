using DG.Tweening;
using General.Extension;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Main.Handler
{
    internal sealed class HandleManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private new Collider2D collider;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite hoverSprite;
        [SerializeField] private Transform submitText;

        private bool clicked = false;
        internal bool Clicked => clicked;

        private bool onClickInterval = false;
        private float clickIntervalTimer = 0.0f;

        private bool isPreviewSameThisFrame = false;
        private bool isPreviewSamePrevFrame = false;
        private Tween submitTextTween = null;

        private static readonly float clickInterval = 0.8f;
        private static readonly float selectSePitch = 1.2f;

        private void OnEnable() => SetSprite(normalSprite);

        private void OnDisable()
        {
            submitTextTween = null;
        }

        private void Update()
        {
            if (collider != null) collider.enabled = GameManager.Instance.State == GameState.OnGoing;

            if (onClickInterval)
            {
                clickIntervalTimer += Time.deltaTime;
                if (clickIntervalTimer >= clickInterval)
                {
                    onClickInterval = false;
                    clickIntervalTimer = 0.0f;
                }
            }

            isPreviewSameThisFrame = GameManager.Instance.IsPreviewNumberSameAsTargetThisFrame;
            if (isPreviewSameThisFrame && !isPreviewSamePrevFrame)
            {
                submitTextTween = submitText.DOLocalMoveY(2.9f, 0.3f)
                                            .SetLoops(-1, LoopType.Yoyo)
                                            .SetEase(Ease.InOutSine);
            }
            else if (!isPreviewSameThisFrame && isPreviewSamePrevFrame)
            {
                submitTextTween.Kill();
                submitTextTween = null;
                submitText.SetLocalPositionY(2.5f);
            }
            isPreviewSamePrevFrame = isPreviewSameThisFrame;
        }

        private void LateUpdate() => clicked = false;

        public void OnPointerEnter(PointerEventData _)
        {
            if (GameManager.Instance.IsHoldingSymbol) return;
            GameManager.Instance.PlaySelectSE(selectSePitch);
            SetSprite(hoverSprite);
            UpdateHandleSize(true);
        }

        public void OnPointerExit(PointerEventData _)
        {
            if (GameManager.Instance.IsHoldingSymbol) return;
            GameManager.Instance.PlaySelectSE(selectSePitch);
            SetSprite(normalSprite);
            UpdateHandleSize(false);
        }

        public void OnPointerClick(PointerEventData _)
        {
            if (onClickInterval) return;
            if (GameManager.Instance.IsHoldingSymbol) return;
            clicked = true;
            onClickInterval = true;
        }

        private void SetSprite(Sprite sprite)
        {
            if (spriteRenderer == null) return;
            if (sprite == null) return;
            spriteRenderer.sprite = sprite;
        }

        private void UpdateHandleSize(bool beingBig)
        {
            if (spriteRenderer == null) return;
            Vector3 localScale = beingBig ? new(1.1f, 1.1f, 1.0f) : new(1.0f, 1.0f, 1.0f);
            spriteRenderer.transform.localScale = localScale;
        }
    }
}