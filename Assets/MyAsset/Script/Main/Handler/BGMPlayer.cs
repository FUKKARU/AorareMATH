using Cysharp.Threading.Tasks;
using DG.Tweening;
using General;
using General.Extension;
using SO;
using UnityEngine;

namespace Main.Handler
{
    internal sealed class BGMPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        private bool hasPlayed = false;
        private bool hasFaded = false;

        internal void Play()
        {
            if (hasPlayed) return;
            if (_audioSource == null) return;
            hasPlayed = true;
            _audioSource.Raise(SO_Sound.Entity.MainBGM, SoundType.BGM);
        }

        internal void Fade()
        {
            if (hasFaded) return;
            if (_audioSource == null) return;
            hasFaded = true;
            _audioSource.DOFade(0, 3).ConvertToUniTask(_audioSource, destroyCancellationToken).Forget();
        }
    }
}