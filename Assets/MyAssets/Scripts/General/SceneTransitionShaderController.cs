using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Ct = System.Threading.CancellationToken;

namespace General
{
    internal sealed class SceneTransitionShaderController : MonoBehaviour
    {
        [SerializeField, Range(0.01f, 10.0f)] private float duration = 1.5f;

        private Material copiedMaterial = null;
        private bool onTransition = false;

        private void Awake()
        {
            if (TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                copiedMaterial = new(spriteRenderer.material);
                spriteRenderer.material = copiedMaterial;
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

            copiedMaterial.SetFloat("_FlipX", beforeSceneChange ? 0 : 1);
            float beginValue = beforeSceneChange ? 0 : 1;
            float endValue = beforeSceneChange ? 1 : 0;

            await DOTween.To
            (
                () => beginValue,
                x => copiedMaterial.SetFloat("_FillAmount", x),
                endValue,
                duration
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