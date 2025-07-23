using UnityEngine;
using SO;

namespace General
{
    internal sealed class BGMPlayer : AFadeableBgmPlayer
    {
        [SerializeField] private Scene thisScene; // 現在のシーンにあったBGMを鳴らすため
        [SerializeField] private bool playOnAwake;

        private bool hasPlayed = false;

        private void Awake()
        {
            if (playOnAwake)
                Play();
        }

        internal void Play()
        {
            if (hasPlayed) return;
            if (audioSource != null) return;
            hasPlayed = true;

            audioSource = AudioSourceManager.Instance.Play(clip, SoundType.BGM);
        }

        private AudioClip clip => thisScene switch
        {
            Scene.Title => SO_Sound.Entity.TitleBGM,
            Scene.Main => SO_Sound.Entity.MainBGM,
            _ => null
        };
    }
}