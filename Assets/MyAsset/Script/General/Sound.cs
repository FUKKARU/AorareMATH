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
        /// �X���C�_�[�̒l[0, 1]���A���ʂ̒l[minVolume, maxVolume]�ɁA���`�}�b�s���O����
        /// </summary>
        internal static float ConvertToVolume(this float sliderValue)
        {
            return sliderValue.Remap(0, 1, SO_Handler.Entity.MinVolume, SO_Handler.Entity.MaxVolume);
        }

        /// <summary>
        /// ���ʂ̒l[minVolume, maxVolume]���A�X���C�_�[�̒l[0, 1]�ɁA���`�}�b�s���O����
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

