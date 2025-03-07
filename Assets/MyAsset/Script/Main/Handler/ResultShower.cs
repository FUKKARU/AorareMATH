using Cysharp.Threading.Tasks;
using DG.Tweening;
using General.Extension;
using Main.Handler.ResultShowerNameSpace;
using System;
using System.Threading;
using TMPro;
using UnityEngine;

namespace Main.Handler
{
    namespace ResultShowerNameSpace
    {
        [Serializable]
        internal struct Value
        {
            [SerializeField] internal float BaseImageStartY;
            [SerializeField] internal float BaseImageEndY;
        }

        [Serializable]
        internal struct Duration
        {
            [SerializeField] internal float BeforeBaseImageMove;
            [SerializeField] internal float BaseImageMove;
            [SerializeField] internal float AfterBaseImageMoved;
            [SerializeField] internal float CorrectAmountTextCount;
            [SerializeField] internal float AfterCorrectAmountTextCounted;
            [SerializeField] internal float AfterGuideTextAppeared;
        }
    }

    internal sealed class ResultShower : MonoBehaviour
    {
        [SerializeField] private RectTransform baseImageRt;
        [SerializeField] private TextMeshProUGUI correctAmountText;
        [SerializeField] private GameObject guideText;
        [SerializeField] private Value value;
        [SerializeField] private Duration duration;

        private void OnEnable()
        {
            baseImageRt.localPosition = new(0, value.BaseImageStartY, 0);
            correctAmountText.text = string.Empty;
            guideText.SetActive(false);
        }

        internal async UniTask Play(int correctAmount, bool hasForciblyCleared, CancellationToken ct)
        {
            if (hasForciblyCleared) correctAmountText.color = Color.yellow;

            await duration.BeforeBaseImageMove.SecondsWait(ct);

            await baseImageRt.DOAnchorPosY(value.BaseImageEndY, duration.BaseImageMove).ConvertToUniTask(baseImageRt, ct);
            await duration.AfterBaseImageMoved.SecondsWait(ct);

            await ShowText(correctAmountText, correctAmount, duration.CorrectAmountTextCount, ct);
            await duration.AfterCorrectAmountTextCounted.SecondsWait(ct);

            guideText.SetActive(true);
            await duration.AfterGuideTextAppeared.SecondsWait(ct);
        }

        private async UniTask ShowText(TextMeshProUGUI tmpro, int num, float duration, CancellationToken ct)
        {
            float t = 0;
            while (true)
            {
                int n = (int)t.Remap(0, duration, 0, num);
                tmpro.text = n.ToString();

                await UniTask.Yield(cancellationToken: ct);
                t += Time.deltaTime;
                if (duration <= t)
                {
                    tmpro.text = num.ToString();
                    break;
                }
            }
        }
    }
}