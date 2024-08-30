using General.Extension;
using Main.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Main.Handler
{
    internal sealed class NumberSpriteFollow : MonoBehaviour
    {
        [SerializeField] private SymbolType _type;
        internal SymbolType Type => _type;

        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private EventTrigger eventTrigger;

        [SerializeField, Header("インスタンスのz座標")] private float z;

        private bool isFollowingMouse = false;

        private void OnEnable()
        {
            spriteRenderer.sprite = _type.GetSprite();

            eventTrigger.AddListener(EventTriggerType.PointerDown, OnPointerDown);
            eventTrigger.AddListener(EventTriggerType.PointerUp, OnPointerUp);
        }

        private void OnDisable()
        {
            spriteRenderer = null;
            eventTrigger = null;
        }

        private void Update()
        {
            if (isFollowingMouse) transform.position = MouseToWorld(z);
        }

        private void OnPointerDown()
        {
            isFollowingMouse = true;
        }

        private void OnPointerUp()
        {
            isFollowingMouse = false;
        }

        private Vector3 MouseToWorld(float z)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = z;
            return pos;
        }
    }
}