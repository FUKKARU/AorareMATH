using General.Extension;
using UnityEngine;
using UnityEngine.UI;
using SO;
using UnityEngine.EventSystems;

namespace General
{
    internal sealed class SoundVolumeChanger : MonoBehaviour
    {
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider seSlider;
        [SerializeField] private EventTrigger seSampleEventTrigger;
        [SerializeField] private AudioSource seSampleAudioSource;

        private void OnEnable()
        {
            if (bgmSlider == null) return;
            if (seSlider == null) return;

            bgmSlider.value = SoundManager.GetVolume(SoundType.BGM).ConvertToSliderValue();
            seSlider.value = SoundManager.GetVolume(SoundType.SE).ConvertToSliderValue();

            bgmSlider.onValueChanged.AddListener(value => SoundManager.SetVolume(SoundType.BGM, value.ConvertToVolume()));
            seSlider.onValueChanged.AddListener(value => SoundManager.SetVolume(SoundType.SE, value.ConvertToVolume()));

            seSampleEventTrigger.Bind(EventTriggerType.PointerClick, () => seSampleAudioSource.Raise(SO_Sound.Entity.JustAttackedSE, SoundType.SE));
            seSampleEventTrigger.Bind(EventTriggerType.PointerUp, () => seSampleAudioSource.Raise(SO_Sound.Entity.JustAttackedSE, SoundType.SE));
        }
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