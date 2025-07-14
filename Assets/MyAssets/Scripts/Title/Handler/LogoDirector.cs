using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Ct = System.Threading.CancellationToken;
using SO;
using General.Extension;

namespace Title.Handler
{
    internal sealed class LogoDirector : MonoBehaviour
    {
        [SerializeField] private GameObject startButton;
        [SerializeField] private Transform logoTf;
        [SerializeField] private BillMover billMover;

        private async UniTaskVoid OnEnable()
        {
            if (billMover != null) billMover.Play(destroyCancellationToken).Forget();
            await DoLogo(destroyCancellationToken);
            if (startButton != null) startButton.SetActive(true);
        }

        private async UniTask DoLogo(Ct ct)
        {
            if (SO_Handler.Entity.DoFastenDirections == false)
            {
                logoTf.DOLocalMoveY(1.15f, duration: 1.2f).SetEase(Ease.OutExpo).WithCancellation(ct).Forget();
                logoTf.DOScale(new Vector2(0.8f, 0.8f), duration: 2.0f).SetEase(Ease.InOutExpo).WithCancellation(ct).Forget();
                await UniTask.WaitForSeconds(1.8f, cancellationToken: ct);
            }
            else
            {
                logoTf.SetLocalPosY(1.15f);
                logoTf.SetScaleXY(0.8f, 0.8f);
            }
        }
    }
}