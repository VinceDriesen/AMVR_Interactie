using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class MovingTarget : MonoBehaviour
{
    [Header("Ghost Settings")]
    [Tooltip("Prefab voor de zichtbare bal")]
    public GameObject visualPrefab;
    [Tooltip("Snelheid van de bal wanneer deze in de lamp zit")]
    public float slowMoSpeed = 2.0f;     
    [Tooltip("Snelheid van de bal wanneer deze uit de lamp is")]
    public float catchUpSpeed = 50f;     

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

    // Interne referenties
    private Rigidbody rb;
    private Renderer myRenderer;

    private VisualBallLink visualLinkScript;
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
    private float lastMoveSpeed = 0f;
    private bool isSelected = false;
    

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        myRenderer = GetComponent<Renderer>();
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

            visualLinkScript = visualObject.GetComponent<VisualBallLink>();
            if (visualLinkScript != null)
            {
                visualLinkScript.myGhost = this; // Zeg tegen de visual: IK ben jouw ghost
            }

            visualRenderer = visualObject.GetComponent<Renderer>();
            if (visualRenderer) originalColor = visualRenderer.material.color;
        }
        else
        {
            Debug.LogError("VERGEET NIET DE VISUAL PREFAB TE KOPPELEN IN DE INSPECTOR!");
        }
    }

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

        // Handle orbit
        HandleOrbitMovement();
        HandleSpeedVariation();

        // Queue huidige positie voor VisualBall, en voer uit
        movementHistory.Enqueue(new Pose(transform.position, transform.rotation));
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

        float targetMoveSpeed = isInVisor ? slowMoSpeed : catchUpSpeed;
        float currentMoveSpeed = Mathf.Lerp(lastMoveSpeed, targetMoveSpeed, Time.deltaTime * 5f);
        lastMoveSpeed = currentMoveSpeed;

        Vector3 targetPosition;
        Quaternion targetRotation;

        if (movementHistory.Count > 0)
        {
            Pose historyPose = movementHistory.Peek();
            targetPosition = historyPose.position;
            targetRotation = historyPose.rotation;

            while (movementHistory.Count > 0 && Vector3.Distance(visualObject.transform.position, movementHistory.Peek().position) < 0.05f)
            {
                movementHistory.Dequeue();

                if (movementHistory.Count > 0)
                {
                    historyPose = movementHistory.Peek();
                    targetPosition = historyPose.position;
                    targetRotation = historyPose.rotation;
                }
            }
        }
        else
        {
            transform.GetPositionAndRotation(out targetPosition, out targetRotation);
        }
        
        visualObject.transform.SetPositionAndRotation(Vector3.MoveTowards(
            visualObject.transform.position,
            targetPosition,
            currentMoveSpeed * Time.deltaTime
        ), Quaternion.Slerp(
            visualObject.transform.rotation,
            targetRotation,
            currentMoveSpeed * 2f * Time.deltaTime
        ));
    }

    public void SelectTarget()
    {
        isSelected = true;
        visualRenderer.material.color = selectedColor;

        if (visualObject != null)
        {
            Destroy(visualObject);
            visualObject = null;
        }

        Debug.Log("Orbit Target gevangen!");
    }

    public void SetHover(bool active)
    {
        if (isSelected) return;
        myRenderer.material.color = active ? highlightColor : originalColor;
    }

    public void SetSlowMo(bool active)
    {
        isInVisor = active;
    }

    // Zorg dat de visual verdwijnt als dit object vernietigd wordt
    void OnDestroy()
    {
        if (visualObject) Destroy(visualObject);
    }
}