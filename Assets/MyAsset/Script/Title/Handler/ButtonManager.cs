using Cysharp.Threading.Tasks;
using DG.Tweening;
using General;
using SO;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Title.Handler
{
    internal sealed class ButtonManager : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer sr;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite hoverSprite;
        [SerializeField] private Transform carTf;
        [SerializeField] private Transform loadImageTf;
        [SerializeField] private AudioSource onStartCarSEAudioSource;
        [SerializeField] private BGMPlayer bgmPlayer;
        [SerializeField] private EndValue endValue;
        [SerializeField] private Duration duration;

        private bool isActive = true;

        private CancellationToken ct;

        private void OnEnable()
        {
            if (!sr) return;
            if (!normalSprite) return;

            sr.sprite = normalSprite;

            ct = this.GetCancellationTokenOnDestroy();
        }

        private void OnDisable()
        {
            sr = null;
            normalSprite = null;
            hoverSprite = null;
            carTf = null;
            loadImageTf = null;
            onStartCarSEAudioSource = null;
            bgmPlayer = null;
        }

        public void OnEnter()
        {
            if (!isActive) return;
            if (!sr) return;
            if (!hoverSprite) return;

            sr.sprite = hoverSprite;
        }

        public void OnExit()
        {
            if (!isActive) return;
            if (!sr) return;
            if (!normalSprite) return;

            sr.sprite = normalSprite;
        }

        public void OnClick()
        {
            if (!isActive) return;

            isActive = false;

            Load(ct).Forget();
        }

        private void PlayOnStartCarSE() => onStartCarSEAudioSource.Raise(SO_Sound.Entity.EnemyCarSE, SoundType.SE);

        private async UniTask Load(CancellationToken ct)
        {
            await Direction(endValue, duration, ct);
            await SceneManager.LoadSceneAsync(SO_SceneName.Entity.Main).ToUniTask(cancellationToken: ct);
        }

        private async UniTask Direction
            (EndValue endValue, Duration duration, CancellationToken ct)
        {
            PlayOnStartCarSE();
            bgmPlayer.AudioSource.DOFade(endValue.BGMVolume, duration.BGMFade).ToUniTask(cancellationToken: ct).Forget();
            carTf.DOLocalMoveX(endValue.CarX, duration.CarMove).ToUniTask(cancellationToken: ct).Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(duration.UntilLoadImageMove), cancellationToken: ct);
            await loadImageTf.DOLocalMoveX(endValue.LoadImageX, duration.LoadImageMove).ToUniTask(cancellationToken: ct);
            await UniTask.Delay(TimeSpan.FromSeconds(duration.AfterLoadImageMoved), cancellationToken: ct);
        }

        [Serializable]
        internal struct EndValue
        {
            [SerializeField] internal float CarX;
            [SerializeField] internal float LoadImageX;
            [SerializeField] internal float BGMVolume;
        }

        [Serializable]
        internal struct Duration
        {
            [SerializeField] internal float BGMFade;
            [SerializeField] internal float CarMove;
            [SerializeField] internal float LoadImageMove;
            [SerializeField] internal float UntilLoadImageMove;
            [SerializeField] internal float AfterLoadImageMoved;
        }
    }
}