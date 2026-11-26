using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FoodSpawner : MonoBehaviour
{
    [Header("Настройки зоны")]
    public BoxCollider spawnArea;

    [Header("Настройки еды")]
    public GameObject[] foodPrefabs;
    public int maxFoodCount = 20;
    public float spawnInterval = 2f;

    [Header("Физика и Земля")]
    [Tooltip("Слой земли (чтобы луч находил поверхность)")]
    public LayerMask groundLayer;
    
    [Tooltip("Слой препятствий (камни), где спавнить НЕЛЬЗЯ")]
    public LayerMask obstacleLayer;
    
    [Tooltip("Приподнять еду над землей (чтобы не застряла в текстурах)")]
    public float spawnHeightOffset = 0.5f;

    [Tooltip("Радиус проверки препятствий")]
    public float checkRadius = 0.5f;

    private List<GameObject> currentFoodObjects = new List<GameObject>();

    void Start()
    {
        if (spawnArea == null) spawnArea = GetComponent<BoxCollider>();
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            currentFoodObjects.RemoveAll(item => item == null);

            if (currentFoodObjects.Count < maxFoodCount)
            {
                SpawnFood();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnFood()
    {
        if (foodPrefabs.Length == 0) return;

        for (int i = 0; i < 10; i++) // 10 попыток найти место
        {
            // 1. Берем случайные X и Z внутри зоны
            Bounds bounds = spawnArea.bounds;
            float x = Random.Range(bounds.min.x, bounds.max.x);
            float z = Random.Range(bounds.min.z, bounds.max.z);
            
            // Начинаем пускать луч высоко сверху (например, Y + 20 от центра зоны)
            Vector3 rayStart = new Vector3(x, bounds.center.y + 20f, z);

            // 2. Пускаем луч ВНИЗ, ищем ТОЛЬКО землю
            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 100f, groundLayer))
            {
                Vector3 spawnPoint = hit.point;

                // 3. Проверяем, нет ли в этой точке препятствий (камней)
                if (!Physics.CheckSphere(spawnPoint, checkRadius, obstacleLayer))
                {
                    // Место хорошее! 
                    GameObject prefab = foodPrefabs[Random.Range(0, foodPrefabs.Length)];

                    // Добавляем отступ вверх, чтобы объект не заспавнился внутри полигона земли
                    spawnPoint.y += spawnHeightOffset;

                    GameObject newFood = Instantiate(prefab, spawnPoint, Quaternion.identity);

                    // Случайный поворот вокруг оси Y
                    newFood.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

                    currentFoodObjects.Add(newFood);
                    return;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (spawnArea != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.2f);
            Gizmos.DrawCube(spawnArea.bounds.center, spawnArea.bounds.size);
        }
    }
}