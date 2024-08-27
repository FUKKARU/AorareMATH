using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/SO_GameState", fileName = "SO_GameState")]
    public class SO_GameState : ScriptableObject
    {
        #region
        public const string PATH = "SO_GameState";

        private static SO_GameState _entity = null;
        public static SO_GameState Entity
        {
            get
            {
                if (_entity == null)
                {
                    _entity = Resources.Load<SO_GameState>(PATH);

                    if (_entity == null)
                    {
                        Debug.LogError(PATH + " not found");
                    }
                }

                return _entity;
            }
        }
        #endregion

        [Header("解像度(横×縦)")] public Vector2Int Resolution;
        [Header("フルスクリーンにするか")] public bool IsFullScreen;
        [Header("Vsyncをオンにするか")] public bool IsVsyncOn;
        [Header("Vsyncがオフの場合、ターゲットフレームレート")] public byte TargetFrameRate;
    }
}