namespace General
{
    internal static class GameQuitter
    {
        internal static void Quit()
        {
            // モバイル・WebGL は、何もしない
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
            UnityEngine.Application.Quit();
#endif
        }
    }
}