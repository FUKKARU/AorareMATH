﻿using SO;
using UnityEngine;

namespace General
{
    internal enum SoundType
    {
        Master,
        BGM,
        SE
    }

    internal static class SoundManager
    {
        private static string GetAMGroupString(SoundType soundType)
        {
            return soundType switch
            {
                SoundType.Master => "MasterParam",
                SoundType.BGM => "BGMParam",
                SoundType.SE => "SEParam",
                _ => throw new System.Exception("不正な値です")
            };
        }

        internal static float GetVolume(SoundType soundType, out bool muted)
        {
            SO_Sound.Entity.AudioMixer.GetFloat(GetAMGroupString(soundType), out float volume);
            muted = volume <= SO_Handler.Entity.MinVolume;
            return volume;
        }

        internal static void SetVolume(SoundType soundType, float newVolume, out bool muted)
        {
            muted = false;
            if (newVolume <= SO_Handler.Entity.MinVolume)
            {
                newVolume = -80;
                muted = true;
            }

            SO_Sound.Entity.AudioMixer.SetFloat(GetAMGroupString(soundType), newVolume);
        }

        internal static void Raise
            (this AudioSource source, AudioClip clip, SoundType type, float volume = 1, float pitch = 1)
        {
            if (source == null) return;
            if (clip == null) return;

            source.volume = volume;
            source.pitch = pitch;

            if (type == SoundType.BGM)
            {
                source.clip = clip;
                source.outputAudioMixerGroup = SO_Sound.Entity.AMGroupBGM;
                source.playOnAwake = false;
                source.loop = true;
                source.Play();
            }
            else if (type == SoundType.SE)
            {
                source.outputAudioMixerGroup = SO_Sound.Entity.AMGroupSE;
                source.playOnAwake = false;
                source.loop = false;
                source.PlayOneShot(clip);
            }
            else
            {
                throw new System.Exception("Masterは使えません");
            }
        }
    }
}