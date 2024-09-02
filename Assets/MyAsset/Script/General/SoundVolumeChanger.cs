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

        private void OnEnable()
        {
            if (bgmSlider == null) return;
            if (seSlider == null) return;

            bgmSlider.value = UpdateValueFromVolume(SoundType.BGM);
            seSlider.value = UpdateValueFromVolume(SoundType.SE);
        }

        private void OnDisable()
        {
            bgmSlider = null;
            seSlider = null;
        }

        private void Update()
        {
            if (bgmSlider == null) return;
            if (seSlider == null) return;

            UpdateVolumeFromValue(bgmSlider.value, SoundType.BGM);
            UpdateVolumeFromValue(seSlider.value, SoundType.SE);
        }

        private void UpdateVolumeFromValue(float sliderValue, SoundType type)
            => SoundManager.SetVolume(type, sliderValue.ConvertToVolume());

        private float UpdateValueFromVolume(SoundType type)
            => SoundManager.GetVolume(type).ConvertToSliderValue();
    }

    internal static class SoundVolumeChangerEx
    {
        /// <summary>
        /// スライダーの値[0, 1]を、音量の値[minVolume, maxVolume]に、線形マッピングする
        /// </summary>
        internal static float ConvertToVolume(this float sliderValue)
            => sliderValue.Remap(0, 1, SO_Handler.Entity.MinVolume, SO_Handler.Entity.MaxVolume);

        /// <summary>
        /// 音量の値[minVolume, maxVolume]を、スライダーの値[0, 1]に、線形マッピングする
        /// </summary>
        internal static float ConvertToSliderValue(this float volume)
            => volume.Remap(SO_Handler.Entity.MinVolume, SO_Handler.Entity.MaxVolume, 0, 1);
    }
}