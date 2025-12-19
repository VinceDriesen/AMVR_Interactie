using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

public class WallCatcher : MonoBehaviour
{
    public GameObject ghostPrefab;
    
    [Range(0, 1)] public float hapticIntensity = 0.4f;
    public float hapticDuration = 0.15f;

    [Header("Geluid")]
    public AudioClip passSound;
    private AudioSource audioSource;

    private bool isMoving = false;

    private bool hasCaught = false; // Om bij te houden of er al een bal is geweest

    private UnityEngine.XR.InputDevice leftController;
    private UnityEngine.XR.InputDevice rightController;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) 
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        var rightHandDevices = new List<UnityEngine.XR.InputDevice>()

        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, rightHandDevices);

        if (rightHandDevices.Count > 0)
        {
            rightController = rightHandDevices[0];
        }

        var leftHandDevices = new List<UnityEngine.XR.InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, leftHandDevices);

        if (leftHandDevices.Count > 0)
        {
            leftController = leftHandDevices[0];
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (isMoving) return;
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

    public void setMoving(bool active)
    {
        isMoving = active;
    }

    private void TriggerHaptics()
    {
        if (leftController != null)
            leftController.SendHapticImpulse((uint)hapticIntensity, hapticDuration);
        
        if (rightController != null)
            rightController.SendHapticImpulse((uint)hapticIntensity, hapticDuration);
    }   
}
