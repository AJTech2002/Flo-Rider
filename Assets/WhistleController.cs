using UnityEngine;
using UnityEngine.InputSystem;

public class WhistleController : MonoBehaviour
{
    public Camera cam;
    public Vector3 offset;
    public float maxDistance = 100f;

    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }
    }

    void Update()
    {
        Vector3 groundPosition = ShootRayFromMousePosition();
        transform.position = groundPosition + offset;
    }

    Vector3 ShootRayFromMousePosition()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = cam.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            // Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red);
            return hit.point;
        }
        else
        {
            return new Vector3(0.0f, 0.0f, 0.0f);
        }
    }
}