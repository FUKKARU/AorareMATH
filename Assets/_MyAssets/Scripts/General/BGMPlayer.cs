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

            // タイトルシーンのBGMファイルは再生開始まで遅延があるので、演出スキップの時は少し早めて再生する
            float time = (thisScene == Scene.Title && SaveDataHolder.CacheData.DoFastenDirections) ? 1.0f : 0.0f;
            audioSource = AudioSourceManager.Instance.Play(clip, SoundType.BGM, time: time);
        }

        private AudioClip clip => thisScene switch
        {
            Scene.Title => SO_Sound.Entity.TitleBGM,
            Scene.Main => SO_Sound.Entity.MainBGM,
            _ => null
        };
    }
}