using Cysharp.Threading.Tasks;
using DG.Tweening;
using General;
using General.Extension;
using SO;
using System;
using UnityEngine;
using UnityEngine.UI;
using Ct = System.Threading.CancellationToken;

namespace Main.Handler
{
    [Serializable]
    internal struct SpriteRenderers
    {
        [SerializeField] internal SpriteRenderer Green;
        [SerializeField] internal SpriteRenderer Yellow;
        [SerializeField] internal SpriteRenderer Red;
    }

    [Serializable]
    internal struct Sprites
    {
        [SerializeField] internal Sprite Green;
        [SerializeField] internal Sprite Yellow;
        [SerializeField] internal Sprite Red;
    }

    internal sealed class CountDown : MonoBehaviour
    {
        [SerializeField] private Image onBeginBlockingImage;
        [SerializeField] private RectTransform beginDescriptionTransform;
        [SerializeField] private Transform counterTransform;
        [SerializeField] private SpriteRenderers spriteRenderers;
        [SerializeField] private Sprites sprites;
        [SerializeField] private AudioSource audioSource;
        [SerializeField, Range(0.1f, 3.0f)] private float oneCountDuration;

        private void OnEnable()
        {
            onBeginBlockingImage.enabled = true;

            spriteRenderers.Green.enabled = false;
            spriteRenderers.Yellow.enabled = false;
            spriteRenderers.Red.enabled = false;
        }

        internal async UniTask Play(Ct ct)
        {
            await beginDescriptionTransform.DOAnchorPosX(0, 0.1f).ConvertToUniTask(beginDescriptionTransform, ct);
            await 1.0f.SecondsWait(ct);
            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: ct);
            await 0.1f.SecondsWait(ct);
            beginDescriptionTransform.gameObject.SetActive(false);

            await counterTransform.DOLocalMoveY(1.4f, 0.3f).ConvertToUniTask(counterTransform, ct);
            if (audioSource != null) audioSource.Raise(SO_Sound.Entity.CountDownSE, SoundType.SE, pitch: 1.0f / oneCountDuration, volume: 0.5f);
            spriteRenderers.Red.enabled = true;
            await WaitForOneCount(ct);
            spriteRenderers.Yellow.enabled = true;
            await WaitForOneCount(ct);
            spriteRenderers.Green.enabled = true;
            await WaitForOneCount(ct);
            spriteRenderers.Red.sprite = spriteRenderers.Green.sprite;
            spriteRenderers.Yellow.sprite = spriteRenderers.Green.sprite;
            await WaitForOneCount(ct);

            await counterTransform.DOLocalMoveY(8.75f, 0.3f).ConvertToUniTask(counterTransform, ct);
            onBeginBlockingImage.enabled = false;
        }

        private async UniTask WaitForOneCount(Ct ct)
        {
            await UniTask.WaitForSeconds(oneCountDuration, cancellationToken: ct);
        }
    }
}