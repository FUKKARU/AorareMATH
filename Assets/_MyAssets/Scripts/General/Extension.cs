using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using Random = UnityEngine.Random;
using Text = TMPro.TextMeshProUGUI;
using Ct = System.Threading.CancellationToken;

namespace General.Extension
{
    internal static class Extension
    {
        internal static async UniTask SecAwait(
            this float sec,
            bool ignoreTimeScale = false,
            PlayerLoopTiming timing = PlayerLoopTiming.Update,
            Ct ct = default,
            bool cancelImmediately = false
        ) => await UniTask.WaitForSeconds(sec, ignoreTimeScale, timing, ct, cancelImmediately);
        internal static async UniTask SecAwaitThenDo(
            this float sec,
            Action act,
            bool ignoreTimeScale = false,
            PlayerLoopTiming timing = PlayerLoopTiming.Update,
            Ct ct = default,
            bool cancelImmediately = false
        )
        {
            await sec.SecAwait(ignoreTimeScale, timing, ct, cancelImmediately);
            act?.Invoke();
        }
        internal static async UniTask SecAwaitThenAwait(
            this float sec,
            Func<Ct, UniTask> task,
            bool ignoreTimeScale = false,
            PlayerLoopTiming timing = PlayerLoopTiming.Update,
            Ct ct = default,
            bool cancelImmediately = false
        )
        {
            await sec.SecAwait(ignoreTimeScale, timing, ct, cancelImmediately);
            if (task != null)
                await task(ct);
        }

        /// <summary>
        /// EventTriggerにイベントを登録する
        /// </summary>
        internal static void AddListener(this EventTrigger eventTrigger, EventTriggerType type, Action<PointerEventData> action)
        {
            EventTrigger.Entry entry = new() { eventID = type };
            entry.callback.AddListener(data =>
            {
                if (data is PointerEventData pointerData)
                    action?.Invoke(pointerData);
            });
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

        internal static void SetPosX(this Transform tf, float x)
        {
            Vector3 pos = tf.position;
            pos.x = x;
            tf.position = pos;
        }
        internal static void SetPosY(this Transform tf, float y)
        {
            Vector3 pos = tf.position;
            pos.y = y;
            tf.position = pos;
        }
        internal static void SetPosZ(this Transform tf, float z)
        {
            Vector3 pos = tf.position;
            pos.z = z;
            tf.position = pos;
        }
        internal static void SetPosXY(this Transform tf, float x, float y)
        {
            Vector3 pos = tf.position;
            pos.x = x;
            pos.y = y;
            tf.position = pos;
        }
        internal static void SetPosXZ(this Transform tf, float x, float z)
        {
            Vector3 pos = tf.position;
            pos.x = x;
            pos.z = z;
            tf.position = pos;
        }
        internal static void SetPosYZ(this Transform tf, float y, float z)
        {
            Vector3 pos = tf.position;
            pos.y = y;
            pos.z = z;
            tf.position = pos;
        }
        internal static void SetPosXYZ(this Transform tf, float x, float y, float z)
        {
            Vector3 pos = tf.position;
            pos.x = x;
            pos.y = y;
            pos.z = z;
            tf.position = pos;
        }
        internal static void SetLocalPosX(this Transform tf, float x)
        {
            Vector3 pos = tf.localPosition;
            pos.x = x;
            tf.localPosition = pos;
        }
        internal static void SetLocalPosY(this Transform tf, float y)
        {
            Vector3 pos = tf.localPosition;
            pos.y = y;
            tf.localPosition = pos;
        }
        internal static void SetLocalPosZ(this Transform tf, float z)
        {
            Vector3 pos = tf.localPosition;
            pos.z = z;
            tf.localPosition = pos;
        }
        internal static void SetLocalPosXY(this Transform tf, float x, float y)
        {
            Vector3 pos = tf.localPosition;
            pos.x = x;
            pos.y = y;
            tf.localPosition = pos;
        }
        internal static void SetLocalPosXZ(this Transform tf, float x, float z)
        {
            Vector3 pos = tf.localPosition;
            pos.x = x;
            pos.z = z;
            tf.localPosition = pos;
        }
        internal static void SetLocalPosYZ(this Transform tf, float y, float z)
        {
            Vector3 pos = tf.localPosition;
            pos.y = y;
            pos.z = z;
            tf.localPosition = pos;
        }
        internal static void SetLocalPosXYZ(this Transform tf, float x, float y, float z)
        {
            Vector3 pos = tf.localPosition;
            pos.x = x;
            pos.y = y;
            pos.z = z;
            tf.localPosition = pos;
        }
        internal static void SetRotX(this Transform tf, float x)
        {
            Vector3 rot = tf.eulerAngles;
            rot.x = x;
            tf.eulerAngles = rot;
        }
        internal static void SetRotY(this Transform tf, float y)
        {
            Vector3 rot = tf.eulerAngles;
            rot.y = y;
            tf.eulerAngles = rot;
        }
        internal static void SetRotZ(this Transform tf, float z)
        {
            Vector3 rot = tf.eulerAngles;
            rot.z = z;
            tf.eulerAngles = rot;
        }
        internal static void SetRotXY(this Transform tf, float x, float y)
        {
            Vector3 rot = tf.eulerAngles;
            rot.x = x;
            rot.y = y;
            tf.eulerAngles = rot;
        }
        internal static void SetRotXZ(this Transform tf, float x, float z)
        {
            Vector3 rot = tf.eulerAngles;
            rot.x = x;
            rot.z = z;
            tf.eulerAngles = rot;
        }
        internal static void SetRotYZ(this Transform tf, float y, float z)
        {
            Vector3 rot = tf.eulerAngles;
            rot.y = y;
            rot.z = z;
            tf.eulerAngles = rot;
        }
        internal static void SetRotXYZ(this Transform tf, float x, float y, float z)
        {
            Vector3 rot = tf.eulerAngles;
            rot.x = x;
            rot.y = y;
            rot.z = z;
            tf.eulerAngles = rot;
        }
        internal static void SetLocalRotX(this Transform tf, float x)
        {
            Vector3 rot = tf.localEulerAngles;
            rot.x = x;
            tf.localEulerAngles = rot;
        }
        internal static void SetLocalRotY(this Transform tf, float y)
        {
            Vector3 rot = tf.localEulerAngles;
            rot.y = y;
            tf.localEulerAngles = rot;
        }
        internal static void SetLocalRotZ(this Transform tf, float z)
        {
            Vector3 rot = tf.localEulerAngles;
            rot.z = z;
            tf.localEulerAngles = rot;
        }
        internal static void SetLocalRotXY(this Transform tf, float x, float y)
        {
            Vector3 rot = tf.localEulerAngles;
            rot.x = x;
            rot.y = y;
            tf.localEulerAngles = rot;
        }
        internal static void SetLocalRotXZ(this Transform tf, float x, float z)
        {
            Vector3 rot = tf.localEulerAngles;
            rot.x = x;
            rot.z = z;
            tf.localEulerAngles = rot;
        }
        internal static void SetLocalRotYZ(this Transform tf, float y, float z)
        {
            Vector3 rot = tf.localEulerAngles;
            rot.y = y;
            rot.z = z;
            tf.localEulerAngles = rot;
        }
        internal static void SetLocalRotXYZ(this Transform tf, float x, float y, float z)
        {
            Vector3 rot = tf.localEulerAngles;
            rot.x = x;
            rot.y = y;
            rot.z = z;
            tf.localEulerAngles = rot;
        }
        internal static void SetScaleX(this Transform tf, float x)
        {
            Vector3 scale = tf.localScale;
            scale.x = x;
            tf.localScale = scale;
        }
        internal static void SetScaleY(this Transform tf, float y)
        {
            Vector3 scale = tf.localScale;
            scale.y = y;
            tf.localScale = scale;
        }
        internal static void SetScaleZ(this Transform tf, float z)
        {
            Vector3 scale = tf.localScale;
            scale.z = z;
            tf.localScale = scale;
        }
        internal static void SetScaleXY(this Transform tf, float x, float y)
        {
            Vector3 scale = tf.localScale;
            scale.x = x;
            scale.y = y;
            tf.localScale = scale;
        }
        internal static void SetScaleXZ(this Transform tf, float x, float z)
        {
            Vector3 scale = tf.localScale;
            scale.x = x;
            scale.z = z;
            tf.localScale = scale;
        }
        internal static void SetScaleYZ(this Transform tf, float y, float z)
        {
            Vector3 scale = tf.localScale;
            scale.y = y;
            scale.z = z;
            tf.localScale = scale;
        }
        internal static void SetScaleXYZ(this Transform tf, float x, float y, float z)
        {
            Vector3 scale = tf.localScale;
            scale.x = x;
            scale.y = y;
            scale.z = z;
            tf.localScale = scale;
        }
        internal static void SetAnchorX(this RectTransform rtf, float x)
        {
            Vector2 anchoredPos = rtf.anchoredPosition;
            anchoredPos.x = x;
            rtf.anchoredPosition = anchoredPos;
        }
        internal static void SetAnchorY(this RectTransform rtf, float y)
        {
            Vector2 anchoredPos = rtf.anchoredPosition;
            anchoredPos.y = y;
            rtf.anchoredPosition = anchoredPos;
        }
        internal static void SetAlpha(this Text text, float alpha)
        {
            if (text == null) return;
            Color color = text.color;
            color.a = alpha;
            text.color = color;
        }
        internal static void SetAlpha(this Image image, float alpha)
        {
            if (image == null) return;
            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }
        internal static void SetAlpha(this SpriteRenderer sr, float alpha)
        {
            if (sr == null) return;
            Color color = sr.color;
            color.a = alpha;
            sr.color = color;
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

        // PCの場合、pointerIdは-1のままでOK(無視される). モバイルの場合、対象にしたい指のIDを指定する.
        internal static Vector3 PointerPositionToWorldPosition(this Camera camera, float z, int pointerId = -1)
        {
            if (camera == null) return Vector3.zero;

            Vector3 pointerPosition = Vector3.zero;
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
            pointerPosition = Input.mousePosition;
#elif UNITY_IOS || UNITY_ANDROID
            foreach (var touch in Input.touches)
            {
                if (touch.fingerId == pointerId)
                {
                    pointerPosition = touch.position;
                    break;
                }
            }
#else
            return Vector3.zero;  // 対応していないプラットフォーム
#endif

            Vector3 screenPos = camera.ScreenToWorldPoint(pointerPosition);
            screenPos.z = z;
            return screenPos;
        }
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