using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using General.Extension;
using Ct = System.Threading.CancellationToken;

namespace General
{
    [Serializable]
    public sealed class SaveData
    {
        private static readonly int CorrectAmountRankingLength = 100;

        // 正解数 (ランキング用, TOP 100)
        // データを追加する用に、最後の一つ分多く確保
        public int[] CorrectAmountRanking = new int[CorrectAmountRankingLength + 1];

        // 演出の高速化
        public bool DoFastenDirections = false;
    }

    public static class SaveDataHolder
    {
        private const string SavePath = "gameData.json";
        private static SaveData saveData = new();

        public static SaveData Data => saveData;

        private static readonly float AutoSaveIntervalSec = 30.0f;

        // SaveDataをセーブする
        //! 外部依存
        public static void Save()
        {
            try
            {
                string json = JsonUtility.ToJson(saveData);
                using StreamWriter writer = new(Path.Combine(Application.persistentDataPath, SavePath), false);
                writer.WriteLine(json);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Failed to save SaveData: {e.Message}");
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
                UnityEngine.Debug.LogWarning($"Failed to load SaveData: {e.Message}. A new SaveData will be created.");

                saveData = new SaveData();
                Save();
            }
            finally
            {
                // オートセーブを開始
                SavePeriodically(AutoSaveIntervalSec).Forget();
            }
        }

        // ゲーム実行中ずっと回すので、Ctは渡さない
        private static async UniTaskVoid SavePeriodically(float intervalSec)
        {
            Ct noneCt = Ct.None;

            while (true)
            {
                await intervalSec.SecAwait(noneCt);
                Save();

                UnityEngine.Debug.Log("SaveData was saved periodically.");
            }
        }
    }
}