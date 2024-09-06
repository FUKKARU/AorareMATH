using General.Extension;
using System;
using UnityEngine;
using UnityEngine.Serialization;

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

        [SerializeField, FormerlySerializedAs("_whiteLineProperty")] private SceneryElementProperty _whitelineProperty;
        internal SceneryElementProperty WhitelineProperty => _whitelineProperty;

        [SerializeField] private SceneryElementProperty _treeProperty;
        internal SceneryElementProperty TreeProperty => _treeProperty;

        [SerializeField] private SceneryElementProperty _poleProperty;
        internal SceneryElementProperty PoleProperty => _poleProperty;
    }

    [Serializable]
    internal sealed class SceneryElementProperty : IDisposable, INullExistable
    {
        [SerializeField, Header("このエレメントを表示するならチェック")] internal bool IsDisplay;

        [SerializeField] internal float Interval;
        [SerializeField] internal float Duration;

        [SerializeField] internal Sprite Sprite;

        [SerializeField] internal Vector3 StartVelocity;
        [SerializeField] internal Vector3 StartPosition;
        [SerializeField] internal float StartLocalScale;
        [SerializeField] internal float VelocityCoefficient;
        [SerializeField] internal float ScaleCoefficient;

        public void Dispose()
        {

        }

        public bool IsNullExist()
        {
            if (Sprite == null) return true;
            return false;
        }
    }
}