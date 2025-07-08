using UnityEngine;

namespace General.Shaders
{
    internal sealed class PostProcessGrayscaleController : MonoBehaviour
    {
        // ポストプロセスのため、アセットを直接書き換える
        // 終了時に元に戻すのを厳格に!
        [SerializeField] private Material materialAsset;
        [SerializeField, Range(1.0f, 5.0f)] private float grayscaleStrength = 1.0f;

        private static readonly int GrayScaleEnabledID = Shader.PropertyToID("_Enabled");
        private static readonly int GrayScaleStrengthID = Shader.PropertyToID("_Strength");

        private float grayscaleEnabledInitialValue = 0.0f;
        private float grayscaleStrengthInitialValue = 1.0f;

        private bool isEnabled = false;
        internal bool IsEnabled
        {
            get => isEnabled;
            set
            {
                if (isEnabled == value) return;
                isEnabled = value;

                if (materialAsset != null)
                {
                    materialAsset.SetFloat(GrayScaleEnabledID, isEnabled ? 1.0f : 0.0f);
                    // materialAsset.SetFloat(GrayScaleStrengthID, grayscaleStrength);  // 今のところは、無くて大丈夫
                }
            }
        }

        private void Awake()
        {
            if (materialAsset != null)
            {
                grayscaleEnabledInitialValue = materialAsset.GetFloat(GrayScaleEnabledID);
                grayscaleStrengthInitialValue = materialAsset.GetFloat(GrayScaleStrengthID);

                materialAsset.SetFloat(GrayScaleEnabledID, 0.0f);
                materialAsset.SetFloat(GrayScaleStrengthID, grayscaleStrength);
            }
        }

        private void OnDestroy()
        {
            if (materialAsset != null)
            {
                materialAsset.SetFloat(GrayScaleEnabledID, grayscaleEnabledInitialValue);
                materialAsset.SetFloat(GrayScaleStrengthID, grayscaleStrengthInitialValue);
            }
            materialAsset = null;
        }
    }
}
