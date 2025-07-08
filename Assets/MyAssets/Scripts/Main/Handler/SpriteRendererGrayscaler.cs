using UnityEngine;

namespace Main.Handler
{
    internal sealed class SpriteRendererGrayscaler : MonoBehaviour
    {
        [SerializeField] private Shader shader;
        [SerializeField] private SpriteRenderer target;
        [SerializeField, Range(1.0f, 5.0f)] private float grayscaleStrength = 1.0f;

        private static readonly int GrayscaleEnabledID = Shader.PropertyToID("_GrayscaleEnabled");
        private static readonly int GrayscaleStrengthID = Shader.PropertyToID("_GrayscaleStrength");

        private Material material = null;

        private void Awake()
        {
            if (shader != null)
            {
                material = new Material(shader);
                material.SetFloat(GrayscaleStrengthID, grayscaleStrength);
                if (target != null)
                    target.material = material;
            }
        }

        private void OnDestroy()
        {
            if (material != null)
            {
                Destroy(material);
                material = null;
            }
        }

        internal void SetEnabled(bool isEnabled)
        {
            if (material != null)
            {
                material.SetFloat(GrayscaleEnabledID, isEnabled ? 1.0f : 0.0f);
            }
        }
    }
}
