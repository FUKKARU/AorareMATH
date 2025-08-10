namespace Main.Handler
{
    /// <summary>
    /// 数字のスプライトのプレハブにのみ、アタッチする
    /// </summary>
    internal sealed class SpriteAnimator : MonoBehaviour
    {
        [SerializeField] private SpriteFollow sprite;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private Vector3 initPosition; // local
        private Vector3 initScale; // local
        private bool hasAnimationDone = false;

        private void Awake()
        {
            // 初期位置とスケールを保存する
            if (sprite != null)
            {
                initPosition = sprite.transform.localPosition;
                initScale = sprite.transform.localScale;
            }

            // スクリプトを無効化する
            if (sprite != null)
                sprite.enabled = false;

            // 見えなくする
            if (spriteRenderer != null)
                spriteRenderer.enabled = false;
        }

        //! パネルに嵌め込んでおくスプライトは無効化しておいて、このメソッドで有効化する
        //! 最初の問題なら、アニメーションしてから有効化する
        internal void Enable(bool isFirstQuestion = false)
        {
            if (isFirstQuestion)
                DoAnimation(destroyCancellationToken).Forget();
            else
            {
                // スクリプトを有効化する
                if (sprite != null)
                    sprite.enabled = true;

                // 見えるようにする
                if (spriteRenderer != null)
                    spriteRenderer.enabled = true;
            }
        }

        // アニメーションを開始し、終わったらFollowスクリプトを有効にする
        private async UniTaskVoid DoAnimation(Ct ct)
        {
            if (hasAnimationDone) return;
            hasAnimationDone = true;

            // 見えるようにする
            if (spriteRenderer != null)
                spriteRenderer.enabled = true;

            // アニメーションの準備をする
            sprite.transform.localPosition += new Vector3(0, 1, 0);
            sprite.transform.localScale = Vector3.zero;

            {
                await UniTask.WaitForSeconds(0.2f, cancellationToken: ct);

                await sprite.transform.DOScale(initScale, 0.15f).SetEase(Ease.OutQuad).WithCancellation(ct);
                await UniTask.WaitForSeconds(0.12f, cancellationToken: ct);
                await sprite.transform.DOLocalMove(initPosition, 0.25f).SetEase(Ease.OutSine).WithCancellation(ct);

                await UniTask.WaitForSeconds(0.05f, cancellationToken: ct);
            }

            // 初期位置とスケールに戻す
            sprite.transform.localPosition = initPosition;
            sprite.transform.localScale = initScale;

            // スクリプトを有効化する
            if (sprite != null)
                sprite.enabled = true;
        }
    }
}