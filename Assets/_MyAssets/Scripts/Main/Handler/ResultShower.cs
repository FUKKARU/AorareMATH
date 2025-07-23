using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using General;
using General.Button;
using General.Extension;
using Text = TMPro.TextMeshProUGUI;
using Ct = System.Threading.CancellationToken;

namespace Main.Handler
{
    internal sealed class ResultShower : MonoBehaviour
    {
        [SerializeField] private ResultBackgroundShaderController shaderController;
        [SerializeField] private RectTransform baseImageRt;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text rankingText;
        [SerializeField] private ASceneChangeButtonManager[] buttons;

        private void OnEnable()
        {
            baseImageRt.localPosition = new(0, 1000, 0);
            scoreText.text = string.Empty;
            rankingText.text = string.Empty;
            SetButtonsEnabled(false);
        }

        internal async UniTask Play(int correctAmount, int rank, bool hasForciblyCleared, Ct ct)
        {
            if (hasForciblyCleared)
            {
                scoreText.color = Color.yellow;
            }

            if (SaveDataHolder.CacheData.DoFastenDirections == false)
            {
                await 0.1f.SecAwait(ct: ct);
                await baseImageRt.DOAnchorPosY(-50, 0.5f).WithCancellation(ct);
                await 0.1f.SecAwait(ct: ct);

                await DOTween.To(
                    () => 0,
                    x => scoreText.text = x.ToString(),
                    correctAmount,
                    1.0f
                ).WithCancellation(ct);

                await 0.2f.SecAwait(ct: ct);
            }
            else
            {
                await baseImageRt.DOAnchorPosY(-50, 0.5f).WithCancellation(ct);
                scoreText.text = correctAmount.ToString();
            }

            if (rankingText.text != null)
            {
                if (rank <= 0)
                {
                    rankingText.text = "圏外";
                }
                else
                {
                    rankingText.text = $"{rank}位";

                    if (rank <= 3)
                    {
                        rankingText.color = Color.yellow;
                    }
                }
            }

            if (SaveDataHolder.CacheData.DoFastenDirections == false)
                await 0.5f.SecAwait(ct: ct);

            SetButtonsEnabled(true);

            if (shaderController != null)
                shaderController.Enabled = true;
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