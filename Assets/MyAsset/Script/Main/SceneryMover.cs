using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace Main
{
    internal sealed class SceneryMover : MonoBehaviour
    {
        [SerializeField] private SceneryElementProperty whiteLine;
        [SerializeField] private SceneryElementProperty building;

        private CancellationToken ct;

        private SceneryMoverBhv impl;

        private void OnEnable()
        {
            ct = this.GetCancellationTokenOnDestroy();
            impl = new(whiteLine, building, ct);
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

    internal sealed class SceneryMoverBhv : System.IDisposable
    {
        private readonly CancellationToken ct;

        private SceneryElement[] whiteLines;
        private SceneryElement[] buildings;

        private bool isFirstUpdate = true;

        private static readonly float dur = 1f;

        internal SceneryMoverBhv(SceneryElementProperty whiteLine, SceneryElementProperty building, CancellationToken ct)
        {
            this.ct = ct;
            this.whiteLines =
                new SceneryElement[5] { new(whiteLine), new(whiteLine), new(whiteLine), new(whiteLine), new(whiteLine) };
            this.buildings =
                new SceneryElement[5] { new(building), new(building), new(building), new(building), new(building) };
        }

        public void Dispose()
        {
            foreach (var e in whiteLines) e.Dispose();
            foreach (var e in buildings) e.Dispose();
            whiteLines = null;
            buildings = null;
        }

        internal void Update()
        {
            foreach (var e in whiteLines) if (e.IsNullExist()) return;
            foreach (var e in buildings) if (e.IsNullExist()) return;

            if (isFirstUpdate)
            {
                CreateWhiteLine(ct).Forget();
                isFirstUpdate = false;
            }

            foreach (var e in whiteLines) e.Update();
        }

        private async UniTask CreateWhiteLine(CancellationToken ct)
        {
            int i = 0;
            while (true)
            {
                await UniTask.Delay(System.TimeSpan.FromSeconds(dur));
                whiteLines[i].IsActive = true;

                i++;
                if (i == 5) i = 0;
            }
        }
    }

    [System.Serializable]
    internal struct SceneryElementProperty
    {
        public Transform tf;
        public SpriteRenderer sr;
        public Sprite sprite;

        public float dur;
        public Vector2 startPosition;
        public Vector2 endPosition;
        public float startLocalScale;
        public float endLocalScale;
    }

    internal sealed class SceneryElement : System.IDisposable
    {
        private Transform tf;
        private SpriteRenderer sr;
        private Sprite sprite;

        private float dur;
        private Vector2 startPosition;
        private Vector2 endPosition;
        private float startLocalScale;
        private float endLocalScale;

        internal bool IsActive { get; set; } = false;

        private float t = 0;

        internal SceneryElement(SceneryElementProperty property)
        {
            this.tf = property.tf;
            this.sr = property.sr;
            this.sprite = property.sprite;
            this.dur = property.dur;
            this.startPosition = property.startPosition;
            this.endPosition = property.endPosition;
            this.startLocalScale = property.startLocalScale;
            this.endLocalScale = property.endLocalScale;
            this.IsActive = false;
        }

        public void Dispose()
        {
            tf = null;
            sr = null;
            sprite = null;
        }

        internal void Update()
        {
            if (IsNullExist()) return;

            if (!IsActive)
            {
                if (sr.enabled) sr.enabled = false;
            }
            else
            {
                if (!sr.enabled) sr.enabled = true;

                tf.position = t.Remap(0, dur, startPosition, endPosition);
                tf.localScale = Vector3.one * t.Remap(0, dur, startLocalScale, endLocalScale);

                t += Time.deltaTime;
                if (t >= dur)
                {
                    t = 0;
                    IsActive = false;
                }
            }
        }

        internal bool IsNullExist()
        {
            if (!tf) return true;
            if (!sr) return true;
            if (!sprite) return true;
            return false;
        }
    }

    internal static class Ex
    {
        internal static float Remap(this float x, float a, float b, float c, float d)
        {
            return (x - a) * (d - c) / (b - a) + c;
        }

        internal static Vector2 Remap(this float x, float a, float b, Vector2 c, Vector2 d)
        {
            return new((x - a) * (d.x - c.x) / (b - a) + c.x, (x - a) * (d.y - c.y) / (b - a) + c.y);
        }
    }
}