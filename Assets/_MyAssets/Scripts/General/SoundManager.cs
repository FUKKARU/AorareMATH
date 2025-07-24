using UnityEngine;
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

    internal static class SoundManager
    {
        private static readonly AmParamTable amParamTable = new()
        {
            { SoundType.Master, "MasterParam" },
            { SoundType.BGM, "BGMParam" },
            { SoundType.SE, "SEParam" }
        };

        // 閾値以下のボリュームであったら、muted が true になる
        internal static float GetVolume(SoundType soundType, out bool muted)
        {
            if (!amParamTable.TryGetValue(soundType, out string param))
            {
                muted = true;
                return 0.0f;
            }

            SO_Sound.Entity.AudioMixer.GetFloat(param, out float volume);
            muted = volume <= SO_Handler.Entity.MinVolume;
            return volume;
        }

        // 閾値以下のボリュームがセットされたら、muted が true になる
        internal static void SetVolume(SoundType soundType, float newVolume, out bool muted)
        {
            if (!amParamTable.TryGetValue(soundType, out string param))
            {
                muted = true;
                return;
            }

            muted = false;
            if (newVolume <= SO_Handler.Entity.MinVolume)
            {
                newVolume = -80.0f;
                muted = true;
            }

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