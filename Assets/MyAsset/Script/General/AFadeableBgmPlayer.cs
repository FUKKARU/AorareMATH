using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace General
{
    internal interface IFadeable
    {
        void Fade();
    }

    internal abstract class AFadeableBgmPlayer : MonoBehaviour, IFadeable
    {
        [SerializeField] protected AudioSource _audioSource;

        private bool hasFaded = false;

        public void Fade()
        {
            if (hasFaded) return;
            if (_audioSource == null) return;
            hasFaded = true;
            _audioSource.DOFade(0, 3).WithCancellation(destroyCancellationToken);
        }
    }
}