using General;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/SO_GameState", fileName = "SO_GameState")]
    internal sealed class SO_GameState : AResourceLoadableScriptableObject<SO_GameState>
    {
        [SerializeField, Header("スタンドアローンのみ : 解像度(横×縦)")] private Vector2Int _resolution;
        internal Vector2Int Resolution => _resolution;

        [SerializeField, Header("スタンドアローンのみ : フルスクリーンにするか")] private bool _isFullScreen;
        internal bool IsFullScreen => _isFullScreen;

        [SerializeField, Header("ターゲットフレームレート")] private byte _targetFrameRate;
        internal byte TargetFrameRate => _targetFrameRate;
    }
}