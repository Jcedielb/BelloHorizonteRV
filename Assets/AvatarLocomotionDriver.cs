using UnityEngine;
using UnityEngine.Animations;

public class AvatarLocomotionDriver : MonoBehaviour
{
    public CharacterController xrCharacter;
    public Animator animator;
    public float walkThreshold = 0.2f;  // para filtrar vibración
    public float maxRunSpeed = 3.5f;

    void Update()
    {
        if (!xrCharacter || !animator) return;
        Vector3 v = xrCharacter.velocity; v.y = 0;
        float speed = v.magnitude;

        // Normaliza para la BlendTree [0..maxRunSpeed]
        float param = (speed <= walkThreshold) ? 0f : Mathf.Clamp(speed, 0f, maxRunSpeed);
        animator.SetFloat("Speed", param);
    }
}
