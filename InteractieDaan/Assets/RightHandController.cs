using UnityEngine;
using UnityEngine.InputSystem;

public class RightHandController : MonoBehaviour
{
    [Header("Instellingen")]
    public GameObject cone;

    [Header("Schaal Instellingen")]
    public float scaleStep = 0.1f;
    public float minScaleY = 0.1f;
    public float maxScaleY = 2.0f;

    [Header("Input Instellingen")]
    [Tooltip("Vergroten")]
    public InputActionProperty pushInput;
    [Tooltip("Verkleinen")]
    public InputActionProperty pullInput;

    void Start()
    {
        
    }

    void Update()
    {
        HandleScaling();
    }

    void HandleScaling()
    {
        bool tryingToEnlarge = pullInput.action.IsPressed();
        bool tryingToShrink = pushInput.action.IsPressed();

        Vector3 currentScale = cone.transform.localScale;

        if (tryingToEnlarge)
        {
            currentScale.y += scaleStep * Time.deltaTime;
        }
        else if (tryingToShrink)
        {
            currentScale.y -= scaleStep * Time.deltaTime;
        }

        currentScale.y = Mathf.Clamp(currentScale.y, minScaleY, maxScaleY);

        cone.transform.localScale = currentScale;
    }
}