using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using General;
using General.Extension;
using Ct = System.Threading.CancellationToken;

namespace Title.Handler.Menu
{
    internal sealed class InputIntervalManagerOnTogglingUi : ASingletonMonoBehaviour<InputIntervalManagerOnTogglingUi>
    {
        [SerializeField] private Image blockingImage;
        [SerializeField, Range(0.0f, 1.0f)] private float duration;

        private bool isBlocking = false;

        private void Start()
        {
            if (blockingImage != null) blockingImage.enabled = false;
        }

        // UIトグル時に呼び出し、一定秒数だけ BlockingImage をオンにする
        public void InvokeBlockingImage() => EnableBlockingImageTemporarily(destroyCancellationToken).Forget();

        private async UniTaskVoid EnableBlockingImageTemporarily(Ct ct)
        {
            if (isBlocking) return;

            isBlocking = true;
            if (blockingImage != null) blockingImage.enabled = true;

            await duration.SecAwait(ct: ct);

            if (blockingImage != null) blockingImage.enabled = false;
            isBlocking = false;
        }
    }
}