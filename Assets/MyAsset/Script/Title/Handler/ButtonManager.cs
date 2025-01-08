using Cysharp.Threading.Tasks;
using DG.Tweening;
using General;
using SO;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Title.Handler
{
    /// <summary>
    /// nullチェック控えめ
    /// </summary>
    internal sealed class ButtonManager : MonoBehaviour
    {
        [SerializeField] private ButtonHandler startWithAssistButton;
        [SerializeField] private ButtonHandler startWithoutAssistButton;

        [SerializeField] private Transform logoTf;
        [SerializeField] private Transform loadImageTf;

        [SerializeField] private BGMPlayer bgmPlayer;
        [SerializeField] private BillMover billMover;

        private async UniTaskVoid OnEnable()
        {
            startWithAssistButton.OnClicked = () => Load(Difficulty.Type.Assist2, destroyCancellationToken).Forget();
            startWithoutAssistButton.OnClicked = () => Load(Difficulty.Type.AssistNone, destroyCancellationToken).Forget();

            billMover.Play();
            await DoLogo(destroyCancellationToken);
            startWithAssistButton.SetActive(true);
            startWithoutAssistButton.SetActive(true);
        }

        private async UniTask DoLogo(CancellationToken ct)
        {
            logoTf.DOLocalMoveY(1.15f, duration: 1.2f).SetEase(Ease.OutExpo).ToUniTask(cancellationToken: ct).Forget();
            logoTf.DOScale(new Vector2(0.8f, 0.8f), duration: 2.0f).SetEase(Ease.InOutExpo).ToUniTask(cancellationToken: ct).Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(1.8f), cancellationToken: ct);
        }

        private async UniTask Load(Difficulty.Type difficultyType, CancellationToken ct)
        {
            startWithAssistButton.SetActive(false);
            startWithoutAssistButton.SetActive(false);

            Difficulty.Value = difficultyType;
            bgmPlayer.Fade();
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: ct);
            await loadImageTf.DOLocalMoveX(0, 1).ToUniTask(cancellationToken: ct);
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f), cancellationToken: ct);
            await SceneManager.LoadSceneAsync(SO_SceneName.Entity.Main).ToUniTask(cancellationToken: ct);
        }
    }
}