using UnityEngine;

public class CarAnimation : MonoBehaviour
{
    public float rotationSpeed = 5.0f;
    public float rotationAngle = 5.0f;
    private Quaternion initialRotation;
    private float rotationOffset;
    private float rotationMultiplier;
    public float maxRotationMultiplier = 2.0f;
    
    void Start()
    {
        initialRotation = transform.localRotation;
        rotationOffset = Random.Range(0f, 100f);
        rotationMultiplier = Random.Range(1f, maxRotationMultiplier);
    }
    
    void Update()
    {
        if (GameManager.Instance.isGameRunning())
        {
            RockCar();
        }
    }
    
    private void RockCar()
    {
        float sinValue = Mathf.Sin(Time.time * rotationSpeed + rotationOffset);
        float angle = sinValue * rotationAngle * rotationMultiplier;

        Quaternion targetRotation = initialRotation * Quaternion.Euler(0, 0, angle);
        transform.localRotation = targetRotation;
    }
}
