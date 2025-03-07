using Cysharp.Threading.Tasks;
using DG.Tweening;
using General.Extension;
using System;
using System.Threading;
using UnityEngine;

namespace Title.Handler
{
    internal sealed class LogoDirector : MonoBehaviour
    {
        [SerializeField] private GameObject startButton;
        [SerializeField] private Transform logoTf;
        [SerializeField] private BillMover billMover;

        private async UniTaskVoid OnEnable()
        {
            if (billMover != null) billMover.Play();
            await DoLogo(destroyCancellationToken);
            if (startButton != null) startButton.SetActive(true);
        }

        private async UniTask DoLogo(CancellationToken ct)
        {
            logoTf.DOLocalMoveY(1.15f, duration: 1.2f).SetEase(Ease.OutExpo).ConvertToUniTask(logoTf, ct).Forget();
            logoTf.DOScale(new Vector2(0.8f, 0.8f), duration: 2.0f).SetEase(Ease.InOutExpo).ConvertToUniTask(logoTf, ct).Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(1.8f), cancellationToken: ct);
        }
    }
}