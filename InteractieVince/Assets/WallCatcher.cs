using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit; // Vereist voor de trillingen

public class WallCatcher : MonoBehaviour
{
    public GameObject ghostPrefab;

    [Header("XR Feedback Instellingen")]
    public ActionBasedController leftController;
    public ActionBasedController rightController;
    
    [Range(0, 1)] public float hapticIntensity = 0.4f;
    public float hapticDuration = 0.15f;

    [Header("Geluid")]
    public AudioClip passSound;
    private AudioSource audioSource;

    void Start()
    {
        // Zorg dat er een AudioSource is voor het geluidje
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) 
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        MovingTarget target = other.GetComponent<MovingTarget>();

        // Check of het wel echt een target is
        if (target != null)
        {
            target.OnWallPass();

            // 1. Speel geluid af
            if (passSound != null)
            {
                audioSource.PlayOneShot(passSound);
            }

            // 2. Laat controllers trillen
            TriggerHaptics();

            // 3. Bestaande GhostPrefab logica
            if (ghostPrefab != null)
            {
                GameObject ghostObj = Instantiate(ghostPrefab, other.transform.position, Quaternion.identity);

                GhostBall ghostScript = ghostObj.GetComponent<GhostBall>();
                if (ghostScript != null)
                {
                    ghostScript.Setup(target);
                }
            }
        }
    }

    private void TriggerHaptics()
    {
        if (leftController != null)
            leftController.SendHapticImpulse(hapticIntensity, hapticDuration);
        
        if (rightController != null)
            rightController.SendHapticImpulse(hapticIntensity, hapticDuration);
    }
}
