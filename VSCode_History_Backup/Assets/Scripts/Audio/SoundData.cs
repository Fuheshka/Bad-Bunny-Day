using UnityEngine;

namespace CDB.Audio
{
    /// <summary>
    /// ScriptableObject для хранения звуковых эффектов.
    /// Позволяет централизованно управлять звуками.
    /// </summary>
    [CreateAssetMenu(fileName = "SoundData", menuName = "CDB/Audio/Sound Data")]
    public class SoundData : ScriptableObject
    {
        [Header("Информация")]
        [SerializeField] private string _soundName;
        
        [Header("Клипы")]
        [Tooltip("Массив аудио клипов (случайный выбор при воспроизведении)")]
        [SerializeField] private AudioClip[] _clips;
        
        [Header("Настройки")]
        [Range(0f, 1f)]
        [SerializeField] private float _volume = 1f;
        
        [Range(0.1f, 3f)]
        [SerializeField] private float _pitchMin = 0.95f;
        
        [Range(0.1f, 3f)]
        [SerializeField] private float _pitchMax = 1.05f;
        
        [SerializeField] private bool _loop;
        
        [Header("3D Sound Settings")]
        [SerializeField] private float _spatialBlend = 0f; // 0 = 2D, 1 = 3D
        [SerializeField] private float _minDistance = 1f;
        [SerializeField] private float _maxDistance = 50f;

        public string SoundName => _soundName;
        public AudioClip[] Clips => _clips;
        public float Volume => _volume;
        public float PitchMin => _pitchMin;
        public float PitchMax => _pitchMax;
        public bool Loop => _loop;
        public float SpatialBlend => _spatialBlend;
        public float MinDistance => _minDistance;
        public float MaxDistance => _maxDistance;

        /// <summary>
        /// Получить случайный клип
        /// </summary>
        public AudioClip GetRandomClip()
        {
            if (_clips == null || _clips.Length == 0) return null;
            return _clips[Random.Range(0, _clips.Length)];
        }

        /// <summary>
        /// Получить случайный питч
        /// </summary>
        public float GetRandomPitch()
        {
            return Random.Range(_pitchMin, _pitchMax);
        }
    }
}
