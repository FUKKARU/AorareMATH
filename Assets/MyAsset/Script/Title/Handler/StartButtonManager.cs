using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using General;
using General.Extension;
using SO;
using Ct = System.Threading.CancellationToken;

namespace Title.Handler
{
    internal sealed class StartButtonManager : AButtonManager
    {
        [SerializeField] private Transform loadImageTf;
        [SerializeField] private BGMPlayer bgmPlayer;

        protected override void OnClickImpl() => Load(destroyCancellationToken).Forget();

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