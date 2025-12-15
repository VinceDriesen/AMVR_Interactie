using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Renderer))]
public class MovingTarget : MonoBehaviour
{
    [Header("Orbit Settings (Worden overschreven door Spawner)")]
    public float radius = 5f;
    public float currentHeight = 1.5f;
    public float rotationSpeed = 20f; // Graden per seconde

    [Header("Variatie Settings")]
    public float heightWobbleAmount = 0.5f; // Hoeveel hij omhoog/omlaag golft
    public float heightWobbleSpeed = 1.0f;  // Hoe snel hij golft
    public Vector2 speedRange = new Vector2(10f, 40f); // Min en Max draaisnelheid
    public float speedChangeInterval = 3.0f; // Hoe vaak de snelheid verandert

    [Header("Interaction Settings")]
    public Color highlightColor = Color.yellow;
    public Color selectedColor = Color.green;
    public Color errorColor = Color.red;
    public Color questTargetColor = Color.blue;

    public static event Action<MovingTarget> OnBallCaptured;

    private Rigidbody rb;
    private Renderer myRenderer;
    private Color originalColor;

    // Orbit Variabelen
    private float angle; // Huidige hoek in de cirkel (0 tot 360)
    private float targetSpeed;
    private float currentSpeed;
    private float speedTimer;
    private float initialHeight;

    private bool isQuestTarget = false;
    private bool isSelected = false;
    private bool isHovering = false;
    private bool isHighlighted = false;

    // Ghost Referentie
    private GhostBall activeGhost;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.isKinematic = true;

        myRenderer = GetComponent<Renderer>();
        originalColor = myRenderer.material.color;
    }

    public void InitializeOrbit(float startRadius, float startHeight, float startAngle)
    {
        radius = startRadius;
        initialHeight = startHeight;
        angle = startAngle;

        targetSpeed = UnityEngine.Random.Range(speedRange.x, speedRange.y);
        currentSpeed = targetSpeed;

        heightWobbleSpeed += UnityEngine.Random.Range(-0.2f, 0.2f);
    }

    void Update()
    {
        if (isSelected) return;

        HandleOrbitMovement();
        HandleSpeedVariation();
    }

    void HandleOrbitMovement()
    {
        angle += currentSpeed * Time.deltaTime;
        if (angle > 360f) angle -= 360f;

        float newY = initialHeight + Mathf.Sin(Time.time * heightWobbleSpeed + radius) * heightWobbleAmount;

        float rad = angle * Mathf.Deg2Rad;
        Vector3 newPos = new Vector3(Mathf.Cos(rad) * radius, newY, Mathf.Sin(rad) * radius);

        transform.position = newPos;

        transform.LookAt(new Vector3(newPos.x - Mathf.Sin(rad), newPos.y, newPos.z + Mathf.Cos(rad)));
    }

    void HandleSpeedVariation()
    {
        speedTimer += Time.deltaTime;
        if (speedTimer > speedChangeInterval)
        {
            speedTimer = 0f;
            targetSpeed = UnityEngine.Random.Range(speedRange.x, speedRange.y);
            if (UnityEngine.Random.value > 0.8f) targetSpeed *= 1.5f;
        }

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 2.0f);
    }

    public void SetQuestTarget(bool active)
    {
        isQuestTarget = active;
        UpdateColorState();
    }   

    public void RegisterGhost(GhostBall ghost)
    {
        if (activeGhost != null) Destroy(activeGhost.gameObject);
        activeGhost = ghost;
    }

    public void OnWallPass()
    {
        isHighlighted = !isHighlighted;
        UpdateColorState();
    }

    public void SetHover(bool active)
    {
        if (isSelected) return;
        isHovering = !isHovering;
        UpdateColorState();
    }

    public void SelectTarget()
    {
        if (isSelected) return;
        
        StartCoroutine(SelectRoutine());
    }

    private IEnumerator SelectRoutine()
    {
        isSelected = true;
        UpdateColorState();

        OnBallCaptured?.Invoke(this);
        yield return new WaitForSeconds(2.0f);

        isSelected = false;

        if (isQuestTarget) isQuestTarget = false;

        UpdateColorState();
    }

    private void UpdateColorState()
    {
        if (myRenderer == null) return;

        if (isSelected)
        {
            if (isQuestTarget)
            {
                myRenderer.material.color = selectedColor; // Goed (Groen)
            }
            else
            {
                myRenderer.material.color = errorColor;    // Fout (Rood)
            }
        }
        else if (isHovering)
        {
            myRenderer.material.color = highlightColor;
        }
        else if (isQuestTarget)
        {
            myRenderer.material.color = questTargetColor;
        }
        else
        {
            myRenderer.material.color = originalColor;
        }
    }
}