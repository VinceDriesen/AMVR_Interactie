using UnityEngine;

public class VisorTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        VisualBallLink link = other.GetComponent<VisualBallLink>();
        
        if (link != null)
        {
            link.SetSlowMo(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        VisualBallLink link = other.GetComponent<VisualBallLink>();
        
        if (link != null)
        {
            link.SetSlowMo(false);
        }
    }
}