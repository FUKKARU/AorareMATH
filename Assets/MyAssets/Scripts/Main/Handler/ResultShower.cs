using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using General;
using General.Extension;
using Text = TMPro.TextMeshProUGUI;
using Ct = System.Threading.CancellationToken;

namespace Main.Handler
{
    internal sealed class ResultShower : MonoBehaviour
    {
        [SerializeField] private RectTransform baseImageRt;
        [SerializeField] private Text correctAmountText;
        [SerializeField] private Text userscoreRankText;
        [SerializeField] private ASceneChangeButtonManager[] buttons;

        private void OnEnable()
        {
            baseImageRt.localPosition = new(0, 900, 0);
            correctAmountText.text = string.Empty;
            userscoreRankText.text = string.Empty;
            SetButtonsEnabled(false);
        }

        internal async UniTask Play(int correctAmount,int rank, bool hasForciblyCleared, Ct ct)
        {
            if (hasForciblyCleared)
            {
                correctAmountText.color = Color.yellow;
                userscoreRankText.color = Color.yellow;
            }
            await 0.1f.SecondsWait(ct);
            await baseImageRt.DOAnchorPosY(0, 0.5f).WithCancellation(ct);
            await 0.1f.SecondsWait(ct);

            await DOTween.To(
                () => 0,
                x => correctAmountText.text = x.ToString(),
                correctAmount,
                1.0f
            ).WithCancellation(ct);

            await DOTween.To(
                () => 0,
                x => userscoreRankText.text = $"{rank}位",
                rank,
                1.0f
            ).WithCancellation(ct);
            

            await 0.2f.SecondsWait(ct);
            SetButtonsEnabled(true);
        }

        private void SetButtonsEnabled(bool enabled)
        {
            if (buttons == null) return;

            foreach (var button in buttons)
            {
                if (button == null) continue;
                button.gameObject.SetActive(enabled);
            }
        }
    }
}