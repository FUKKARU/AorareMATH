using UnityEngine;

namespace Main.Handler
{
    internal sealed class TargetNumberShower : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI text;

        private void Update()
        {
            if (text == null) return;

            text.text =
                GameManager.Instance.State == GameState.OnGoing ? GameManager.Instance.Question.Target.ToString() : "";
        }

        private void OnDisable()
        {
            text = null;
        }
    }
}