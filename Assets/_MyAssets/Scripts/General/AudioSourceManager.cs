namespace General
{
    // 全てのシーンに配置する
    internal sealed class AudioSourceManager : ASingletonMonoBehaviour<AudioSourceManager>
    {
        private int MaxAmount = 64;

        private AudioSource[] audioSources = null;
        private int currentIndex = 0;

        private void Awake()
        {
            audioSources = new AudioSource[MaxAmount];

            for (int i = 0; i < MaxAmount; ++i)
                audioSources[i] = AddNewAudioSource(this.gameObject);
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

            // 全て埋まっていたら、サイズを2倍に増やし、それも上限に達したら再生しない
            if (found == null)
            {
                if (MaxAmount <= (int.MaxValue >> 1))
                {
                    $"All AudioSources are in use. Doubled the maximum amount, then playing the sound."
                        .LogWarning();

                    int lastAmount = MaxAmount;
                    MaxAmount <<= 1;

                    Array.Resize(ref audioSources, MaxAmount);
                    for (int i = lastAmount; i < MaxAmount; ++i)
                        audioSources[i] = AddNewAudioSource(this.gameObject);

                    currentIndex = lastAmount;
                    found = audioSources[currentIndex];
                    if (found == null)
                    {
                        "Failed to create a new AudioSource. Cannot play the sound."
                            .LogError();
                        return null;
                    }
                    found.Play(clip, type, volume, pitch, time);
                    return found;
                }
                else
                {
                    "All AudioSources are in use. Tried to double the maximum amount, but it has reached the limit. Cannot play the sound."
                        .LogError();
                    return null;
                }
            }

            found.Play(clip, type, volume, pitch, time);
            return found;
        }

        private static AudioSource AddNewAudioSource(GameObject go)
        {
            if (go == null) return null;

            AudioSource @new = go.AddComponent<AudioSource>();
            if (@new != null)
                @new.playOnAwake = false;

            return @new;
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