namespace General;

internal static class Cleanupper
{
    /// <summary>
    /// 未使用アセットのアンロード・ガベージコレクションを実行する
    /// </summary>
    internal static async UniTask RunAsync(Ct ct)
    {
        "Cleanup started...".Log();

        await Resources.UnloadUnusedAssets().WithCancellation(ct);
        await UniTask.NextFrame(cancellationToken: ct);
        GC.Collect();

        "Cleanup completed.".Log();
    }

    /// <summary>
    /// ガベージコレクションのみを実行する
    /// 非同期処理が嫌なケースでは、こっちを実行するのがオススメ
    /// </summary>
    internal static void RunOnlyGC()
    {
        "Garbage collection started...".Log();

        GC.Collect();

        "Garbage collection completed.".Log();
    }
}