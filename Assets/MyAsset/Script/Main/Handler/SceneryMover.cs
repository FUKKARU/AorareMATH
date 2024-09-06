using Cysharp.Threading.Tasks;
using System;
using System.Linq;
using System.Threading;
using UnityEngine;
using SO;
using General.Extension;

namespace Main.Handler
{
    internal sealed class SceneryMover : MonoBehaviour
    {
        [SerializeField] private SceneryElementReference[] whitelineReference;
        [SerializeField] private SceneryElementReference[] treeReference;
        [SerializeField] private SceneryElementReference[] poleReference;

        private SceneryMoverImpl impl;

        private void OnEnable()
        {
            impl = new
                (whitelineReference, SO_Scenery.Entity.WhitelineProperty,
                treeReference, SO_Scenery.Entity.TreeProperty,
                poleReference, SO_Scenery.Entity.PoleProperty,
                this.GetCancellationTokenOnDestroy());
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

    internal sealed class SceneryMoverImpl : IDisposable, INullExistable
    {
        private readonly CancellationToken ct;

        private SceneryElement[] whitelines;
        private SceneryElement[] trees;
        private SceneryElement[] poles;

        private bool isFirstUpdate = true;

        internal SceneryMoverImpl
            (SceneryElementReference[] whitelineReference, SceneryElementProperty whitelineProperty,
            SceneryElementReference[] treeReference, SceneryElementProperty treeProperty,
            SceneryElementReference[] poleReference, SceneryElementProperty poleProperty,
            CancellationToken ct)
        {
            this.ct = ct;

            whitelines = whitelineReference.Select(e => new SceneryElement(e, whitelineProperty)).ToArray();
            trees = treeReference.Select(e => new SceneryElement(e, treeProperty)).ToArray();
            poles = poleReference.Select(e => new SceneryElement(e, poleProperty)).ToArray();
        }

        public void Dispose()
        {
            foreach (var e in whitelines) e.Dispose();
            foreach (var e in trees) e.Dispose();
            foreach (var e in poles) e.Dispose();

            whitelines = null;
            trees = null;
            poles = null;
        }

        public bool IsNullExist()
        {
            if (whitelines == null) return true;
            if (trees == null) return true;
            if (poles == null) return true;

            foreach (var e in whitelines) if (e.IsNullExist()) return true;
            foreach (var e in trees) if (e.IsNullExist()) return true;
            foreach (var e in poles) if (e.IsNullExist()) return true;

            return false;
        }

        internal void Update()
        {
            if (isFirstUpdate)
            {
                isFirstUpdate = false;

                CreateElements(whitelines, ct).Forget();
                CreateElements(trees, ct).Forget();
                CreateElements(poles, ct).Forget();
            }

            foreach (var e in whitelines) e.Update();
            foreach (var e in trees) e.Update();
            foreach (var e in poles) e.Update();
        }

        private async UniTask CreateElements(SceneryElement[] elements, CancellationToken ct)
        {
            if (elements == null) return;  // 配列の参照が存在する
            if (elements.Length <= 0) return; // 配列の要素数が1以上
            if (elements[0] == null) return;  // 最初の要素について、その参照が存在する
            if (elements[0].IsPropertyNull) return;  // 最初の要素について、その中に、プロパティの参照が存在する
            if (!elements[0].IsDisplay) return;  // 最初の要素について、その中のプロパティの参照において、表示のフラグが有効である
            foreach (var e in elements) if (e.IsNullExist()) return;  //　配列の全ての要素について、そのメンバにnullが存在しない

            int len = elements.Length;
            int i = 0;
            while (true)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(elements[0].Interval), cancellationToken: ct);
                elements[i].IsActive = true;
                i = Looped(++i, 0, len - 1);
            }
        }

        private int Looped(int value, int min, int max)
        {
            if (max <= min) return value;

            int len = max - min + 1;
            while (value < min) value += len;
            while (max < value) value -= len;
            return value;
        }
    }

    internal sealed class SceneryElement : IDisposable, INullExistable
    {
        private SceneryElementReference reference;
        private SceneryElementProperty property;
        internal bool IsReferenceNull => reference == null;
        internal bool IsPropertyNull => property == null;
        internal float Interval => property.Interval;
        internal bool IsDisplay => property.IsDisplay;

        internal bool IsActive { get; set; } = false;

        private float t = 0;

        internal SceneryElement(SceneryElementReference reference, SceneryElementProperty property)
        {
            this.reference = reference;
            this.property = property;
            this.reference.SpriteRenderer.sprite = this.property.Sprite;
            Init();
        }

        public void Dispose()
        {
            reference.Dispose();
            property.Dispose();

            reference = null;
            property = null;
        }

        public bool IsNullExist()
        {
            if (reference.IsNullExist()) return true;
            if (property.IsNullExist()) return true;
            return false;
        }

        internal void Update()
        {
            if (IsNullExist()) return;

            if (!IsActive)
            {
                if (reference.SpriteRenderer.enabled) reference.SpriteRenderer.enabled = false;
            }
            else
            {
                if (!reference.SpriteRenderer.enabled) reference.SpriteRenderer.enabled = true;

                reference.Transform.position = reference.Transform.position + CalcVelocity(t) * Time.deltaTime;
                reference.Transform.localScale = CalcLocalScale(t);

                t += Time.deltaTime;
                if (t >= property.Duration)
                {
                    t = 0;
                    Init();
                }
            }
        }

        private void Init()
        {
            reference.Transform.position = property.StartPosition;
            reference.Transform.localScale = Vector3.one * property.StartLocalScale;
            IsActive = false;
        }

        private Vector3 CalcVelocity(float t)
        {
            return property.StartVelocity.normalized * (t * t * property.VelocityCoefficient) + property.StartVelocity;
        }

        private Vector3 CalcLocalScale(float t)
        {
            float s = t * property.ScaleCoefficient + property.StartLocalScale;
            return new(s, s, 1);
        }
    }

    [Serializable]
    internal sealed class SceneryElementReference : IDisposable, INullExistable
    {
        [SerializeField] internal Transform Transform;
        [SerializeField] internal SpriteRenderer SpriteRenderer;

        public void Dispose()
        {
            Transform = null;
            SpriteRenderer = null;
        }

        public bool IsNullExist()
        {
            if (Transform == null) return true;
            if (SpriteRenderer == null) return true;
            return false;
        }
    }
}