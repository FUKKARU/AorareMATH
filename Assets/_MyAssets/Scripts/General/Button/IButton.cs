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
}