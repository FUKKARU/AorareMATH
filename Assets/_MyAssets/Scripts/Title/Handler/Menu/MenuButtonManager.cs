using UnityEngine;
using General.Button;

namespace Title.Handler.Menu
{
    internal sealed class MenuButtonManager : ATextButtonManager
    {
        [SerializeField] private StartButtonManager startButton;
        [SerializeField] private SpriteRenderer titleLogo;
        [SerializeField] private GameObject menuUi;
        [SerializeField] private GameObject menuBgUi;
        [SerializeField] private string displayTextWhenMenuIsActive;

        private bool isMenuActive = false;

        private void Start()
        {
            if (menuUi != null) menuUi.SetActive(false);
            if (menuBgUi != null) menuBgUi.SetActive(false);
        }

        protected sealed override void OnClickSucceeded()
        {
            isMenuActive = !isMenuActive;

            if (startButton != null) startButton.gameObject.SetActive(!isMenuActive);
            if (menuUi != null) menuUi.SetActive(isMenuActive);
            if (menuBgUi != null) menuBgUi.SetActive(isMenuActive);
            if (titleLogo != null) titleLogo.enabled = !isMenuActive;
            if (Text != null) Text.text = isMenuActive ? displayTextWhenMenuIsActive : DisplayText;
        }
    }
}