using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/SO_Scenery", fileName = "SO_Scenery")]
    public class SO_Scenery : ScriptableObject
    {
        #region
        public const string PATH = "SO_Scenery";

        private static SO_Scenery _entity = null;
        public static SO_Scenery Entity
        {
            get
            {
                if (_entity == null)
                {
                    _entity = Resources.Load<SO_Scenery>(PATH);

                    if (_entity == null)
                    {
                        Debug.LogError(PATH + " not found");
                    }
                }

                return _entity;
            }
        }
        #endregion

        [SerializeField] private SceneryElementProperty _whiteLineProperty;
        internal SceneryElementProperty WhiteLineProperty => _whiteLineProperty;

        [SerializeField] private SceneryElementProperty _buildingProperty;
        internal SceneryElementProperty BuildingProperty => _buildingProperty;
    }

    [System.Serializable]
    internal struct SceneryElementProperty
    {
        public float Interval;
        public float Duration;

        public Sprite Sprite;

        public Vector3 StartVelocity;
        public Vector3 StartPosition;
        public float StartLocalScale;
        public float VelocityCoefficient;
        public float ScaleCoefficient;
    }
}
