using UnityEngine;

namespace Main.Handler
{
    internal sealed class InputFrameOutliner : MonoBehaviour
    {
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite hoverSprite;

        [SerializeField, Header("E_1 - E_12 の順番")] private SpriteRenderer[] symbolFrames;

        private void Update()
        {
            UpdateInputFrameOutline();
        }

        private void UpdateInputFrameOutline()
        {
            int i = -1;
            if (GameManager.Instance.State == GameState.OnGoing)
            {
                GameManager.Instance.CheckMouseHoverSymbolFrame(out bool hovering, out int index);
                if (hovering) i = index;
            }

            for (int j = 0; j < symbolFrames.Length; j++)
            {
                SpriteRenderer symbolFrame = symbolFrames[j];
                if (symbolFrame == null) continue;
                symbolFrame.sprite = j == i ? hoverSprite : normalSprite;
            }
        }
    }
}