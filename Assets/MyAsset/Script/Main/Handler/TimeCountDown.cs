using UnityEngine;

namespace Main.Handler
{
    internal sealed class TimeCountDown : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI text;

        private float t;

        private void Start()
        {
            t = SO.SO_Handler.Entity.InitTimeLimt;
        }

        private void Update()
        {
            if (t > 0) t -= Time.deltaTime;
            t = Mathf.Max(0, t);

            if (text) text.text = $"{(int)t:D2}";
        }
    }
}