using UnityEngine;

namespace CDB.Audio
{
    /// <summary>
    /// Компонент для воспроизведения звуков на объекте.
    /// Используется для объектов, которым нужно воспроизводить звуки.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SoundEmitter : MonoBehaviour
    {
        [Header("Audio Sources")]
        [Tooltip("Для одиночных звуков (выстрел, удар)")]
        [SerializeField] private AudioSource _sfxSource;
        
        [Tooltip("Для зацикленных звуков (бормотание, шаги)")]
        [SerializeField] private AudioSource _loopSource;
        
        // Кэшированное значение громкости
        private float _volumeMultiplier = 1f;

        private void Awake()
        {
            if (_sfxSource == null)
                _sfxSource = GetComponent<AudioSource>();
                
            if (_loopSource == null)
            {
                // Создаем отдельный источник для лупов
                GameObject loopObj = new GameObject("LoopAudioSource");
                loopObj.transform.SetParent(transform);
                _loopSource = loopObj.AddComponent<AudioSource>();
                _loopSource.playOnAwake = false;
                _loopSource.loop = true;
            }
        }

        private void OnEnable()
        {
            SettingsManager.OnSFXVolumeChanged += OnVolumeChanged;
            SettingsManager.OnMasterVolumeChanged += OnMasterChanged;
            UpdateVolumeMultiplier();
        }

        private void OnDisable()
        {
            SettingsManager.OnSFXVolumeChanged -= OnVolumeChanged;
            SettingsManager.OnMasterVolumeChanged -= OnMasterChanged;
        }

        private void OnVolumeChanged(float value) => UpdateVolumeMultiplier();
        private void OnMasterChanged(float value) => UpdateVolumeMultiplier();

        private void UpdateVolumeMultiplier()
        {
            if (SettingsManager.Instance != null)
                _volumeMultiplier = SettingsManager.Instance.EffectiveSFXVolume;
        }

        /// <summary>
        /// Воспроизвести звук из SoundData
        /// </summary>
        public void PlaySound(SoundData soundData)
        {
            if (soundData == null || _sfxSource == null) return;
            
            AudioClip clip = soundData.GetRandomClip();
            if (clip == null) return;
            
            _sfxSource.pitch = soundData.GetRandomPitch();
            _sfxSource.PlayOneShot(clip, soundData.Volume * _volumeMultiplier);
        }

        /// <summary>
        /// Воспроизвести случайный клип из массива
        /// </summary>
        public void PlayRandomClip(AudioClip[] clips, float volume = 1f)
        {
            if (clips == null || clips.Length == 0 || _sfxSource == null) return;
            
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            _sfxSource.pitch = Random.Range(0.95f, 1.05f);
            _sfxSource.PlayOneShot(clip, volume * _volumeMultiplier);
        }

        /// <summary>
        /// Воспроизвести одиночный клип
        /// </summary>
        public void PlayOneShot(AudioClip clip, float volume = 1f)
        {
            if (clip == null || _sfxSource == null) return;
            
            _sfxSource.pitch = Random.Range(0.95f, 1.05f);
            _sfxSource.PlayOneShot(clip, volume * _volumeMultiplier);
        }

        /// <summary>
        /// Запустить зацикленный звук
        /// </summary>
        public void PlayLoop(SoundData soundData)
        {
            if (soundData == null || _loopSource == null) return;
            if (_loopSource.isPlaying && _loopSource.loop) return;
            
            AudioClip clip = soundData.GetRandomClip();
            if (clip == null) return;
            
            _loopSource.Stop();
            _loopSource.clip = clip;
            _loopSource.volume = soundData.Volume * _volumeMultiplier;
            _loopSource.loop = true;
            _loopSource.pitch = 1f;
            _loopSource.Play();
        }

        /// <summary>
        /// Запустить зацикленный звук из массива
        /// </summary>
        public void PlayLoopFromClips(AudioClip[] clips, float volume = 1f)
        {
            if (clips == null || clips.Length == 0 || _loopSource == null) return;
            if (_loopSource.isPlaying && _loopSource.loop) return;
            
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            
            _loopSource.Stop();
            _loopSource.clip = clip;
            _loopSource.volume = volume * _volumeMultiplier;
            _loopSource.loop = true;
            _loopSource.pitch = 1f;
            _loopSource.Play();
        }

        /// <summary>
        /// Остановить зацикленный звук
        /// </summary>
        public void StopLoop()
        {
            if (_loopSource != null && _loopSource.isPlaying)
            {
                _loopSource.Stop();
                _loopSource.loop = false;
                _loopSource.clip = null;
            }
        }

        /// <summary>
        /// Остановить все звуки
        /// </summary>
        public void StopAll()
        {
            if (_sfxSource != null) _sfxSource.Stop();
            StopLoop();
        }
    }
}
