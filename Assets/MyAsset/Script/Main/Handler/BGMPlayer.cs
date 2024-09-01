using General;
using SO;
using UnityEngine;

namespace Main.Handler
{
    internal sealed class BGMPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        private void OnEnable()
        {
            if (audioSource != null) audioSource.Raise(SO_Sound.Entity.MainBGM, SoundType.BGM);
        }

        private void OnDisable()
        {
            audioSource = null;
        }
    }
}