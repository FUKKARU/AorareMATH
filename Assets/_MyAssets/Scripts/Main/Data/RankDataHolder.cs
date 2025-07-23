using System;
using UnityEngine;
using General;

namespace Main.Data
{
    public sealed class RankDataHolder : IInittable
    {
        private int[] rankingCache = null;
        private int rankingLength = 0;

        public void Init()
        {
            rankingLength = SaveDataHolder.Data.CorrectAmountRanking.Length;
            rankingCache = new int[rankingLength];
            Array.Copy(SaveDataHolder.Data.CorrectAmountRanking, rankingCache, rankingLength);
            SortDescending(rankingCache); // 一応ソートしておく
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
                rankingCache[^1] = _correctAmount;
                SortDescending(rankingCache);
                rankingCache[^1] = 0;

                // セーブデータを更新
                Array.Copy(rankingCache, SaveDataHolder.Data.CorrectAmountRanking, rankingLength);
            }
        }

        // 1始まり、ランキング外だったら0を返す
        public int GetRank()
        {
            for (int i = 0; i < rankingCache.Length - 1; i++)
            {
                if (rankingCache[i] == _correctAmount)
                {
                    return i + 1;
                }
            }
            return 0;
        }

        private void SortDescending<T>(T[] array)
        {
            Array.Sort(array);
            Array.Reverse(array);
        }
    }
}