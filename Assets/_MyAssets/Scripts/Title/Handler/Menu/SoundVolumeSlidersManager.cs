using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SO;
using General;
using Text = TMPro.TextMeshProUGUI;

namespace Title.Handler.Menu
{
    //TODO: Remap を毎回計算しているので、デフォルト値にピッタリ戻せなくなっている
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

            // スライダーの value を設定
            {
                SetFromAmToSlider(bgmSlider, SoundType.BGM);
                SetFromAmToSlider(seSlider, SoundType.SE);

                UpdateSliderLabel(bgmText, SoundType.BGM);
                UpdateSliderLabel(seText, SoundType.SE);
            }

            // スライダーの値が変更されたときの処理をデリゲート
            {
                bgmSlider.onValueChanged.AddListener(wholeValue =>
                {
                    SetFromSliderToAm(bgmSlider, SoundType.BGM);
                    UpdateSliderLabel(bgmText, SoundType.BGM);
                });

                seSlider.onValueChanged.AddListener(wholeValue =>
                {
                    SetFromSliderToAm(seSlider, SoundType.SE);
                    UpdateSliderLabel(seText, SoundType.SE);
                });
            }

            // SEスライダーのサンプル音再生処理をデリゲート
            {
                seSampleEventTrigger.AddListener(EventTriggerType.PointerClick, PlaySeSample);
                seSampleEventTrigger.AddListener(EventTriggerType.PointerUp, PlaySeSample);



                static void PlaySeSample(PointerEventData _)
                    => AudioSourceManager.Instance.Play(SO_Sound.Entity.SymbolSE, SoundType.SE);
            }
        }

        private void SetFromAmToSlider(Slider slider, SoundType type)
        {
            if (slider == null) return;

            float volume = SoundManager.GetVolume(type);
            float value = volume.Remap(SO_Handler.Entity.MinVolume, SO_Handler.Entity.MaxVolume, slider.minValue, slider.maxValue);

            slider.value = value;
        }

        private void SetFromSliderToAm(Slider slider, SoundType type)
        {
            if (slider == null) return;

            float value = slider.value;
            float volume = value.Remap(slider.minValue, slider.maxValue, SO_Handler.Entity.MinVolume, SO_Handler.Entity.MaxVolume);

            SoundManager.SetVolume(type, volume);

            // セーブデータも更新する
            if (type == SoundType.BGM)
                SaveDataHolder.Data.BgmVolume = volume;
            else if (type == SoundType.SE)
                SaveDataHolder.Data.SeVolume = volume;
        }

        // スライダーのラベルを更新する (種類・ミュート状態)
        private void UpdateSliderLabel(Text label, SoundType type)
        {
            if (label == null) return;

            string text = type switch
            {
                SoundType.BGM => "BGM",
                SoundType.SE => "SE",
                _ => string.Empty
            };
            if (SoundManager.IsMuted(type))
                text = $"<color=#ffffff08>{text}</color>";

            label.text = text;
        }
    }
}