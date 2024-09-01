using General;
using SO;
using UnityEngine;

namespace Title.Handler
{
    internal sealed class BGMPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        private void OnEnable()
        {
            if (audioSource != null) audioSource.Raise(SO_Sound.Entity.TitleBGM, SoundType.BGM);
        }

        private void OnDisable()
        {
            audioSource = null;
        }
    }
}