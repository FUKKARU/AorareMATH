using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using General.Extension;
using SO;
using UnityEngine;
using UnityEngine.UI;

namespace Main.Handler
{
    internal sealed class TimeShower : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        private CancellationTokenSource ctsAny = new();

        private void OnDisable()
        {
            ctsAny?.Cancel();
            ctsAny?.Dispose();
            ctsAny = null;
        }

        internal void UpdateTimeUI(float remainTime)
        {
            if (fillImage != null) fillImage.fillAmount = remainTime.Remap(SO_Handler.Entity.InitTimeLimt, 0, 1, 0);
        }

        internal void PlayTimeIncreaseAnimation()
        {
            ctsAny?.Cancel();
            ctsAny?.Dispose();
            ctsAny = new();
            PlayAnimation(CancellationTokenSource.CreateLinkedTokenSource(ctsAny.Token, destroyCancellationToken).Token).Forget();
        }

        private async UniTaskVoid PlayAnimation(CancellationToken ct)
        {
            fillImage.color = Color.blue;
            await 1.0f.SecondsWait(ct);
            await fillImage.DOColor(Color.red, 1.0f).ConvertToUniTask(fillImage, ct);
        }
    }
}