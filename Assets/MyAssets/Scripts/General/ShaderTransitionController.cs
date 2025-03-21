using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Ct = System.Threading.CancellationToken;

namespace General
{
    internal sealed class SceneTransitionShaderController : ASingletonMonoBehaviour<SceneTransitionShaderController>
    {
        [SerializeField, Range(0.01f, 10.0f)] private float duration = 1.5f;

        private SpriteRenderer spriteRenderer;
        private new Collider2D collider2D;
        private Material copiedMaterial = null;
        private bool onTransition = false;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            collider2D = GetComponent<Collider2D>();

            if (spriteRenderer != null)
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

        internal async UniTask Play(Ct ct)
        {
            if (onTransition) return;

            if (collider2D != null)
                collider2D.enabled = true;

            await DOTween.To
            (
                () => 2.0f,
                x => copiedMaterial.SetFloat("_Height", x),
                -1.0f,
                duration
            ).WithCancellation(ct);

            onTransition = false;
        }
    }
}