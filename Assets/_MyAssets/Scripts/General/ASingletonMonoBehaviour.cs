using UnityEngine;

namespace General
{
    public abstract class ASingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance = null;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<T>();

                    if (instance == null)
                    {
                        UnityEngine.Debug.LogError(typeof(T).Name + " not found");
                    }
                }

                return instance;
            }
        }
    }
}