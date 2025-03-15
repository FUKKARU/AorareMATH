using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using General;
using General.Extension;
using SO;
using Ct = System.Threading.CancellationToken;
using Text = TMPro.TextMeshProUGUI;

namespace Title.Handler
{
    internal sealed class StartButtonManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private GameObject startButtonParent;
        [SerializeField] private Text text;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color hoverColor;

        [SerializeField] private Transform loadImageTf;
        [SerializeField] private BGMPlayer bgmPlayer;
        [SerializeField] private AudioSource clickSeAuidoSource;

        private bool hasClicked = false;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (hasClicked) return;
            text.color = hoverColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (hasClicked) return;
            text.color = normalColor;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (hasClicked) return;
            hasClicked = true;
            clickSeAuidoSource.Raise(SO_Sound.Entity.ClickSE, SoundType.SE);
            Load(destroyCancellationToken).Forget();
        }

        private async UniTaskVoid Load(Ct ct)
        {
            bgmPlayer.Fade();
            await UniTask.WaitForSeconds(0.2f, cancellationToken: ct);
            await loadImageTf.DOLocalMoveX(0, 0.5f).ConvertToUniTask(loadImageTf, ct);
            await UniTask.WaitForSeconds(1.5f, cancellationToken: ct);
            SO_SceneName.Entity.Main.LoadAsync().Forget();
        }
    }
}