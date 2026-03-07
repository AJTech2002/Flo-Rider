using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera camera;
    public float zoomSpeed = 0.05f;
    public float maxSize = 20.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float newSize = camera.orthographicSize + zoomSpeed * Time.deltaTime;
        camera.orthographicSize = Mathf.Min(newSize, maxSize);
    }
}
