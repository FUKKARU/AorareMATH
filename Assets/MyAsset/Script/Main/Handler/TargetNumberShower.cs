using UnityEngine;

namespace Main.Handler
{
    internal sealed class TargetNumberShower : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI text;

        private void OnEnable()
        {
            text.text = "";
        }

        private void OnDisable()
        {
            text = null;
        }

        private void Update()
        {
            if (text == null) return;

            text.text =
                GameManager.Instance.State == GameState.OnGoing ? GameManager.Instance.Question.Target.ToString() : "";
        }
    }
}