using UnityEngine;
using CDB.Audio;

namespace CDB.Enemy.Components
{
    /// <summary>
    /// Компонент звуков врага.
    /// Управляет воспроизведением звуков для разных состояний.
    /// </summary>
    [RequireComponent(typeof(SoundEmitter))]
    public class EnemySounds : MonoBehaviour
    {
        [Header("Audio Clips")]
        [SerializeField] private AudioClip[] _fireClips;
        [SerializeField] private AudioClip[] _reactToNoiseClips;
        [SerializeField] private AudioClip[] _searchingClips;
        [SerializeField] private AudioClip[] _giveUpClips;
        [SerializeField] private AudioClip[] _dodgeClips;
        [SerializeField] private AudioClip[] _damageRabbitClips;

        [Header("Volume Settings")]
        [Range(0f, 1f)] public float fireVolume = 0.5f;
        [Range(0f, 1f)] public float reactVolume = 0.7f;
        [Range(0f, 1f)] public float searchVolume = 0.3f;
        [Range(0f, 1f)] public float giveUpVolume = 0.5f;
        [Range(0f, 1f)] public float dodgeVolume = 0.6f;
        [Range(0f, 1f)] public float damageVolume = 0.8f;

        private SoundEmitter _soundEmitter;

        private void Awake()
        {
            _soundEmitter = GetComponent<SoundEmitter>();
        }

        public void PlayFire() => _soundEmitter.PlayRandomClip(_fireClips, fireVolume);
        public void PlayReactToNoise() => _soundEmitter.PlayRandomClip(_reactToNoiseClips, reactVolume);
        public void PlaySearching() => _soundEmitter.PlayLoopFromClips(_searchingClips, searchVolume);
        public void PlayGiveUp() => _soundEmitter.PlayRandomClip(_giveUpClips, giveUpVolume);
        public void PlayDodge() => _soundEmitter.PlayRandomClip(_dodgeClips, dodgeVolume);
        public void PlayDamageRabbit() => _soundEmitter.PlayRandomClip(_damageRabbitClips, damageVolume);
        
        public void StopSearching() => _soundEmitter.StopLoop();
        public void StopAll() => _soundEmitter.StopAll();
    }
}
