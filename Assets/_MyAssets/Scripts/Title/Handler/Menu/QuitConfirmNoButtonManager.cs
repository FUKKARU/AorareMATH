using General.Button;

namespace Title.Handler.Menu
{
    internal sealed class QuitConfirmNoButtonManager : ATextButtonManager
    {
        [SerializeField] private GameObject menuUi;
        [SerializeField] private GameObject quitConfirmUi;
        [SerializeField] private AButton menuButton;

        protected sealed override void OnClickSucceeded()
        {
            InputIntervalManagerOnTogglingUi.Instance.InvokeBlockingImage();

            if (menuUi != null) menuUi.SetActive(true);
            if (quitConfirmUi != null) quitConfirmUi.SetActive(false);
            if (menuButton != null) menuButton.gameObject.SetActive(true);
        }
    }
}