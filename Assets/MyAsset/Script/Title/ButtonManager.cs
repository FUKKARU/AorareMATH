using UnityEngine;

namespace Title.Button
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

            // スプライトをホバーにする
        }

        // マウスのポインターが離れた
        public void OnExit()
        {
            if (!isActive) return;
            if (!sr) return;
            if (!normalSprite) return;

            // スプライトをノーマルにする
        }

        // マウスのポインターがオブジェクトの上でクリックされた
        public void OnClick()
        {
            if (!isActive) return;
            if (!sr) return;
            if (!clickSprite) return;

            // スプライトをクリックにする

            isActive = false;

            // クリック時の処理
        }
    }
}
