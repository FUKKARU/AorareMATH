#if UNITY_WEBGL && !UNITY_EDITOR

using System.Runtime.InteropServices;

namespace General;

internal static class WebGLFileSync
{
    [DllImport("__Internal")]
    private static extern void JS_FileSystem_Sync();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void HookPageEvents()
    {
        Application.focusChanged += _ => Sync(); // タブ切替など
        Application.quitting += Sync; // 終了時
    }

    internal static void Sync()
    {
        JS_FileSystem_Sync();
    }
}

#endif