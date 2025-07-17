using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Ct = System.Threading.CancellationToken;
using SO;

namespace General.Shaders
{
    internal sealed class SceneTransitionShaderController : MonoBehaviour
    {
        [SerializeField, Range(0.01f, 10.0f)] private float duration;
        [SerializeField, Range(0.01f, 10.0f)] private float durationOnFasten;

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

            float dur = SO_Handler.Entity.DoFastenDirections ? durationOnFasten : duration;

            await DOTween.To
            (
                () => beginValue,
                x => copiedMaterial.SetFloat("_FillAmount", x),
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