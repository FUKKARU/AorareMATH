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
        /// �X���C�_�[�̒l[0, 1]���A���ʂ̒l[minVolume, maxVolume]�ɁA���`�}�b�s���O����
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

