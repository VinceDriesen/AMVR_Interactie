using System;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Renderer))]
public class GhostBall : MonoBehaviour
{
    [Header("Visuals")]
    public Color ghostColor = new(1, 1, 1, 0.5f);
    public Color highlightColor = Color.cyan;

    private MovingTarget linkedRealBall;
    private bool isHovering = false;

    private LineRenderer lineRenderer;
    private Renderer myRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        myRenderer = GetComponent<Renderer>();

        // Setup LineRenderer
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.enabled = false;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    void Update()
    {
        if (isHovering)
        {
            UpdateLine();
        }
    }

    public void Setup(MovingTarget realBall)
    {
        linkedRealBall = realBall;
        myRenderer.material.color = ghostColor;

        realBall.RegisterGhost(this);
    }

    public void SetHover(bool active)
    {
        isHovering = active;
        lineRenderer.enabled = active;

        if (active)
        {
            myRenderer.material.color = highlightColor;
            if (linkedRealBall != null) linkedRealBall.SetHover(true);
        }
        else
        {
            myRenderer.material.color = ghostColor;
            if (linkedRealBall != null) linkedRealBall.SetHover(false);
        }
    }

    private void UpdateLine()
    {
        if (linkedRealBall != null)
        {
            lineRenderer.SetPosition(0, transform.position); // Ghost positie
            lineRenderer.SetPosition(1, linkedRealBall.transform.position); // Bal positie
        }
    }

    public MovingTarget GetRealBall()
    {
        return linkedRealBall;
    }    
}