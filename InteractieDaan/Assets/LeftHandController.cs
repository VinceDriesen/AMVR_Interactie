using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private VisualBallLink lastHoveredObject;

    [Header("Instellingen")]
    public float rayDistance = 10f;

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

    void HandleHover(VisualBallLink currentObj)
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
            else
            {
                lineRenderer.startColor = Color.red;
            }

            lastHoveredObject = currentObj;
        }

        // Input check voor grijpen (G of Trigger)
        if (Input.GetAxis("XRI_Left_Grip") > .5f)
        {
            if (currentObj != null)
            {
                currentObj.SelectTarget();
            }
        }
    }
}