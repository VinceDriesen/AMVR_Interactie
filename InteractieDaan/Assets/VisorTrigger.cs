using UnityEngine;

public class VisorTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // We zoeken nu naar het LINK scriptje op de visuele bal
        VisualBallLink link = other.GetComponent<VisualBallLink>();
        
        if (link != null)
        {
            // Zet slow-mo AAN op de ghost via de link
            link.myGhost.SetSlowMo(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Als de visuele bal de lamp verlaat...
        VisualBallLink link = other.GetComponent<VisualBallLink>();
        
        if (link != null)
        {
            // Zet slow-mo UIT
            link.myGhost.SetSlowMo(false);
        }
    }
}