using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace General
{
    internal sealed class Sound : MonoBehaviour,IPointerUpHandler 
    {
        [SerializeField]
        private Slider bgmSlider;
        [SerializeField]
        private Slider seSlider;
        [SerializeField]
        private bool isBGM;

        private static readonly float MAX_VOLUME = 20.0f;
        private static readonly float MIN_VOLUME = -20.0f;

        private void Start()
        {

        }

        private void Update()
        {
            
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if(isBGM)
            {
                float value = bgmSlider.value;
                float volume = value.Remap(0,1,-20,20) ;
                volume = Mathf.Clamp(volume, MIN_VOLUME,MAX_VOLUME);
                SoundManager.SetVolume(SoundType.BGM, volume);
            }
            else
            {
                float value = seSlider.value;
                float volume = value.Remap(0, 1, -20, 20);
                volume = Mathf.Clamp(volume, MIN_VOLUME, MAX_VOLUME);
                SoundManager.SetVolume(SoundType.SE, volume);
            }
        }
    }

    internal static class Ex
    {
        internal static float Remap(this float x, float a, float b, float c, float d)
        {
            return (x - a) * (d - c) / (b - a) + c;
        }
    }
}

