using UnityEngine;
using UnityEngine.InputSystem; // Deze is nodig voor XR controls

public class NetLauncher : MonoBehaviour
{
    [Header("Wat spawnen we?")]
    public GameObject netPrefab;
    public Transform spawnPoint; // Sleep hier bijv. je 'RightHand Controller' in

    [Header("Welke knop?")]
    // Dit maakt een mooi vakje in de inspector
    public InputActionProperty spawnButton; 

    void Update()
    {
        // Checken of de actie (knop) ingedrukt wordt in dit frame
        if (spawnButton.action != null && spawnButton.action.WasPressedThisFrame())
        {
            SpawnNet();
        }
    }

    void SpawnNet()
    {
        if (netPrefab != null)
        {
            // Gebruik positie van de hand/spawnpoint, of 0,0,0 als die leeg is
            Vector3 pos = (spawnPoint != null) ? spawnPoint.position : Vector3.zero;
            
            // Spawn met rotatie van de hand (of identity als je hem recht wilt houden)
            Quaternion rot = (spawnPoint != null) ? spawnPoint.rotation : Quaternion.identity;

            Instantiate(netPrefab, pos, rot);
            Debug.Log("XR Spawn!");
        }
    }
}
