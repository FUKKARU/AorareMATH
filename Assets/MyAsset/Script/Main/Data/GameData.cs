using UnityEngine;

namespace Main.Data
{
    internal sealed class GameData
    {
        private int _defeatedEnemyNum = 0;
        internal int DefeatedEnemyNum
        {
            get { return _defeatedEnemyNum; }
            set { _defeatedEnemyNum = Mathf.Clamp(value, ushort.MinValue, ushort.MaxValue); }
        }

        internal void Reset()
        {
            _defeatedEnemyNum = 0;
        }
    }
}