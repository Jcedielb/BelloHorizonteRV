using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [Header("Configuración")]
    public Transform cameraTransform; // La cámara XR
    public float distanceFromCamera = 2f; // Distancia frente a la cámara
    public float heightOffset = 0f; // Altura relativa (0 = al nivel de ojos)
    public bool smoothFollow = true; // Seguir suavemente
    public float smoothSpeed = 5f; // Velocidad de seguimiento

    void Start()
    {
        // Si no se asignó la cámara, buscarla automáticamente
        if (cameraTransform == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                cameraTransform = mainCam.transform;
            }
        }
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // Calcular posición frente a la cámara
        UnityEngine.Vector3 targetPosition = cameraTransform.position +
                                             cameraTransform.forward * distanceFromCamera;

        // Añadir offset de altura si es necesario
        targetPosition.y += heightOffset;

        // Mover el canvas
        if (smoothFollow)
        {
            // Seguimiento suave
            transform.position = UnityEngine.Vector3.Lerp(
                transform.position,
                targetPosition,
                smoothSpeed * Time.deltaTime
            );
        }
        else
        {
            // Seguimiento instantáneo
            transform.position = targetPosition;
        }

        // Hacer que el canvas SIEMPRE mire hacia la cámara
        transform.LookAt(cameraTransform);

        // Rotar 180° para que el texto se vea de frente (no al revés)
        transform.Rotate(0, 180, 0);
    }
}
