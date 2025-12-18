using UnityEngine;

public class BeachBoundaryWarning : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject warningPrefab;
    public float pushBackForce = 5f;
    public AudioClip warningSound;

    [Header("Referencias XR")]
    public Transform xrOrigin;
    public CharacterController playerController;
    public Transform cameraTransform;

    [Header("Posicionamiento del Warning")]
    public float warningDistance = 2f;
    public float warningHeightOffset = 0f;
    public float warningScale = 2f;

    [Header("Rotación del Cartel")]
    public float rotationX = 0f;
    public float rotationY = 0f;
    public float rotationZ = 0f;

    [Header("Movimiento Suavizado")]
    public float smoothSpeed = 5f; // Velocidad de suavizado (mayor = más rápido)

    [Header("Detección de Mirada")]
    public bool onlyShowWhenLooking = true; // Solo mostrar si mira hacia el límite
    public float visionCone = 90f; // Ángulo de visión (0-180, mayor = más amplio)

    private AudioSource audioSource;
    private bool isPlayerInBoundary = false;
    private bool isWarningVisible = false;
    private GameObject warningInstance;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (xrOrigin == null)
        {
            GameObject xrOriginGO = GameObject.Find("XR Origin");
            if (xrOriginGO != null)
                xrOrigin = xrOriginGO.transform;
        }

        if (cameraTransform == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
                cameraTransform = mainCam.transform;
        }

        if (playerController == null)
        {
            CharacterController[] controllers = FindObjectsOfType<CharacterController>();
            if (controllers.Length > 0)
                playerController = controllers[0];
        }
    }

    void LateUpdate()
    {
        if (isPlayerInBoundary && cameraTransform != null)
        {
            // Verificar si el jugador mira hacia el límite
            bool lookingAtLimit = IsLookingAtLimit();

            if (onlyShowWhenLooking && lookingAtLimit)
            {
                // Mostrar el cartel con suavizado
                if (warningInstance == null)
                {
                    ShowWarning();
                }
                UpdateWarningPosition();
                isWarningVisible = true;
            }
            else if (!onlyShowWhenLooking && warningInstance != null)
            {
                // Si no hay restricción de visión, simplemente actualizar
                UpdateWarningPosition();
                isWarningVisible = true;
            }
            else if (onlyShowWhenLooking && !lookingAtLimit && isWarningVisible)
            {
                // Ocultar si no está mirando
                HideWarning();
                isWarningVisible = false;
            }
        }
    }

    bool IsLookingAtLimit()
    {
        // Dirección hacia el límite
        UnityEngine.Vector3 directionToLimit = (transform.position - cameraTransform.position).normalized;

        // Dirección donde mira la cámara
        UnityEngine.Vector3 cameraForward = cameraTransform.forward;

        // Calcular el ángulo entre la dirección de la cámara y la dirección al límite
        float angle = UnityEngine.Vector3.Angle(cameraForward, directionToLimit);

        // Si el ángulo es menor que el cono de visión, está mirando hacia el límite
        return angle < visionCone;
    }

    void ShowWarning()
    {
        if (warningPrefab != null && warningInstance == null)
        {
            warningInstance = Instantiate(warningPrefab);
            warningInstance.transform.localScale = new UnityEngine.Vector3(
                warningScale,
                warningScale,
                warningScale
            );
        }
    }

    void HideWarning()
    {
        if (warningInstance != null)
        {
            Destroy(warningInstance);
            warningInstance = null;
        }
    }

    void UpdateWarningPosition()
    {
        if (warningInstance == null || cameraTransform == null) return;

        // Posición objetivo frente a la cámara
        UnityEngine.Vector3 targetPosition = cameraTransform.position +
                                             cameraTransform.forward * warningDistance;
        targetPosition.y += warningHeightOffset;

        // Suavizar la posición
        warningInstance.transform.position = UnityEngine.Vector3.Lerp(
            warningInstance.transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );

        // Rotación hacia la cámara
        UnityEngine.Quaternion targetRotation = UnityEngine.Quaternion.LookRotation(
            cameraTransform.position - warningInstance.transform.position
        );

        // Aplicar rotaciones personalizadas
        targetRotation *= UnityEngine.Quaternion.Euler(rotationX, rotationY, rotationZ);

        // Suavizar la rotación
        warningInstance.transform.rotation = UnityEngine.Quaternion.Lerp(
            warningInstance.transform.rotation,
            targetRotation,
            smoothSpeed * Time.deltaTime
        );
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") ||
            other.name.Contains("Camera") ||
            other.name.Contains("Head") ||
            other.name.Contains("Hand") ||
            other.GetComponent<CharacterController>() != null)
        {
            ActivateWarning();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (isPlayerInBoundary && xrOrigin != null)
        {
            PushPlayerBack();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") ||
            other.name.Contains("Camera") ||
            other.name.Contains("Head") ||
            other.name.Contains("Hand") ||
            other.GetComponent<CharacterController>() != null)
        {
            DeactivateWarning();
        }
    }

    void ActivateWarning()
    {
        if (isPlayerInBoundary) return;

        isPlayerInBoundary = true;

        if (onlyShowWhenLooking == false)
        {
            ShowWarning();
        }

        // Reproducir sonido
        if (warningSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(warningSound);
        }
    }

    void DeactivateWarning()
    {
        isPlayerInBoundary = false;
        HideWarning();
    }

    void PushPlayerBack()
    {
        UnityEngine.Vector3 limitPos = transform.position;
        UnityEngine.Vector3 playerPos = xrOrigin.position;

        UnityEngine.Vector3 pushDirection = (playerPos - limitPos).normalized;
        pushDirection.y = 0;

        if (playerController != null)
        {
            playerController.Move(pushDirection * pushBackForce * Time.deltaTime);
        }
        else if (xrOrigin != null)
        {
            xrOrigin.position += pushDirection * pushBackForce * Time.deltaTime;
        }
    }
}
