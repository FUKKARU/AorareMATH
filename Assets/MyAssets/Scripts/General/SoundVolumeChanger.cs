using General.Extension;
using UnityEngine;
using UnityEngine.UI;
using SO;
using UnityEngine.EventSystems;
using Text = TMPro.TextMeshProUGUI;

namespace General
{
    internal sealed class SoundVolumeChanger : MonoBehaviour
    {
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider seSlider;
        [SerializeField] private Text bgmText;
        [SerializeField] private Text seText;
        [SerializeField] private EventTrigger seSampleEventTrigger;
        [SerializeField] private AudioSource seSampleAudioSource;

        private void OnEnable()
        {
            if (bgmSlider == null) return;
            if (seSlider == null) return;

            bgmSlider.value = GetVolumeAsSliderValue(SoundType.BGM, bgmText);
            seSlider.value = GetVolumeAsSliderValue(SoundType.SE, seText);

            bgmSlider.onValueChanged.AddListener(value => SetVolumeFromSliderValue(SoundType.BGM, value, bgmText));
            seSlider.onValueChanged.AddListener(value => SetVolumeFromSliderValue(SoundType.SE, value, seText));

            seSampleEventTrigger.AddListener(EventTriggerType.PointerClick, PlaySeSample);
            seSampleEventTrigger.AddListener(EventTriggerType.PointerUp, PlaySeSample);
        }

        private float GetVolumeAsSliderValue(SoundType soundType, Text sliderText = null)
        {
            float sliderValue = SoundManager.GetVolume(soundType, out bool muted).ConvertToSliderValue();
            UpdateSliderText(sliderText, soundType, muted);
            return sliderValue;
        }

        private void SetVolumeFromSliderValue(SoundType soundType, float sliderValue, Text sliderText = null)
        {
            SoundManager.SetVolume(soundType, sliderValue.ConvertToVolume(), out bool muted);
            UpdateSliderText(sliderText, soundType, muted);
        }

        private void UpdateSliderText(Text sliderText, SoundType soundType, bool muted)
        {
            if (sliderText == null) return;

            string text = soundType switch
            {
                SoundType.BGM => "BGM",
                SoundType.SE => "SE",
                _ => string.Empty
            };

            if (muted)
            {
                text = $"<color=#ffffff20>{text}</color>";
            }

            sliderText.text = text;
        }

        private void PlaySeSample() => seSampleAudioSource.Raise(SO_Sound.Entity.SymbolSE, SoundType.SE);
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