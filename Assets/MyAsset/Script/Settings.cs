using SO;
using UnityEngine;

namespace General
{
    public class GameStateSetter
    {
        [RuntimeInitializeOnLoadMethod]
        static void RuntimeInitializeOnLoadMethods()
        {
            // 解像度とフルスクリーンにするかどうかを設定
            Screen.SetResolution(SO_GameState.Entity.Resolution.x, SO_GameState.Entity.Resolution.y, SO_GameState.Entity.IsFullScreen);

            // Vsync（とターゲットフレームレート）の設定
            if (SO_GameState.Entity.IsVsyncOn)
            {
                QualitySettings.vSyncCount = 1; // VSyncをONにする
            }
            else
            {
                QualitySettings.vSyncCount = 0; // VSyncをOFFにする
                Application.targetFrameRate = SO_GameState.Entity.TargetFrameRate; // ターゲットフレームレートの設定
            }
        }
    }
}