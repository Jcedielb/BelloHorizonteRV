using UnityEngine;
using UnityEngine.Rendering.Universal; // por si necesitas tipos de URP

public class FootstepDecalSpawner : MonoBehaviour
{
    [Header("Refs")]
    public Animator avatarAnimator;           // Humanoid animator
    public Transform leftFoot;                // Opcional: se autollenan
    public Transform rightFoot;               // si no los asignas
    public LayerMask groundMask = ~0;         // Terreno / suelo

    [Header("Decal")]
    public DecalProjector decalPrefab;        // Prefab con M_Footprint
    public int poolSize = 40;
    public float decalWidth = 0.25f;
    public float decalHeight = 0.40f;
    public float decalDepth = 0.05f;
    public float fadeOutSeconds = 15f;        // se desvanece con el tiempo

    [Header("Paso")]
    public float stepDistance = 0.35f;        // separación entre huellas
    public float raycastHeight = 0.5f;        // desde dónde lanzamos el rayo
    public float raycastDown = 1.5f;

    DecalProjector[] pool;
    int poolIndex;
    Vector3 lastStepPos;
    bool leftNext = true;

    void Awake()
    {
        if (!avatarAnimator) avatarAnimator = GetComponentInChildren<Animator>();
        if (!leftFoot && avatarAnimator) leftFoot = avatarAnimator.GetBoneTransform(HumanBodyBones.LeftFoot);
        if (!rightFoot && avatarAnimator) rightFoot = avatarAnimator.GetBoneTransform(HumanBodyBones.RightFoot);

        // Pool
        pool = new DecalProjector[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            pool[i] = Instantiate(decalPrefab);
            pool[i].gameObject.SetActive(false);
        }
        lastStepPos = transform.position;
    }

    void Update()
    {
        // Distancia recorrida por el jugador (posición raíz)
        float moved = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                                       new Vector3(lastStepPos.x, 0, lastStepPos.z));
        if (moved >= stepDistance)
        {
            // alterna pie
            Transform foot = leftNext ? leftFoot : rightFoot;
            if (foot) TrySpawnDecal(foot.position, foot.forward);
            else TrySpawnDecal(transform.position, transform.forward); // respaldo

            lastStepPos = transform.position;
            leftNext = !leftNext;
        }
    }

    void TrySpawnDecal(Vector3 worldFootPos, Vector3 forward)
    {
        Vector3 rayOrigin = worldFootPos + Vector3.up * raycastHeight;
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, raycastDown + raycastHeight, groundMask))
        {
            var d = NextFromPool();
            d.size = new Vector3(decalWidth, decalHeight, decalDepth);
            d.transform.position = hit.point + hit.normal * 0.005f;

            // Decal Projector mira su -Z; alineamos normal y “punta del pie”
            var rotToNormal = Quaternion.LookRotation(-hit.normal, Vector3.up);
            var yaw = Quaternion.AngleAxis(Vector3.SignedAngle(Vector3.forward,
                                                               Vector3.ProjectOnPlane(forward, hit.normal).normalized,
                                                               hit.normal),
                                           hit.normal);
            d.transform.rotation = yaw * rotToNormal;

            d.fadeFactor = 1f;
            d.gameObject.SetActive(true);
            StartCoroutine(FadeOut(d));
        }
    }

    DecalProjector NextFromPool()
    {
        var d = pool[poolIndex++ % pool.Length];
        return d;
    }

    System.Collections.IEnumerator FadeOut(DecalProjector d)
    {
        float t = 0;
        while (t < fadeOutSeconds)
        {
            t += Time.deltaTime;
            d.fadeFactor = Mathf.Lerp(1f, 0f, t / fadeOutSeconds);
            yield return null;
        }
        d.gameObject.SetActive(false);
    }
}
