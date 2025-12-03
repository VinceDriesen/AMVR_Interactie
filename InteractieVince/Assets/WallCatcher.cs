using UnityEngine;

public class WallCatcher : MonoBehaviour
{
    // Wat gebeurt er als iets door de muur vliegt?
    private void OnTriggerEnter(Collider other)
    {
        // Check of het object dat ons raakt wel een 'Target' is
        if (other.CompareTag("Target"))
        {
            Debug.Log("GEVANGEN: " + other.name);
            
            // --- VISUELE FEEDBACK ---
            // Voor nu maken we hem groen om te testen
            other.GetComponent<Renderer>().material.color = Color.green;

            // --- AUDIO / HAPTICS ---
            // Hier speel je straks je geluidje af en laat je de controller trillen
            
            // --- LOGICA ---
            // Hier zou je het object aan de 'geselecteerde lijst' toevoegen
        }
    }
}
