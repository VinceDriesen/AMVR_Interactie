using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public GameObject targetPrefab;
    public int numberOfTargets = 10;
    
    // Hier slepen we straks de Arena (met de BoxCollider) in
    public Collider spawnZone; 

    void Start()
    {
        if (spawnZone == null)
        {
            Debug.LogError("Vergeet niet de Spawn Zone (Collider) toe te wijzen in de Inspector!");
            return;
        }

        for (int i = 0; i < numberOfTargets; i++)
        {
            SpawnTarget();
        }
    }

    void SpawnTarget()
    {
        // We vragen de grenzen (Bounds) van de collider op
        Bounds bounds = spawnZone.bounds;

        // Kies een willekeurige plek binnen die grenzen
        Vector3 randomPos = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );

        Instantiate(targetPrefab, randomPos, Quaternion.identity);
    }
}
