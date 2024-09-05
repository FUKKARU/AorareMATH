using General;
using SO;
using UnityEngine;

namespace Main.Handler
{
    internal sealed class BGMPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;

        private bool isDone = false;

        private void Update()
        {
            if (isDone) return;

            if (GameManager.Instance.State == GameState.OnGoing)
            {
                isDone = true;

                if (audioSource == null) return;

                audioSource.Raise(SO_Sound.Entity.MainBGM, SoundType.BGM);
            }
        }

        private void OnDisable()
        {
            audioSource = null;
        }
    }
}