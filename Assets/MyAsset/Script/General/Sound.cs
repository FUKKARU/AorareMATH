using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SO;

namespace General
{
    internal sealed class Sound : MonoBehaviour, IPointerUpHandler
    {
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider seSlider;
        [SerializeField] private bool isBGM;

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isBGM)
            {
                float volume = bgmSlider.value.Convert().ClampedVolume();
                SoundManager.SetVolume(SoundType.BGM, volume);
            }
            else
            {
                float volume = seSlider.value.Convert().ClampedVolume();
                SoundManager.SetVolume(SoundType.SE, volume);
            }
        }
    }

    internal static class Ex
    {
        private static float Remap(this float x, float a, float b, float c, float d)
        {
            return (x - a) * (d - c) / (b - a) + c;
        }

        /// <summary>
        /// スライダーの値[0, 1]を、音量の値[minVolume, maxVolume]に、線形マッピングする
        /// </summary>
        internal static float Convert(this float sliderValue)
        {
            return sliderValue.Remap(0, 1, SO_Handler.Entity.MinVolume, SO_Handler.Entity.MaxVolume);
        }

        internal static float ClampedVolume(this float volume)
        {
            return Mathf.Clamp(volume, SO_Handler.Entity.MinVolume, SO_Handler.Entity.MaxVolume);
        }
    }
}

