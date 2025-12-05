using UnityEngine;

public class WallCatcher : MonoBehaviour
{
    [Header("Instellingen")]
    public float lifeTime = 1.0f; // Zet deze op 1 seconde zoals gevraagd

    private BoxCollider myCollider;

    void Start()
    {
        myCollider = GetComponent<BoxCollider>();
        // Vernietig de muur na 1 seconde
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target"))
        {
            // --- DE WISKUNDE ---
            // We moeten de 3D botsing omzetten naar een 2D percentage (0.0 tot 1.0)
            
            // 1. Waar is het object t.o.v. het centrum van de muur?
            Vector3 localHitPoint = transform.InverseTransformPoint(other.transform.position);

            // 2. Hoe groot is de muur (halve grootte)?
            Vector3 halfSize = myCollider.size / 2f;

            // 3. Bereken percentage. 
            // localHitPoint.x loopt van -halfSize tot +halfSize.
            // We tellen halfSize erbij op, en delen door de totale grootte.
            // Resultaat: 0 is linkerkant, 1 is rechterkant.
            float relativeX = (localHitPoint.x + halfSize.x) / myCollider.size.x;
            float relativeY = (localHitPoint.y + halfSize.y) / myCollider.size.y;

            // --- STUUR NAAR UI ---
            Debug.Log($"Hit op: X={relativeX:P0}, Y={relativeY:P0}");

            if (UIWallManager.Instance != null)
            {
                // Stuur de coördinaten én de bal zelf naar de UI manager
                UIWallManager.Instance.RegisterHitOnUI(relativeX, relativeY, other.gameObject);
            }
        }
    }
}
