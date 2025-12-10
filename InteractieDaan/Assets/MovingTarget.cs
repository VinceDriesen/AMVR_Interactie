using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
// Renderer is niet meer verplicht op DIT object, want dit wordt onzichtbaar
public class MovingTarget : MonoBehaviour
{
    [Header("Ghost & Visual Settings")]
    public GameObject visualPrefab;      // Sleep hier een bal-prefab in ZONDER scripts
    public float slowMoSpeed = 2.0f;     // Snelheid IN de lamp (erg traag)
    public float catchUpSpeed = 50f;     // Snelheid UIT de lamp (erg snel)

    [Header("Orbit Settings")]
    public float radius = 5f;
    public float currentHeight = 1.5f;
    public float rotationSpeed = 20f;

    [Header("Variatie Settings")]
    public float heightWobbleAmount = 0.5f;
    public float heightWobbleSpeed = 1.0f;
    public Vector2 speedRange = new Vector2(10f, 40f);
    public float speedChangeInterval = 3.0f;

    [Header("Interaction Settings")]
    public Color highlightColor = Color.yellow;
    public Color selectedColor = Color.green;
    public Color wallHitColor = Color.red;

    // Interne referenties
    private Rigidbody rb;
    private GameObject visualObject;      // De zichtbare bal
    private Renderer visualRenderer;      // De renderer van de zichtbare bal
    private Color originalColor;

    // Path Recorder
    private Queue<Pose> movementHistory = new Queue<Pose>(); // Slaat positie én rotatie op
    private bool isInVisor = false;

    // Orbit Variabelen
    private float angle;
    private float targetSpeed;
    private float currentSpeed;
    private float speedTimer;
    private float initialHeight;
    private bool isSelected = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        // 1. Zet de renderer van DIT object (de Ghost) uit
        if (GetComponent<Renderer>())
            GetComponent<Renderer>().enabled = false;

        // 2. Spawn de Visual Bal
        if (visualPrefab != null)
        {
            visualObject = Instantiate(visualPrefab, transform.position, transform.rotation);
            // Verwijder colliders van de visual, anders botst hij met de ghost of triggers!
            foreach (var c in visualObject.GetComponentsInChildren<Collider>()) Destroy(c);

            visualRenderer = visualObject.GetComponent<Renderer>();
            if (visualRenderer) originalColor = visualRenderer.material.color;
        }
        else
        {
            Debug.LogError("VERGEET NIET DE VISUAL PREFAB TE KOPPELEN IN DE INSPECTOR!");
        }
    }

    // Initialize wordt aangeroepen door je Spawner
    public void InitializeOrbit(float startRadius, float startHeight, float startAngle)
    {
        radius = startRadius;
        initialHeight = startHeight;
        angle = startAngle;
        targetSpeed = Random.Range(speedRange.x, speedRange.y);
        currentSpeed = targetSpeed;
        heightWobbleSpeed += Random.Range(-0.2f, 0.2f);
    }

    void Update()
    {
        if (isSelected) return;

        // --- STAP 1: De Ghost berekent zijn positie (Jouw originele logica) ---
        HandleOrbitMovement();
        HandleSpeedVariation();

        // --- STAP 2: Voeg huidige Ghost positie toe aan geschiedenis ---
        movementHistory.Enqueue(new Pose(transform.position, transform.rotation));

        // --- STAP 3: Beweeg de Visual Bal ---
        HandleVisualMovement();
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
            targetSpeed = Random.Range(speedRange.x, speedRange.y);
            if (Random.value > 0.8f) targetSpeed *= 1.5f;
        }
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 2.0f);
    }

    void HandleVisualMovement()
    {
        if (visualObject == null) return;

        // Bepaal snelheid: Heel traag in vizier, heel snel erbuiten
        float currentMoveSpeed = isInVisor ? slowMoSpeed : catchUpSpeed;

        // Als we achterlopen op de geschiedenis
        if (movementHistory.Count > 0)
        {
            // Kijk naar het oudste punt in de lijst (waar de ghost X frames geleden was)
            Pose targetPose = movementHistory.Peek();

            // Beweeg erheen
            visualObject.transform.position = Vector3.MoveTowards(
                visualObject.transform.position,
                targetPose.position,
                currentMoveSpeed * Time.deltaTime
            );

            // Roteer erheen
            visualObject.transform.rotation = Quaternion.RotateTowards(
                visualObject.transform.rotation,
                targetPose.rotation,
                currentMoveSpeed * 10f * Time.deltaTime
            );

            // Als we dicht genoeg bij dit punt zijn, verwijder het uit de lijst en ga naar het volgende
            if (Vector3.Distance(visualObject.transform.position, targetPose.position) < 0.05f)
            {
                movementHistory.Dequeue();
            }
        }
        else
        {
            // We zijn helemaal bij! Sync direct met de ghost
            visualObject.transform.position = transform.position;
            visualObject.transform.rotation = transform.rotation;
        }
    }

    // --- INTERACTIE (Aangepast voor Visual) ---

    // Aangeroepen door de Lamp
    public void SetSlowMo(bool active)
    {
        isInVisor = active;

        // Visuele feedback: Ijsblauw als hij vertraagd is
        if (visualRenderer)
        {
            visualRenderer.material.color = active ? Color.cyan : (isSelected ? selectedColor : originalColor);
        }
    }

    public void OnWallPass()
    {
        originalColor = wallHitColor;
        // Pas kleur aan op de VISUAL, niet op de onzichtbare ghost
        if (!isSelected && visualRenderer) visualRenderer.material.color = wallHitColor;
    }

    public void SetHover(bool active)
    {
        if (isSelected || visualRenderer == null) return;
        visualRenderer.material.color = active ? highlightColor : originalColor;
    }

    // Zorg dat de visual verdwijnt als dit object vernietigd wordt
    void OnDestroy()
    {
        if (visualObject) Destroy(visualObject);
    }
}