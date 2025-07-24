using System.Linq;
using SO;

namespace Main.Handler
{
    internal sealed class SceneryMover : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer elementPrefab;
        [SerializeField] private Transform parent;

        private List<SceneryMoverImpl> impls = new(5);

        private bool isPaused = false;
        internal bool IsPaused
        {
            get => isPaused;
            set
            {
                if (isPaused == value) return;
                isPaused = value;

                foreach (var impl in impls)
                {
                    if (impl == null) continue;
                    impl.IsPaused = value;
                }
            }
        }

        private void OnEnable()
        {
            InstantiateThis(SO_Scenery.Entity.WhitelineProperty);
            InstantiateThis(SO_Scenery.Entity.BuildingsLeftProperty);
            InstantiateThis(SO_Scenery.Entity.BuildingsRightProperty);
#if false
            InstantiateThis(SO_Scenery.Entity.LampLeftProperty);
            InstantiateThis(SO_Scenery.Entity.LampRightProperty);
#endif

            void InstantiateThis(SceneryElementProperty property)
            {
                if (property == null) return;

                LinkedList<SceneryElement> elements = new();

                for (int i = 0; i < 100; i++)
                {
                    SpriteRenderer instance = Instantiate(elementPrefab, parent);
                    if (instance == null) continue;
                    SceneryElementReference reference = new(instance.transform, instance);
                    SceneryElement element = new(reference, property);
                    elements.AddLast(element);
                }

                impls?.Add(new(elements.ToArray()));
            }
        }

        private void OnDisable()
        {
            foreach (var impl in impls)
                impl?.Dispose();
            impls?.Clear();
            impls = null;
        }

        private void Update()
        {
            foreach (var impl in impls)
                impl?.Update();
        }
    }

    internal sealed class SceneryMoverImpl : IDisposable
    {
        private Cts cts = new();
        private SceneryElement[] elements;
        private bool isFirstUpdate = true;

        private bool isPaused = false;
        internal bool IsPaused
        {
            get => isPaused;
            set
            {
                if (isPaused == value) return;
                isPaused = value;

                foreach (var e in elements)
                    e.IsPaused = value;
            }
        }

        internal SceneryMoverImpl(SceneryElement[] elements) => this.elements = elements;

        public void Dispose()
        {
            cts.Cancel();
            cts.Dispose();
            cts = null;

            if (elements != null) foreach (var e in elements) e.Dispose();
            Array.Clear(elements, 0, elements.Length);
        }

        internal void Update()
        {
            if (isFirstUpdate)
            {
                isFirstUpdate = false;

                CreateElements(elements, cts.Token).Forget();
            }

            foreach (var e in elements) e.Update();
        }

        private async UniTask CreateElements(SceneryElement[] elements, Ct ct)
        {
            if (elements == null) return;

            int len = elements.Length;
            if (len <= 0) return;

            await UniTask.WaitForSeconds(elements[0].TimeOffset, cancellationToken: ct);

            int i = 0;
            while (true)
            {
                await UniTask.WaitForSeconds(elements[0].Interval, cancellationToken: ct);
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

    internal sealed class SceneryElement : IDisposable
    {
        private SceneryElementReference reference;
        private SceneryElementProperty property;
        internal float Interval => property.Interval;
        internal float TimeOffset => property.TimeOffset;

        internal bool IsPaused { get; set; } = false;

        private bool isActive = false;
        internal bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                if (reference != null) reference.IsActive = value;
            }
        }

        private float t = 0;

        internal SceneryElement(SceneryElementReference reference, SceneryElementProperty property)
        {
            if (reference == null) return;
            if (property == null) return;
            this.reference = reference;
            this.property = property;

            Sprite sprite = property.Sprite;
            if (sprite == null) return;
            reference.Sprite = sprite;
            Init();
        }

        public void Dispose()
        {
            reference.Dispose();

            reference = null;
            property = null;
        }

        internal void Update()
        {
            if (!IsActive) return;
            if (IsPaused) return;

            reference.Position += CalcVelocity(t) * Time.deltaTime;
            reference.LocalScale = CalcLocalScale(t);

            t += Time.deltaTime;
            if (t >= property.Duration)
            {
                t = 0;
                Init();
            }
        }

        private void Init()
        {
            reference.Position = property.StartPosition;
            reference.LocalScale = Vector3.one * property.StartLocalScale;
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
    internal sealed class SceneryElementReference : IDisposable
    {
        [SerializeField] private Transform transform;
        [SerializeField] private SpriteRenderer spriteRenderer;

        internal SceneryElementReference(Transform transform, SpriteRenderer spriteRenderer)
        {
            if (transform == null) return;
            if (spriteRenderer == null) return;

            this.transform = transform;
            this.spriteRenderer = spriteRenderer;
        }

        internal Vector3 Position
        {
            get
            {
                if (transform == null) return default;
                return transform.position;
            }
            set
            {
                if (transform == null) return;
                transform.position = value;
            }
        }

        internal Vector3 LocalScale
        {
            get
            {
                if (transform == null) return default;
                return transform.localScale;
            }
            set
            {
                if (transform == null) return;
                transform.localScale = value;
            }
        }

        internal bool IsActive
        {
            get
            {
                if (spriteRenderer == null) return false;
                return spriteRenderer.enabled;
            }
            set
            {
                if (spriteRenderer == null) return;
                spriteRenderer.enabled = value;
            }
        }

        internal Sprite Sprite
        {
            set
            {
                if (spriteRenderer == null) return;
                spriteRenderer.sprite = value;
            }
        }

        public void Dispose()
        {
            transform = null;
            spriteRenderer = null;
        }
    }
}