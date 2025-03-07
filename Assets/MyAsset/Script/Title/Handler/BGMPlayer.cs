using Cysharp.Threading.Tasks;
using DG.Tweening;
using General;
using General.Extension;
using SO;
using UnityEngine;

namespace Title.Handler
{
    internal sealed class BGMPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        private bool hasFaded = false;

        private void OnEnable() => _audioSource.Raise(SO_Sound.Entity.TitleBGM, SoundType.BGM);

        internal void Fade()
        {
            if (hasFaded) return;
            if (_audioSource == null) return;
            hasFaded = true;
            _audioSource.DOFade(0, 3).ConvertToUniTask(_audioSource, destroyCancellationToken).Forget();
        }
    }
}