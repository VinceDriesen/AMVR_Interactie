using UnityEngine;

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
    public Color wallHitColor = Color.red;

    private Rigidbody rb;
    private Renderer myRenderer;
    private Color originalColor;

    // Orbit Variabelen
    private float angle; // Huidige hoek in de cirkel (0 tot 360)
    private float targetSpeed;
    private float currentSpeed;
    private float speedTimer;
    private float initialHeight;
    private bool isSelected = false;

    // Ghost Referentie
    private GhostBall activeGhost;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // BELANGRIJK: Voor orbit movement zetten we physics uit, 
        // maar we houden de Rigidbody voor de Trigger detectie (WallCatcher).
        rb.useGravity = false;
        rb.isKinematic = true;

        myRenderer = GetComponent<Renderer>();
        originalColor = myRenderer.material.color;
    }

    // Deze functie wordt aangeroepen door de Spawner om de startwaarden te geven
    public void InitializeOrbit(float startRadius, float startHeight, float startAngle)
    {
        radius = startRadius;
        initialHeight = startHeight;
        angle = startAngle;

        // Zet een random startsnelheid
        targetSpeed = Random.Range(speedRange.x, speedRange.y);
        currentSpeed = targetSpeed;

        // Randomize de wobble zodat ze niet allemaal synchroon bewegen
        heightWobbleSpeed += Random.Range(-0.2f, 0.2f);
    }

    void Update()
    {
        if (isSelected) return; // Stop als we geselecteerd zijn

        HandleOrbitMovement();
        HandleSpeedVariation();
    }

    void HandleOrbitMovement()
    {
        // 1. Update de hoek (draaien rondom origin)
        angle += currentSpeed * Time.deltaTime;
        if (angle > 360f) angle -= 360f;

        // 2. Bereken de golvende hoogte (Sinus golf)
        // We gebruiken Time.time + radius om variatie te krijgen per baan
        float newY = initialHeight + Mathf.Sin(Time.time * heightWobbleSpeed + radius) * heightWobbleAmount;

        // 3. Zet om van Poolcoördinaten (Hoek & Radius) naar Wereldcoördinaten (X & Z)
        // Wiskunde: X = Cos(hoek) * straal, Z = Sin(hoek) * straal
        float rad = angle * Mathf.Deg2Rad;
        Vector3 newPos = new Vector3(Mathf.Cos(rad) * radius, newY, Mathf.Sin(rad) * radius);

        // 4. Pas positie toe (Relative aan World Origin 0,0,0)
        transform.position = newPos;

        // 5. Draai de bal zodat hij naar zijn vliegrichting kijkt (optioneel)
        transform.LookAt(new Vector3(newPos.x - Mathf.Sin(rad), newPos.y, newPos.z + Mathf.Cos(rad)));
    }

    void HandleSpeedVariation()
    {
        // Timer om af en toe de snelheid te veranderen (Speed-up / Slow-down)
        speedTimer += Time.deltaTime;
        if (speedTimer > speedChangeInterval)
        {
            speedTimer = 0f;
            // Kies een nieuwe willekeurige snelheid
            targetSpeed = Random.Range(speedRange.x, speedRange.y);
            // Soms (20% kans) doen we een plotse stop of versnelling
            if (Random.value > 0.8f) targetSpeed *= 1.5f;
        }

        // Beweeg de snelheid vloeiend naar het doel (Lerp)
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 2.0f);
    }

    public void RegisterGhost(GhostBall ghost)
    {
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
        myRenderer.material.color = active ? highlightColor : originalColor;
    }

    public void SelectTarget()
    {
        isSelected = true;
        myRenderer.material.color = selectedColor;

        if (activeGhost != null)
        {
            Destroy(activeGhost.gameObject);
            activeGhost = null;
        }

        Debug.Log("Orbit Target gevangen!");
    }
}