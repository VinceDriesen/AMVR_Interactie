using UnityEngine;
using System;

[RequireComponent(typeof(LineRenderer))]
public class RightControllerBehaviour : MonoBehaviour
{
    private MonoBehaviour lastHoveredObject; // Kan GhostBall OF MovingTarget zijn
    private LineRenderer lineRenderer;

    [Header("Instellingen")]
    public float rayDistance = 100f;

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
        lineRenderer.SetPosition(0, transform.position);
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, rayDistance);
        Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        MonoBehaviour currentFoundObj = null;
        Vector3 endPoint = transform.position + (transform.forward * rayDistance);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.isTrigger) continue;

            endPoint = hit.point;

            // Check 1: Is het een Echte Bal?
            MovingTarget target = hit.collider.GetComponent<MovingTarget>();
            if (target != null)
            {
                currentFoundObj = target;
                break;
            }

            // Check 2: Is het een Ghost?
            GhostBall ghost = hit.collider.GetComponent<GhostBall>();
            if (ghost != null)
            {
                currentFoundObj = ghost;
                break;
            }

            // Iets anders geraakt (muur etc)
            break;
        }

        lineRenderer.SetPosition(1, endPoint);
        HandleHover(currentFoundObj);
    }

    void HandleHover(MonoBehaviour currentObj)
    {
        // Is er iets veranderd?
        if (lastHoveredObject != currentObj)
        {
            // Zet oude uit
            if (lastHoveredObject != null)
            {
                if (lastHoveredObject is MovingTarget t) t.SetHover(false);
                if (lastHoveredObject is GhostBall g) g.SetHover(false);
            }

            // Zet nieuwe aan
            if (currentObj != null)
            {
                if (currentObj is MovingTarget t) t.SetHover(true);
                if (currentObj is GhostBall g) g.SetHover(true);

                lineRenderer.startColor = Color.yellow; // Feedback dat je iets raakt
            }
            else
            {
                lineRenderer.startColor = Color.red;
            }

            lastHoveredObject = currentObj;
        }

        // Input check voor grijpen (G of Trigger)
        // Let op: Je grijpt normaal alleen de ECHTE bal, niet de ghost.
        // Maar als je op de ghost mikt en 'grijpt', zou je eventueel de echte bal kunnen selecteren?
        // Voor nu: Alleen echte bal selecteren.
        if (Input.GetAxis("XRI_Right_Grip") > .5f) // Voorbeeld input
        {
            if (currentObj is MovingTarget target)
            {
                target.SelectTarget();
            }
            else if (currentObj is GhostBall ghost)
            {
                ghost.GetRealBall().SelectTarget();
            }
        }
    }
}