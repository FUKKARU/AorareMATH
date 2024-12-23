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

        [SerializeField] private Transform billTf;
        [SerializeField] private Transform billTf_2;
        [SerializeField] private Transform billTf_3;
        [SerializeField] private Transform billTf_4;
        [SerializeField] private Transform billTf_5;
        [SerializeField] private Transform billTf_6;
        [SerializeField] private Transform billTf_7;
        [SerializeField] private Transform billTf_8;
        [SerializeField] private Transform billTf_9;
        [SerializeField] private Transform billTf_10;

        [SerializeField] private AnimationCurve _customEasing;
        [SerializeField] private AnimationCurve _customEasing_2;
        private void OnEnable()
        {
            /// ビルの初期配置
            DOVirtual.DelayedCall(2.0f, () =>
            billTf.DOLocalMove(new Vector3(-7.0f,1.5f,105), duration: 1.0f).SetEase(_customEasing)
            , false);
            DOVirtual.DelayedCall(2.0f, () =>
            billTf.DOScale(new Vector2(1.0f,1.0f), duration: 1.0f).SetEase(_customEasing)
            , false);
            DOVirtual.DelayedCall(2.0f, () =>
            billTf.DOLocalRotate(new Vector3(0, 0, 55), 1.0f).SetEase(_customEasing), false);

            DOVirtual.DelayedCall(2.0f, () =>
            billTf_2.DOLocalMove(new Vector3(-2.3f, 2.0f, 105), duration: 1.0f).SetEase(_customEasing)
            , false);
            DOVirtual.DelayedCall(2.0f, () =>
            billTf_2.DOScale(new Vector2(1.0f, 1.0f), duration: 1.0f).SetEase(_customEasing)
            , false);
            DOVirtual.DelayedCall(2.0f, () =>
            billTf_2.DOLocalRotate(new Vector3(0, 0, 12), 1.0f).SetEase(_customEasing), false);

            DOVirtual.DelayedCall(2.0f, () =>
            billTf_3.DOLocalMove(new Vector3(2.3f, 2.0f, 105), duration: 1.0f).SetEase(_customEasing)
            , false);
            DOVirtual.DelayedCall(2.0f, () =>
            billTf_3.DOScale(new Vector2(1.0f, 1.0f), duration: 1.0f).SetEase(_customEasing)
            , false);
            DOVirtual.DelayedCall(2.0f, () =>
            billTf_3.DOLocalRotate(new Vector3(0, 0, -12), 1.0f).SetEase(_customEasing), false);

            DOVirtual.DelayedCall(2.0f, () =>
            billTf_4.DOLocalMove(new Vector3(7.0f, 1.5f, 105), duration: 1.0f).SetEase(_customEasing)
            , false);
            DOVirtual.DelayedCall(2.0f, () =>
            billTf_4.DOScale(new Vector2(1.0f, 1.0f), duration: 1.0f).SetEase(_customEasing)
            , false);
            DOVirtual.DelayedCall(2.0f, () =>
            billTf_4.DOLocalRotate(new Vector3(0, 0, -55), 1.0f).SetEase(_customEasing), false);

            /// ビルのフェードアウト
            DOVirtual.DelayedCall(3.0f, () =>
            billTf.DOLocalPath(
            new[]
            {
            new Vector3(0f, 2.0f,105),
            new Vector3(9.0f, 0.2f,105),
            new Vector3(12, -2.0f,105),
            },
            4f, PathType.CatmullRom).SetEase(_customEasing), false);
            DOVirtual.DelayedCall(3.0f, () =>
            billTf.DOLocalRotate(new Vector3(0, 0, -90), 4.0f).SetEase(_customEasing), false);

            DOVirtual.DelayedCall(3.0f, () =>
            billTf_2.DOLocalPath(
            new[]
            {
            new Vector3(0f, 2.0f,105),
            new Vector3(9.0f, 0.2f,105),
            new Vector3(12, -2.0f,105),
            },
            3f, PathType.CatmullRom).SetEase(_customEasing), false);
            DOVirtual.DelayedCall(3.0f, () =>
            billTf_2.DOLocalRotate(new Vector3(0, 0, -90), 3.0f).SetEase(_customEasing), false);

            DOVirtual.DelayedCall(3.0f, () =>
            billTf_3.DOLocalPath(
            new[]
            {
            new Vector3(9.0f, 0.2f,105),
            new Vector3(12, -2.0f,105),
            },
            2f, PathType.CatmullRom).SetEase(_customEasing)
            , false);
            DOVirtual.DelayedCall(3.0f, () =>
            billTf_3.DOLocalRotate(new Vector3(0, 0, -90), 2.0f).SetEase(_customEasing), false);

            DOVirtual.DelayedCall(3.0f, () =>
            billTf_4.DOLocalPath(
            new[]
            {
            new Vector3(9.0f, 0.2f,105),
            new Vector3(12, -2.0f,105),
            },
            1f, PathType.CatmullRom).SetEase(_customEasing)
            , false);
            DOVirtual.DelayedCall(3.0f, () =>
            billTf_4.DOLocalRotate(new Vector3(0, 0, -90), 1.0f).SetEase(_customEasing), false);

            /// ビルのループ移動
            DOVirtual.DelayedCall(3f, () =>
            billTf_5.DOLocalPath(
            new[]
            {
            new Vector3(-6.5f,1.8f,105),
            new Vector3(0f, 2.5f,105),
            new Vector3(6.5f,1.8f,105),
            new Vector3(12, -2.0f,105),
            },
            5.0f, PathType.CatmullRom).SetEase(_customEasing).SetLoops(-1, LoopType.Restart), false);

            DOVirtual.DelayedCall(3f, () =>
            billTf_5.DOLocalRotate(new Vector3(0,0,-90),5.0f).SetEase(_customEasing).SetLoops(-1, LoopType.Restart), false);


            DOVirtual.DelayedCall(4f, () =>
            billTf_6.DOLocalPath(
            new[]
            {
            new Vector3(-6.5f,1.8f,105),
            new Vector3(0f, 2.5f,105),
            new Vector3(6.5f,1.8f,105),
            new Vector3(12, -2.0f,105),
            },
            5.0f, PathType.CatmullRom).SetEase(_customEasing).SetLoops(-1, LoopType.Restart), false);

            DOVirtual.DelayedCall(4f, () =>
            billTf_6.DOLocalRotate(new Vector3(0, 0, -90), 5.0f).SetEase(_customEasing).SetLoops(-1, LoopType.Restart), false);


            DOVirtual.DelayedCall(5f, () =>
            billTf_7.DOLocalPath(
            new[]
            {
            new Vector3(-6.5f,1.8f,105),
            new Vector3(0f, 2.5f,105),
            new Vector3(6.5f,1.8f,105),
            new Vector3(12, -2.0f,105),
            },
            5.0f, PathType.CatmullRom).SetEase(_customEasing).SetLoops(-1, LoopType.Restart), false);

            DOVirtual.DelayedCall(5f, () =>
            billTf_7.DOLocalRotate(new Vector3(0, 0, -90), 5.0f).SetEase(_customEasing).SetLoops(-1, LoopType.Restart), false);

            DOVirtual.DelayedCall(6f, () =>
            billTf_8.DOLocalPath(
            new[]
            {
            new Vector3(-6.5f,1.8f,105),
            new Vector3(0f, 2.5f,105),
            new Vector3(6.5f,1.8f,105),
            new Vector3(12, -2.0f,105),
            },
            5.0f, PathType.CatmullRom).SetEase(_customEasing).SetLoops(-1, LoopType.Restart), false);

            DOVirtual.DelayedCall(6f, () =>
            billTf_8.DOLocalRotate(new Vector3(0, 0, -90), 5.0f).SetEase(_customEasing).SetLoops(-1, LoopType.Restart), false);

            DOVirtual.DelayedCall(7f, () =>
            billTf_9.DOLocalPath(
            new[]
            {
            new Vector3(-6.5f,1.8f,105),
            new Vector3(0f, 2.5f,105),
            new Vector3(6.5f,1.8f,105),
            new Vector3(12, -2.0f,105),
            },
            5.0f, PathType.CatmullRom).SetEase(_customEasing).SetLoops(-1, LoopType.Restart), false);

            DOVirtual.DelayedCall(7f, () =>
            billTf_9.DOLocalRotate(new Vector3(0, 0, -90), 5.0f).SetEase(_customEasing).SetLoops(-1, LoopType.Restart), false);
            /*
            DOVirtual.DelayedCall(6.95f, () =>
            BillTf_10.DOLocalPath(
            new[]
            {
            new Vector3(0f, 2.0f,105),
            new Vector3(11, -1.0f,105),
            },
            5.0f, PathType.CatmullRom).SetEase(_customEasing).SetLoops(-1, LoopType.Restart), false);
            */
        }
    }

}

