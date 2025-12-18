using UnityEngine;
using Unity.XR.CoreUtils; // para XROrigin

public class BodyFollowHMD : MonoBehaviour
{
    [SerializeField] Transform hmd;       // arrastra Main Camera
    [SerializeField] Transform hips;      // opcional: si tienes un bone "Hips", arrástralo
    [SerializeField] float yawFollowLerp = 12f;
    [SerializeField] Vector3 localOffset = new Vector3(0f, -1.1f, 0f); // altura aprox. de cadera respecto HMD

    void LateUpdate()
    {
        if (!hmd) return;

        // Coloca el cuerpo debajo de la cabeza (X/Z), mantén offset Y (cintura)
        var targetPos = new Vector3(hmd.position.x, hmd.position.y, hmd.position.z) + hmd.TransformVector(localOffset);
        // fijar los pies al suelo si quieres: targetPos.y = groundY; (más adelante con raycast)

        transform.position = targetPos;

        // Solo rotación Y (yaw) para que el torso mire donde miras
        var yaw = Quaternion.Euler(0f, hmd.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, yaw, Time.deltaTime * yawFollowLerp);
    }
}
