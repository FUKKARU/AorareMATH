using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/SO_Handler", fileName = "SO_Handler")]
    public class SO_Handler : ScriptableObject
    {
        #region
        public const string PATH = "SO_Handler";

        private static SO_Handler _entity = null;
        public static SO_Handler Entity
        {
            get
            {
                if (_entity == null)
                {
                    _entity = Resources.Load<SO_Handler>(PATH);

                    if (_entity == null)
                    {
                        Debug.LogError(PATH + " not found");
                    }
                }

                return _entity;
            }
        }
        #endregion

        [SerializeField, Header("音量の最小値/最大値(db)")] private Vector2 _soundVolumeRange;
        internal float MinVolume => _soundVolumeRange.x;
        internal float MaxVolume => _soundVolumeRange.y;

        [SerializeField, Header("ゲームの初期制限時間(秒)")] private float _initTimeLimit;
        internal float InitTimeLimt => _initTimeLimit;

        [SerializeField, Header("式の値の差分と、時間増加量の、対応関係\n(差分：0,1,2,3,4,...)")]
        private float[] _timeIncreaseAmountList;
        internal float[] TimeIncreaseAmountList => _timeIncreaseAmountList;

        [SerializeField, Header("攻撃成功になる、差分の境界値")] private float _diffLimit;
        internal float DiffLimit => _diffLimit;
    }
}