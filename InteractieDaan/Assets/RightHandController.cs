using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class RightHandController : MonoBehaviour
{
    [Header("Instellingen")]
    public GameObject cone;

    [Header("Schaal Instellingen")]
    public float scaleSpeed = 0.1f; // Snelheid van groter/kleiner maken
    public float minScaleY = 0.1f;  // Minimale lengte
    public float maxScaleY = 2.0f; // Maximale lengte

    [Header("Input Instellingen")]
    public InputActionProperty pushInput; // Gebruik dit om te vergroten
    public InputActionProperty pullInput; // Gebruik dit om te verkleinen

    void Start()
    {
        
    }

    void Update()
    {
        HandleScaling();
    }

    // --- DE NIEUWE FUNCTIE ---
    void HandleScaling()
    {
        bool tryingToEnlarge = pullInput.action.IsPressed();
        bool tryingToShrink = pushInput.action.IsPressed();

        Vector3 currentScale = cone.transform.localScale;

        // 1. Eerst alleen de Scale berekenen
        if (tryingToEnlarge)
        {
            currentScale.y += scaleSpeed * Time.deltaTime;
        }
        else if (tryingToShrink)
        {
            currentScale.y -= scaleSpeed * Time.deltaTime;
        }

        // 2. Clamp de schaal (zodat hij niet te groot/klein wordt)
        currentScale.y = Mathf.Clamp(currentScale.y, minScaleY, maxScaleY);

        // 3. Pas de schaal toe
        cone.transform.localScale = currentScale;

        // 4. PAS DE POSITIE AAN (De "Translate" fix)
        // Omdat de cone 2 units hoog is, zetten we de positie gelijk aan de schaal
    }

}