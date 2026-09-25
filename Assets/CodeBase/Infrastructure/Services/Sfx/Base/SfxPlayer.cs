using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace _Root._Scripts.Infrastructure.Services.Sfx.Base
{
    public class SfxPlayer : ISfxPlayer ,IDisposable
    {
        private List<AudioSource> _pool = new List<AudioSource>();
        private List<AudioSource> _available = new List<AudioSource>();
        private AudioMixerGroup _sfxGroup;
        private readonly ICoroutineRunner _coroutineRunner;
        private GameObject _parentObject;

        public SfxPlayer(AudioMixerGroup sfxGroup,ICoroutineRunner coroutineRunner,  int poolSize = 100)
        {
            _sfxGroup = sfxGroup;
            _coroutineRunner = coroutineRunner;
            InitializePool(poolSize);
        }

        private void InitializePool(int poolSize)
        {
            _parentObject = new GameObject("SfxPlayer_Pool");
            UnityEngine.Object.DontDestroyOnLoad(_parentObject);

            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = new GameObject($"SfxSource_{i}");
                obj.transform.SetParent(_parentObject.transform);
                
                AudioSource source = obj.AddComponent<AudioSource>();
                if (_sfxGroup != null)
                    source.outputAudioMixerGroup = _sfxGroup;
                
                source.playOnAwake = false;
                source.spatialBlend = 0f;
                
                _pool.Add(source);
                _available.Add(source);
            }
        }

        public void PlaySfxByRange(List<AudioClip> range, float pitch = 1f,bool isLoop = false)
        {
            if (range != null)
            {
                AudioClip randomClip = range[Random.Range(0, range.Count)];
                PlaySfx(randomClip,pitch,isLoop);
            }
        }

        public void PlaySfxByRange(List<AudioClip> range, Vector3 position, float pitch = 1,bool isLoop = false)
        {
            if (range != null)
            {
                AudioClip randomClip = range[Random.Range(0, range.Count)];
                PlaySfx(randomClip,position,pitch,isLoop);
            }
        }

        public void SetSfxGroup(AudioMixerGroup sfxGroup)
        {
            _sfxGroup = sfxGroup;
            foreach (var source in _pool)
            {
                source.outputAudioMixerGroup = _sfxGroup;
            }
        }

        public void PlaySfx(AudioClip clip,float pitch = 1f,bool isLoop = false)
        {
            Vector3 listenerPosition = Vector3.zero;
            PlaySfx(clip, listenerPosition,pitch,isLoop);
        }

        public void PlaySfx(AudioClip clip, Vector3 position,float pitch = 1f,bool isLoop = false,float volume = 1f)
        {
            if (clip == null)
            {
                Debug.LogWarning("[SfxPlayer] AudioClip is null");
                return;
            }

            if (_available.Count == 0)
            {
                Debug.LogWarning("[SfxPlayer] No available audio sources");
                return;
            }

            AudioSource source = _available[0];
            _available.RemoveAt(0);

            source.outputAudioMixerGroup = _sfxGroup;
            source.transform.position = position;
            source.loop = isLoop;
            source.pitch = pitch;
            source.volume = volume;
            source.PlayOneShot(clip);

            _coroutineRunner.StartCoroutine(ReturnToPool(source));
        }

        private IEnumerator ReturnToPool(AudioSource source)
        {
            while (source.isPlaying)
                yield return null;

            if (!_available.Contains(source))
                _available.Add(source);
        }

        public void Dispose()
        {
            if (_parentObject != null)
                UnityEngine.Object.Destroy(_parentObject);
        }
    }
}