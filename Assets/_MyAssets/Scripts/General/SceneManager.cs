using UniSceneManager = UnityEngine.SceneManagement.SceneManager;
using SceneNameTable = System.Collections.Generic.Dictionary<General.Scene, string>;

namespace General
{
    internal enum Scene : byte
    {
        Title = 0,
        Main = 1,
    }

    internal static class SceneManager
    {
        private static readonly SceneNameTable sceneNameTable = new()
        {
            { Scene.Title, "Title" },
            { Scene.Main, "Main" },
        };

        internal static async UniTask LoadAsync(this Scene scene, Ct ct = default)
        {
            if (!sceneNameTable.TryGetValue(scene, out string sceneName))
            {
                $"Scene '{scene}' is not defined.".LogError();
                return;
            }

            await Cleanupper.RunAsync(ct);

            await UniSceneManager.LoadSceneAsync(sceneName);
        }
    }
}