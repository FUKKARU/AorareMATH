using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine.EventSystems;

namespace General.Extension
{
    internal static class Extension
    {
        internal static async UniTask SecondsWaitAndDo(this float waitSeconds, System.Action act, CancellationToken ct)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(waitSeconds));
            act();
        }

        /// <summary>
        /// EventTrigger‚ÉƒCƒxƒ“ƒg‚ð“o˜^‚·‚é
        /// </summary>
        internal static void AddListener(this EventTrigger eventTrigger, EventTriggerType type, System.Action action)
        {
            EventTrigger.Entry entry = new() { eventID = type };
            entry.callback.AddListener(_ => { action(); });
            eventTrigger.triggers.Add(entry);
        }
    }
}