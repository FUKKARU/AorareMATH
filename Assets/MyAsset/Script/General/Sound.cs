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

        private void Start()
        {
            if (isBGM)
            {
                float value = SoundManager.GetVolume(SoundType.BGM).ConvertToSliderValue();
                bgmSlider.value = value;
            }
            else
            {
                float value = SoundManager.GetVolume(SoundType.SE).ConvertToSliderValue();
                seSlider.value = value;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (isBGM)
            {
                float volume = bgmSlider.value.ConvertToVolume().ClampedVolume();
                SoundManager.SetVolume(SoundType.BGM, volume);
            }
            else
            {
                float volume = seSlider.value.ConvertToVolume().ClampedVolume();
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
        internal static float ConvertToVolume(this float sliderValue)
        {
            return sliderValue.Remap(0, 1, SO_Handler.Entity.MinVolume, SO_Handler.Entity.MaxVolume);
        }

        /// <summary>
        /// 音量の値[minVolume, maxVolume]を、スライダーの値[0, 1]に、線形マッピングする
        /// </summary>
        internal static float ConvertToSliderValue(this float volume)
        {
            return volume.Remap(SO_Handler.Entity.MinVolume, SO_Handler.Entity.MaxVolume, 0, 1);
        }

        internal static float ClampedVolume(this float volume)
        {
            return Mathf.Clamp(volume, SO_Handler.Entity.MinVolume, SO_Handler.Entity.MaxVolume);
        }
    }
}

