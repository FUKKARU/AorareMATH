using DG.Tweening;
using UnityEngine;

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


        internal void Play()
        {
            try
            {
                DOVirtual.DelayedCall(2.6f, () => billTf.DOLocalMoveY(49.2403f, duration: 0.6f).SetEase(_customEasing), false);
                DOVirtual.DelayedCall(2.6f, () => billTf_2.DOLocalMoveY(50.0f, duration: 0.6f).SetEase(_customEasing), false);
                DOVirtual.DelayedCall(2.6f, () => billTf_3.DOLocalMoveY(49.2403f, duration: 0.6f).SetEase(_customEasing), false);
                DOVirtual.DelayedCall(2.6f, () => billTf_4.DOLocalMoveY(49.8097f, duration: 0.6f).SetEase(_customEasing), false);
                DOVirtual.DelayedCall(2.6f, () => billTf_5.DOLocalMoveY(49.8097f, duration: 0.6f).SetEase(_customEasing), false);

                DOVirtual.DelayedCall(3.4f, () => centerTf.DOLocalRotate(new Vector3(0, 0, -60.0f), duration: 7.5f).SetLoops(-1, LoopType.Incremental).SetEase(_customEasing), false);
            }
            catch { }
        }
    }
}
