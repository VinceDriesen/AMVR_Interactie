using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Renderer))]
public class MovingTarget : MonoBehaviour
{
    [Header("Variatie Settings")]
    public float heightDifference = 0.5f;
    public float heightDifferenceSpeed = 1.0f;
    public Vector2 speedRange = new(10f, 40f);
    public float speedChangeInterval = 3.0f;

    [Header("Interaction Settings")]
    public Color highlightColor = Color.yellow;
    public Color selectedColor = Color.green;
    public Color errorColor = Color.red;
    public Color questTargetColor = Color.blue;

    [Header("Geluid")]
    public AudioClip passSound;
    private AudioSource audioSource;

    public static event Action<MovingTarget> OnBallCaptured;

    private Rigidbody rb;
    private Renderer myRenderer;
    private Color originalColor;

    private float angle;
    private float radius = 5f;
    private float targetSpeed;
    private float currentSpeed;
    private float speedTimer;
    private float initialHeight;

    private bool isQuestTarget = false;
    private bool isSelected = false;
    private bool isHovering = false;
    private bool isHighlighted = false;

    private GhostBall activeGhost;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.isKinematic = true;

        myRenderer = GetComponent<Renderer>();
        originalColor = myRenderer.material.color;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.volume = 1.0f;
        audioSource.mute = false;
    }

    void Update()
    {
        if (isSelected) return;

        HandleOrbitMovement();
        HandleSpeedVariation();
    }

    public void InitializeOrbit(float startRadius)
    {
        radius = startRadius;

        targetSpeed = UnityEngine.Random.Range(speedRange.x, speedRange.y);
        currentSpeed = targetSpeed;

        heightDifferenceSpeed += UnityEngine.Random.Range(-0.2f, 0.2f);
    }

    private void HandleOrbitMovement()
    {
        angle += currentSpeed * Time.deltaTime;
        if (angle > 360f) angle -= 360f;

        float newY = initialHeight + Mathf.Sin(Time.time * heightDifferenceSpeed + radius) * heightDifference;

        float rad = angle * Mathf.Deg2Rad;
        Vector3 newPos = new(Mathf.Cos(rad) * radius, newY, Mathf.Sin(rad) * radius);

        transform.position = newPos;

        transform.LookAt(new Vector3(newPos.x - Mathf.Sin(rad), newPos.y, newPos.z + Mathf.Cos(rad)));
    }

    private void HandleSpeedVariation()
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
        isHovering = active;
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

        if (passSound != null && isQuestTarget)
        {
            audioSource.PlayOneShot(passSound);
        }

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
                myRenderer.material.color = selectedColor;
            }
            else
            {
                myRenderer.material.color = errorColor;
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