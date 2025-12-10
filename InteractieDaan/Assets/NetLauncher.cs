using UnityEngine;
using UnityEngine.InputSystem;

public class NetLauncher : MonoBehaviour
{
    [Header("Wat spawnen we?")]
    public GameObject netPrefab;
    public Transform spawnPoint;

    [Header("Welke knop?")]
    public InputActionProperty spawnButton; 

    // --- VOEG DIT TOE ---
    // Zodra dit object actief wordt, zet de 'oren' open voor de knop
    private void OnEnable()
    {
        spawnButton.action.Enable();
    }

    // Zodra dit object uit gaat, stop met luisteren (voorkomt errors)
    private void OnDisable()
    {
        spawnButton.action.Disable();
    }
    // --------------------

    void Update()
    {
        // De check blijft hetzelfde
        if (spawnButton.action != null && spawnButton.action.WasPressedThisFrame())
        {
            Debug.Log("clicked");
            SpawnNet();
        }
    }

    void SpawnNet()
    {
        if (netPrefab != null)
        {
            Vector3 pos = (spawnPoint != null) ? spawnPoint.position : Vector3.zero;
            Quaternion rot = (spawnPoint != null) ? spawnPoint.rotation : Quaternion.identity;
            Instantiate(netPrefab, pos, rot);
            Debug.Log("XR Spawn!");
        }
    }
}
