using UnityEngine;
using System;

[RequireComponent(typeof(LineRenderer))]
public class RightControllerBehaviour : MonoBehaviour
{
    [Header("Instellingen")]
    public float rayDistance = 100f;
    public string grabButton = "XRI_Right_Grip";

    private MonoBehaviour lastHoveredObject;
    private LineRenderer lineRenderer;

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

            MovingTarget target = hit.collider.GetComponent<MovingTarget>();
            if (target != null)
            {
                currentFoundObj = target;
                break;
            }

            GhostBall ghost = hit.collider.GetComponent<GhostBall>();
            if (ghost != null)
            {
                currentFoundObj = ghost;
                break;
            }

            break;
        }

        lineRenderer.SetPosition(1, endPoint);
        HandleHover(currentFoundObj);
        CheckSelection(currentFoundObj);
    }

    private void HandleHover(MonoBehaviour currentObj)
    {
        // Is er iets veranderd?
        if (lastHoveredObject != currentObj)
        {
            // Zet oude uit
            if (lastHoveredObject != null)
            {
                if (lastHoveredObject is MovingTarget t) t.SetHover(false);
                else if (lastHoveredObject is GhostBall g) g.SetHover(false);
            }

            // Zet nieuwe aan
            if (currentObj != null)
            {
                if (currentObj is MovingTarget t) t.SetHover(true);
                else if (currentObj is GhostBall g) g.SetHover(true);

                lineRenderer.startColor = Color.yellow;
            }
            // Geen nieuwe geselecteerd
            else
            {
                lineRenderer.startColor = Color.red;
            }

            lastHoveredObject = currentObj;
        }
    }

    private void CheckSelection(MonoBehaviour currentObj)
    {
        if (Input.GetAxis(grabButton) > .5f)
        {
            if (currentObj is MovingTarget target) target.SelectTarget();
            else if (currentObj is GhostBall ghost) ghost.GetRealBall().SelectTarget();
        }
    }
}