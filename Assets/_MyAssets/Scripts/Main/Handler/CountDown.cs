using System;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using General;
using General.Extension;
using SO;
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
        [SerializeField, Range(0.1f, 3.0f)] private float oneCountDurationOnFasten;

        private void OnEnable()
        {
            onBeginBlockingImage.enabled = true;

            spriteRenderers.Green.enabled = false;
            spriteRenderers.Yellow.enabled = false;
            spriteRenderers.Red.enabled = false;
        }

        internal async UniTask Play(Ct ct)
        {
            if (SaveDataHolder.CacheData.DoFastenDirections == false)
            {
                await beginDescriptionTransform.DOAnchorPosX(0, 0.1f).WithCancellation(ct);
                await 1.0f.SecAwait(ct: ct);
                await UniTask.WaitUntil(() => IsTouchedThisFrame(), cancellationToken: ct);
                await 0.1f.SecAwait(ct: ct);
                beginDescriptionTransform.gameObject.SetActive(false);
            }
            else
            {
                beginDescriptionTransform.SetLocalPosY(0); // 一応この処理も揃えておく
                beginDescriptionTransform.gameObject.SetActive(false);
            }

            float counterDuration = SaveDataHolder.CacheData.DoFastenDirections ? 0.15f : 0.3f;
            float oneCountSec = SaveDataHolder.CacheData.DoFastenDirections ? oneCountDurationOnFasten : oneCountDuration;

            await counterTransform.DOLocalMoveY(1.4f, counterDuration).WithCancellation(ct);
            // 演出が速いと音が高くなり過ぎたので、むしろ鳴らさないようにした
            if (SaveDataHolder.CacheData.DoFastenDirections == false && audioSource != null)
                audioSource.Raise(SO_Sound.Entity.CountDownSE, SoundType.SE, pitch: 1.0f / oneCountSec, volume: 0.5f);
            spriteRenderers.Red.enabled = true;
            await oneCountSec.SecAwait(ct: ct);
            spriteRenderers.Yellow.enabled = true;
            await oneCountSec.SecAwait(ct: ct);
            spriteRenderers.Green.enabled = true;
            await oneCountSec.SecAwait(ct: ct);
            spriteRenderers.Red.sprite = spriteRenderers.Green.sprite;
            spriteRenderers.Yellow.sprite = spriteRenderers.Green.sprite;
            await oneCountSec.SecAwait(ct: ct);

            await counterTransform.DOLocalMoveY(8.75f, counterDuration).WithCancellation(ct);
            onBeginBlockingImage.enabled = false;
        }

        // ちょうどこのフレームでタッチされたかどうか
        private bool IsTouchedThisFrame()
        {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
            return Input.GetMouseButtonDown(0);
#elif UNITY_IOS || UNITY_ANDROID
            for (int i = 0; i < Input.touchCount; ++i)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began)
                    return true;
            }
            return false;
#else
            return false;  // 他のプラットフォームはサポートしていない
#endif
        }
    }
}