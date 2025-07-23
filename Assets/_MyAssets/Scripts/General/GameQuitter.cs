namespace General
{
    internal static class GameQuitter
    {
        internal static void Quit()
        {
            SaveDataHolder.Save();
            UnityEngine.Debug.Log("SaveData was saved before quitting the game.");

            // WebGL は、何もしない
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID
            UnityEngine.Application.Quit();
#endif
        }
    }
}