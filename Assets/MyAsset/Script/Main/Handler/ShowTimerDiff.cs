using General.Extension;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Main.Handler
{
    internal sealed class ShowTimerDiff : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI tmpro;

        private Vector3 initPosition;
        private Coroutine coroutine;

        private void OnDisable()
        {
            tmpro = null;
        }

        private void Start()
        {
            initPosition = tmpro.rectTransform.localPosition;
            Init();
        }

        private void Init()
        {
            tmpro.rectTransform.localPosition = initPosition;
            tmpro.text = "";
            tmpro.color = new(0, 0, 0, 0);
        }

        internal void PlayAnimation(float duration, float endPositionY, string text, Color color)
        {
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(PlayAnimationImpl(duration, endPositionY, text, color));
        }

        private IEnumerator PlayAnimationImpl(float duration, float endPositionY, string text, Color color)
        {
            Init();

            float t = 0;

            tmpro.text = text;
            tmpro.color = color;

            while (true)
            {
                Vector3 pos = tmpro.rectTransform.localPosition;
                pos.y = t.Remap(0, duration, initPosition.y, endPositionY);
                tmpro.rectTransform.localPosition = pos;

                Color col = tmpro.color;
                col.a = t.Remap(0, duration, 255, 0);
                tmpro.color = col;

                yield return null;
                t += Time.deltaTime;
                if (duration <= t) break;
            }

            Init();
        }
    }
}