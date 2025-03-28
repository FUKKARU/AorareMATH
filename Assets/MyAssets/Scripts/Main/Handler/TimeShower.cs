using UnityEngine;
using UnityEngine.UI;
using General.Extension;
using SO;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

namespace Main.Handler
{
    internal sealed class TimeShower : MonoBehaviour
    {
        [SerializeField] private Volume volume;
        [SerializeField] private Image timerImage;
        [SerializeField] private RectTransform needleTransform;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color dangerColor;
        [SerializeField, Range(0.01f, 0.99f)] private float timerRedThresholdRatio;
        [SerializeField, Range(0.01f, 30.0f)] private float screenRedThresholdTime;

        private Vignette vignette;
        private Tween vignetteTween;
        private bool isVignetteActiveThisFrame = false;
        private bool isVignetteActivePrevFrame = false;

        private void OnEnable()
        {
            if (volume != null)
            {
                if (volume.profile.TryGet(out Vignette vignette))
                {
                    this.vignette = vignette;
                    this.vignette.intensity.Override(0);
                }
            }
        }

        private void LateUpdate()
        {
            if (isVignetteActiveThisFrame && !isVignetteActivePrevFrame)
                StartVignetteTween();
            else if (!isVignetteActiveThisFrame && isVignetteActivePrevFrame)
                StopVignetteTween();
            isVignetteActivePrevFrame = isVignetteActiveThisFrame;
            isVignetteActiveThisFrame = false;
        }

        private void OnDisable()
        {
            StopVignetteTween();
        }

        internal void UpdateTimeUI(float remainTime)
        {
            float fillAmount = remainTime.Remap(SO_Handler.Entity.InitTimeLimt, 0, 1, 0);

            if (timerImage != null)
            {
                timerImage.fillAmount = fillAmount.Remap(0.0f, 1.0f, 0.5f, 1.0f);
                timerImage.color = fillAmount < timerRedThresholdRatio ? dangerColor : normalColor;
            }

            if (needleTransform != null)
            {
                Vector3 rot = needleTransform.localEulerAngles;
                rot.z = fillAmount.Remap(0.0f, 1.0f, 90.0f, -90.0f);
                needleTransform.localEulerAngles = rot;

                float th = fillAmount.Remap(0.0f, 1.0f, Mathf.PI, 0.0f);
                float x = 253 * Mathf.Cos(th) + 456;
                float y = 140 * Mathf.Sin(th) - 505;
                needleTransform.localPosition = new Vector3(x, y, 0.0f);
            }

            isVignetteActiveThisFrame =
                GameManager.Instance.State == GameState.OnGoing
                && remainTime < screenRedThresholdTime;
        }

        private void StartVignetteTween()
        {
            StopVignetteTween();

            vignetteTween = DOTween.To(
                () => 0,
                x => vignette.intensity.Override(x),
                0.6f,
                0.8f
            ).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
        }

        private void StopVignetteTween()
        {
            if (vignetteTween != null)
            {
                vignetteTween.Kill();
                vignetteTween = null;
                vignette.intensity.Override(0);
            }
        }
    }
}