using System.Diagnostics;
using UnityEngine;

public class CoastTrigger : MonoBehaviour
{
    public AudioSource warningSound;

    private void OnTriggerEnter(Collider other)
    {
        // ¿El objeto que entra tiene un CharacterController?
        var cc = other.GetComponent<CharacterController>();
        if (cc != null)
        {
            UnityEngine.Debug.Log("⚠️ XR Rig ha tocado el límite de la costa");

            if (warningSound != null && !warningSound.isPlaying)
            {
                warningSound.Play();
            }
        }
    }
}
