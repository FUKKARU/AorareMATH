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

        [SerializeField, Header("‰¹—Ê‚ÌÅ¬’l/Å‘å’l(db)")] private Vector2 _soundVolumeRange;
        internal float MinVolume => _soundVolumeRange.x;
        internal float MaxVolume => _soundVolumeRange.y;

        [SerializeField, Header("ƒQ[ƒ€‚Ì‰Šú§ŒÀŽžŠÔ(•b)")] private float _initTimeLimit;
        internal float InitTimeLimt => _initTimeLimit;

        [SerializeField, Header("Ž®‚Ì’l‚Ì·•ª‚ÆAŽžŠÔ‘‰Á—Ê‚ÌA‘Î‰žŠÖŒW\n(·•ªF0,1,2,3,4,...)")]
        private float[] _timeIncreaseAmountList;
        internal float[] TimeIncreaseAmountList => _timeIncreaseAmountList;

        [SerializeField, Header("UŒ‚¬Œ÷‚É‚È‚éA·•ª‚Ì‹«ŠE’l")] private float _diffLimit;
        internal float DiffLimit => _diffLimit;
    }
}