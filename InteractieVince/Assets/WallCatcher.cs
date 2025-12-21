using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class WallCatcher : MonoBehaviour
{
    public GameObject ghostPrefab;
    
    [Range(0, 1)] public float hapticIntensity = 0.4f;
    public float hapticDuration = 0.15f;

    [Header("Geluid")]
    public AudioClip passSound;
    private AudioSource audioSource;

    private bool isMoving = false;

    private InputDevice leftController;
    private InputDevice rightController;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) 
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        var rightHandDevices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, rightHandDevices);

        if (rightHandDevices.Count > 0)
        {
            rightController = rightHandDevices[0];
        }

        var leftHandDevices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller, leftHandDevices);

        if (leftHandDevices.Count > 0)
        {
            leftController = leftHandDevices[0];
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (isMoving) return;

        MovingTarget target = other.GetComponent<MovingTarget>();

        if (target != null)
        {

            target.OnWallPass();
            TriggerHaptics();

            if (ghostPrefab != null)
            {
                GameObject ghostObj = Instantiate(ghostPrefab, other.transform.position, Quaternion.identity);

                GhostBall ghostScript = ghostObj.GetComponent<GhostBall>();
                if (ghostScript != null)
                {
                    ghostScript.Setup(target);
                }
            }

            Destroy(gameObject, 2.0f);
        }
    }

    public void SetMoving(bool active)
    {
        isMoving = active;
    }

    private void TriggerHaptics()
    {
        if (leftController != null)
        {
            leftController.SendHapticImpulse((uint)hapticIntensity, hapticDuration);
        }
        
        if (rightController != null)
        {
            rightController.SendHapticImpulse((uint)hapticIntensity, hapticDuration);
        }
    }   
}
