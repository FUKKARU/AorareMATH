namespace Main.Handler
{
    internal sealed class ResultBackgroundShaderController : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Material material;

        [Header("Properties")]
        [SerializeField, Tooltip("Automatically normalized.")] private Vector2 direction;
        [SerializeField, Range(-10.0f, 10.0f)] private float speed;

        internal bool Enabled { get; set; } = false; // スクロールしているか

        private Material materialInstance; // マテリアルのインスタンスを保存
        private Vector2 offset; // スクロールのオフセットを保存

        private static readonly int EnabledID = Shader.PropertyToID("_Enabled");
        private static readonly int OffsetID = Shader.PropertyToID("_Offset");

        private void Awake()
        {
            if (material != null)
            {
                materialInstance = new Material(material);
                if (image != null)
                    image.material = materialInstance;
            }

            direction.Normalize();
        }

        private void OnDestroy()
        {
            if (materialInstance != null)
            {
                Destroy(materialInstance);
                materialInstance = null;
            }
        }

        private void Update()
        {
            if (Enabled)
                offset += direction * (speed * Time.deltaTime);

            if (materialInstance != null)
            {
                materialInstance.SetFloat(EnabledID, Enabled ? 1 : 0);
                materialInstance.SetVector(OffsetID, offset);
            }
        }
    }
}