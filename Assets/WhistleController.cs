using UnityEngine;
using UnityEngine.InputSystem;

public class WhistleController : MonoBehaviour
{
    public Camera cam;
    public Vector3 offset = new Vector3(0.0f, 1.5f, 0.0f);
    public float maxDistance = 100f;

    public Vector3 activeScale = new Vector3(1.0f, 1.0f, 1.0f);
    public Quaternion activeRotation;
    public float rotationSpeed = 1.0f;
    public float scaleSpeed = 1.0f;


    private Vector3 originalScale;
    private Quaternion originalRotation;
    private bool isPressed = false;

    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }

        originalScale = transform.localScale;
        originalRotation = transform.rotation;
    }

    void Update()
    {
        Vector3 groundPosition = ShootRayFromMousePosition();
        transform.position = groundPosition + offset;

        isPressed = Mouse.current.leftButton.isPressed;

        Vector3 targetScale = isPressed ? activeScale : originalScale;
        Quaternion targetRotation = isPressed ? activeRotation : originalRotation;

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            Time.deltaTime * rotationSpeed
        );
        transform.localScale = Vector3.MoveTowards(
            transform.localScale,
            targetScale,
            Time.deltaTime * scaleSpeed
        );
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