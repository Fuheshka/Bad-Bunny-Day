using UnityEngine;

public class EdibleObject : MonoBehaviour
{
    [Header("Параметры еды")]
    [Tooltip("Насколько увеличится насыщение (0.0 - 1.0)")]
    public float nutritionValue = 0.1f;
    
    [Tooltip("Сколько секунд удерживать кнопку E")]
    public float eatTime = 1.5f;

    [Tooltip("Очки за съедение")]
    public int scoreValue = 100;
}