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

        [SerializeField, Header("ƒQ[ƒ€‚Ì‰Šú§ŒÀŽžŠÔ(•b)")] private float _initTimeLimit;
        internal float InitTimeLimt => _initTimeLimit;
    }
}