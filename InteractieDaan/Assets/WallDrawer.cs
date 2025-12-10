using UnityEngine;
using UnityEngine.InputSystem;

public class WallDrawer : MonoBehaviour
{
    [Header("Wat tekenen we?")]
    public GameObject wallPrefab;
    public GameObject previewPrefab;

    [Header("Input")]
    public InputActionProperty drawButton;

    [Header("Instellingen")]
    public float minThickness = 0.05f;

    // Zorg dat deze naam PRECIES overeenkomt met je Layer in Unity
    public string targetLayerName = "Wall";

    private GameObject currentPreview;
    private Vector3 startPoint;
    private bool isDrawing = false;

    void Update()
    {
        if (drawButton.action.WasPressedThisFrame())
        {
            StartDrawing();
        }

        if (isDrawing)
        {
            UpdatePreview();
        }

        if (drawButton.action.WasReleasedThisFrame())
        {
            FinishDrawing();
        }
    }

    void StartDrawing()
    {
        isDrawing = true;
        startPoint = transform.position;

        if (previewPrefab != null)
        {
            currentPreview = Instantiate(previewPrefab, startPoint, Quaternion.identity);
        }
    }

    void UpdatePreview()
    {
        Vector3 currentPoint = transform.position;
        Vector3 centerPosition = (startPoint + currentPoint) / 2f;

        float sizeX = Mathf.Abs(startPoint.x - currentPoint.x);
        float sizeY = Mathf.Abs(startPoint.y - currentPoint.y);
        float sizeZ = Mathf.Abs(startPoint.z - currentPoint.z);

        sizeX = Mathf.Max(sizeX, minThickness);
        sizeY = Mathf.Max(sizeY, minThickness);
        sizeZ = Mathf.Max(sizeZ, minThickness);

        if (currentPreview != null)
        {
            currentPreview.transform.position = centerPosition;
            currentPreview.transform.localScale = new Vector3(sizeX, sizeY, sizeZ);
        }
    }

    void FinishDrawing()
    {
        isDrawing = false;

        if (currentPreview != null)
        {
            Vector3 finalPos = currentPreview.transform.position;
            Vector3 finalScale = currentPreview.transform.localScale;

            Destroy(currentPreview);

            if (wallPrefab != null)
            {
                // 1. Muur aanmaken
                GameObject realWall = Instantiate(wallPrefab, finalPos, Quaternion.identity);
                realWall.transform.localScale = finalScale;

                // 2. Tag Instellen (Voor Manipulatie)
                // Let op: Tag 'Wall' moet bestaan in Unity Editor!
                realWall.tag = "Wall";

                // 3. Layer Instellen (Voor Raycast detectie)
                int layerIndex = LayerMask.NameToLayer(targetLayerName);

                if (layerIndex != -1)
                {
                    // We gebruiken een hulpfunctie om de layer op de muur ÉN alle children te zetten
                    SetLayerRecursively(realWall, layerIndex);
                }
                else
                {
                    Debug.LogWarning($"Let op: De layer '{targetLayerName}' bestaat niet in de Project Settings!");
                }
            }
        }
    }

    // Hulpfunctie: Zet layer op object en al zijn kinderen (belangrijk voor Colliders!)
    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (child == null) continue;
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}