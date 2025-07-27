using General;

namespace Main.Data
{
    internal sealed class RankDataHolder
    {
        private int[] rankingCache = null;
        private int rankingLength = 0;

        internal static RankDataHolder Create()
        {
            int rankingLength = SaveDataHolder.Data.CorrectAmountRanking.Length;
            RankDataHolder @new = new()
            {
                rankingLength = rankingLength,
                rankingCache = new int[rankingLength]
            };

            Array.Copy(SaveDataHolder.Data.CorrectAmountRanking, @new.rankingCache, @new.rankingLength);
            SortDescending(@new.rankingCache); // 一応ソートしておく

            return @new;
        }

        private int _correctAmount = 0;
        internal int CorrectAmount
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
        internal int GetRank()
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

        private static void SortDescending<T>(T[] array)
        {
            Array.Sort(array);
            Array.Reverse(array);
        }
    }
}