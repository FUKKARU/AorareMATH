using UnityEngine;
using Cysharp.Threading.Tasks;
using General.Extension;
using Ct = System.Threading.CancellationToken;

namespace General.Button
{
    internal abstract class ASceneChangeButtonManager : ATextButtonManager
    {
        [SerializeField] private AFadeableBgmPlayer bgmPlayer;
        [SerializeField] private SceneTransitionShaderController sceneTransitionShaderController;
        [SerializeField] private ASceneChangeButtonManager[] linkedButtons; // どれか一つが押されたら、他のボタンも押せなくなる

        protected abstract string toSceneName { get; }

        protected sealed override void OnClickSucceeded()
        {
            SetLinkedButtonsClicked();
            Load(destroyCancellationToken).Forget();
        }

        private void SetLinkedButtonsClicked()
        {
            if (linkedButtons == null) return;

            foreach (var linkedButton in linkedButtons)
            {
                if (linkedButton == null) continue;
                linkedButton.MakeClickEventDisabled();
            }
        }

        private async UniTaskVoid Load(Ct ct)
        {
            if (bgmPlayer != null) bgmPlayer.Fade();

            await UniTask.WaitForSeconds(0.2f, cancellationToken: ct);
            if (sceneTransitionShaderController != null)
                await sceneTransitionShaderController.Play(true, ct);
            await UniTask.WaitForSeconds(1.5f, cancellationToken: ct);

            toSceneName.LoadAsync().Forget();
        }
    }
}