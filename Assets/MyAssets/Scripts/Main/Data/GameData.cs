using System;
using System.IO;
using General;
using General.Debug;
using UnityEngine;

namespace Main.Data
{
    [Serializable]
    public sealed class GameData : IDisposable
    {
        private const string savePath = "gameData.json";

        private static readonly int correctAmountRankingMaxAmount = 100;
        public int[] CorrectAmountRanking = new int[correctAmountRankingMaxAmount + 1]; // データを追加する用に、最後の一つ分多く確保

        public static void Save(GameData gameData)
        {
            string json = JsonUtility.ToJson(gameData);
            using StreamWriter writer = new(Path.Combine(Application.persistentDataPath, savePath), false);
            writer.WriteLine(json);

            "Saved".Show();
            gameData.CorrectAmountRanking.Look();
        }

        public static void Load(out GameData gameData)
        {
            try
            {
                using StreamReader reader = new(Path.Combine(Application.persistentDataPath, savePath));
                string json = reader.ReadToEnd();
                gameData = JsonUtility.FromJson<GameData>(json);

                "Loaded".Show();
                gameData.CorrectAmountRanking.Look();
            }
            catch (Exception)
            {
                GameData newGameData = new();
                Save(newGameData);
                gameData = newGameData;
            }
        }

        public void Dispose()
        {
            Array.Clear(CorrectAmountRanking, 0, CorrectAmountRanking.Length);
            CorrectAmountRanking = null;
        }
    }

    public sealed class GameDataHolder : IDisposable, IInittable
    {
        private GameData gameData = new();

        public void Init()
        {
            GameData.Load(out gameData);
            SortDescending(gameData.CorrectAmountRanking); // 一応ソートしておく
        }

        private int _correctAmount = 0;
        public int CorrectAmount
        {
            get => _correctAmount;
            set
            {
                value = Mathf.Clamp(value, 0, 999);

                _correctAmount = value;

                // ランキングに追加
                gameData.CorrectAmountRanking[^1] = _correctAmount;
                SortDescending(gameData.CorrectAmountRanking);
                gameData.CorrectAmountRanking[^1] = 0;
            }
        }

        // 負荷軽減のため、プロパティの更新とは別個で呼び出すこと
        public void SaveRanking() => GameData.Save(gameData);

        // 1始まり、ランキング外だったら0を返す
        public int GetRank()
        {
            for (int i = 0; i < gameData.CorrectAmountRanking.Length; i++)
            {
                if (gameData.CorrectAmountRanking[i] == _correctAmount)
                {
                    return i + 1;
                }
            }
            return 0;
        }

        public void Dispose()
        {
            gameData?.Dispose();
            gameData = null;
        }

        private void SortDescending<T>(T[] array)
        {
            Array.Sort(array);
            Array.Reverse(array);
        }
    }
}