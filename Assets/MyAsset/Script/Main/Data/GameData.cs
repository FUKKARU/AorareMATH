using UnityEngine;
using unityroom.Api;

namespace Main.Data
{
    internal sealed class GameData
    {
        private int _correctAmount = 0;
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