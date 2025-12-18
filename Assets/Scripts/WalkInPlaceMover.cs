using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(CharacterController))]
public class WalkInPlaceMover : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;                        // Main Camera
    public ActionBasedContinuousMoveProvider joystickMove;   // Opcional: proveedor de joystick para habilitar/deshabilitar
    CharacterController cc;

    [Header("Walk-In-Place")]
    public bool useWalkInPlace = true;
    [Tooltip("Altura (m) mínima de oscilación para contar paso.")]
    public float stepAmplitude = 0.02f;
    [Tooltip("Velocidad vertical (m/s) mínima para detectar cruce arriba-abajo.")]
    public float verticalVelThreshold = 0.15f;
    [Tooltip("Tiempo mínimo entre pasos (s) para filtrar ruido.")]
    public float minStepInterval = 0.25f;
    [Tooltip("Longitud de zancada virtual (m) por paso detectado.")]
    public float strideLength = 0.6f;
    [Tooltip("Factor de suavizado de la velocidad (0-1).")]
    [Range(0f, 1f)] public float speedSmoothing = 0.15f;
    [Tooltip("Máxima velocidad (m/s) por WIP.")]
    public float maxWipSpeed = 2.0f;
    [Tooltip("Arrástralo al plano (XZ). 'Forward' será la dirección del movimiento.")]
    public bool headRelativeForward = true;

    [Header("Gravity")]
    public float gravity = -9.81f;

    UnityEngine.Vector3 lastCamLocalPos;
    float lastStepTime;
    float currentSpeed;     // m/s suavizada
    float verticalVel;      // m/s local Y
    float accumulatedDistance; // suma de stride que convertimos a velocidad

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (!cameraTransform) cameraTransform = Camera.main ? Camera.main.transform : null;
        lastCamLocalPos = cameraTransform ? cameraTransform.localPosition : UnityEngine.Vector3.zero;
    }

    void OnEnable()
    {
        if (joystickMove) joystickMove.enabled = !useWalkInPlace;
    }

    void Update()
    {
        if (!cameraTransform) return;

        // 1) Velocidad vertical local del HMD
        UnityEngine.Vector3 camLocal = cameraTransform.localPosition;
        float dy = camLocal.y - lastCamLocalPos.y;
        verticalVel = dy / Mathf.Max(Time.deltaTime, 1e-4f);

        // 2) Detectar cruce de umbral como "paso"
        // Condición simple: velocidad vertical supera umbral y cambio de dirección (pico o valle).
        // Patrón: cuando veníamos bajando y ahora sube con fuerza, o viceversa.
        bool upStroke = verticalVel > verticalVelThreshold && (dy > 0f);
        bool downStroke = verticalVel < -verticalVelThreshold && (dy < 0f);

        if (useWalkInPlace)
        {
            float t = Time.time;
            if ((upStroke || downStroke) && (t - lastStepTime) > minStepInterval && Mathf.Abs(dy) > stepAmplitude)
            {
                lastStepTime = t;
                accumulatedDistance += strideLength; // convertiremos a velocidad
            }
        }

        // 3) Convertir distancia acumulada a velocidad objetivo (m/s)
        float targetSpeed = 0f;
        if (useWalkInPlace)
        {
            // Pasar "distancia" a velocidad por segundo de manera suave
            targetSpeed = Mathf.Clamp(accumulatedDistance / Mathf.Max(Time.deltaTime, 1e-4f), 0f, maxWipSpeed);
        }

        // Suavizado exponencial
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, 1f - Mathf.Pow(1f - speedSmoothing, Time.deltaTime * 60f));
        accumulatedDistance = 0f;

        // 4) Dirección de avance (proyección a XZ)
        UnityEngine.Vector3 fwd = headRelativeForward ? cameraTransform.forward : transform.forward;
        fwd.y = 0f; fwd.Normalize();

        // 5) Movimiento + gravedad
        UnityEngine.Vector3 velocity = fwd * currentSpeed;
        if (!cc.isGrounded) velocity.y += gravity * Time.deltaTime;

        cc.Move(velocity * Time.deltaTime);

        lastCamLocalPos = camLocal;
    }

    // Para activar/desactivar WIP en runtime
    public void SetWalkInPlace(bool enabledWip)
    {
        useWalkInPlace = enabledWip;
        if (joystickMove) joystickMove.enabled = !enabledWip;
    }
}
