using UnityEngine;

public class MovingTarget : MonoBehaviour
{
    [Header("Settings")]
    public float minSpeed = 5f;
    public float maxSpeed = 15f;

    private Rigidbody rb;
    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 1. Kies een willekeurige richting
        Vector3 randomDirection = Random.onUnitSphere;

        // 2. Kies een willekeurige snelheid
        currentSpeed = Random.Range(minSpeed, maxSpeed);

        // 3. Geef de eerste zet
        rb.velocity = randomDirection * currentSpeed;
    }

    void FixedUpdate()
    {
        // Optioneel: Forceer constante snelheid
        // Physics engines kunnen soms heel iets energie verliezen bij botsingen.
        // Dit zorgt ervoor dat de snelheid altijd constant blijft.
        if (rb.velocity.magnitude != currentSpeed)
        {
            rb.velocity = rb.velocity.normalized * currentSpeed;
        }
    }
    
    // Visuele debug (optioneel): Kleur veranderen bij botsing
    private void OnCollisionEnter(Collision collision)
    {
        // Hier kunnen we later geluid of vonken toevoegen
    }
}
