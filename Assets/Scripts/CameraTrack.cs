using UnityEngine;

public class CameraTrack : MonoBehaviour
{
    public Transform trackObject;
    public float camOffset;
    void Start()
    {
        
    }


    private void LateUpdate()
    {

        if (!trackObject) return;

        transform.position = new Vector3(
            trackObject.position.x,
            trackObject.position.y,
            trackObject.position.z + camOffset
        );
    }

}

