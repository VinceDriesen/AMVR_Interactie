using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHandController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private MonoBehaviour lastHoveredObject;

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
        var currentFoundObj = SetRaycastToClosestObject();
        HandleHover(currentFoundObj);
    }

    private MonoBehaviour SetRaycastToClosestObject()
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
            lineRenderer.SetPosition(1, endPoint);

            MovingTarget target = hit.collider.GetComponent<MovingTarget>();
            if (target != null)
            {
                return target;
            }
            VisualBallLink ghost = hit.collider.GetComponent<VisualBallLink>();
            if (ghost != null)
            {
                return ghost;
            }

            return null;
        }

        return currentFoundObj;
    }

    void HandleHover(MonoBehaviour currentObj)
    {
        if (lastHoveredObject != currentObj)
        {
            // Zet oude uit
            if (lastHoveredObject != null)
            {
                if (lastHoveredObject is MovingTarget t) t.SetHover(false);
                if (lastHoveredObject is VisualBallLink g) g.SetHover(false);
            }

            // Zet nieuwe aan
            if (currentObj != null)
            {
                if (currentObj is MovingTarget t) t.SetHover(true);
                if (currentObj is VisualBallLink g) g.SetHover(true);

                lineRenderer.startColor = Color.yellow;
            }
            else
            {
                lineRenderer.startColor = Color.red;
            }

            lastHoveredObject = currentObj;
        }

        // Input check voor grijpen (G of Trigger)
        if (Input.GetAxis("XRI_Right_Grip") > .5f)
        {
            if (currentObj is MovingTarget target)
            {
                target.SelectTarget();
            }
            else if (currentObj is VisualBallLink ghost)
            {
                ghost.GetRealBall().SelectTarget();
            }
        }
    }
}



