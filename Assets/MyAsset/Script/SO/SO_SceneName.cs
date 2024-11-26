using UnityEngine;

namespace SO
{
    [CreateAssetMenu(menuName = "SO/SO_SceneName", fileName = "SO_SceneName")]
    public class SO_SceneName : AResourceLoadableScriptableObject<SO_SceneName>
    {
        [SerializeField] private string _title;
        internal string Title => _title;

        [SerializeField] private string _main;
        internal string Main => _main;
    }
}