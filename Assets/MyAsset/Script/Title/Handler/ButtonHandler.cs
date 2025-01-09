using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Title.Handler
{
    internal sealed class ButtonHandler : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private TextMeshProUGUI text;

        private static readonly Color normalColor = Color.white;
        private static readonly Color hoverColor = Color.yellow;

        public Action OnClicked { get; set; } = null;

        private bool hasClicked = false;

        private void OnEnable()
        {
            text.color = normalColor;

            EventTrigger eventTrigger = spriteRenderer.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry enter = new() { eventID = EventTriggerType.PointerEnter };
            enter.callback.AddListener(_ => OnEnter());
            eventTrigger.triggers.Add(enter);

            EventTrigger.Entry exit = new() { eventID = EventTriggerType.PointerExit };
            exit.callback.AddListener(_ => OnExit());
            eventTrigger.triggers.Add(exit);

            EventTrigger.Entry click = new() { eventID = EventTriggerType.PointerClick };
            click.callback.AddListener(_ => OnClick());
            eventTrigger.triggers.Add(click);
        }

        private void OnEnter() => text.color = hoverColor;
        private void OnExit() => text.color = normalColor;
        private void OnClick()
        {
            if (hasClicked) return;
            hasClicked = true;
            OnClicked?.Invoke();
        }

        public void SetActive(bool value)
        {
            spriteRenderer.enabled = value;
            text.enabled = value;
        }
    }
}
