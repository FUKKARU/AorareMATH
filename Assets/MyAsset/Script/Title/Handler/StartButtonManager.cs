using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using General.Extension;
using SO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Title.Handler
{
    internal sealed class StartButtonManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private GameObject startButtonParent;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color hoverColor;

        [SerializeField] private Transform loadImageTf;
        [SerializeField] private BGMPlayer bgmPlayer;

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
            Load(destroyCancellationToken).Forget();
        }

        private async UniTaskVoid Load(CancellationToken ct)
        {
            if (startButtonParent != null) startButtonParent.SetActive(false);

            bgmPlayer.Fade();
            await UniTask.WaitForSeconds(0.2f, cancellationToken: ct);
            await loadImageTf.DOLocalMoveX(0, 0.5f).ConvertToUniTask(loadImageTf, ct);
            await UniTask.WaitForSeconds(0.2f, cancellationToken: ct);
            SO_SceneName.Entity.Main.LoadAsync().Forget();
        }
    }
}