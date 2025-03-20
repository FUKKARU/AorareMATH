using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using General.Extension;
using Main.Data;
using SO;
using Image = UnityEngine.UI.Image;
using Text = TMPro.TextMeshProUGUI;
using Ct = System.Threading.CancellationToken;

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

        // 1回しかクリックできなくするためのフラグ
        private bool _hasClicked = false;

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
        protected bool hasClicked
        {
            get => _hasClicked;
            set
            {
                _hasClicked = value;
                if (value) UpdateAppearences(false);
            }
        }

        protected virtual void OnEnterImpl() { }
        protected virtual void OnExitImpl() { }
        protected virtual void OnClickImpl() { }
    }

    internal abstract class ASceneChangeButtonManager : AButtonManager
    {
        [SerializeField] private Transform loadImage;
        [SerializeField] private AFadeableBgmPlayer bgmPlayer;

        // どれか一つが押されたら、他のボタンも押せなくなる
        [SerializeField] private ASceneChangeButtonManager[] linkedButtons;

        protected abstract string toSceneName { get; }

        protected override void OnClickImpl()
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
                linkedButton.hasClicked = true;
            }
        }

        private async UniTaskVoid Load(Ct ct)
        {
            if (bgmPlayer != null) bgmPlayer.Fade();

            await UniTask.WaitForSeconds(0.2f, cancellationToken: ct);

            await SceneTransitionShaderController.Instance.Play(true, ct);
            await UniTask.WaitForSeconds(0.8f, cancellationToken: ct);

            if (loadImage != null)
            {
                loadImage.SetPositionX(-18.5f);
                await loadImage.DOLocalMoveX(0, 0.5f).WithCancellation(ct);
            }

            await UniTask.WaitForSeconds(1.5f, cancellationToken: ct);

            toSceneName.LoadAsync().Forget();

            await SceneTransitionShaderController.Instance.Play(false, ct);
        }
    }
}