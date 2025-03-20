using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Ct = System.Threading.CancellationToken;

namespace General
{
    internal sealed class SceneTransitionShaderController : ASingletonMonoBehaviour<SceneTransitionShaderController>
    {
        [SerializeField] private Material material;
        [SerializeField, Range(0.01f, 10.0f)] private float duration = 3.0f;

        private bool onTransition = false;

        internal async UniTask Play(bool beforeSceneChange, Ct ct)
        {
            if (onTransition) return;

            float beginValue = beforeSceneChange ? 2.0f : -1.0f;
            float endValue = beforeSceneChange ? -1.0f : 2.0f;

            await DOTween.To
            (
                () => beginValue,
                x => material.SetFloat("_Height", x),
                endValue,
                duration
            ).WithCancellation(ct);

            onTransition = false;
        }
    }
}