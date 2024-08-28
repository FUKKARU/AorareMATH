using UnityEngine;

namespace Title.Handler
{
    internal sealed class ButtonManager : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite hoverSprite;
        [SerializeField] private Sprite clickSprite;

        private bool isActive = true;

        private void OnEnable()
        {
            if (!sr) return;
            if (!normalSprite) return;

            sr.sprite = normalSprite;
        }

        private void OnDisable()
        {
            sr = null;
            normalSprite = null;
            hoverSprite = null;
            clickSprite = null;
        }

        // マウスのポインターが乗っかった
        public void OnEnter()
        {
            if (!isActive) return;
            if (!sr) return;
            if (!hoverSprite) return;

            sr.sprite = hoverSprite;
        }

        // マウスのポインターが離れた
        public void OnExit()
        {
            if (!isActive) return;
            if (!sr) return;
            if (!normalSprite) return;

            sr.sprite = normalSprite;
        }

        // マウスのポインターがオブジェクトの上でクリックされた
        public void OnClick()
        {
            if (!isActive) return;
            if (!sr) return;
            if (!clickSprite) return;

            sr.sprite = clickSprite;

            isActive = false;

            Debug.LogWarning("今シーン遷移！");
        }
    }
}
