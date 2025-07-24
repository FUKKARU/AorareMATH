using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SO;
using General;
using Text = TMPro.TextMeshProUGUI;

namespace Title.Handler.Menu
{
    internal sealed class SoundVolumeSlidersManager : MonoBehaviour
    {
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider seSlider;
        [SerializeField] private Text bgmText;
        [SerializeField] private Text seText;
        [SerializeField] private EventTrigger seSampleEventTrigger;

        private void Start()
        {
            if (bgmSlider == null) return;
            if (seSlider == null) return;
            if (bgmText == null) return;
            if (seText == null) return;
            if (seSampleEventTrigger == null) return;

            {
                InitializeSoundSettings(bgmSlider, SoundType.BGM, out bool bgmMuted);
                UpdateSliderLabel(bgmText, SoundType.BGM, bgmMuted);

                InitializeSoundSettings(seSlider, SoundType.SE, out bool seMuted);
                UpdateSliderLabel(seText, SoundType.SE, seMuted);
            }

            {
                bgmSlider.onValueChanged.AddListener(value =>
                {
                    UpdateSoundVolumeFromSlider(value, SoundType.BGM, out bool muted);
                    UpdateSliderLabel(bgmText, SoundType.BGM, muted);
                });

                seSlider.onValueChanged.AddListener(value =>
                {
                    UpdateSoundVolumeFromSlider(value, SoundType.SE, out bool muted);
                    UpdateSliderLabel(seText, SoundType.SE, muted);
                });
            }

            {
                seSampleEventTrigger.AddListener(EventTriggerType.PointerClick, PlaySeSample);
                seSampleEventTrigger.AddListener(EventTriggerType.PointerUp, PlaySeSample);



                static void PlaySeSample(PointerEventData data)
                    => AudioSourceManager.Instance.Play(SO_Sound.Entity.SymbolSE, SoundType.SE);
            }
        }

        // ロードされたセーブデータからサウンドボリュームの値を読み取り、AudioMixer と Slider にセットする
        private static void InitializeSoundSettings(Slider slider, SoundType type, out bool muted)
        {
            float volume = type switch
            {
                SoundType.BGM => SaveDataHolder.Data.BgmVolume,
                SoundType.SE => SaveDataHolder.Data.SeVolume,
                _ => 0.0f
            };

            SoundManager.SetVolume(type, volume, out muted);

            if (slider != null)
                slider.value = VolumeToSlider(volume);



            static float VolumeToSlider(float volume)
                => volume.Remap(SO_Handler.Entity.MinVolume, SO_Handler.Entity.MaxVolume, 0, 1);
        }

        // スライダーの値からサウンドボリュームを計算し、SaveData と AudioMixer にセットする
        private static void UpdateSoundVolumeFromSlider(float value, SoundType type, out bool muted)
        {
            float volume = SliderToVolume(value);

            if (type == SoundType.BGM) SaveDataHolder.Data.BgmVolume = volume;
            else if (type == SoundType.SE) SaveDataHolder.Data.SeVolume = volume;

            SoundManager.SetVolume(type, volume, out muted);



            static float SliderToVolume(float slider)
                => slider.Remap(0, 1, SO_Handler.Entity.MinVolume, SO_Handler.Entity.MaxVolume);
        }

        // スライダーのラベルを更新する (種類・ミュート状態)
        private void UpdateSliderLabel(Text label, SoundType type, bool muted)
        {
            if (label == null) return;

            string text = type switch
            {
                SoundType.BGM => "BGM",
                SoundType.SE => "SE",
                _ => string.Empty
            };
            if (muted)
                text = $"<color=#ffffff08>{text}</color>";

            label.text = text;
        }
    }
}