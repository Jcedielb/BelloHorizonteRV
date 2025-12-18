using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class ShorelineAudioFollower : MonoBehaviour
{
    [Header("Referencias")]
    public SplineContainer coastlineSpline;
    public Transform player;
    public AudioSource waveAudioSource;

    [Header("Configuración")]
    public float audioHeightOffset = 0.5f;
    public float updateThreshold = 0.5f;

    private UnityEngine.Vector3 lastPosition;

    void Start()
    {
        if (!ValidateReferences()) return;

        ConfigureAudioSource();
        waveAudioSource.Play();
        lastPosition = player.position;
        UpdateAudioPosition();
    }

    void Update()
    {
        if (UnityEngine.Vector3.Distance(player.position, lastPosition) > updateThreshold)
        {
            UpdateAudioPosition();
            lastPosition = player.position;
        }
    }

    void UpdateAudioPosition()
    {
        // Convertir posición del jugador al espacio local del spline
        UnityEngine.Vector3 playerLocalPos = coastlineSpline.transform.InverseTransformPoint(player.position);
        float3 playerPosLocal = new float3(playerLocalPos.x, playerLocalPos.y, playerLocalPos.z);

        // Encontrar punto más cercano en el spline
        SplineUtility.GetNearestPoint(coastlineSpline.Spline, playerPosLocal, out float3 nearestPointLocal, out float t);

        // Convertir punto de vuelta al espacio mundial
        UnityEngine.Vector3 nearestPointLocalVec = new UnityEngine.Vector3(nearestPointLocal.x, nearestPointLocal.y, nearestPointLocal.z);
        UnityEngine.Vector3 audioPosition = coastlineSpline.transform.TransformPoint(nearestPointLocalVec);
        audioPosition.y += audioHeightOffset;

        waveAudioSource.transform.position = audioPosition;
    }

    void ConfigureAudioSource()
    {
        waveAudioSource.loop = true;
        waveAudioSource.playOnAwake = false;
        waveAudioSource.spatialBlend = 1.0f;
        waveAudioSource.rolloffMode = AudioRolloffMode.Linear;
        waveAudioSource.minDistance = 5f;
        waveAudioSource.maxDistance = 150f;
        waveAudioSource.dopplerLevel = 0.3f;
        waveAudioSource.spread = 90f;
    }

    bool ValidateReferences()
    {
        if (coastlineSpline == null || waveAudioSource == null)
        {
            enabled = false;
            return false;
        }

        if (player == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null) player = mainCam.transform;
            else { enabled = false; return false; }
        }

        return true;
    }

    void OnDrawGizmos()
    {
        if (waveAudioSource != null && player != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(player.position, waveAudioSource.transform.position);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(waveAudioSource.transform.position, 2f);
        }
    }
}
