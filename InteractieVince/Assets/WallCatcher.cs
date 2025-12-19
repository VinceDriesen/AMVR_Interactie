using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WallCatcher : MonoBehaviour
{
    public GameObject ghostPrefab;

    [Header("XR Feedback Instellingen")]
    public XRBaseController leftController;
    public XRBaseController rightController;
    
    [Range(0, 1)] public float hapticIntensity = 0.4f;
    public float hapticDuration = 0.15f;

    [Header("Geluid")]
    public AudioClip passSound;
    private AudioSource audioSource;

    private bool hasCaught = false; // Om bij te houden of er al een bal is geweest

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) 
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Als we al een bal hebben gepakt, doen we niets meer
        if (hasCaught) return;

        MovingTarget target = other.GetComponent<MovingTarget>();

        if (target != null)
        {
            // Zet op true zodat volgende ballen worden genegeerd
            hasCaught = true;

            target.OnWallPass();

            // 1. Geluid
            if (passSound != null)
            {
                audioSource.PlayOneShot(passSound);
            }

            // 2. Trilling
            TriggerHaptics();

            // 3. GhostPrefab
            if (ghostPrefab != null)
            {
                GameObject ghostObj = Instantiate(ghostPrefab, other.transform.position, Quaternion.identity);

                GhostBall ghostScript = ghostObj.GetComponent<GhostBall>();
                if (ghostScript != null)
                {
                    ghostScript.Setup(target);
                }
            }

            // --- NIEUW: Zelfvernietiging na 2 seconden ---
            Destroy(gameObject, 2.0f);
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
