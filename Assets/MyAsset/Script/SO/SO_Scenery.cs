using General.Extension;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/SO_Scenery", fileName = "SO_Scenery")]
    public class SO_Scenery : AResourceLoadableScriptableObject<SO_Scenery>
    {
        [SerializeField, FormerlySerializedAs("_whiteLineProperty")] private SceneryElementProperty _whitelineProperty;
        internal SceneryElementProperty WhitelineProperty => _whitelineProperty;

        [SerializeField] private SceneryElementProperty _buildingsLeftProperty;
        internal SceneryElementProperty BuildingsLeftProperty => _buildingsLeftProperty;

        [SerializeField] private SceneryElementProperty _buildingsRightProperty;
        internal SceneryElementProperty BuildingsRightProperty => _buildingsRightProperty;
    }

    [Serializable]
    internal sealed class SceneryElementProperty
    {
        [SerializeField] internal float Interval;
        [SerializeField] internal float Duration;
        [SerializeField] internal float TimeOffset;

        [SerializeField] internal Sprite Sprite;

        [SerializeField] internal Vector3 StartVelocity;
        [SerializeField] internal Vector3 StartPosition;
        [SerializeField] internal float StartLocalScale;
        [SerializeField] internal float VelocityCoefficient;
        [SerializeField] internal float ScaleCoefficient;
    }
}