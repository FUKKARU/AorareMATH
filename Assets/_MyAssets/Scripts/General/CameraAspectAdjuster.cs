using SO;

namespace General
{
    [ExecuteAlways]
    internal sealed class CameraAspectAdjuster : MonoBehaviour
    {
        [SerializeField] private new Camera camera;

        private Vector2Int lastResolution = Vector2Int.zero;

        private void Start()
        {
            if (camera != null)
            {
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = Color.black;
            }

            lastResolution = new(Screen.width, Screen.height);
            Adjust(lastResolution, SO_GameState.Entity.Resolution);
        }

        private void LateUpdate()
        {
            Vector2Int currResolution = new(Screen.width, Screen.height);
            if (lastResolution != currResolution)
            {
                lastResolution = currResolution;
                Adjust(lastResolution, SO_GameState.Entity.Resolution);
            }
        }

        private void Adjust(Vector2Int currResolution, Vector2Int targetResolution)
        {
            if (camera == null || currResolution.x <= 0 || currResolution.y <= 0 || targetResolution.x <= 0 || targetResolution.y <= 0)
                return;

            float currAspect = 1.0f * currResolution.x / currResolution.y;
            float targetAspect = 1.0f * targetResolution.x / targetResolution.y;

            float scaleHeight = currAspect / targetAspect;

            // 横が狭い(縦長すぎる) → 上下に黒帯
            if (scaleHeight < 1.0f)
            {
                float inset = (1.0f - scaleHeight) * 0.5f;
                camera.rect = new Rect(0, inset, 1.0f, scaleHeight);
            }
            // 縦が狭い(横長すぎる) → 左右に黒帯
            else
            {
                float scaleWidth = 1.0f / scaleHeight;
                float inset = (1.0f - scaleWidth) * 0.5f;
                camera.rect = new Rect(inset, 0, scaleWidth, 1.0f);
            }
        }
    }
}