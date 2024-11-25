using Cysharp.Threading.Tasks;
using DG.Tweening;
using General;
using SO;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Title.Handler
{
    internal sealed class ButtonManager : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer easyWithAssistStartButton;
        [SerializeField] private SpriteRenderer easyWithoutAssistStartButton;
        [SerializeField] private SpriteRenderer normalStartButton;

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
            if (!easyWithAssistStartButton) return;
            if (!easyWithoutAssistStartButton) return;
            if (!normalStartButton) return;
            if (!normalSprite) return;

            easyWithAssistStartButton.sprite = normalSprite;
            easyWithoutAssistStartButton.sprite = normalSprite;
            normalStartButton.sprite = normalSprite;

            ct = this.GetCancellationTokenOnDestroy();

            MakeThisToButton(easyWithAssistStartButton, Difficulty.Type.Assist1);
            MakeThisToButton(easyWithoutAssistStartButton, Difficulty.Type.Assist2);
            MakeThisToButton(normalStartButton, Difficulty.Type.Assist3);
        }

        private void OnDisable()
        {
            easyWithAssistStartButton = null;
            easyWithoutAssistStartButton = null;
            normalStartButton = null;

            normalSprite = null;
            hoverSprite = null;
            carTf = null;
            loadImageTf = null;
            onStartCarSEAudioSource = null;
            bgmPlayer = null;
        }

        private void MakeThisToButton(SpriteRenderer sr, Difficulty.Type difficultyType)
        {
            if (sr == null) return;
            EventTrigger et = sr.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry enter = new() { eventID = EventTriggerType.PointerEnter };
            enter.callback.AddListener(_ => OnEnter(sr));
            et.triggers.Add(enter);

            EventTrigger.Entry exit = new() { eventID = EventTriggerType.PointerExit };
            exit.callback.AddListener(_ => OnExit(sr));
            et.triggers.Add(exit);

            EventTrigger.Entry click = new() { eventID = EventTriggerType.PointerClick };
            click.callback.AddListener(_ => OnClick(difficultyType));
            et.triggers.Add(click);
        }

        private void OnEnter(SpriteRenderer sr)
        {
            if (!isActive) return;
            if (!sr) return;
            if (!hoverSprite) return;

            sr.sprite = hoverSprite;
        }

        private void OnExit(SpriteRenderer sr)
        {
            if (!isActive) return;
            if (!sr) return;
            if (!normalSprite) return;

            sr.sprite = normalSprite;
        }

        private void OnClick(Difficulty.Type difficultyType)
        {
            if (!isActive) return;

            isActive = false;

            Load(difficultyType, ct).Forget();
        }

        private void PlayOnStartCarSE() => onStartCarSEAudioSource.Raise(SO_Sound.Entity.EnemyCarSE, SoundType.SE);

        private async UniTask Load(Difficulty.Type difficultyType, CancellationToken ct)
        {
            Difficulty.Value = difficultyType;
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