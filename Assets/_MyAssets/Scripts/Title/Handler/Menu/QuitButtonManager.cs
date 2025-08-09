using General.Button;

namespace Title.Handler.Menu
{
    internal sealed class QuitButtonManager : ATextButtonManager
    {
        [SerializeField] private GameObject menuUi;
        [SerializeField] private GameObject quitConfirmUi;
        [SerializeField] private AButton menuButton;

        private protected sealed override void OnClickSucceeded()
        {
            InputIntervalManagerOnTogglingUi.Instance.InvokeBlockingImage();

            if (menuUi != null) menuUi.SetActive(false);
            if (quitConfirmUi != null) quitConfirmUi.SetActive(true);
            if (menuButton != null) menuButton.gameObject.SetActive(false);
        }
    }
}