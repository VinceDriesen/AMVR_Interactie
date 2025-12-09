using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(Renderer))]
public class GhostBall : MonoBehaviour
{
    private MovingTarget linkedRealBall; // Referentie naar de echte bal
    private LineRenderer lineRenderer;
    private Renderer myRenderer;
    private bool isHovering = false;

    [Header("Visuals")]
    public Color ghostColor = new Color(1, 1, 1, 0.5f); // Transparant wit
    public Color highlightColor = Color.cyan; // Kleur bij hoveren

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        myRenderer = GetComponent<Renderer>();

        // Setup LineRenderer
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.enabled = false; // Lijn standaard uit
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Simpel materiaal
    }

    // Wordt aangeroepen door de WallCatcher direct na spawnen
    public void Setup(MovingTarget realBall)
    {
        linkedRealBall = realBall;
        myRenderer.material.color = ghostColor;

        // Vertel de echte bal dat WIJ zijn ghost zijn
        realBall.RegisterGhost(this);
    }

    void Update()
    {
        // Als de echte bal vernietigd is (bv door timeout), vernietig de ghost ook
        if (linkedRealBall == null)
        {
            Destroy(gameObject);
            return;
        }

        // Teken de lijn alleen als we hoveren
        if (isHovering)
        {
            UpdateLine();
        }
    }

    // Aangeroepen door PlayerRaycast
    public void SetHover(bool active)
    {
        isHovering = active;
        lineRenderer.enabled = active;

        if (active)
        {
            myRenderer.material.color = highlightColor;

            // Highlight OOK de echte bal (via zijn script)
            if (linkedRealBall != null) linkedRealBall.SetHover(true);
        }
        else
        {
            myRenderer.material.color = ghostColor;

            // Stop highlight op echte bal
            if (linkedRealBall != null) linkedRealBall.SetHover(false);
        }
    }

    public MovingTarget GetRealBall()
    {
        return linkedRealBall;
    }

    void UpdateLine()
    {
        if (linkedRealBall != null)
        {
            lineRenderer.SetPosition(0, transform.position); // Ghost positie
            lineRenderer.SetPosition(1, linkedRealBall.transform.position); // Bal positie
        }
    }
}