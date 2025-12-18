using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ArmSwingMover : MonoBehaviour
{
    public Transform cameraTransform;
    public Transform leftController;
    public Transform rightController;

    public bool useArmSwing = true;
    public float swingSensitivity = 1.0f;     // Ganancia de velocidad
    public float minSwingSpeed = 0.1f;        // Umbral para ignorar micro-movimientos
    public float maxSpeed = 2.5f;
    [Range(0, 1)] public float smoothing = 0.15f;
    public bool headRelativeForward = true;

    CharacterController cc;
    Vector3 lastL, lastR;
    float currentSpeed;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        if (!cameraTransform) cameraTransform = Camera.main ? Camera.main.transform : null;
        if (leftController) lastL = leftController.position;
        if (rightController) lastR = rightController.position;
    }

    void Update()
    {
        if (!useArmSwing || !leftController || !rightController || !cameraTransform) return;

        Vector3 dl = (leftController.position - lastL) / Mathf.Max(Time.deltaTime, 1e-4f);
        Vector3 dr = (rightController.position - lastR) / Mathf.Max(Time.deltaTime, 1e-4f);

        // Solo componente horizontal (XZ)
        dl.y = 0f; dr.y = 0f;
        float swing = (dl.magnitude + dr.magnitude) * 0.5f;

        float targetSpeed = (swing > minSwingSpeed) ? Mathf.Clamp((swing - minSwingSpeed) * swingSensitivity, 0f, maxSpeed) : 0f;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, 1f - Mathf.Pow(1f - smoothing, Time.deltaTime * 60f));

        Vector3 fwd = headRelativeForward ? cameraTransform.forward : transform.forward;
        fwd.y = 0f; fwd.Normalize();

        cc.Move(fwd * currentSpeed * Time.deltaTime);

        lastL = leftController.position;
        lastR = rightController.position;
    }
}
