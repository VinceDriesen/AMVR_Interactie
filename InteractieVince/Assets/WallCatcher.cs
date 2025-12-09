using UnityEngine;

public class WallCatcher : MonoBehaviour
{
    public GameObject ghostPrefab; // Sleep hier je Ghost Prefab in!

    // Wanneer de bal de trigger VERLAAT (dus erdoorheen is)
    void OnTriggerExit(Collider other)
    {
        // LOG 1: Check of de trigger überhaupt werkt
        MovingTarget target = other.GetComponent<MovingTarget>();

            target.OnWallPass();

            if (ghostPrefab != null)
            {
                Debug.Log($"[WallCatcher] Spawning ghost op positie: {other.transform.position}");
                GameObject ghostObj = Instantiate(ghostPrefab, other.transform.position, Quaternion.identity);

                GhostBall ghostScript = ghostObj.GetComponent<GhostBall>();
                if (ghostScript != null)
                {
                    ghostScript.Setup(target);
                }
            }
    }
}