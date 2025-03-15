using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using General.Extension;
using Text = TMPro.TextMeshProUGUI;
using Ct = System.Threading.CancellationToken;

namespace Main.Handler
{
    internal sealed class CorrectAmountTextShower : MonoBehaviour
    {
        [SerializeField] private Text text;

        private bool hasAppeared = false;

        private void Update()
        {
            UpdateText();
        }

        private void UpdateText()
        {
            if (GameManager.Instance.State != GameState.OnGoing) return;
            if (!hasAppeared) return;

            int correctAmount = GameManager.Instance.GameData.CorrectAmount;
            if (correctAmount <= 0) return;

            if (text == null) return;
            text.text = $"{GameManager.Instance.GameData.CorrectAmount}<size=60><color=black>問正解中</color></size>";
        }

        internal async UniTaskVoid Appear(Ct ct)
        {
            if (hasAppeared) return;
            if (text == null) return;
            await text.DOFade(1, 0.5f).ConvertToUniTask(text, ct);
            hasAppeared = true;
        }
    }
}