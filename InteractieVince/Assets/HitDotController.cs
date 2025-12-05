using UnityEngine;
using UnityEngine.UI;

public class HitDotController : MonoBehaviour
{
    private GameObject linkedRealBall; // De echte 3D bal
    public Color highlightColor = Color.yellow; // Kleur bij selectie

    void Start()
    {
        // Zorg dat de knop luistert naar een klik
        GetComponent<Button>().onClick.AddListener(OnDotClicked);
    }

    // Deze functie wordt aangeroepen door de manager als de stip gemaakt wordt
    public void SetupDot(GameObject realBall)
    {
        linkedRealBall = realBall;
    }

    void OnDotClicked()
    {
        if (linkedRealBall != null)
        {
            Debug.Log("Stip geklikt! Highlight bal: " + linkedRealBall.name);
            
            // Verander de kleur van de ECHTE bal
            Renderer balRenderer = linkedRealBall.GetComponent<Renderer>();
            if (balRenderer != null)
            {
                balRenderer.material.color = highlightColor;
            }
        }
        else
        {
            Debug.LogWarning("Deze stip is de connectie met de echte bal kwijt!");
            Destroy(gameObject); // Ruim de stip op als de bal er niet meer is
        }
    }
}
