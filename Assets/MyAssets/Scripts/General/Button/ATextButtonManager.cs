using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using General.Extension;
using Main.Data;
using SO;
using Image = UnityEngine.UI.Image;
using Text = TMPro.TextMeshProUGUI;

namespace General.Button
{
    /// <summary>
    /// Image, Text で構成される
    /// 見た目の変化などは、基本的にこのクラス内で行う
    /// Awakeを使用
    /// </summary>
    internal abstract class ATextButtonManager : MonoBehaviour, IButton
    {
        [SerializeField] private EventTrigger eventTrigger;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Text text;
        [SerializeField] private AudioSource seAudioSource;

        [SerializeField] private string displayText;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color hoverColor;

        private Vector3 imageInitialScale;
        private Vector3 textInitialScale;

        private enum AppearanceState : byte
        {
            Default,  // 通常
            BeingHovered,  // ホバーされている
            BeingClicked,  // クリックされている
        }

        private AppearanceState appearanceState = AppearanceState.Default;

        // PointerUpの時、ホバー状態に戻すか・通常状態に戻すか、判別するためのもの
        private bool isPointerInside = false;

        // これがfalseなら、クリックしても何も起こらない
        private bool isClickEnabled = true;

        // Down/Upの所では、最初にDownされたポインターのみを追跡するようにする
        // DownされてからUpされたら、追跡状態はリセット(-1)される
        private int trackingPointerId = -1;

        private void Awake()
        {
            if (backgroundImage != null)
                imageInitialScale = backgroundImage.rectTransform.localScale;

            if (text != null)
            {
                textInitialScale = text.rectTransform.localScale;

                text.text = displayText;
                text.color = normalColor;
            }

            if (eventTrigger != null)
            {
                eventTrigger.AddListener(EventTriggerType.PointerEnter, OnEnter);
                eventTrigger.AddListener(EventTriggerType.PointerExit, OnExit);
                eventTrigger.AddListener(EventTriggerType.PointerDown, OnDown);
                eventTrigger.AddListener(EventTriggerType.PointerUp, OnUp);
            }
        }

        // 概ねPCのみ
        // カーソルが範囲内に入った
        // カーソルが中にあるかのフラグを更新
        public void OnEnter(PointerEventData data)
        {
            // モバイルのみ
            // 他の指からのEnterは無視
            if (trackingPointerId != -1 && trackingPointerId != data.pointerId)
                return;

            isPointerInside = true;

            if (appearanceState != AppearanceState.Default) return;
            appearanceState = AppearanceState.BeingHovered;

            PlayClickSE(Pitch.Hover);
            UpdateAppearences();

            OnEnterImpl();
        }

        // 概ねPCのみ
        // カーソルが範囲内から出た
        // カーソルが中にあるかのフラグを更新
        public void OnExit(PointerEventData data)
        {
            // モバイルのみ
            // 他の指からのExitは無視
            if (trackingPointerId != -1 && trackingPointerId != data.pointerId)
                return;

            isPointerInside = false;

            if (appearanceState != AppearanceState.BeingHovered) return;
            appearanceState = AppearanceState.Default;

            UpdateAppearences();

            OnExitImpl();
        }

        // 範囲内でボタンを押す(タップ)した時
        public void OnDown(PointerEventData data)
        {
            // モバイルのみ
            // IDを追跡開始
            if (trackingPointerId != -1) return;
            trackingPointerId = data.pointerId;

            if (appearanceState != AppearanceState.BeingHovered) return;
            appearanceState = AppearanceState.BeingClicked;

            PlayClickSE();
            UpdateAppearences();

            OnDownImpl();
        }

        // PointerDown後にボタン(指)を放した時
        public void OnUp(PointerEventData data)
        {
            // モバイルのみ
            // IDを追跡終了
            if (trackingPointerId != data.pointerId) return;
            trackingPointerId = -1;

            if (appearanceState != AppearanceState.BeingClicked) return;
            appearanceState = isPointerInside ? AppearanceState.BeingHovered : AppearanceState.Default;

            UpdateAppearences();

            OnUpImpl();

            // 自身の範囲内でボタン(指)を放した場合、クリック成功
            if (isPointerInside && isClickEnabled)
                OnClickSucceeded();
        }

        private void UpdateAppearences()
        {
            (Color textColor, float scaleCoef) = appearanceState switch
            {
                AppearanceState.Default => (normalColor, 1.0f),
                AppearanceState.BeingHovered => (hoverColor, 1.05f),
                AppearanceState.BeingClicked => (hoverColor, 1.1f),
                _ => (normalColor, 1.0f)
            };

            if (backgroundImage != null)
            {
                backgroundImage.rectTransform.DOScale(imageInitialScale * scaleCoef, 0.1f).SetEase(Ease.OutBack);
            }
            if (text != null)
            {
                text.color = textColor;
                text.rectTransform.DOScale(textInitialScale * scaleCoef, 0.1f).SetEase(Ease.OutBack);
            }
        }

        private void PlayClickSE(float pitch = 1.0f) => seAudioSource.Raise(SO_Sound.Entity.ClickSE, SoundType.SE, pitch: pitch);

        protected void MakeClickEventDisabled() => isClickEnabled &= false;

        protected virtual void OnEnterImpl() { }
        protected virtual void OnExitImpl() { }
        protected virtual void OnDownImpl() { }
        protected virtual void OnUpImpl() { }
        protected virtual void OnClickSucceeded() { }

        // このスクリプトでやっていないプロパティ操作を行いたい場合に限る.
        protected Image BackgroundImage => backgroundImage;
        protected Text Text => text;
    }
}