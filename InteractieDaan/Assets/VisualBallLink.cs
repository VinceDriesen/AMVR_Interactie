using UnityEngine;
using System;
using System.Collections;

public class VisualBallLink : MonoBehaviour
{
    public MovingTarget myGhost;

    public static event Action<VisualBallLink> OnBallCaptured;

    private Renderer myRenderer;
    private Color originalColor;

    // States
    private bool isSlowMo = false;
    private bool isQuestTarget = false;
    private bool isSelected = false;
    private bool isHovering = false;

    [Header("Kleuren")]
    public Color highlightColor = Color.yellow;
    public Color selectedColor = Color.green; // Correcte keuze
    public Color errorColor = Color.red;      // Foute keuze (NIEUW)
    public Color questTargetColor = Color.blue;

    public void Awake()
    {
        myRenderer = GetComponent<Renderer>();
        if (myRenderer != null)
        {
            originalColor = myRenderer.material.color;
        }
    }

    public void SetHover(bool active)
    {
        isHovering = active;
        UpdateColorState();

        if (myGhost != null) myGhost.SetHover(active);
    }

    public void SetSlowMo(bool active)
    {
        isSlowMo = active;
        UpdateColorState();

        if (myGhost != null) myGhost.SetSlowMo(active);
    }

    public void SetQuestTarget(bool active)
    {
        isQuestTarget = active;
        UpdateColorState();
    }

    private void UpdateColorState()
    {
        if (myRenderer == null) return;

        if (isSelected)
        {
            if (isQuestTarget)
            {
                myRenderer.material.color = selectedColor; // Goed (Groen)
            }
            else
            {
                myRenderer.material.color = errorColor;    // Fout (Rood)
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

    public void SelectTarget()
    {
        if (isSelected) return;

        StartCoroutine(SelectRoutine());
    }

    private IEnumerator SelectRoutine()
    {
        isSelected = true;
        UpdateColorState();

        OnBallCaptured?.Invoke(this);
        yield return new WaitForSeconds(2.0f);

        isSelected = false;

        if (isQuestTarget) isQuestTarget = false;

        UpdateColorState();
    }

    public MovingTarget GetRealBall()
    {
        return myGhost;
    }
}