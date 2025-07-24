using General.Shaders;

namespace Main.Handler
{
    /// <summary>
    /// ゲームロジックは止まらない
    /// 視覚的に、「ゲームが止まっているな〜」感を演出するためのクラス
    /// </summary>
    internal sealed class GamePauseActor : MonoBehaviour
    {
        [SerializeField] private PostProcessGrayscaleController grayscaleController;
        [SerializeField] private SceneryMover sceneryMover;

        private void UpdateVisual(bool becamePaused)
        {
            if (grayscaleController != null)
                grayscaleController.IsEnabled = becamePaused;

            if (sceneryMover != null)
                sceneryMover.IsPaused = becamePaused;
        }
    }
}