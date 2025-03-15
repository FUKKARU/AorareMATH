using System;
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

        private bool clicked = false;
        internal bool Clicked => clicked;

        private bool isOnGoing => GameManager.Instance.State == GameState.OnGoing;

        private void OnEnable() => SetSprite(normalSprite);
        private void Update()
        {
            if (collider == null) return;
            if (isOnGoing && !collider.enabled) collider.enabled = true;
            else if (!isOnGoing && collider.enabled) collider.enabled = false;
        }
        private void LateUpdate() => clicked = false;

        public void OnPointerEnter(PointerEventData _) => DoThisAction(() => SetSprite(hoverSprite));
        public void OnPointerExit(PointerEventData _) => DoThisAction(() => SetSprite(normalSprite));
        public void OnPointerClick(PointerEventData _) => DoThisAction(() => clicked = true);

        private void DoThisAction(Action action)
        {
            if (GameManager.Instance.IsHoldingSymbol) return;
            action?.Invoke();
        }

        private void SetSprite(Sprite sprite)
        {
            if (spriteRenderer == null) return;
            if (sprite == null) return;
            spriteRenderer.sprite = sprite;
        }
    }
}