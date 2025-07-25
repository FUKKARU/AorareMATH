using UnityEngine.EventSystems;

namespace General.Button
{
    internal interface IButton
    {
        void OnEnter(PointerEventData data);
        void OnExit(PointerEventData data);
        void OnDown(PointerEventData data);
        void OnUp(PointerEventData data);
    }

    internal abstract class AButton : MonoBehaviour, IButton
    {
        public abstract void OnEnter(PointerEventData data);
        public abstract void OnExit(PointerEventData data);
        public abstract void OnDown(PointerEventData data);
        public abstract void OnUp(PointerEventData data);
    }
}