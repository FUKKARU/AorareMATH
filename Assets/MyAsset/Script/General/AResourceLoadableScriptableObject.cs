using UnityEngine;

namespace General
{
    public abstract class AResourceLoadableScriptableObject<T> : ScriptableObject where T : ScriptableObject
    {
        private static readonly string Path = typeof(T).Name;

        private static T _entity = null;
        public static T Entity
        {
            get
            {
                if (_entity == null)
                {
                    _entity = Resources.Load<T>(Path);

                    if (_entity == null)
                    {
                        UnityEngine.Debug.LogError(Path + " not found");
                    }
                }

                return _entity;
            }
        }
    }
}