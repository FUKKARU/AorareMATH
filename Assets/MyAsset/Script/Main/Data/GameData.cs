using UnityEngine;
using System.Collections.Generic;
using CI.QuickSave;
using unityroom.Api;

namespace Main.Data
{
    internal sealed class GameData
    {
        private int _correctAmount = 0;
        private static readonly int CorrectAmountRankingMax = 20;
        private readonly List<int> correctAmoutRanking;

        private readonly QuickSaveReader reader;
        private readonly QuickSaveWriter writer;

        private static readonly string AesKey = "Iput_Fukkaru";
        private static readonly string RootPath = "userData";
        private static readonly string CorrectAmountRankingKey = "correctAmountRanking";


        internal GameData()
        {
            // //クイックセーブのインスタンスの作成
            // var settings = new QuickSaveSettings()
            // {
            //     //暗号化の方法
            //     SecurityMode = SecurityMode.Aes,
            //     //Aesの暗号化キー
            //     Password = AesKey,
            //     //圧縮方法
            //     CompressionMode = CompressionMode.Gzip
            // };

            // reader = QuickSaveReader.Create(RootPath, settings);
            // writer = QuickSaveWriter.Create(RootPath, settings);
            // correctAmoutRanking = new List<int>();
            // for(int i = 0; i < CorrectAmountRankingMax; i++)
            // {
            //     correctAmoutRanking.Add(0);
            // }

            // if (reader.Exists(RootPath))
            // {
            //     correctAmoutRanking = reader.Read<List<int>>(CorrectAmountRankingKey);
            // }
            // foreach(int i in correctAmoutRanking)
            // {
            //     UnityEngine.Debug.Log(i);
            // }
        }

        internal void Save()
        {
            correctAmoutRanking.Add(_correctAmount);
            correctAmoutRanking.Sort((a, b) => b - a);
            correctAmoutRanking.RemoveAt(CorrectAmountRankingMax);
            writer.Write(CorrectAmountRankingKey, correctAmoutRanking);
            writer.Commit();
        }
        internal int CorrectAmount
        {
            get { return _correctAmount; }
            set { _correctAmount = Mathf.Clamp(value, 0, 999); }
        }

        internal void Reset() => _correctAmount = 0;

        private void SendToUnityroom()
        {
            UnityroomApiClient.Instance.SendScore(1, _correctAmount, ScoreboardWriteMode.HighScoreDesc);
        }
    }
}