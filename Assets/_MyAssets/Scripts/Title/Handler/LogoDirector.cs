using General;

namespace Title.Handler
{
    internal sealed class LogoDirector : MonoBehaviour
    {
        [SerializeField] private GameObject startButton;
        [SerializeField] private GameObject menuButton;
        [SerializeField] private Transform logoTf;
        [SerializeField] private BillMover billMover;

        private void Start() => DoLogo(destroyCancellationToken).Forget();

        private async UniTask DoLogo(Ct ct)
        {
            if (startButton != null) startButton.SetActive(false);
            if (menuButton != null) menuButton.SetActive(false);

            if (billMover != null) billMover.Play(destroyCancellationToken).Forget();

            if (SaveDataHolder.Data.DoFastenDirections == false)
            {
                logoTf.DOLocalMoveY(1.15f, duration: 1.2f).SetEase(Ease.OutExpo).WithCancellation(ct).Forget();
                logoTf.DOScale(new Vector2(0.8f, 0.8f), duration: 2.0f).SetEase(Ease.InOutExpo).WithCancellation(ct).Forget();
                await UniTask.WaitForSeconds(1.8f, cancellationToken: ct);
            }
            else
            {
                logoTf.SetLocalPosY(1.15f);
                logoTf.SetScaleXY(0.8f, 0.8f);
            }

            if (startButton != null) startButton.SetActive(true);
            if (menuButton != null) menuButton.SetActive(true);
        }
    }
}