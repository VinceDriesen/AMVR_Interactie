using UnityEngine;
using System;
using System.Collections;

public class VisualBallLink : MonoBehaviour
{
    public MovingTarget myGhost;

    public static event Action<VisualBallLink> OnBallCaptured;

    [Header("Interaction Settings")]
    public Color highlightColor = Color.yellow;
    public Color selectedColor = Color.green;
    public Color errorColor = Color.red;
    public Color questTargetColor = Color.blue;

    [Header("Geluid")]
    public AudioClip passSound;
    private AudioSource audioSource;

    private Renderer myRenderer;
    private Color originalColor;

    private bool isQuestTarget = false;
    private bool isSelected = false;
    private bool isHovering = false;

    public void Awake()
    {
        myRenderer = GetComponent<Renderer>();
        if (myRenderer != null)
        {
            originalColor = myRenderer.material.color;
        }
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

    public void SetHover(bool active)
    {
        isHovering = active;
        UpdateColorState();

        if (myGhost != null) myGhost.SetHover(active);
    }

    public void SetSlowMo(bool active)
    {
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

    public void SelectTarget()
    {
        if (isSelected) return;

        StartCoroutine(SelectRoutine());
    }

    private IEnumerator SelectRoutine()
    {
        isSelected = true;
        if (passSound != null && isQuestTarget)
        {
            audioSource.PlayOneShot(passSound);
        }
        UpdateColorState();

        OnBallCaptured?.Invoke(this);
        yield return new WaitForSeconds(2.0f);

        isSelected = false;

        if (isQuestTarget) isQuestTarget = false;

        UpdateColorState();
    }
}