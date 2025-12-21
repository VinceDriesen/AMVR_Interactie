using System;
using UnityEngine;

public class LeftHandController : MonoBehaviour
{
    [Header("Instellingen")]
    public float rayDistance = 10f;
    public string grabButton = "XRI_Left_Grip";

    private LineRenderer lineRenderer;
    private VisualBallLink lastHoveredObject;

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
        VisualBallLink currentFoundObj = SetRaycastToClosestObject();
        HandleHover(currentFoundObj);
        CheckSelection(currentFoundObj);
    }

    private VisualBallLink SetRaycastToClosestObject()
    {
        lineRenderer.SetPosition(0, transform.position);

        Vector3 endPoint = transform.position + (transform.forward * rayDistance);

        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, rayDistance);
        Array.Sort(hits, (x, y) => x.distance.CompareTo(y.distance));

        VisualBallLink currentFoundObj = null;

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform == this.transform) continue;
            
            VisualBallLink ghost = hit.collider.GetComponentInParent<VisualBallLink>();

            if (ghost != null)
            {
                currentFoundObj = ghost;
                endPoint = hit.point;
                break;
            }

            if (hit.collider.isTrigger) continue;

            endPoint = hit.point;
            break;
        }

        lineRenderer.SetPosition(1, endPoint);

        return currentFoundObj;
    }

    private void HandleHover(VisualBallLink currentObj)
    {
        if (lastHoveredObject != currentObj)
        {
            // Zet oude uit
            if (lastHoveredObject != null)
            {
                lastHoveredObject.SetHover(false);
            }
            // Zet nieuwe aan
            if (currentObj != null)
            {
                currentObj.SetHover(true);
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

    private void CheckSelection(VisualBallLink currentObject)
    {
        if (Input.GetAxis(grabButton) > .5f)
        {
            if (currentObject != null)
            {
                currentObject.SelectTarget();
            }
        }
    }
}