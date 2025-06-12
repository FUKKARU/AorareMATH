using UnityEngine;
using UnityEngine.EventSystems;
using General.Extension;
using Main.Data;

namespace Main.Handler
{
    internal sealed class SkipButtonObserver : MonoBehaviour
    {
        [SerializeField] private new Transform transform;
        [SerializeField] private EventTrigger eventTrigger;
        [SerializeField, Range(1.0f, 2.0f)] private float hoverScale;
        private Vector3 initScale = Vector3.one;

        internal bool IsClickedThisFrame { get; private set; } = false;

        private void Start()
        {
            initScale = transform.localScale;

            if (eventTrigger != null)
            {
                eventTrigger.AddListener(EventTriggerType.PointerEnter, OnHover);
                eventTrigger.AddListener(EventTriggerType.PointerExit, OnUnHover);
                eventTrigger.AddListener(EventTriggerType.PointerClick, OnClick);
            }
        }

        private void LateUpdate()
        {
            IsClickedThisFrame = false;
        }

        private void OnHover()
        {
            GameManager.Instance.PlaySelectSE(Pitch.Hover);
            transform.localScale = initScale * hoverScale;
        }

        private void OnUnHover()
        {
            transform.localScale = initScale;
        }

        private void OnClick()
        {
            GameManager.Instance.PlaySelectSE();
            IsClickedThisFrame = true;
        }
    }
}
