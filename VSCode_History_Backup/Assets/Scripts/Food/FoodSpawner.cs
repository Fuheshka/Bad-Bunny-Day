using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CDB.Food
{
    /// <summary>
    /// Спавнер еды - создаёт съедобные объекты в заданной области.
    /// </summary>
    public class FoodSpawner : MonoBehaviour
    {
        [Header("Настройки зоны")]
        [SerializeField] private BoxCollider _spawnArea;

        [Header("Настройки еды")]
        [SerializeField] private GameObject[] _foodPrefabs;
        [SerializeField] private int _maxFoodCount = 20;
        [SerializeField] private float _spawnInterval = 2f;

        [Header("Физика и Земля")]
        [Tooltip("Слой земли (чтобы луч находил поверхность)")]
        [SerializeField] private LayerMask _groundLayer;
        
        [Tooltip("Слой препятствий (камни), где спавнить НЕЛЬЗЯ")]
        [SerializeField] private LayerMask _obstacleLayer;
        
        [Tooltip("Приподнять еду над землей")]
        [SerializeField] private float _spawnHeightOffset = 0.5f;

        [Tooltip("Радиус проверки препятствий")]
        [SerializeField] private float _checkRadius = 0.5f;

        [Header("Настройки спавна")]
        [SerializeField] private int _maxSpawnAttempts = 10;

        // Для обратной совместимости (публичные поля)
        public BoxCollider spawnArea { get => _spawnArea; set => _spawnArea = value; }
        public GameObject[] foodPrefabs { get => _foodPrefabs; set => _foodPrefabs = value; }
        public int maxFoodCount { get => _maxFoodCount; set => _maxFoodCount = value; }
        public float spawnInterval { get => _spawnInterval; set => _spawnInterval = value; }
        public LayerMask groundLayer { get => _groundLayer; set => _groundLayer = value; }
        public LayerMask obstacleLayer { get => _obstacleLayer; set => _obstacleLayer = value; }
        public float spawnHeightOffset { get => _spawnHeightOffset; set => _spawnHeightOffset = value; }
        public float checkRadius { get => _checkRadius; set => _checkRadius = value; }

        private List<GameObject> _currentFoodObjects = new List<GameObject>();

        private void Start()
        {
            if (_spawnArea == null)
                _spawnArea = GetComponent<BoxCollider>();
                
            StartCoroutine(SpawnRoutine());
        }

        private IEnumerator SpawnRoutine()
        {
            while (true)
            {
                _currentFoodObjects.RemoveAll(item => item == null);

                if (_currentFoodObjects.Count < _maxFoodCount)
                {
                    SpawnFood();
                }
                yield return new WaitForSeconds(_spawnInterval);
            }
        }

        private void SpawnFood()
        {
            if (_foodPrefabs == null || _foodPrefabs.Length == 0) return;

            for (int i = 0; i < _maxSpawnAttempts; i++)
            {
                Vector3? spawnPoint = FindValidSpawnPoint();
                if (spawnPoint.HasValue)
                {
                    SpawnFoodAt(spawnPoint.Value);
                    return;
                }
            }
        }

        private Vector3? FindValidSpawnPoint()
        {
            Bounds bounds = _spawnArea.bounds;
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float z = Random.Range(bounds.min.z, bounds.max.z);
            
            Vector3 rayStart = new Vector3(x, bounds.center.y + 20f, z);

            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 100f, _groundLayer))
            {
                Vector3 spawnPoint = hit.point;

                if (!Physics.CheckSphere(spawnPoint, _checkRadius, _obstacleLayer))
                {
                    spawnPoint.y += _spawnHeightOffset;
                    return spawnPoint;
                }
            }

            return null;
        }

        private void SpawnFoodAt(Vector3 position)
        {
            GameObject prefab = _foodPrefabs[Random.Range(0, _foodPrefabs.Length)];
            GameObject newFood = Instantiate(prefab, position, Quaternion.identity);
            newFood.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            _currentFoodObjects.Add(newFood);
        }

        public void SpawnImmediate(int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                SpawnFood();
            }
        }

        public void ClearAllFood()
        {
            foreach (var food in _currentFoodObjects)
            {
                if (food != null)
                    Destroy(food);
            }
            _currentFoodObjects.Clear();
        }

        private void OnDrawGizmos()
        {
            if (_spawnArea != null)
            {
                Gizmos.color = new Color(0, 1, 0, 0.2f);
                Gizmos.DrawCube(_spawnArea.bounds.center, _spawnArea.bounds.size);
            }
        }
    }
}