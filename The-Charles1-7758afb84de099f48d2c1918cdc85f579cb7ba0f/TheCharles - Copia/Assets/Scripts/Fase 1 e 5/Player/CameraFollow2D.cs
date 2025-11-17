using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [SerializeField] private Transform target; // O Player
    [SerializeField] private float smoothSpeed = 0.125f; // Suavização
    [SerializeField] private Vector3 offset; // Offset da câmera

    void LateUpdate()
    {
        if (target == null) return;

        // Posição desejada com offset
        Vector3 desiredPosition = target.position + offset;

        // Suaviza a movimentação
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Aplica na câmera
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }
}
