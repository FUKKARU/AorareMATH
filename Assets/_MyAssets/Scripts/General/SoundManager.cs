using SO;
using AmParamTable = System.Collections.Generic.Dictionary<General.SoundType, string>;

namespace General
{
    internal enum SoundType : byte
    {
        Master,
        BGM,
        SE
    }

    // セーブの処理は行わない
    internal static class SoundManager
    {
        private static readonly float mutedVolume = -80.0f;

        private static readonly AmParamTable amParamTable = new()
        {
            { SoundType.Master, "MasterParam" },
            { SoundType.BGM, "BGMParam" },
            { SoundType.SE, "SEParam" }
        };

        internal static bool IsMuted(SoundType soundType)
            => GetVolume(soundType) <= SO_Handler.Entity.MinVolume;

        internal static float GetVolume(SoundType soundType)
        {
            if (!amParamTable.TryGetValue(soundType, out string param))
            {
                $"param not found for sound type: {soundType}".LogError();
                return 0;
            }

            SO_Sound.Entity.AudioMixer.GetFloat(param, out float volume);
            return volume;
        }

        internal static void SetVolume(SoundType soundType, float newVolume)
        {
            if (!amParamTable.TryGetValue(soundType, out string param))
            {
                $"param not found for sound type: {soundType}".LogError();
                return;
            }

            if (newVolume <= SO_Handler.Entity.MinVolume)
                newVolume = mutedVolume;

            SO_Sound.Entity.AudioMixer.SetFloat(param, newVolume);
        }

        internal static void Play
            (this AudioSource source, AudioClip clip, SoundType type, float volume = 1, float pitch = 1, float time = 0)
        {
            if (source == null) return;
            if (clip == null) return;

            source.playOnAwake = false;

            source.clip = clip;
            source.volume = volume;
            source.pitch = pitch;
            source.time = time;

            if (type == SoundType.BGM)
            {
                source.outputAudioMixerGroup = SO_Sound.Entity.AMGroupBGM;
                source.loop = true;
            }
            else if (type == SoundType.SE)
            {
                source.outputAudioMixerGroup = SO_Sound.Entity.AMGroupSE;
                source.loop = false;
            }
            else
                return;

            source.Play();
        }
    }
}