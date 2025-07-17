using UnityEngine;
using General;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/SO_GameState", fileName = "SO_GameState")]
    public class SO_GameState : AResourceLoadableScriptableObject<SO_GameState>
    {
        [Header("スタンドアローンのみ : 解像度(横×縦)")] public Vector2Int Resolution;
        [Header("スタンドアローンのみ : フルスクリーンにするか")] public bool IsFullScreen;
        [Header("ターゲットフレームレート")] public byte TargetFrameRate;
    }
}