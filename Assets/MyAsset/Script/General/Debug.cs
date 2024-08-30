using System.Collections.Generic;

namespace General.Debug
{
    internal static class Debug
    {
#if UNITY_EDITOR && true
        internal static T Show<T>(this T val)
        {
            UnityEngine.Debug.Log(val);
            return val;
        }

        internal static T1 Show<T1, T2>(this T1 val, System.Func<T1, T2> f)
        {
            UnityEngine.Debug.Log(f(val));
            return val;
        }

        internal static IEnumerable<T> Look<T>(this IEnumerable<T> itr)
        {
            foreach (T e in itr)
            {
                UnityEngine.Debug.Log(e);
            }
            return itr;
        }

        internal static IEnumerable<T1> Look<T1, T2>(this IEnumerable<T1> itr, System.Func<T1, T2> f)
        {
            foreach (T1 e in itr)
            {
                UnityEngine.Debug.Log(f(e));
            }
            return itr;
        }
#endif
    }
}