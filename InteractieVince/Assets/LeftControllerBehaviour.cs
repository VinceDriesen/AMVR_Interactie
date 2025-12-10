using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RightHandController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Transform selectedWall;
    private bool isGrabbing = false;

    [Header("Raycast Instellingen")]
    public float rayDistance = 20f;
    public LayerMask wallLayer;

    [Header("Schaal Instellingen (Lamp Grootte)")]
    public float scaleSpeed = 1.0f; // Hoe snel hij groeit/krimpt
    public float minScaleY = 0.1f;  // Niet kleiner dan dit
    public float maxScaleY = 15.0f; // Niet groter dan dit

    [Header("Input Instellingen")]
    // Let op: Dit was Left, nu standaard Right gezet voor de zekerheid
    public string grabButton = "XRI_Right_Grip";

    // Koppel hier je Joystick Up/Down of knoppen A/B aan
    public InputActionProperty scaleUpInput;
    public InputActionProperty scaleDownInput;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.useWorldSpace = true;

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    void Update()
    {
        // 1. Check of we de GRIP knop indrukken (Oude Input Manager stijl, via string)
        float gripValue = Input.GetAxis(grabButton);
        bool gripPressed = gripValue > 0.5f;

        // 2. Logica: Vastpakken of Loslaten
        if (!gripPressed && isGrabbing)
        {
            ReleaseWall();
        }

        if (isGrabbing && selectedWall != null)
        {
            HandleScaling(); // <--- Alleen nog maar schalen
            DrawLineToTarget(selectedWall.position);
        }
        else
        {
            ScanForWalls(gripPressed);
        }
    }

    void ScanForWalls(bool gripJustPressed)
    {
        lineRenderer.SetPosition(0, transform.position);
        RaycastHit hit;

        // Schiet straal naar voren
        if (Physics.Raycast(transform.position, transform.forward, out hit, rayDistance, wallLayer, QueryTriggerInteraction.Collide))
        {
            lineRenderer.SetPosition(1, hit.point);

            // Check of we een geldig object raken (Muur, Catcher of Target)
            if (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("WallCatcher") || hit.collider.GetComponent<MovingTarget>() != null)
            {
                lineRenderer.startColor = Color.yellow;
                lineRenderer.endColor = Color.yellow;

                if (gripJustPressed && !isGrabbing)
                {
                    GrabWall(hit.transform);
                }
            }
            else
            {
                lineRenderer.startColor = Color.red; // Wel hit, maar geen geldig object
                lineRenderer.endColor = Color.red;
            }
        }
        else
        {
            // Geen hit, teken straal in de lucht
            lineRenderer.SetPosition(1, transform.position + (transform.forward * rayDistance));
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
        }
    }

    void GrabWall(Transform wall)
    {
        isGrabbing = true;
        selectedWall = wall;

        lineRenderer.startColor = Color.green; // Visuele feedback dat je vast hebt
        lineRenderer.endColor = Color.green;
    }

    void ReleaseWall()
    {
        isGrabbing = false;
        selectedWall = null;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    void HandleScaling()
    {
        // Lees de inputs uit (Nieuwe Input System)
        bool tryingToEnlarge = scaleUpInput.action != null && scaleUpInput.action.IsPressed();
        bool tryingToShrink = scaleDownInput.action != null && scaleDownInput.action.IsPressed();

        // Pak huidige schaal
        Vector3 currentScale = selectedWall.localScale;

        if (tryingToEnlarge)
        {
            currentScale.y += scaleSpeed * Time.deltaTime;
        }
        else if (tryingToShrink)
        {
            currentScale.y -= scaleSpeed * Time.deltaTime;
        }

        // Beveiliging: Zorg dat hij binnen de limieten blijft
        currentScale.y = Mathf.Clamp(currentScale.y, minScaleY, maxScaleY);

        // Pas toe
        selectedWall.localScale = currentScale;
    }

    void DrawLineToTarget(Vector3 targetPos)
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, targetPos);
    }
}