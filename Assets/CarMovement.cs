using UnityEngine;

public class CarMovement : MonoBehaviour
{
    public float speed = 1.0f;
    public float rotationAngle = 5.0f;
    public float rotationSpeed = 5.0f;

    public float maxPosZ = 11.0f;
    public float minPosZ = -12.0f;

    public float maxRotationMultiplier = 2.0f;

    private Quaternion initialRotation;
    private float rotationOffset;
    private float rotationMultiplier;

    void Start()
    {
        initialRotation = transform.rotation;
        rotationOffset = Random.Range(0f, 100f);
        rotationMultiplier = Random.Range(1f, maxRotationMultiplier);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
        Vector3 currentPosition = transform.position;
        if (transform.position.z > maxPosZ)
        {
            transform.position = new Vector3(currentPosition.x, currentPosition.y, minPosZ);
        }
        else if (transform.position.z < minPosZ)
        {
            transform.position = new Vector3(currentPosition.x, currentPosition.y, maxPosZ);
        }

        float sinValue = Mathf.Sin(Time.time * rotationSpeed + rotationOffset);
        float angle = sinValue * rotationAngle * rotationMultiplier;

        Quaternion targetRotation = initialRotation * Quaternion.Euler(0, 0, angle);
        transform.rotation = targetRotation;
    }
}
