using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using General;
using SO;

namespace Main.Handler
{
    internal sealed class TimeShower : MonoBehaviour
    {
        [SerializeField] private Volume volume;
        // 真ん中に向かって縮んでいくため、Image, Needle をそれぞれ2つ併用している
        [SerializeField] private Image[] timerImages;
        [SerializeField] private RectTransform needleLeftTransform;
        [SerializeField] private RectTransform needleRightTransform;
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

            if (timerImages != null)
            {
                foreach (Image timerImage in timerImages)
                {
                    if (timerImage == null) continue;

                    timerImage.fillAmount = fillAmount;
                    timerImage.color = fillAmount < timerRedThresholdRatio ? dangerColor : normalColor;
                }
            }

            if (needleLeftTransform != null && needleRightTransform != null)
            {
                float leftX = fillAmount.Remap(0.0f, 1.0f, 0.0f, -900.0f);
                float rightX = fillAmount.Remap(0.0f, 1.0f, 0.0f, 900.0f);

                needleLeftTransform.SetLocalPosX(leftX);
                needleRightTransform.SetLocalPosX(rightX);
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