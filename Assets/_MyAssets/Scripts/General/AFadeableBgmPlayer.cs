using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Ct = System.Threading.CancellationToken;

namespace General
{
    internal interface IFadeable
    {
        void Fade();
    }

    internal abstract class AFadeableBgmPlayer : MonoBehaviour, IFadeable
    {
        // 派生クラスで取得し、格納する
        protected AudioSource audioSource = null;

        private bool hasFaded = false;

        public void Fade() => Impl(destroyCancellationToken).Forget();

        private async UniTaskVoid Impl(Ct ct)
        {
            if (hasFaded) return;
            if (audioSource == null) return;
            hasFaded = true;

            await audioSource.DOFade(0, 3).WithCancellation(ct);
            if (audioSource != null)
            {
                audioSource.Stop();
                audioSource = null;
            }
        }
    }
}