using UnityEngine;
using General.Button;

namespace Title.Handler
{
    internal sealed class MenuButtonManager : ATextButtonManager
    {
        [SerializeField] private StartButtonManager startButton;
        [SerializeField] private SpriteRenderer titleLogo;
        [SerializeField] private GameObject menuRoot;
        [SerializeField] private string displayTextWhenMenuIsActive;

        private bool isMenuActive = false;

        private void Start()
        {
            if (menuRoot != null) menuRoot.SetActive(false);
        }

        protected sealed override void OnClickSucceeded()
        {
            isMenuActive = !isMenuActive;

            if (startButton != null) startButton.gameObject.SetActive(!isMenuActive);
            if (menuRoot != null) menuRoot.SetActive(isMenuActive);
            if (titleLogo != null) titleLogo.enabled = !isMenuActive;
            if (Text != null) Text.text = isMenuActive ? displayTextWhenMenuIsActive : DisplayText;
        }
    }
}