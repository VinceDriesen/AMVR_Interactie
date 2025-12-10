using UnityEngine;

public class VisualBallLink : MonoBehaviour
{
    // Hierin onthouden we wie de "echte" onzichtbare bal is
    public MovingTarget myGhost;
    private Renderer myRenderer;
    private LineRenderer lineRenderer;
    private bool isHovering = false;
    private Color originalColor;

    public Color highlightColor = Color.yellow;
    public Color slowdownColor = Color.cyan;

    public void Awake()
    {
        myRenderer = GetComponent<Renderer>();
        originalColor = myRenderer.material.color;

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.enabled = false;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    public void Update()
    {
        if (isHovering)
        {
            UpdateLine();
        }
    }

    public void SetHover(bool active)
    {
        isHovering = active;
        lineRenderer.enabled = active;

        if (active)
        {
            myRenderer.material.color = highlightColor;
            if (myGhost != null) myGhost.SetHover(true);
        }
        else
        {
            myRenderer.material.color = originalColor;
            if (myGhost != null) myGhost.SetHover(false);
        }
    }

    public void SetSlowMo(bool active)
    {
        if (myRenderer)
        {
            myRenderer.material.color = active ? slowdownColor : originalColor;
        }
        if (myGhost != null)
        {
            myGhost.SetSlowMo(active);
        }
    }

    void UpdateLine()
    {
        if (myGhost != null)
        {
            lineRenderer.SetPosition(0, transform.position); // Ghost positie
            lineRenderer.SetPosition(1, myGhost.transform.position); // Bal positie
        }
    }

    public MovingTarget GetRealBall()
    {
        return myGhost;
    }
}

