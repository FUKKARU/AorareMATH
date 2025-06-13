using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Ct = System.Threading.CancellationToken;

namespace Main.Handler
{
    internal sealed class UnNumberSpritesAnimator : MonoBehaviour
    {
        [SerializeField, Tooltip("左のものから順にアタッチすること")] private UnNumberSpriteFollow[] sprites;

        private Vector3[] initPositions = null; // local
        private Vector3[] initScales = null; // local
        private int finishedAmount = 0; // アニメーションが終了したスプライトの数
        private bool hasDone = false;

        private void Start()
        {
            if (sprites == null || sprites.Length <= 0) return;

            initPositions = new Vector3[sprites.Length];
            initScales = new Vector3[sprites.Length];
            for (int i = 0; i < sprites.Length; ++i)
            {
                var sprite = sprites[i];
                if (sprite == null) continue;

                // スクリプトを無効化
                sprite.enabled = false;

                // 初期位置とスケールを保存し、アニメーションの準備をする
                initPositions[i] = sprite.transform.localPosition;
                initScales[i] = sprite.transform.localScale;
                sprite.transform.localPosition += new Vector3(0, 1, 0);
                sprite.transform.localScale = Vector3.zero;
            }
        }

        // アニメーションを開始し、終わったらFollowスクリプトを有効にする
        internal async UniTaskVoid BeginAnimation(Ct ct)
        {
            if (hasDone) return;
            hasDone = true;

            {
                await UniTask.WaitForSeconds(0.2f, cancellationToken: ct);

                // 少しずつずらしながら、各アニメーションを再生
                for (int i = 0; i < sprites.Length; ++i)
                {
                    var sprite = sprites[i];
                    if (sprite == null) continue;

                    DoEach(sprite, i, ct).Forget();
                    await UniTask.WaitForSeconds(0.08f, cancellationToken: ct);
                }

                // 全てのアニメーションが終了するのを待つ
                await UniTask.WaitUntil(() => finishedAmount >= sprites.Length, cancellationToken: ct);

                await UniTask.WaitForSeconds(0.05f, cancellationToken: ct);
            }

            // 終了処理 (スクリプトを有効化)
            for (int i = 0; i < sprites.Length; ++i)
            {
                var sprite = sprites[i];
                if (sprite == null) continue;

                sprite.enabled = true;
            }
        }

        private async UniTaskVoid DoEach(UnNumberSpriteFollow sprite, int index, Ct ct)
        {
            if (sprite == null) return;

            await sprite.transform.DOScale(initScales[index], 0.15f).SetEase(Ease.OutQuad).WithCancellation(ct);
            await UniTask.WaitForSeconds(0.12f, cancellationToken: ct);
            await sprite.transform.DOLocalMove(initPositions[index], 0.25f).SetEase(Ease.OutSine).WithCancellation(ct);

            // 初期位置とスケールに戻す
            sprite.transform.localPosition = initPositions[index];
            sprite.transform.localScale = initScales[index];

            ++finishedAmount;
        }
    }
}
