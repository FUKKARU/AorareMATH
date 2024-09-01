using General.Extension;
using UnityEngine;
using UnityEngine.UI;
using SO;

namespace General
{
    internal sealed class SoundVolumeChanger : MonoBehaviour
    {
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider seSlider;

        private void OnDisable()
        {
            bgmSlider = null;
            seSlider = null;
        }

        private void Update()
        {
            bgmSlider.UpdateVolume(SoundType.BGM);
            seSlider.UpdateVolume(SoundType.SE);
        }
    }

    internal static class SoundVolumeChangerEx
    {
        internal static void UpdateVolume(this Slider slider, SoundType type)
        {
            if (slider == null) return;
            SoundManager.SetVolume(type, slider.value.ConvertToVolume().ToClampedVolume());
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

        internal static float ToClampedVolume(this float volume)
        {
            return Mathf.Clamp(volume, SO_Handler.Entity.MinVolume, SO_Handler.Entity.MaxVolume);
        }
    }
}