using UnityEngine;
using UnityEngine.EventSystems;
using General.Extension;
using Main.Data.Formula;

namespace Main.Handler
{
    internal sealed class HandleManager : MonoBehaviour
    {
        [SerializeField] private EventTrigger eventTrigger;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite hoverSprite;

        private HandleManagerBhv impl;

        private void OnEnable()
        {
            impl = new(eventTrigger, spriteRenderer, normalSprite, hoverSprite);
        }

        private void OnDisable()
        {
            impl.Dispose();
            impl = null;
        }

        private void Update()
        {
            impl.Flip();
        }
    }

    internal sealed class HandleManagerBhv : System.IDisposable
    {
        private EventTrigger eventTrigger;
        private SpriteRenderer spriteRenderer;
        private Sprite normalSprite;
        private Sprite hoverSprite;

        private bool isFirstFlip = true;

        internal HandleManagerBhv
            (EventTrigger eventTrigger, SpriteRenderer spriteRenderer, Sprite normalSprite, Sprite hoverSprite)
        {
            this.eventTrigger = eventTrigger;
            this.spriteRenderer = spriteRenderer;
            this.normalSprite = normalSprite;
            this.hoverSprite = hoverSprite;
        }

        public void Dispose()
        {
            eventTrigger = null;
            spriteRenderer = null;
            normalSprite = null;
            hoverSprite = null;
        }

        internal bool IsNullExist()
        {
            if (eventTrigger == null) return true;
            if (spriteRenderer == null) return true;
            if (normalSprite == null) return true;
            if (hoverSprite == null) return true;
            return false;
        }

        internal void Flip()
        {
            if (IsNullExist()) return;

            if (isFirstFlip)
            {
                isFirstFlip = false;
                Start();
            }

            Update();
        }

        private void Start()
        {
            eventTrigger.AddListener(EventTriggerType.PointerEnter, OnPointerEnter);
            eventTrigger.AddListener(EventTriggerType.PointerExit, OnPointerExit);
            eventTrigger.AddListener(EventTriggerType.PointerClick, OnPointerClick);
        }

        private void Update()
        {
            if (GameManager.Instance.State == GameState.Over)
                spriteRenderer.sprite = normalSprite;
        }

        private void OnPointerEnter()
        {
            if (spriteRenderer == null) return;
            if (hoverSprite == null) return;
            if (GameManager.Instance.State != GameState.OnGoing) return;

            spriteRenderer.sprite = hoverSprite;
        }

        private void OnPointerExit()
        {
            if (spriteRenderer == null) return;
            if (normalSprite == null) return;
            if (GameManager.Instance.State != GameState.OnGoing) return;

            spriteRenderer.sprite = normalSprite;
        }

        private void OnPointerClick()
        {
            if (GameManager.Instance.State != GameState.OnGoing) return;
            if (!GameManager.Instance.IsAttackable) return;

            GameManager.Instance.Attack(new(Symbol.N5, Symbol.OA, Symbol.N5));
        }
    }
}