﻿using System;
using UnityEngine;
using UnityEngine.Serialization;
using General;

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

        [SerializeField] private SceneryElementProperty _lampLeftProperty;
        internal SceneryElementProperty LampLeftProperty => _lampLeftProperty;

        [SerializeField] private SceneryElementProperty _lampRightProperty;
        internal SceneryElementProperty LampRightProperty => _lampRightProperty;
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