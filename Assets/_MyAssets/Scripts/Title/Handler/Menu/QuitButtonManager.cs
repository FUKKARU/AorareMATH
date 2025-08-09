using General.Button;

namespace Title.Handler.Menu
{
    internal sealed class QuitButtonManager : ATextButtonManager
    {
        [SerializeField] private GameObject menuUi;
        [SerializeField] private GameObject quitConfirmUi;
        [SerializeField] private AButton menuButton;

        private void Start()
        {
            // モバイル・WebGL では、ゲーム終了ボタンを出さない
#if !(UNITY_EDITOR || UNITY_STANDALONE)
            gameObject.SetActive(false);
#endif
        }

        private protected sealed override void OnClickSucceeded()
        {
            InputIntervalManagerOnTogglingUi.Instance.InvokeBlockingImage();

            if (menuUi != null) menuUi.SetActive(false);
            if (quitConfirmUi != null) quitConfirmUi.SetActive(true);
            if (menuButton != null) menuButton.gameObject.SetActive(false);
        }
    }
}