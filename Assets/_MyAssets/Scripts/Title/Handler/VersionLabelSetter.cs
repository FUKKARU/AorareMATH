namespace Title.Handler
{
    internal sealed class VersionLabelSetter : MonoBehaviour
    {
        [SerializeField] private Text label;

        private void Start()
        {
            if (label != null)
                label.text = $"v{Application.version}";
        }
    }
}