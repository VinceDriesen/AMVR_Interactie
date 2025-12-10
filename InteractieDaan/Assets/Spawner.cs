using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Target Settings")]
    public GameObject targetPrefab;
    public int numberOfTargets = 10;

    [Header("Orbit Zone Settings")]
    [Tooltip("Minimale afstand tot het midden (0,0,0)")]
    public float minRadius = 3f;
    [Tooltip("Maximale afstand tot het midden")]
    public float maxRadius = 8f;
    [Tooltip("Laagste punt van de baan")]
    public float minHeight = 1.0f;
    [Tooltip("Hoogste punt van de baan")]
    public float maxHeight = 3.0f;

    void Start()
    {
        for (int i = 0; i < numberOfTargets; i++)
        {
            SpawnOrbitingTarget();
        }
    }

    void SpawnOrbitingTarget()
    {
        // 1. Kies willekeurige parameters binnen de grenzen
        float randomRadius = Random.Range(minRadius, maxRadius);
        float randomHeight = Random.Range(minHeight, maxHeight);
        float randomStartAngle = Random.Range(0f, 360f);

        // 2. Bereken startpositie
        float rad = randomStartAngle * Mathf.Deg2Rad;
        Vector3 startPos = new Vector3(
            Mathf.Cos(rad) * randomRadius,
            randomHeight,
            Mathf.Sin(rad) * randomRadius
        );

        // 3. Spawn
        GameObject newTarget = Instantiate(targetPrefab, startPos, Quaternion.identity);

        // 4. Geef de parameters door aan het script op de bal
        MovingTarget movementScript = newTarget.GetComponent<MovingTarget>();
        if (movementScript != null)
        {
            movementScript.InitializeOrbit(randomRadius, randomHeight, randomStartAngle);
        }
    }

    // Debug: Teken de grenzen in de Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        // Teken min radius ringen
        DrawCircle(minRadius, minHeight);
        DrawCircle(minRadius, maxHeight);

        Gizmos.color = Color.blue;
        // Teken max radius ringen
        DrawCircle(maxRadius, minHeight);
        DrawCircle(maxRadius, maxHeight);
    }

    void DrawCircle(float r, float y)
    {
        Vector3 prevPos = new Vector3(Mathf.Cos(0) * r, y, Mathf.Sin(0) * r);
        for (int i = 1; i <= 360; i += 10)
        {
            float rad = i * Mathf.Deg2Rad;
            Vector3 nextPos = new Vector3(Mathf.Cos(rad) * r, y, Mathf.Sin(rad) * r);
            Gizmos.DrawLine(prevPos, nextPos);
            prevPos = nextPos;
        }
    }
}