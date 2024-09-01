using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Title.Handler
{
    internal sealed class WhiteLineMover : MonoBehaviour
    {
        [SerializeField] private Transform[] line1;
        [SerializeField] private Transform[] line2;

        private WhiteLineMoverBhv impl;

        private void OnEnable()
        {
            impl = new(Array.AsReadOnly(line1), Array.AsReadOnly(line2));
        }

        private void OnDisable()
        {
            impl.Dispose();
            impl = null;
        }

        private void Update()
        {
            impl.Update();
        }
    }

    internal sealed class WhiteLineMoverBhv : IDisposable
    {
        private ReadOnlyCollection<Transform> line1;
        private ReadOnlyCollection<Transform> line2;

        private static readonly float speedX = 1, sx1 = 13, ex1 = -11, sx2 = 10, ex2 = -14;
        private float x1 = sx1, x2 = sx2;

        internal WhiteLineMoverBhv(ReadOnlyCollection<Transform> line1, ReadOnlyCollection<Transform> line2)
        {
            this.line1 = line1;
            this.line2 = line2;
        }

        public void Dispose()
        {
            line1 = null;
            line2 = null;
        }

        internal void Update()
        {
            if (line1 == null) return;
            if (line2 == null) return;

            UpdateLine(line1, ref x1, sx1, ex1);
            UpdateLine(line2, ref x2, sx2, ex2);
        }

        private void UpdateLine(ReadOnlyCollection<Transform> line, ref float x, float sx, float ex)
        {
            if (line == null || line.Count == 0) return;
            if (sx <= ex) throw new Exception($"{sx}‚Í{ex}‚æ‚è‚à‘å‚«‚¢’l‚Å‚ ‚é•K—v‚ª‚ ‚è‚Ü‚·");

            for (int i = 0; i < line.Count; i++)
            {
                x = Loop(x - speedX * Time.deltaTime, ex, sx);
                float _x = Loop(x + i * (ex - sx) / (line.Count - 1), ex, sx);
                SetPositionX(line[i], _x);
            }
        }

        /// <summary>
        /// s < e
        /// </summary>
        private float Loop(float val, float s, float e)
        {
            float ret = val;
            float len = e - s;
            while (e < ret) ret -= len;
            while (ret < s) ret += len;
            return ret;
        }

        private void SetPositionX(Transform tf, float x)
        {
            if (tf == null) return;

            Vector3 pos = tf.position;
            pos.x = x;
            tf.position = pos;
        }
    }
}