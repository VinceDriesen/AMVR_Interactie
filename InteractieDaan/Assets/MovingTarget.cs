using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class MovingTarget : MonoBehaviour
{
    [Header("Ghost Settings")]
    [Tooltip("Prefab voor de zichtbare bal")]
    public GameObject visualPrefab;

    [Tooltip("Factor waarmee de bal vertraagt (0.1 = 10% snelheid)")]
    [Range(0.01f, 1f)]
    public float slowdownFactor = 0.2f;

    [Tooltip("Maximale inhaalsnelheid van de visuele bal")]
    public float catchUpSpeed = 50f;

    [Header("Orbit Settings")]
    public float radius = 5f;
    public float initialHeight = 1.5f;
    public float rotationSpeed = 20f;

    [Header("Variatie Settings")]
    public float heightWobbleAmount = 0.5f;
    public float heightWobbleSpeed = 1.0f;
    public Vector2 speedRange = new Vector2(10f, 40f);
    public float speedChangeInterval = 3.0f;

    private Color highlightColor = Color.yellow;
    private Color originalColor = Color.white;

    private Rigidbody rb;
    private Renderer myRenderer;

    private VisualBallLink visualLinkScript;
    private GameObject visualObject;

    private Queue<Pose> movementHistory = new Queue<Pose>();
    private bool isInVisor = false;

    private float angle;
    private float targetSpeed;
    private float currentSpeed;
    private float speedTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        myRenderer = GetComponent<Renderer>();
        rb.useGravity = false;
        rb.isKinematic = true;

        if (myRenderer)
        {
            originalColor = myRenderer.material.color;
            myRenderer.enabled = false;
        }

        SpawnVisualBall();
    }

    void SpawnVisualBall()
    {
        if (visualPrefab != null)
        {
            visualObject = Instantiate(visualPrefab, transform.position, transform.rotation);

            // Trigger & Physics setup
            foreach (var c in visualObject.GetComponentsInChildren<Collider>()) c.isTrigger = true;

            Rigidbody visualRb = visualObject.GetComponent<Rigidbody>();
            if (visualRb == null) visualRb = visualObject.AddComponent<Rigidbody>();
            visualRb.useGravity = false;
            visualRb.isKinematic = true;

            // Link leggen
            visualLinkScript = visualObject.GetComponent<VisualBallLink>();
            if (visualLinkScript != null)
            {
                visualLinkScript.myGhost = this;
            }
        }
        else
        {
            Debug.LogError("Geen Visual Prefab gekoppeld!");
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
        HandleOrbitMovement();
        HandleSpeedVariation();

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

        float currentMoveSpeed = isInVisor ? (catchUpSpeed * slowdownFactor) : catchUpSpeed;

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

    public void SetHover(bool active)
    {
        if (myRenderer == null) return;
        myRenderer.material.color = active ? highlightColor : originalColor;
    }

    public void SetSlowMo(bool active)
    {
        isInVisor = active;
    }
}