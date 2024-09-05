using UnityEngine;

namespace Main.Data
{
    internal sealed class GameData
    {
        private int _defeatedEnemyNum = 0;
        internal int DefeatedEnemyNum
        {
            get { return _defeatedEnemyNum; }
            set { _defeatedEnemyNum = Mathf.Clamp(value, 0, 999); }
        }

        private int _perfectlyDefeatedEnemyNum = 0;
        internal int PerfectlyDefeatedEnemyNum
        {
            get { return _perfectlyDefeatedEnemyNum; }
            set { _perfectlyDefeatedEnemyNum = Mathf.Clamp(value, 0, 999); }
        }

        internal void Reset()
        {
            _defeatedEnemyNum = 0;
            _perfectlyDefeatedEnemyNum = 0;
        }
    }
}