namespace Title.Handler.Menu
{
    internal sealed class UiActiveStatesInitializer : MonoBehaviour
    {
        [SerializeField] private GameObject menuBgUi;
        [SerializeField] private GameObject menuUi;
        [SerializeField] private GameObject quitConfirmUi;

        private void Start()
        {
            if (menuBgUi != null) menuBgUi.SetActive(false);
            if (menuUi != null) menuUi.SetActive(false);
            if (quitConfirmUi != null) quitConfirmUi.SetActive(false);
        }
    }
}