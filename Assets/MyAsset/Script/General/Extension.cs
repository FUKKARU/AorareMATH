using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using Ct = System.Threading.CancellationToken;
using Cts = System.Threading.CancellationTokenSource;

namespace General.Extension
{
    internal enum ClientMode
    {
        Editor_Editing,
        Editor_Playing,
        Build
    }

    internal static class Extension
    {
        internal static ClientMode GetClientMode()
        {
#if UNITY_EDITOR
            return UnityEditor.EditorApplication.isPlaying ? ClientMode.Editor_Playing : ClientMode.Editor_Editing;
#else
            return ClientMode.Build;
#endif
        }

        internal static async UniTask SecondsWaitAndDo(this float waitSeconds, Action act, Ct ct)
        {
            await UniTask.WaitForSeconds(waitSeconds, cancellationToken: ct);
            act?.Invoke();
        }

        internal static async UniTask SecondsWait(this float waitSeconds, Ct ct)
        {
            await UniTask.WaitForSeconds(waitSeconds, cancellationToken: ct);
        }

        /// <summary>
        /// EventTriggerにイベントを登録する
        /// </summary>
        internal static void AddListener(this EventTrigger eventTrigger, EventTriggerType type, Action action)
        {
            EventTrigger.Entry entry = new() { eventID = type };
            entry.callback.AddListener(_ => action?.Invoke());
            eventTrigger.triggers.Add(entry);
        }

        internal static float Remap(this float x, float a, float b, float c, float d) => (x - a) * (d - c) / (b - a) + c;

        internal static bool IsClose(this float a, float b, float ofst = float.Epsilon) => MathF.Abs(a - b) < ofst;

        internal static bool IsIn(this int val, int min, int max, int ofst = default)
            => min + ofst <= val && val <= max + ofst;
        internal static bool IsIn(this float val, float min, float max, float ofst = default)
            => min + ofst <= val && val <= max + ofst;
        internal static bool IsIn(this Vector2 v, float sx, float ex, float sy, float ey, Vector2 ofst = default)
            => v.x.IsIn(sx, ex, ofst.x) && v.y.IsIn(sy, ey, ofst.y);

        internal static Vector2 ToVector2(this Vector3 v) => new(v.x, v.y);
        internal static Vector3 ToVector3(this Vector2 v, float z) => new(v.x, v.y, z);

        internal static void Pass() { }

        internal static void SetPositionX(this Transform tf, float x)
        {
            Vector3 pos = tf.position;
            pos.x = x;
            tf.position = pos;
        }
        internal static void SetPositionY(this Transform tf, float y)
        {
            Vector3 pos = tf.position;
            pos.y = y;
            tf.position = pos;
        }

        internal static UniTask ConvertToUniTask(this Tween tween, MonoBehaviour targetObject, Ct ct)
        {
            Cts cts = Cts.CreateLinkedTokenSource(ct, targetObject.GetCancellationTokenOnDestroy());
            return tween.ToUniTask(cancellationToken: cts.Token);
        }

        internal static UniTask ConvertToUniTask(this Tween tween, GameObject targetObject, Ct ct)
        {
            Cts cts = Cts.CreateLinkedTokenSource(ct, targetObject.GetCancellationTokenOnDestroy());
            return tween.ToUniTask(cancellationToken: cts.Token);
        }

        internal static UniTask ConvertToUniTask(this Tween tween, Component targetObject, Ct ct)
        {
            Cts cts = Cts.CreateLinkedTokenSource(ct, targetObject.GetCancellationTokenOnDestroy());
            return tween.ToUniTask(cancellationToken: cts.Token);
        }

        internal static void Bind(this EventTrigger eventTrigger, EventTriggerType eventTriggerType, Action action)
        {
            if (eventTrigger == null) return;

            var entry = new EventTrigger.Entry { eventID = eventTriggerType };
            entry.callback.AddListener(_ => action?.Invoke());
            eventTrigger.triggers.Add(entry);
        }

        /// <summary>
        /// キャンセル不可
        /// </summary>
        internal static async UniTaskVoid LoadAsync(this string sceneName)
        {
            if (isSceneLoading) return;
            if (string.IsNullOrEmpty(sceneName)) return;

            isSceneLoading = true;
            var opr = SceneManager.LoadSceneAsync(sceneName);
            opr.allowSceneActivation = false;
            await UniTask.WaitUntil(() => opr.progress >= 0.9f);
            opr.allowSceneActivation = true;
            await UniTask.WaitUntil(() => opr.isDone);
            isSceneLoading = false;
        }
        private static bool isSceneLoading = false;
    }

    internal static class IteratorExtension
    {
        internal static bool All<T>(this T val, params Func<T, bool>[] functions)
        {
            foreach (var f in functions)
            {
                if (!f(val)) return false;
            }
            return true;
        }

        internal static bool Any<T>(this T val, params Func<T, bool>[] functions)
        {
            foreach (var f in functions)
            {
                if (f(val)) return true;
            }
            return false;
        }

        internal static (T Element, int Index, bool IsFound) Find<T>(this IEnumerable<T> itr, Func<T, bool> f)
        {
            int i = 0;
            foreach (T e in itr)
            {
                if (f(e)) return (e, i, true);
                i++;
            }

            return (default, -1, false);
        }

        internal static IEnumerable<(int Index, T Element)> Enumerate<T>(this IEnumerable<T> itr)
        {
            int i = 0;
            foreach (T e in itr)
            {
                yield return (i, e);
                i++;
            }
        }

        internal static void ShuffleSelf<T>(this T[] array)
        {
            if (array == null) return;
            int n = array.Length;
            if (n <= 0) return;
            for (int i = n - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                T tmp = array[i];
                array[i] = array[j];
                array[j] = tmp;
            }
        }
    }
}