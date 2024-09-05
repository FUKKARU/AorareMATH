using Cysharp.Threading.Tasks;
using DG.Tweening;
using General;
using SO;
using System;
using System.Threading;
using UnityEngine;

namespace Main.Handler
{
    [Serializable]
    internal struct Base
    {
        [SerializeField] internal Transform Tf;
        [SerializeField] internal float StartY;
        [SerializeField] internal float EndY;
        [SerializeField] internal float Duration;
    }

    [Serializable]
    internal struct Sr
    {
        [SerializeField] internal SpriteRenderer Green;
        [SerializeField] internal SpriteRenderer Yellow;
        [SerializeField] internal SpriteRenderer Red;
    }

    [Serializable]
    internal struct Sp
    {
        [SerializeField] internal Sprite Green;
        [SerializeField] internal Sprite Yellow;
        [SerializeField] internal Sprite Red;
    }

    internal sealed class CountDown : MonoBehaviour
    {
        [SerializeField] private Base base_;
        [SerializeField] private Sr sr;
        [SerializeField] private Sp sp;
        [SerializeField] private AudioSource as_;

        private void OnEnable()
        {
            sr.Green.sprite = sp.Green;
            sr.Yellow.sprite = sp.Yellow;
            sr.Red.sprite = sp.Red;

            sr.Green.enabled = false;
            sr.Yellow.enabled = false;
            sr.Red.enabled = false;

            Vector3 pos = base_.Tf.position;
            pos.y = base_.StartY;
            base_.Tf.position = pos;
        }

        private void OnDisable()
        {
            as_ = null;
        }

        internal async UniTask Play(CancellationToken ct)
        {
            if (as_ != null) as_.Raise(SO_Sound.Entity.CountDownSE, SoundType.SE);

            sr.Red.enabled = true;
            await WaitForASecond(ct);

            sr.Yellow.enabled = true;
            await WaitForASecond(ct);

            sr.Green.enabled = true;
            await WaitForASecond(ct);

            sr.Red.sprite = sr.Green.sprite;
            sr.Yellow.sprite = sr.Green.sprite;
            await WaitForASecond(ct);

            await base_.Tf.DOLocalMoveY(base_.EndY, base_.Duration).ToUniTask(cancellationToken: ct);
        }

        private async UniTask WaitForASecond(CancellationToken ct)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: ct);
        }
    }
}