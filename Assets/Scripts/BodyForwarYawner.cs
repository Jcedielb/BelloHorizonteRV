using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;

public class BodyForwardYawner : MonoBehaviour
{
    [Header("Refs")]
    public Transform head;                    // Asigna Main Camera
    public InputActionProperty moveAction;    // Asigna XRI Left/RightHand Locomotion/Move

    [Header("Tuning")]
    [Range(0, 20)] public float turnSpeed = 6f;
    [Range(0, 1)] public float deadzone = 0.2f;

    void LateUpdate()
    {
        var move = moveAction.action != null ? moveAction.action.ReadValue<UnityEngine.Vector2>() : UnityEngine.Vector2.zero;
        // Solo reorienta el "cuerpo" si realmente te estás moviendo (evita mareo)
        if (move.sqrMagnitude > deadzone * deadzone)
        {
            float yaw = head.eulerAngles.y;
            var target = UnityEngine.Quaternion.Euler(0f, yaw, 0f);
            transform.rotation = UnityEngine.Quaternion.Slerp(transform.rotation, target, Time.deltaTime * turnSpeed);
        }
    }
}
