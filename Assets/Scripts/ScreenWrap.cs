using UnityEngine;

public class ScreenWrap : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool hasBeenVisible = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(hasBeenVisible == false && spriteRenderer.isVisible)
        {
            hasBeenVisible = true;
        }
        if (hasBeenVisible == false)
        {
            return;
        }

        Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position);

        Vector2 newScreenPos = screenPos;

        if(screenPos.x < 0)
        {
            newScreenPos.x = Screen.width;
        }
        else if (screenPos.x > Screen.width)
        {
            newScreenPos.x = 0;
        }

        if(screenPos.y < 0)
        {
            newScreenPos.y = Screen.height;
        }
        else if(screenPos.y > Screen.height)
        {
            newScreenPos.y = 0;
        }
        if(newScreenPos != screenPos)
        {
            Vector2 newWorldPos = Camera.main.ScreenToWorldPoint(newScreenPos);
            transform.position = newWorldPos;
        }
    }
}
