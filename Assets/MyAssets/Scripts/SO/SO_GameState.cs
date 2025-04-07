using UnityEngine;
using General;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/SO_GameState", fileName = "SO_GameState")]
    public class SO_GameState : AResourceLoadableScriptableObject<SO_GameState>
    {
        [Header("解像度(横×縦)")] public Vector2Int Resolution;
        [Header("フルスクリーンにするか")] public bool IsFullScreen;
        [Header("Vsyncをオンにするか")] public bool IsVsyncOn;
        [Header("Vsyncがオフの場合、ターゲットフレームレート")] public byte TargetFrameRate;
    }
}