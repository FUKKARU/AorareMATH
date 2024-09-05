using UnityEngine;

namespace Main.Handler
{
    internal sealed class TimeShower : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI text;

        private void Update()
        {
            if (!text) return;

            text.text = ((int)GameManager.Instance.Time).ToString();
        }
    }
}