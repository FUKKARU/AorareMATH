using UnityEngine;

namespace Main.Handler
{
    internal sealed class ShowTime : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI text;

        private void Update()
        {
            if (!text) return;
            text.text = $"{(int)GameManager.Instance.Time:D2}";
        }
    }
}