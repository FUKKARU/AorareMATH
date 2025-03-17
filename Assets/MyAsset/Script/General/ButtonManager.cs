using General.Extension;
using Main.Data;
using SO;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;
using Text = TMPro.TextMeshProUGUI;

namespace General
{
    internal interface IButton
    {
        void OnEnter();
        void OnExit();
        void OnClick();
    }

    /// <summary>
    /// 見た目の変化などは、基本的にこのクラス内で行う
    /// </summary>
    internal abstract class AButtonManager : MonoBehaviour, IButton
    {
        [SerializeField] private EventTrigger eventTrigger;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Text text;
        [SerializeField] private AudioSource seAudioSource;

        [SerializeField] private string displayText;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color hoverColor;

        private bool hasClicked = false;

        private void OnEnable()
        {
            if (text != null)
            {
                text.text = displayText;
                text.color = normalColor;
            }

            if (eventTrigger != null)
            {
                eventTrigger.AddListener(EventTriggerType.PointerEnter, OnEnter);
                eventTrigger.AddListener(EventTriggerType.PointerExit, OnExit);
                eventTrigger.AddListener(EventTriggerType.PointerClick, OnClick);
            }
        }

        public void OnEnter()
        {
            if (hasClicked) return;
            PlayClickSE(Pitch.Hover);
            UpdateAppearences(true);

            OnEnterImpl();
        }

        public void OnExit()
        {
            if (hasClicked) return;
            UpdateAppearences(false);

            OnExitImpl();
        }

        public void OnClick()
        {
            if (hasClicked) return;
            hasClicked = true;
            PlayClickSE();
            UpdateAppearences(false);


            OnClickImpl();
        }

        private void UpdateAppearences(bool active)
        {
            if (active)
            {
                if (text != null) text.color = hoverColor;
                if (backgroundImage != null) backgroundImage.rectTransform.localScale = new(1.1f, 1.1f, 1.0f);
                if (text != null) text.rectTransform.localScale = new(1.1f, 1.1f, 1.0f);
            }
            else
            {
                if (text != null) text.color = normalColor;
                if (backgroundImage != null) backgroundImage.rectTransform.localScale = new(1.0f, 1.0f, 1.0f);
                if (text != null) text.rectTransform.localScale = new(1.0f, 1.0f, 1.0f);
            }
        }

        protected void PlayClickSE(float pitch = 1.0f) => seAudioSource.Raise(SO_Sound.Entity.ClickSE, SoundType.SE, pitch: pitch);
        protected void ReEnableClick() => hasClicked = false;

        protected virtual void OnEnterImpl() { }
        protected virtual void OnExitImpl() { }
        protected virtual void OnClickImpl() { }
    }
}