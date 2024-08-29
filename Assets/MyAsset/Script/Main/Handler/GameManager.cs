using Main.Data;
using Main.Data.Formula;
using UnityEngine;

namespace Main.Handler
{
    internal enum GameState
    {
        Stay,
        OnGoing,
        Over
    }

    internal sealed class GameManager : MonoBehaviour
    {
        #region
        internal static GameManager Instance { get; set; } = null;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        #endregion

        internal GameState State { get; set; }
        internal GameData GameData { get; set; }
        internal Formula Formula { get; set; }

        private void Start()
        {
            State = GameState.Stay;

            GameData = new();
            GameData.Reset();

            Formula = new();
            Formula.Reset();
        }

        private void Update()
        {

        }
    }
}