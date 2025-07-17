using UnityEngine;
using Cysharp.Threading.Tasks;
using Text = TMPro.TextMeshProUGUI;
using Ct = System.Threading.CancellationToken;
using DG.Tweening;

namespace Main.Handler
{
    internal sealed class UntilResultCountDown : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Text text;
        [SerializeField, Range(1.0f, 20.0f)] private float durationUntilResult;

        private bool hasDone = false;

        private float _remainTime = 0;
        private float remainTime
        {
            get => _remainTime;
            set
            {
                _remainTime = Mathf.Clamp(value, 0, 99);
                if (text != null)
                    text.text = $"ゲーム終了！  リザルトまで...  {(int)_remainTime}";
            }
        }

        private bool _isActive = false;
        private bool isActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                if (root != null)
                    root.SetActive(_isActive);
            }
        }

        private void Start()
        {
            isActive = false;
            remainTime = durationUntilResult;
        }

        internal async UniTask BeginCountDown(Ct ct)
        {
            if (hasDone) return;
            hasDone = true;

            isActive = true;

            await DOTween.To(() => remainTime, x => remainTime = x, 0, durationUntilResult)
                .SetEase(Ease.Linear)
                .SetUpdate(true)
                .WithCancellation(ct);

            isActive = false;
        }
    }
}
