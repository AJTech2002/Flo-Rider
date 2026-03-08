using UnityEngine;
using UnityEngine.InputSystem;

public class WhistleController : MonoBehaviour
{
    public Camera cam;
    public Vector3 offset = new Vector3(0.0f, 1.5f, 0.0f);
    public float maxDistance = 100f;

    public Transform shape;
    public Vector3 activeScale = new Vector3(1.0f, 1.0f, 1.0f);
    public Quaternion activeRotation;
    public float rotationSpeed = 1.0f;
    public float scaleSpeed = 1.0f;


    private Vector3 originalScale;
    private Quaternion originalRotation;
    public float shakeIntensity = 0.5f;
    
    private bool isPressed = false;
    
    public float whistleRadius = 5.0f;
    public Shapes.Disc whistleEffect;

    public float whistleMultiplier = 0.5f;

    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }

        originalScale = transform.localScale;
        originalRotation = transform.rotation;
        
        UpdateRadius(whistleRadius);
    }

    void Update()
    {
        Vector3 groundPosition = ShootRayFromMousePosition();
        transform.position = groundPosition + offset;
        shape.position = groundPosition + Vector3.up * 0.02f;

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
       

        if (isPressed)
        {
           
            // Shake if pressed
            transform.rotation *= Quaternion.Euler(
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity),
                Random.Range(-shakeIntensity, shakeIntensity)
            );
            
          
        }
        
        Vector3 groundPositionFlat = groundPosition;  
        groundPositionFlat.y = 0.0f; // Ignore height for distance calculation  

        // Handle slowing down cars within radius
        foreach (CarMovement carMovement in GameObject.FindObjectsOfType<CarMovement>())
        {
            Vector3 carPosition = carMovement.transform.position;
            carPosition.y = 0.0f; // Ignore height for distance calculation
                
              
            if (carMovement != null && Vector3.Distance(carPosition, groundPositionFlat) < whistleRadius)
            {
                carMovement.UpdateSpeed(whistleMultiplier); // Example: slow down to 50% speed
            }
            else
            {
                carMovement.UpdateSpeed(1.0f); // Reset to normal speed
            }
        }


    }

    public void UpdateRadius(float newRadius)
    {
        whistleRadius = newRadius;
        whistleEffect.Radius  = newRadius;
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