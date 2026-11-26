using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 3, -3);
    
    [Header("Look Settings")]
    [SerializeField] private bool lookAtTarget = true;

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
        
        transform.position = smoothedPosition;

        if (lookAtTarget)
        {
            transform.LookAt(target.position + Vector3.up * 0.5f);
        }
    }
}
