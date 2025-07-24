using System.IO;
using SO;

namespace General
{
    [Serializable]
    internal sealed class SaveData
    {
        // 正解数 (ランキング用, TOP 100)
        // データを追加する用に、最後の一つ分多く確保
        internal int[] CorrectAmountRanking;

        // 演出の高速化
        internal bool DoFastenDirections;

        // サウンドボリューム
        internal float BgmVolume;
        internal float SeVolume;

        internal void CopyFromOther(SaveData other)
        {
            if (other == null) return;

            int correctAmountRankingLength = other.CorrectAmountRanking.Length;
            CorrectAmountRanking = new int[correctAmountRankingLength];
            Array.Copy(other.CorrectAmountRanking, CorrectAmountRanking, correctAmountRankingLength);

            DoFastenDirections = other.DoFastenDirections;
            BgmVolume = other.BgmVolume;
            SeVolume = other.SeVolume;
        }
    }

    internal static class SaveDataHolder
    {
        private const string SavePath = "gameData.json";
        private static SaveData saveData = new();

        // ロード直後のSaveDataをキャッシュしておく
        //! この中のメンバを書き換えることは、想定していない
        private static SaveData saveDataCache = new();

        internal static SaveData Data => saveData;
        internal static SaveData CacheData => saveDataCache;

        private static readonly float AutoSaveIntervalSec = 30.0f;

        // SaveDataをセーブする
        //! 外部依存
        internal static void Save()
        {
            try
            {
                string json = JsonUtility.ToJson(saveData);
                using StreamWriter writer = new(Path.Combine(Application.persistentDataPath, SavePath), false);
                writer.WriteLine(json);
            }
            catch (Exception e)
            {
                $"Failed to save SaveData: {e.Message}".LogError();
            }
        }

        // ロードしSaveDataに代入する
        // 失敗した場合はSaveDataを新規作成してセーブする
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Load()
        {
            try
            {
                using StreamReader reader = new(Path.Combine(Application.persistentDataPath, SavePath));
                string json = reader.ReadToEnd();
                saveData = JsonUtility.FromJson<SaveData>(json);
            }
            catch (Exception e)
            {
                $"Failed to load SaveData: {e.Message}. A new SaveData will be created.".LogWarning();

                saveData = new SaveData()
                {
                    CorrectAmountRanking = new int[SO_SaveDataDefault.Entity.CorrectAmountRankingLength + 1],
                    DoFastenDirections = SO_SaveDataDefault.Entity.DoFastenDirections,
                    BgmVolume = SO_SaveDataDefault.Entity.BgmVolume,
                    SeVolume = SO_SaveDataDefault.Entity.SeVolume
                };
                Save();
            }
            finally
            {
                saveDataCache?.CopyFromOther(saveData);

                // オートセーブを開始
                SavePeriodically(AutoSaveIntervalSec).Forget();
            }

            // セーブデータをロードした後の初期化処理を行う
            {
                SetVolumeToAm(Ct.None).Forget(); // キャンセルしない



                static async UniTaskVoid SetVolumeToAm(Ct ct)
                {
                    // AudioMixer.SetFloat は Awake 段階では発動しないバグがあるため，タイミング調整
                    await UniTask.Yield(PlayerLoopTiming.LastInitialization);

                    SoundManager.SetVolume(SoundType.BGM, saveData.BgmVolume);
                    SoundManager.SetVolume(SoundType.SE, saveData.SeVolume);
                }
            }
        }

        // ゲーム実行中ずっと回すので、Ctは渡さない
        private static async UniTaskVoid SavePeriodically(float intervalSec)
        {
            while (true)
            {
                await intervalSec.SecAwait(ignoreTimeScale: true, timing: PlayerLoopTiming.PostLateUpdate);
                Save();

                "SaveData was saved periodically.".Log();
            }
        }
    }
}