using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using SO;

namespace Main.Handler
{
    internal sealed class SceneryMover : MonoBehaviour
    {
        [SerializeField] private SceneryElementReference[] whiteLineReference;
        [SerializeField] private SceneryElementReference[] buildingReference;

        private CancellationToken ct;

        private SceneryMoverBhv impl;

        private void OnEnable()
        {
            ct = this.GetCancellationTokenOnDestroy();
            impl = new(whiteLineReference, SO_Scenery.Entity.WhiteLineProperty,
                buildingReference, SO_Scenery.Entity.BuildingProperty, ct);
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

        private float whiteLineInterval;
        private float buildingInterval;

        private bool isFirstUpdate = true;

        internal SceneryMoverBhv(SceneryElementReference[] whiteLineReference, SceneryElementProperty whiteLineProperty,
            SceneryElementReference[] buildingReference, SceneryElementProperty buildingProperty,
            CancellationToken ct)
        {
            this.ct = ct;

            int len = Mathf.Min(whiteLineReference.Length, buildingReference.Length);

            whiteLines = new SceneryElement[len];
            buildings = new SceneryElement[len];

            for (int i = 0; i < len; i++)
            {
                whiteLines[i] = new(whiteLineReference[i], whiteLineProperty);
                buildings[i] = new(buildingReference[i], buildingProperty);
            }

            whiteLineInterval = whiteLineProperty.Interval;
            buildingInterval = buildingProperty.Interval;
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
                await UniTask.Delay(System.TimeSpan.FromSeconds(whiteLineInterval), cancellationToken: ct);
                whiteLines[i].IsActive = true;

                i++;
                if (i == 5) i = 0;
            }
        }
    }

    internal sealed class SceneryElement : System.IDisposable
    {
        private Transform transform;
        private SpriteRenderer spriteRenderer;

        private readonly float duration;

        private readonly Vector3 startVelocity;
        private readonly Vector3 startPosition;
        private readonly float startLocalScale;
        private readonly float velocityCoefficient;
        private readonly float scaleCoefficient;

        internal bool IsActive { get; set; } = false;

        private float t = 0;

        internal SceneryElement(SceneryElementReference reference, SceneryElementProperty property)
        {
            transform = reference.Transform;
            spriteRenderer = reference.SpriteRenderer;

            duration = property.Duration;
            startVelocity = property.StartVelocity;
            startPosition = property.StartPosition;
            startLocalScale = property.StartLocalScale;
            velocityCoefficient = property.VelocityCoefficient;
            scaleCoefficient = property.ScaleCoefficient;

            spriteRenderer.sprite = property.Sprite;
            Init();
        }

        public void Dispose()
        {
            transform = null;
            spriteRenderer = null;
        }

        internal void Update()
        {
            if (IsNullExist()) return;

            if (!IsActive)
            {
                if (spriteRenderer.enabled) spriteRenderer.enabled = false;
            }
            else
            {
                if (!spriteRenderer.enabled) spriteRenderer.enabled = true;

                transform.position = transform.position + CalcVelocity(t) * Time.deltaTime;
                transform.localScale = CalcLocalScale(t);

                t += Time.deltaTime;
                if (t >= duration)
                {
                    t = 0;
                    Init();
                }
            }
        }

        private void Init()
        {
            transform.position = startPosition;
            transform.localScale = Vector3.one * startLocalScale;
            IsActive = false;
        }

        private Vector3 CalcVelocity(float t)
        {
            return startVelocity.normalized * (t * t * velocityCoefficient) + startVelocity;
        }

        private Vector3 CalcLocalScale(float t)
        {
            float s = t * scaleCoefficient + startLocalScale;
            return new(s, s, 1);
        }

        internal bool IsNullExist()
        {
            if (!transform) return true;
            if (!spriteRenderer) return true;
            return false;
        }
    }

    [System.Serializable]
    internal class SceneryElementReference
    {
        public Transform Transform;
        public SpriteRenderer SpriteRenderer;
    }
}