using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace _Root._Scripts.Infrastructure.Services.Sfx.Base
{
    public interface ISfxPlayer
    {
        void PlaySfx(AudioClip clip,float pitch = 1f,bool isLoop = false);
        void PlaySfx(AudioClip clip, Vector3 position,float pitch = 1f,bool isLoop = false,float volume = 1f);

        void PlaySfxByRange(List<AudioClip> range, float pitch = 1f,bool isLoop = false);
        void PlaySfxByRange(List<AudioClip> range,Vector3 position, float pitch = 1f, bool isLoop = false);
        void SetSfxGroup(AudioMixerGroup sfxGroup);
    }
}