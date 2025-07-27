namespace General.Shaders
{
    internal sealed class SceneTransitionShaderController : MonoBehaviour
    {
        [SerializeField, Range(0.01f, 10.0f)] private float duration;
        [SerializeField, Range(0.01f, 10.0f)] private float durationOnFasten;

        private Material copiedMaterial = null;
        private bool onTransition = false;

        private static readonly int FillAmountID = Shader.PropertyToID("_FillAmount");
        private static readonly int FlipXID = Shader.PropertyToID("_FlipX");
        private static readonly int UseNoiseAlphaID = Shader.PropertyToID("_UseNoiseAlpha");

        private void Awake()
        {
            if (TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                copiedMaterial = new(spriteRenderer.material);
                spriteRenderer.material = copiedMaterial;

                // プラットフォームによってうまく動かなかったりしたので、計算方法を条件分岐する
#if UNITY_STANDALONE || UNITY_WEBGL
                copiedMaterial.SetFloat(UseNoiseAlphaID, 1);
#elif UNITY_IOS || UNITY_ANDROID
                copiedMaterial.SetFloat(UseNoiseAlphaID, 0);
#endif
            }
        }

        private void OnDestroy()
        {
            Destroy(copiedMaterial);
            copiedMaterial = null;
        }

        internal async UniTask Play(bool beforeSceneChange, Ct ct)
        {
            if (onTransition) return;

            if (beforeSceneChange)
            {
                if (TryGetComponent(out Collider2D collider))
                    collider.enabled = true;
            }

            copiedMaterial.SetFloat(FlipXID, beforeSceneChange ? 0 : 1);
            float beginValue = beforeSceneChange ? 0 : 1;
            float endValue = beforeSceneChange ? 1 : 0;

            float dur = SaveDataHolder.CacheData.DoFastenDirections ? durationOnFasten : duration;

            await DOTween.To
            (
                () => beginValue,
                x => copiedMaterial.SetFloat(FillAmountID, x),
                endValue,
                dur
            ).WithCancellation(ct);

            if (!beforeSceneChange)
            {
                if (TryGetComponent(out Collider2D collider))
                    collider.enabled = false;
            }

            onTransition = false;
        }
    }
}