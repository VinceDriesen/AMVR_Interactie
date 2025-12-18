using UnityEngine;

public class WallCatcher : MonoBehaviour
{
    public GameObject ghostPrefab;

    void OnTriggerExit(Collider other)
    {
        MovingTarget target = other.GetComponent<MovingTarget>();

            target.OnWallPass();

            if (ghostPrefab != null)
            {
                GameObject ghostObj = Instantiate(ghostPrefab, other.transform.position, Quaternion.identity);

                GhostBall ghostScript = ghostObj.GetComponent<GhostBall>();
                if (ghostScript != null)
                {
                    ghostScript.Setup(target);
                }
            }
    }
}