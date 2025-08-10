using System.IO;
using SO;

namespace General
{
    [Serializable]
    internal sealed class SaveData
    {
        // 正解数 (ランキング用, TOP 100)
        // データを追加する用に、最後の一つ分多く確保
        public int[] CorrectAmountRanking;

        // 演出の高速化
        public bool DoFastenDirections;

        // サウンドボリューム
        public float BgmVolume;
        public float SeVolume;

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

    // Webでは StreamWriter/StreamReader が上手く動作しなかったため、 PlayerPrefs で対応する
    internal static class SaveDataHolder
    {
        private const string SavePath = "gameData.json";
        private const string SavePathWeb = "gameDataWeb.json";
        private static readonly float AutoSaveIntervalSec = 30.0f;

        private static SaveData saveData = new();
        internal static SaveData Data => saveData;

        // ロード直後のSaveDataをキャッシュしておく
        //! この中のメンバを書き換えることは、想定していない
        //! 再起動後に反映するセーブデータ用だが、現状そのようなデータがないため、使用していない
        //! (もし上記の使用用途が必要になった場合、データの更新はDataに対して行う一方で、データの読み取りはCacheDataに対して行えばよい)
        private static readonly SaveData saveDataCache = new();
        // internal static SaveData CacheData => saveDataCache;

        // SaveDataをセーブする
        //! 外部依存
        internal static void Save()
        {
            try
            {
                string json = JsonUtility.ToJson(saveData);

#if !UNITY_WEBGL
                using StreamWriter writer = new(Path.Combine(Application.persistentDataPath, SavePath), false);
                writer.WriteLine(json);
#else
                PlayerPrefs.SetString(SavePathWeb, json);
                PlayerPrefs.Save();
#endif
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
#if !UNITY_WEBGL
                using StreamReader reader = new(Path.Combine(Application.persistentDataPath, SavePath));
                string json = reader.ReadToEnd();
#else
                string json = PlayerPrefs.GetString(SavePathWeb, "{}");
#endif

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