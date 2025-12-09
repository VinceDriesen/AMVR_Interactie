using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Renderer))]
public class MovingTarget : MonoBehaviour
{
    // ... [Al je bestaande variabelen: speed, colors, etc.] ...
    [Header("Movement Settings")]
    public float minSpeed = 5f;
    public float maxSpeed = 15f;
    public bool moveOnGroundOnly = true;

    [Header("Anti-Stuck Settings")]
    public float stuckCheckInterval = 0.5f;
    public float minMoveDistance = 0.1f;

    [Header("Interaction Settings")]
    public Color highlightColor = Color.yellow;
    public Color selectedColor = Color.green;
    public Color wallHitColor = Color.red;

    private Rigidbody rb;
    private Renderer myRenderer;
    private Color originalColor;
    private float targetSpeed;
    private Vector3 lastPosition;
    private bool isSelected = false;

    // --- NIEUW: Referentie naar de ghost ---
    private GhostBall activeGhost;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        myRenderer = GetComponent<Renderer>();
        originalColor = myRenderer.material.color;
    }

    void Start()
    {
        InitializeMovement();
        StartCoroutine(CheckStuckRoutine());
    }

    void FixedUpdate()
    {
        if (!isSelected) MaintainSpeed();
        else rb.velocity = Vector3.zero;
    }

    // --- NIEUW: Functie om ghost te registreren ---
    public void RegisterGhost(GhostBall ghost)
    {
        // Als er al een oude ghost was, ruim die op (optioneel)
        if (activeGhost != null) Destroy(activeGhost.gameObject);
        activeGhost = ghost;
    }

    public void OnWallPass()
    {
        originalColor = wallHitColor;
        if (!isSelected) myRenderer.material.color = wallHitColor;
    }

    public void SetHover(bool active)
    {
        if (isSelected) return;

        if (active) myRenderer.material.color = highlightColor;
        else myRenderer.material.color = originalColor;
    }

    // In MovingTarget.cs

    public void SelectTarget()
    {
        isSelected = true;
        myRenderer.material.color = selectedColor;

        // Stop fysica
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
        }

        // CRUCIAAL: Vernietig de ghost (dus ook degene waar je net naar keek)
        if (activeGhost != null)
        {
            Destroy(activeGhost.gameObject);
            activeGhost = null;
        }

        Debug.Log("Bal gevangen via Ghost of Direct!");
    }

    // ... [Je bestaande movement functies: InitializeMovement, MaintainSpeed, etc.] ...
    private void InitializeMovement()
    {
        targetSpeed = Random.Range(minSpeed, maxSpeed);
        rb.velocity = GetRandomDirection() * targetSpeed;
    }

    private Vector3 GetRandomDirection()
    {
        if (moveOnGroundOnly)
        {
            Vector2 circle = Random.insideUnitCircle.normalized;
            return new Vector3(circle.x, 0f, circle.y);
        }
        return Random.onUnitSphere;
    }

    private void MaintainSpeed()
    {
        if (rb.velocity.sqrMagnitude > 0.1f)
        {
            rb.velocity = rb.velocity.normalized * targetSpeed;
        }
    }

    private IEnumerator CheckStuckRoutine()
    {
        while (true)
        {
            lastPosition = transform.position;
            yield return new WaitForSeconds(stuckCheckInterval);

            if (!isSelected)
            {
                float distanceTravelled = Vector3.Distance(transform.position, lastPosition);
                if (distanceTravelled < minMoveDistance) Unstick();
            }
        }
    }

    private void Unstick()
    {
        Vector3 newDir = GetRandomDirection();
        rb.velocity = newDir * targetSpeed;
        if (moveOnGroundOnly) transform.position += Vector3.up * 0.1f;
    }

}