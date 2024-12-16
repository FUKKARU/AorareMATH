using System;
using System.Collections.ObjectModel;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;

namespace Title.Handler
{
    internal sealed class BillMover : MonoBehaviour
    {

        [SerializeField] private Transform BillTf;
        [SerializeField] private Transform BillTf_2;
        [SerializeField] private Transform BillTf_3;
        [SerializeField] private Transform BillTf_4;
        [SerializeField] private Transform BillTf_5;
        [SerializeField] private Transform BillTf_6;

        [SerializeField] private AnimationCurve _customEasing;
        [SerializeField] private AnimationCurve _customEasing_2;

        //private CancellationToken ct;
        private void OnEnable()
        {
            //ct = this.GetCancellationTokenOnDestroy();

            DOVirtual.DelayedCall(2.0f, () =>
            BillTf.DOLocalMoveX(20.0f, duration: 5.0f).SetEase(_customEasing).SetLoops(-1, LoopType.Restart)
            , false);
            
            DOVirtual.DelayedCall(2.83f, () =>
            BillTf_2.DOLocalMoveX(20.0f, duration: 5.0f).SetEase(_customEasing).SetLoops(-1, LoopType.Restart)
            , false);
            
            DOVirtual.DelayedCall(3.66f, () =>
            BillTf_3.DOLocalMoveX(20.0f, duration: 5.0f).SetEase(_customEasing).SetLoops(-1, LoopType.Restart)
            , false);
            
            DOVirtual.DelayedCall(4.49f, () =>
            BillTf_4.DOLocalMoveX(20.0f, duration: 5.0f).SetEase(_customEasing).SetLoops(-1, LoopType.Restart)
            , false);

            DOVirtual.DelayedCall(5.32f, () =>
            BillTf_5.DOLocalMoveX(20.0f, duration: 5.0f).SetEase(_customEasing).SetLoops(-1, LoopType.Restart)
            , false);

            DOVirtual.DelayedCall(6.15f, () =>
            BillTf_6.DOLocalMoveX(20.0f, duration: 5.0f).SetEase(_customEasing).SetLoops(-1, LoopType.Restart)
            , false);
        }
    }

}

