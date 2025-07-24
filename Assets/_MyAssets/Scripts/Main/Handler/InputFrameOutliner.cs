namespace Main.Handler
{
    internal sealed class InputFrameOutliner : MonoBehaviour
    {
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite hoverSprite;

        [SerializeField, Header("E_1 - E_12 の順番")] private SpriteRenderer[] symbolFrames;

        // 各個のシンボルフレームで、アウトラインがアクティブになっているかどうか
        private bool[] isActives = null;

        private void Start()
        {
            isActives = new bool[symbolFrames.Length];
            ResetFlags();
        }

        private void Update()
        {
            UpdateInputFrameOutline();
        }

        private void UpdateInputFrameOutline()
        {
            if (GameManager.Instance.State == GameState.OnGoing)
            {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
                GameManager.Instance.CheckPointerHoverSymbolFrame(out bool hovering, out int index);
                if (hovering) isActives[index] |= true;
#elif UNITY_ANDROID || UNITY_IOS
                for (int i = 0; i < Input.touchCount; ++i)
                {
                    GameManager.Instance.CheckPointerHoverSymbolFrame(out bool hovering, out int index, i);
                    if (hovering) isActives[index] |= true;
                }
#else
#endif
            }

            for (int i = 0; i < symbolFrames.Length; ++i)
            {
                SpriteRenderer symbolFrame = symbolFrames[i];
                if (symbolFrame == null) continue;
                symbolFrame.sprite = isActives[i] ? hoverSprite : normalSprite;
            }

            ResetFlags();
        }

        private void ResetFlags()
        {
            for (int i = 0; i < isActives.Length; ++i)
                isActives[i] &= false;
        }
    }
}