using UnityEngine;

public class VisualBallLink : MonoBehaviour
{
    public MovingTarget myGhost;

    private Renderer myRenderer;
    private Color originalColor;
    private bool isSlowMo = false;
    private bool isCaught = false;

    [Header("Kleuren")]
    public Color highlightColor = Color.yellow;
    public Color slowdownColor = Color.cyan;
    public Color selectedColor = Color.green;

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
        if (isCaught || myRenderer == null) return;

        if (active)
        {
            myRenderer.material.color = highlightColor;
            if (myGhost != null) myGhost.SetHover(true);
        }
        else
        {
            myRenderer.material.color = isSlowMo ? slowdownColor : originalColor;
            if (myGhost != null) myGhost.SetHover(false);
        }
    }

    public void SetSlowMo(bool active)
    {
        if (isCaught) return;

        isSlowMo = active;

        if (myRenderer != null)
        {
            if (myRenderer.material.color != highlightColor)
            {
                myRenderer.material.color = active ? slowdownColor : originalColor;
            }
        }

        if (myGhost != null)
        {
            myGhost.SetSlowMo(active);
        }
    }

    public void SelectTarget()
    {
        if (isCaught) return;
        isCaught = true;

        if (myRenderer != null) myRenderer.material.color = selectedColor;

        if (myGhost != null)
        {
            myGhost.OnCaught();
            //Destroy(myGhost.gameObject, 0.5f);
        }
        //Destroy(gameObject, 0.5f);
    }
}