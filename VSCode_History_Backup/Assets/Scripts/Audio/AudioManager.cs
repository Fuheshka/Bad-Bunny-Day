using UnityEngine;
using System.Collections.Generic;
using CDB.Core;

namespace CDB.Audio
{
    /// <summary>
    /// Менеджер звуков - централизованное управление аудио.
    /// Синглтон для воспроизведения звуков из любого места.
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;
        
        [Header("Pooling")]
        [SerializeField] private int _poolSize = 10;
        
        private List<AudioSource> _sfxPool = new List<AudioSource>();
        
        // Кэшированные значения громкости
        private float _masterVolume = 1f;
        private float _musicVolume = 1f;
        private float _sfxVolume = 1f;

        protected override void Awake()
        {
            base.Awake();
            InitializePool();
            LoadVolumeSettings();
        }

        private void OnEnable()
        {
            SettingsManager.OnMasterVolumeChanged += OnMasterVolumeChanged;
            SettingsManager.OnMusicVolumeChanged += OnMusicVolumeChanged;
            SettingsManager.OnSFXVolumeChanged += OnSFXVolumeChanged;
        }

        private void OnDisable()
        {
            SettingsManager.OnMasterVolumeChanged -= OnMasterVolumeChanged;
            SettingsManager.OnMusicVolumeChanged -= OnMusicVolumeChanged;
            SettingsManager.OnSFXVolumeChanged -= OnSFXVolumeChanged;
        }

        private void LoadVolumeSettings()
        {
            if (SettingsManager.Instance != null)
            {
                _masterVolume = SettingsManager.Instance.MasterVolume;
                _musicVolume = SettingsManager.Instance.MusicVolume;
                _sfxVolume = SettingsManager.Instance.SFXVolume;
            }
        }

        private void OnMasterVolumeChanged(float value)
        {
            _masterVolume = value;
            UpdateMusicVolume();
        }

        private void OnMusicVolumeChanged(float value)
        {
            _musicVolume = value;
            UpdateMusicVolume();
        }

        private void OnSFXVolumeChanged(float value)
        {
            _sfxVolume = value;
        }

        private void UpdateMusicVolume()
        {
            if (_musicSource != null)
            {
                _musicSource.volume = _masterVolume * _musicVolume;
            }
        }

        private void InitializePool()
        {
            for (int i = 0; i < _poolSize; i++)
            {
                CreatePooledSource();
            }
        }

        private AudioSource CreatePooledSource()
        {
            GameObject obj = new GameObject("PooledAudioSource");
            obj.transform.SetParent(transform);
            AudioSource source = obj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            _sfxPool.Add(source);
            return source;
        }

        private AudioSource GetPooledSource()
        {
            foreach (var source in _sfxPool)
            {
                if (!source.isPlaying)
                    return source;
            }
            return CreatePooledSource();
        }

        /// <summary>
        /// Воспроизвести звук из SoundData
        /// </summary>
        public void PlaySound(SoundData soundData)
        {
            if (soundData == null) return;
            
            AudioClip clip = soundData.GetRandomClip();
            if (clip == null) return;
            
            AudioSource source = GetPooledSource();
            source.clip = clip;
            source.volume = soundData.Volume * _masterVolume * _sfxVolume;
            source.pitch = soundData.GetRandomPitch();
            source.loop = soundData.Loop;
            source.spatialBlend = soundData.SpatialBlend;
            source.minDistance = soundData.MinDistance;
            source.maxDistance = soundData.MaxDistance;
            source.Play();
        }

        /// <summary>
        /// Воспроизвести звук на позиции (3D)
        /// </summary>
        public void PlaySoundAtPosition(SoundData soundData, Vector3 position)
        {
            if (soundData == null) return;
            
            AudioClip clip = soundData.GetRandomClip();
            if (clip == null) return;
            
            AudioSource source = GetPooledSource();
            source.transform.position = position;
            source.clip = clip;
            source.volume = soundData.Volume * _masterVolume * _sfxVolume;
            source.pitch = soundData.GetRandomPitch();
            source.loop = soundData.Loop;
            source.spatialBlend = soundData.SpatialBlend;
            source.minDistance = soundData.MinDistance;
            source.maxDistance = soundData.MaxDistance;
            source.Play();
        }

        /// <summary>
        /// Воспроизвести одиночный клип (OneShot)
        /// </summary>
        public void PlayOneShot(AudioClip clip, float volumeScale = 1f)
        {
            if (clip == null || _sfxSource == null) return;
            _sfxSource.PlayOneShot(clip, volumeScale * _masterVolume * _sfxVolume);
        }

        /// <summary>
        /// Воспроизвести музыку
        /// </summary>
        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (_musicSource == null || clip == null) return;
            
            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.volume = _masterVolume * _musicVolume;
            _musicSource.Play();
        }

        /// <summary>
        /// Остановить музыку
        /// </summary>
        public void StopMusic()
        {
            if (_musicSource != null)
                _musicSource.Stop();
        }

        /// <summary>
        /// Плавное затухание музыки
        /// </summary>
        public void FadeOutMusic(float duration = 1f)
        {
            if (_musicSource != null)
                StartCoroutine(FadeOutRoutine(_musicSource, duration));
        }

        private System.Collections.IEnumerator FadeOutRoutine(AudioSource source, float duration)
        {
            float startVolume = source.volume;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
                yield return null;
            }

            source.Stop();
            source.volume = startVolume;
        }
    }
}
