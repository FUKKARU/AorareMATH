using SO;
using UnityEngine;

namespace General
{
    public class GameStateSetter
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void RuntimeInitializeOnLoadMethods()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            Screen.SetResolution(SO_GameState.Entity.Resolution.x, SO_GameState.Entity.Resolution.y, SO_GameState.Entity.IsFullScreen);
            QualitySettings.vSyncCount = 1;
#endif
            Application.targetFrameRate = SO_GameState.Entity.TargetFrameRate;
        }
    }
}