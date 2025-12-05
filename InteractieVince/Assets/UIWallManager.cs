using UnityEngine;

public class UIWallManager : MonoBehaviour
{
    // Singleton patroon: Zorgt dat andere scripts dit script makkelijk kunnen vinden
    public static UIWallManager Instance;

    [Header("Sleep hier de UI elementen in")]
    public RectTransform miniWallPanel; // Het grijze paneel rechtsboven
    public GameObject hitDotPrefab;    // De rode stip prefab

    void Awake()
    {
        // Singleton opzetten
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Deze functie wordt aangeroepen door de grote GhostNet muur
    // relativeX en relativeY zijn waarden tussen 0 en 1 (0% tot 100% positie op de muur)
    public void RegisterHitOnUI(float relativeX, float relativeY, GameObject realBall)
    {
        // 1. Maak een nieuwe rode stip aan op het mini paneel
        GameObject newDot = Instantiate(hitDotPrefab, miniWallPanel);
        
        // 2. Zet de positie goed op basis van de percentages
        RectTransform dotRect = newDot.GetComponent<RectTransform>();
        
        // We gebruiken anchors om de positie te bepalen (0,0 is linksonder, 1,1 is rechtsboven)
        dotRect.anchorMin = new Vector2(relativeX, relativeY);
        dotRect.anchorMax = new Vector2(relativeX, relativeY);
        dotRect.anchoredPosition = Vector2.zero; // Zet hem op het ankerpunt

        // 3. Vertel de stip bij welke echte bal hij hoort
        HitDotController dotController = newDot.GetComponent<HitDotController>();
        if (dotController != null)
        {
            dotController.SetupDot(realBall);
        }
    }
}
