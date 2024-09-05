using General;
using SO;
using UnityEngine;

namespace Title.Handler
{
    internal sealed class BGMPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        internal AudioSource AudioSource => _audioSource;

        private void OnEnable()
        {
            if (_audioSource != null) _audioSource.Raise(SO_Sound.Entity.TitleBGM, SoundType.BGM);
        }

        private void OnDisable()
        {
            _audioSource = null;
        }
    }
}