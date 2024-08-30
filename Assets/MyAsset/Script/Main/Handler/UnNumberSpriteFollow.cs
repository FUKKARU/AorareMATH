using General.Extension;
using Main.Data;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Main.Handler
{
    internal sealed class UnNumberSpriteFollow : MonoBehaviour
    {
        [SerializeField] private SymbolType _type;
        internal SymbolType Type => _type;

        [SerializeField] private EventTrigger eventTrigger;
        [SerializeField] private SpriteRenderer prefabSr;
        [SerializeField] private SpriteRenderer thisSr;
        private SpriteRenderer prefabInstance = null;
        private SpriteRenderer thisInstance = null;

        [SerializeField, Header("コピーしたインスタンスのz座標")] private float z;

        private bool isFollowingMouse = false;

        private void OnEnable()
        {
            eventTrigger.AddListener(EventTriggerType.PointerDown, OnPointerDown);
            eventTrigger.AddListener(EventTriggerType.PointerUp, OnPointerUp);
        }

        private void OnDisable()
        {
            eventTrigger = null;
            prefabSr = null;
            prefabInstance = null;
            thisInstance = null;
        }

        private void Update()
        {
            if (isFollowingMouse)
            {
                if (thisInstance == null) return;

                thisInstance.transform.position = MouseToWorld(z);
            }
        }

        private void OnPointerDown()
        {
            if (thisInstance != null) return;

            isFollowingMouse = true;
            thisInstance = Instantiate(thisSr, MouseToWorld(z), Quaternion.identity, transform);
            thisInstance.transform.localScale = Vector3.one;
        }

        private void OnPointerUp()
        {
            if (thisInstance == null) return;

            isFollowingMouse = false;
            Destroy(thisInstance.gameObject);
            thisInstance = null;
        }

        private Vector3 MouseToWorld(float z)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = z;
            return pos;
        }
    }
}