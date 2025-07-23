using System;
using UnityEngine;

namespace General
{
    // 全てのシーンに配置する
    internal sealed class AudioSourceManager : ASingletonMonoBehaviour<AudioSourceManager>
    {
        private static readonly int MaxAmount = 100;

        private AudioSource[] audioSources = null;
        private int currentIndex = 0;

        private void Awake()
        {
            audioSources = new AudioSource[MaxAmount];

            for (int i = 0; i < MaxAmount; ++i)
            {
                AudioSource @new = this.gameObject.AddComponent<AudioSource>();

                if (@new != null)
                {
                    @new.playOnAwake = false;

                    audioSources[i] = @new;
                }
            }
        }

        // 見つかった AudioSource を返す. 再生状態・このメソッドの引数にあるメンバのみ、書き換えても良い (このメソッドでリセットできるため).
        internal AudioSource Play(AudioClip clip, SoundType type, float volume = 1, float pitch = 1, float time = 0)
        {
            if (clip == null) return null;
            if (audioSources == null || audioSources.Length <= 0) return null;

            AudioSource found = null;
            for (int i = 0; i < MaxAmount; ++i)
            {
                currentIndex = (currentIndex + 1) % MaxAmount;
                AudioSource target = audioSources[currentIndex];

                if (target == null || target.isPlaying) continue;
                found = target;
                break;
            }

            if (found == null)
            {
                UnityEngine.Debug.LogWarning("All audio sources are busy. Cannot play sound.");
                return null;
            }

            found.Play(clip, type, volume, pitch, time);
            return found;
        }

        private void OnDestroy()
        {
            if (audioSources != null)
            {
                Array.Clear(audioSources, 0, audioSources.Length);
                audioSources = null;
            }
        }
    }
}