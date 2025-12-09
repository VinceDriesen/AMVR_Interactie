using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LeftControllerBehaviour : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Transform selectedWall;
    private bool isGrabbing = false;

    private float currentGrabDistance = 0f;

    [Header("Instellingen")]
    public float rayDistance = 20f;
    public LayerMask wallLayer;

    [Header("Beweging Instellingen")]
    public float pushPullSpeed = 4.0f;
    public float stepSize = 0.5f;
    public float holdDelay = 0.4f;

    [Header("Input Instellingen")]
    public string grabButton = "XRI_Left_Grip";
    public string pushInput = "XRI_Left_PrimaryButton";
    public string pullInput = "XRI_Left_SecondaryButton";

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.useWorldSpace = true;

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    void Update()
    {
        float gripValue = Input.GetAxis(grabButton);
        bool gripPressed = gripValue > 0.5f;

        if (!gripPressed && isGrabbing)
        {
            ReleaseWall();
        }

        if (isGrabbing && selectedWall != null)
        {
            HandleMovement();
            DrawLineToTarget(selectedWall.position);
        }
        else
        {
            ScanForWalls(gripPressed);
        }
    }

    void ScanForWalls(bool gripJustPressed)
    {
        lineRenderer.SetPosition(0, transform.position);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, rayDistance, wallLayer, QueryTriggerInteraction.Collide))
        {
            lineRenderer.SetPosition(1, hit.point);

            if (hit.collider.CompareTag("Wall"))
            {
                lineRenderer.startColor = Color.yellow;
                lineRenderer.endColor = Color.yellow;

                if (gripJustPressed && !isGrabbing)
                {
                    GrabWall(hit.transform, hit.distance);
                }
            }
            else if (hit.collider.CompareTag("WallCatcher"))
            {
                lineRenderer.startColor = Color.blue;
                lineRenderer.endColor = Color.blue;
            }
            else
            {
                lineRenderer.startColor = Color.red;
                lineRenderer.endColor = Color.red;
            }
        }
        else
        {
            lineRenderer.SetPosition(1, transform.position + (transform.forward * rayDistance));
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
        }
    }

    void GrabWall(Transform wall, float distance)
    {
        isGrabbing = true;
        selectedWall = wall;
        currentGrabDistance = distance;

        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;
    }

    void ReleaseWall()
    {
        isGrabbing = false;
        selectedWall = null;
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    void HandleMovement()
    {
        // Werkt niet man
        //float pushRaw = Input.GetAxisRaw(pushInput);
        //float pullRaw = Input.GetAxisRaw(pullInput);
        //bool tryingToPush = pushRaw > .2f;
        //bool tryingToPull = pullRaw > .2f;

        //Debug.Log($"Push: {pushRaw}, Pull: {pullRaw}");

        //if (tryingToPush)
        //{
        //    currentGrabDistance += stepSize;
        //}
        //else if (tryingToPull)
        //{
        //    currentGrabDistance -= stepSize;
        //}

        currentGrabDistance = Mathf.Clamp(currentGrabDistance, 0.5f, rayDistance);
        
        Vector3 newPosition = transform.position + (transform.forward * currentGrabDistance);
        selectedWall.position = newPosition;
    }

    void DrawLineToTarget(Vector3 targetPos)
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, targetPos);
    }
}