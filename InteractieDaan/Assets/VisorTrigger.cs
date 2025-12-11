using UnityEngine;

public class VisorTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Something entered Visor: {other.gameObject.name}");
        VisualBallLink link = other.GetComponent<VisualBallLink>();
        
        if (link != null)
        {
            Debug.Log("Visual ball entered visor trigger: " + link);
            link.SetSlowMo(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        VisualBallLink link = other.GetComponent<VisualBallLink>();
        
        if (link != null)
        {
            Debug.Log("Visual ball exited visor trigger: " + link);
            link.SetSlowMo(false);
        }
    }
}