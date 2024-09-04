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
            [SerializeField] internal float TotalNumTextCount;
            [SerializeField] internal float AfterTotalNumTextCounted;
            [SerializeField] internal float JustNumTextCount;
            [SerializeField] internal float AfterJustNumTextCounted;
            [SerializeField] internal float AfterGuideTextAppeared;
        }
    }

    internal sealed class ResultShower : MonoBehaviour
    {
        [SerializeField] private RectTransform baseImageRt;
        [SerializeField] private TextMeshProUGUI totalNumText;
        [SerializeField] private TextMeshProUGUI justNumText;
        [SerializeField] private GameObject guideText;
        [SerializeField] private Value value;
        [SerializeField] private Duration duration;

        private void OnEnable()
        {
            baseImageRt.localPosition = new(0, value.BaseImageStartY, 0);
            totalNumText.text = "";
            justNumText.text = "";
            guideText.SetActive(false);
        }

        private void OnDisable()
        {
            baseImageRt = null;
            totalNumText = null;
            justNumText = null;
            guideText = null;
        }

        internal async UniTask Play(int totalNum, int justNum, CancellationToken ct)
        {
            await duration.BeforeBaseImageMove.SecondsWait(ct);

            await baseImageRt.DOAnchorPosY(value.BaseImageEndY, duration.BaseImageMove).ToUniTask(cancellationToken: ct);
            await duration.AfterBaseImageMoved.SecondsWait(ct);

            await ShowText(totalNumText, totalNum, duration.TotalNumTextCount, ct);
            await duration.AfterJustNumTextCounted.SecondsWait(ct);
            await ShowText(justNumText, justNum, duration.JustNumTextCount, ct);
            await duration.AfterJustNumTextCounted.SecondsWait(ct);

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
                if (duration <= t) break;
            }
        }
    }
}