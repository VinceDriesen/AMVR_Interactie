using UnityEngine;

public class WallCatcher : MonoBehaviour
{
    [Header("Instellingen")]
    public float lifeTime = 2.0f;

    [Header("Welke kant is 'Breedte' en 'Hoogte'?")]
    // Omdat je Y en Z hebt geschaald, is waarschijnlijk:
    // Width Axis = Z (of Y)
    // Height Axis = Y (of Z)
    public Axis widthAxis = Axis.Z; 
    public Axis heightAxis = Axis.Y;

    public enum Axis { X, Y, Z }

    private BoxCollider myCol;

    void Start()
    {
        myCol = GetComponent<BoxCollider>();
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            // 1. Vind het EXACTE punt op de muur dat het dichtst bij de bal is
            // Dit lost het probleem op dat het midden van de bal nog ver weg is
            Vector3 impactPoint = myCol.ClosestPoint(other.transform.position);

            // 2. Vertaal dit punt naar lokale coördinaten van de muur
            // (Houdt rekening met rotatie en positie van de muur)
            Vector3 localPoint = transform.InverseTransformPoint(impactPoint);

            // 3. Bereken percentages op basis van jouw gekozen assen
            float relativeX = CalculatePercent(localPoint, widthAxis, myCol.size);
            float relativeY = CalculatePercent(localPoint, heightAxis, myCol.size);

            Debug.Log($"Hit op UI: X={(relativeX*100):F0}%, Y={(relativeY*100):F0}%");

            // Stuur naar UI Manager
            if (UIWallManager.Instance != null)
            {
                UIWallManager.Instance.RegisterHitOnUI(relativeX, relativeY, other.gameObject);
            }
        }
    }

    // Hulpfunctie om het percentage (0.0 tot 1.0) te berekenen voor een specifieke as
    float CalculatePercent(Vector3 localPoint, Axis axis, Vector3 colliderSize)
    {
        float pos = 0;
        float size = 0;

        switch (axis)
        {
            case Axis.X: pos = localPoint.x; size = colliderSize.x; break;
            case Axis.Y: pos = localPoint.y; size = colliderSize.y; break;
            case Axis.Z: pos = localPoint.z; size = colliderSize.z; break;
        }

        // Formule: (Positie + HalveGrootte) / TotaleGrootte
        // Dit zet bijv. -0.5 tot 0.5 om naar 0.0 tot 1.0
        return Mathf.Clamp01((pos + (size / 2f)) / size);
    }
}