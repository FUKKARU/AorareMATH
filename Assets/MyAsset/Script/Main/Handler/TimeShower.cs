using UnityEngine;
using UnityEngine.UI;
using General.Extension;
using SO;

namespace Main.Handler
{
    internal sealed class TimeShower : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color dangerColor;
        [SerializeField, Range(0.01f, 0.99f)] private float dangerThreshold;

        internal void UpdateTimeUI(float remainTime)
        {
            if (fillImage != null)
            {
                float fillAmount = remainTime.Remap(SO_Handler.Entity.InitTimeLimt, 0, 1, 0);
                fillImage.fillAmount = fillAmount;
                fillImage.color = fillAmount < dangerThreshold ? dangerColor : normalColor;
            }
        }
    }
}