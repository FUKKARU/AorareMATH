using General.Shaders;

namespace General.Button
{
    internal abstract class ASceneChangeButtonManager : ATextButtonManager
    {
        [SerializeField] private AFadeableBgmPlayer bgmPlayer;
        [SerializeField] private SceneTransitionShaderController sceneTransitionShaderController;
        [SerializeField] private ASceneChangeButtonManager[] linkedButtons; // どれか一つが押されたら、他のボタンは無効になる

        private bool isClickEnabled = true;

        protected abstract Scene toScene { get; }

        protected sealed override void OnClickSucceeded()
        {
            SetLinkedButtonsClicked();
            Load(destroyCancellationToken).Forget();
        }

        private void SetLinkedButtonsClicked()
        {
            if (!isClickEnabled) return;
            if (linkedButtons == null) return;

            foreach (var linkedButton in linkedButtons)
            {
                if (linkedButton == null) continue;
                linkedButton.isClickEnabled = false;
            }
        }

        private async UniTaskVoid Load(Ct ct)
        {
            if (bgmPlayer != null) bgmPlayer.Fade();

            float beforeDirectionDuration = SaveDataHolder.CacheData.DoFastenDirections ? 0.1f : 0.2f;
            float afterDirectionDuration = SaveDataHolder.CacheData.DoFastenDirections ? 1.0f : 1.5f;

            await beforeDirectionDuration.SecAwait(ct: ct);
            if (sceneTransitionShaderController != null)
                await sceneTransitionShaderController.Play(true, ct);
            await afterDirectionDuration.SecAwait(ct: ct);

            toScene.LoadAsync();
        }
    }
}