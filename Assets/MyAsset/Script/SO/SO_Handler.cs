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

        [SerializeField, Header("�{�^������������A�҂b��(�Ή�������̂̂�)")] private float _waitDurOnButtonPlaced;
        internal float WaitDurOnButtonPlaced => _waitDurOnButtonPlaced;

        [SerializeField, Header("���ʂ̍ŏ��l/�ő�l(db)")] private Vector2 _soundVolumeRange;
        internal float MinVolume => _soundVolumeRange.x;
        internal float MaxVolume => _soundVolumeRange.y;

        [SerializeField, Header("�Q�[���̏�����������(�b)")] private float _initTimeLimit;
        internal float InitTimeLimt => _initTimeLimit;
    }
}