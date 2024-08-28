using UnityEngine;

namespace Main.Handler
{
    internal sealed class GameManager : MonoBehaviour
    {
        internal static GameManager Instance { get; set; } = null;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        private void Start()
        {
           
        }
        private void Update()
        {

        }
    }
}