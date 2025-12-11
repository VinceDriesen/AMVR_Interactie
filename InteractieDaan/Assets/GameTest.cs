using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameTest : MonoBehaviour
{
    [Header("Settings")]
    public float cooldownTime = 2.0f;
    public InputActionProperty startKey;

    [Header("UI")]
    public TMP_Text resultText;
    public TMP_Text countdownText;
    public Slider slider;

    private List<VisualBallLink> allBalls = new List<VisualBallLink>();
    private VisualBallLink currentActiveTarget;
    private float startTime;

    private bool isSequenceActive = false;
    private bool isRoundActive = false;

    private int missedCounter = 0;

    // Variabelen om ronde info te onthouden voor de 'Gevonden' tekst
    private int currentRoundIndex = 0;
    private int totalRondesCount = 0;

    void OnEnable()
    {
        VisualBallLink.OnBallCaptured += HandleBallCaptured;
    }

    void OnDisable()
    {
        VisualBallLink.OnBallCaptured -= HandleBallCaptured;
    }

    private void Start()
    {
        if (resultText != null) resultText.text = "Druk op start!";
        if (countdownText != null) countdownText.text = "";
    }

    void Update()
    {
        if (!isSequenceActive && startKey.action != null && startKey.action.WasPressedThisFrame())
        {
            StartGameButton();
        }
    }

    public void StartGameButton()
    {
        if (!isSequenceActive)
        {
            StartCoroutine(RunTestSequence());
        }
    }

    IEnumerator RunTestSequence()
    {
        isSequenceActive = true;

        totalRondesCount = (slider != null) ? Mathf.Max(1, (int)slider.value) : 1;

        Debug.Log($"Start reeks van {totalRondesCount} rondes.");

        for (int i = 0; i < totalRondesCount; i++)
        {
            currentRoundIndex = i + 1; // Opslaan voor gebruik in HandleBallCaptured

            if (resultText != null) resultText.text = "";

            RefreshBallList();

            if (allBalls.Count == 0)
            {
                Debug.LogError("Geen ballen gevonden!");
                if (resultText != null) resultText.text = "Error: Geen ballen";
                isSequenceActive = false;
                yield break;
            }

            // --- COUNTDOWN ---
            float timer = cooldownTime;
            string rondeInfo = $"Ronde {currentRoundIndex} / {totalRondesCount}";

            while (timer > 0)
            {
                if (countdownText != null)
                {
                    string tijdInfo = Mathf.CeilToInt(timer).ToString();
                    countdownText.text = $"{rondeInfo}\n<size=150%>{tijdInfo}</size>";
                }

                timer -= Time.deltaTime;
                yield return null;
            }

            // Toon ZOEK!
            if (countdownText != null)
            {
                countdownText.text = $"{rondeInfo}\n<size=150%><color=yellow>ZOEK!</color></size>";
            }

            int randomIndex = Random.Range(0, allBalls.Count);
            currentActiveTarget = allBalls[randomIndex];

            if (currentActiveTarget != null)
            {
                currentActiveTarget.SetQuestTarget(true);
                startTime = Time.time;

                isRoundActive = true;
                missedCounter = 0;

                yield return new WaitUntil(() => isRoundActive == false);
            }

            yield return new WaitForSeconds(cooldownTime);
        }

        if (resultText != null) resultText.text = "Test Voltooid!";
        if (countdownText != null) countdownText.text = "";
        isSequenceActive = false;
    }

    void HandleBallCaptured(VisualBallLink capturedBall)
    {
        if (!isRoundActive) return;

        if (capturedBall == currentActiveTarget)
        {
            float duration = Time.time - startTime;
            duration = Mathf.Round(duration * 1000f) / 1000f;

            // --- AANPASSING HIER ---
            // Update de grote centrale tekst met "GEVONDEN!" en de tijd
            if (countdownText != null)
            {
                string rondeInfo = $"Ronde {currentRoundIndex} / {totalRondesCount}";
                countdownText.text = $"{rondeInfo}\n<size=150%><color=green>GEVONDEN!\n{duration}s</color></size>";
            }
            // -----------------------

            if (resultText != null) resultText.text = $"Tijd: {duration}s";

            Transform playerHead = Camera.main.transform;
            Transform balPos = currentActiveTarget.transform;

            Debug.Log($"<color=green>Ronde Voltooid in {duration}s. Missers: {missedCounter}</color>");
            Debug.Log($"DATA: BalPos:{balPos.position} | HeadPos:{playerHead.position} | HeadRot:{playerHead.eulerAngles}");

            currentActiveTarget = null;
            isRoundActive = false;
        }
        else
        {
            missedCounter++;
        }
    }

    void RefreshBallList()
    {
        allBalls = FindObjectsOfType<VisualBallLink>()
            .Where(b => b != null && b.gameObject.activeInHierarchy)
            .ToList();

        foreach (var ball in allBalls)
        {
            if (ball != currentActiveTarget)
                ball.SetQuestTarget(false);
        }
    }
}