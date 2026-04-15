using UnityEngine;
using UnityEngine.UI;

public class OffScreenIndicator : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // The object to track
    public Camera mainCamera;

    [Header("UI Settings")]
    public RectTransform indicator; // UI element (arrow/icon)
    public Image image; // UI element (arrow/icon)
    public float edgeBuffer = 50f;   // Distance from screen edge

    private Vector3 screenCenter;
    private Vector3 screenBounds;
    public bool hasBeenSeen;

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;


        screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        screenBounds = screenCenter - new Vector3(edgeBuffer, edgeBuffer, 0);

        transform.localScale = new Vector3(1, 1, 1);
    }


    void Update()
    {
        if (target == null || indicator == null) return;

        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);

        // Check if target is in front of camera
        bool isBehind = screenPos.z < 0;
        if (isBehind)
        {
            // Flip position to point in correct direction
            screenPos *= -1;
        }

        bool isOnScreen = screenPos.x > 0 && screenPos.x < Screen.width &&
                          screenPos.y > 0 && screenPos.y < Screen.height &&
                          !isBehind;

        if(isOnScreen)
        {
            if(hasBeenSeen == false)
            {
                hasBeenSeen = true;
            }
            image.enabled = false;
        }
        else
        {
            // Direction from screen center to target
            Vector3 fromCenter = screenPos - screenCenter;
            fromCenter.z = 0;

            // Clamp to screen bounds
            float angle = Mathf.Atan2(fromCenter.y, fromCenter.x);
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            Vector3 edgePos = screenCenter + new Vector3(cos * screenBounds.x, sin * screenBounds.y, 0);

            // Clamp to rectangle edges
            edgePos.x = Mathf.Clamp(edgePos.x, edgeBuffer, Screen.width - edgeBuffer);
            edgePos.y = Mathf.Clamp(edgePos.y, edgeBuffer, Screen.height - edgeBuffer);

            indicator.position = edgePos;

            // Rotate indicator to point toward target
            indicator.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
            image.enabled = true;
            if(hasBeenSeen == true)
            {
                Destroy(gameObject);
            }
        }
    }
}