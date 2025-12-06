using UnityEngine;
using UnityEngine.InputSystem; // Voor de XR knoppen

public class WallDrawer : MonoBehaviour
{
    [Header("Wat tekenen we?")]
    public GameObject wallPrefab;    // De ECHTE muur (GhostNet/WallCatcher)
    public GameObject previewPrefab; // Het blokje dat je ziet tijdens tekenen (zonder collider)

    [Header("Input")]
    public InputActionProperty drawButton; // Welke knop gebruiken we? (bijv. Trigger)

    [Header("Instellingen")]
    public float minThickness = 0.05f; // Minimale dikte van de muur

    private GameObject currentPreview;
    private Vector3 startPoint;
    private bool isDrawing = false;

    void Update()
    {
        // 1. START TEKENEN (Knop net ingedrukt)
        if (drawButton.action.WasPressedThisFrame())
        {
            StartDrawing();
        }

        // 2. BEZIG MET TEKENEN (Knop ingehouden)
        if (isDrawing)
        {
            UpdatePreview();
        }

        // 3. STOP TEKENEN (Knop losgelaten)
        if (drawButton.action.WasReleasedThisFrame())
        {
            FinishDrawing();
        }
    }

    void StartDrawing()
    {
        isDrawing = true;
        startPoint = transform.position; // Sla het startpunt op (waar je hand nu is)

        // Maak de preview cube aan
        if (previewPrefab != null)
        {
            currentPreview = Instantiate(previewPrefab, startPoint, Quaternion.identity);
        }
    }

    void UpdatePreview()
    {
        // Waar is de hand nu?
        Vector3 currentPoint = transform.position;

        // Berekend het midden tussen Start en Huidig punt
        Vector3 centerPosition = (startPoint + currentPoint) / 2f;

        // Berekend de grootte (verschil tussen start en huidig)
        float sizeX = Mathf.Abs(startPoint.x - currentPoint.x);
        float sizeY = Mathf.Abs(startPoint.y - currentPoint.y);
        float sizeZ = Mathf.Abs(startPoint.z - currentPoint.z);

        // Zorg dat hij niet platter wordt dan de minimale dikte (zodat je hem wel ziet)
        sizeX = Mathf.Max(sizeX, minThickness);
        sizeY = Mathf.Max(sizeY, minThickness);
        sizeZ = Mathf.Max(sizeZ, minThickness);

        // Update de preview
        if (currentPreview != null)
        {
            currentPreview.transform.position = centerPosition;
            currentPreview.transform.localScale = new Vector3(sizeX, sizeY, sizeZ);
        }
    }

    void FinishDrawing()
    {
        isDrawing = false;

        // Als we een preview hebben, gebruiken we die gegevens voor de echte muur
        if (currentPreview != null)
        {
            Vector3 finalPos = currentPreview.transform.position;
            Vector3 finalScale = currentPreview.transform.localScale;

            // Ruim de preview op
            Destroy(currentPreview);

            // MAAK DE ECHTE MUUR
            if (wallPrefab != null)
            {
                GameObject realWall = Instantiate(wallPrefab, finalPos, Quaternion.identity);
                realWall.transform.localScale = finalScale; // Neem de getekende grootte over
            }
        }
    }
}