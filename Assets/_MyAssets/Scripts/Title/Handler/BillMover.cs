using General;

namespace Title.Handler
{
    internal sealed class BillMover : MonoBehaviour
    {
        [SerializeField] private Transform billTf;
        [SerializeField] private Transform billTf_2;
        [SerializeField] private Transform billTf_3;
        [SerializeField] private Transform billTf_4;
        [SerializeField] private Transform billTf_5;

        [SerializeField] private Transform centerTf;

        [SerializeField] private AnimationCurve _customEasing;

        internal async UniTaskVoid Play(Ct ct)
        {
            float billMoveDelay = SaveDataHolder.CacheData.DoFastenDirections ? 0.0f : 2.6f;
            float billMoveDuration = SaveDataHolder.CacheData.DoFastenDirections ? 0.0f : 0.6f;
            float billRotateDelay = SaveDataHolder.CacheData.DoFastenDirections ? 0.0f : 0.8f;

            await billMoveDelay.SecAwait(ct: ct);
            await UniTask.WhenAll(
                billTf.DOLocalMoveY(49.2403f, duration: billMoveDuration).SetEase(_customEasing).WithCancellation(ct),
                billTf_2.DOLocalMoveY(50.0f, duration: billMoveDuration).SetEase(_customEasing).WithCancellation(ct),
                billTf_3.DOLocalMoveY(49.8097f, duration: billMoveDuration).SetEase(_customEasing).WithCancellation(ct),
                billTf_4.DOLocalMoveY(49.8097f, duration: billMoveDuration).SetEase(_customEasing).WithCancellation(ct),
                billTf_5.DOLocalMoveY(49.8097f, duration: billMoveDuration).SetEase(_customEasing).WithCancellation(ct)
            );
            await billRotateDelay.SecAwait(ct: ct);
            await centerTf.DOLocalRotate(new Vector3(0, 0, -60.0f), duration: 7.5f)
                .SetLoops(-1, LoopType.Incremental).SetEase(_customEasing)
                .WithCancellation(ct);
        }
    }
}