using UnityEngine;
using System.Collections.Generic;

enum RoadType
{
    STRAIGHT,
    TURN,
    T_INTERSECTION,
    INTERSECTION,
}

public class CarMovement : MonoBehaviour
{
    public float speed = 1.0f;
    private float speedMultiplier = 1.0f;
    public float turnSpeed = 80.0f;

    public float mapRadius = 15.0f;


    private GameObject road;
    private GameObject startPoint;
    private GameObject endPoint;
    
    public static float carSpeed = 1.0f;

    void Start()
    {
        GameObject currentRoad = findCurrentRoad();
        if (currentRoad != null && currentRoad != road)
        {
            onChangeRoad(currentRoad);
            transform.rotation = Quaternion.LookRotation(endPoint.transform.position - transform.position);
        }
    }
    
    void Update()
    {
        if (!GameManager.Instance.isGameRunning()) return;
        
        GameObject currentRoad = findCurrentRoad();
        if (currentRoad != null && currentRoad != road)
        {
            onChangeRoad(currentRoad);
        }

        if (startPoint != null)
        {
            DrawDebugPoint(startPoint.transform.position, Color.red, 1.0f);
        }
        
        if (endPoint != null)
        {
            DrawDebugPoint(endPoint.transform.position, Color.blue, 1.0f);
            MoveAndTurnTowards(endPoint.transform.position);
        }

        bool inXAxis = transform.position.x < mapRadius && transform.position.x > -1 * mapRadius;
        bool inZAxis = transform.position.z < mapRadius && transform.position.z > -1 * mapRadius;
        if (!inXAxis || !inZAxis)
        {
            Destroy(gameObject);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            Debug.Log("Car Crash!");
            GameManager.Instance.CarCrashed(transform.position);
        }
    }
    
    void MoveAndTurnTowards(Vector3 target)
    {
        Vector3 direction = target - transform.position;

        if (direction.magnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
        }

        transform.position += transform.forward * speed * speedMultiplier * Time.deltaTime * carSpeed;
    }

    void onChangeRoad(GameObject currentRoad)
    {
        road = currentRoad;
        List<GameObject> roadEnds = GetEmptyChildren(road);
        startPoint = FindClosest(gameObject, roadEnds);
        endPoint = GetRandomExcluding(roadEnds, startPoint);
    }
    
    public void RandomizeEndPoint()
    {
        if (road != null && startPoint != null)
        {
            List<GameObject> roadEnds = GetEmptyChildren(road);
            endPoint = GetRandomExcluding(roadEnds, startPoint);
        }
    }
    
    GameObject GetRandomExcluding(List<GameObject> objects, GameObject exclude)
    {
        if (objects == null || objects.Count == 0) return null;
        
        List<GameObject> candidates = new List<GameObject>();
        foreach (GameObject obj in objects)
        {
            if (obj != exclude)
                candidates.Add(obj);
        }

        if (candidates.Count == 0) return null;
        
        int randomIndex = Random.Range(0, candidates.Count);
        return candidates[randomIndex];
    }
    
    void DrawDebugPoint(Vector3 position, Color color, float size = 0.1f)
    {
        Debug.DrawLine(position + Vector3.up * size, position - Vector3.up * size, color);
        Debug.DrawLine(position + Vector3.right * size, position - Vector3.right * size, color);
        Debug.DrawLine(position + Vector3.forward * size, position - Vector3.forward * size, color);
    }
    
    public void UpdateSpeed(float multiplier)
    {
        speedMultiplier = multiplier;
    }
    
    GameObject FindClosest(GameObject reference, List<GameObject> objects)
    {
        if (objects == null || objects.Count == 0) return null;

        GameObject closest = null;
        float minDistanceSqr = Mathf.Infinity;
        Vector3 referencePosition = reference.transform.position;

        foreach (GameObject obj in objects)
        {
            Vector3 direction = obj.transform.position - referencePosition;
            float distanceSqr = direction.sqrMagnitude; 

            if (distanceSqr < minDistanceSqr)
            {
                minDistanceSqr = distanceSqr;
                closest = obj;
            }
        }

        return closest;
    }
    
    List<GameObject> GetEmptyChildren(GameObject parent)
    {
        List<GameObject> emptyChildren = new List<GameObject>();

        foreach (Transform child in parent.transform)
        {
            Component[] components = child.GetComponents<Component>();
            if (components.Length == 1)
            {
                emptyChildren.Add(child.gameObject);
            }
        }

        return emptyChildren;
    }

    private GameObject findCurrentRoad()
    {
        RaycastHit hit;
        Vector3 rayStart = transform.position + Vector3.up * 0.1f;
        
        if (Physics.Raycast(rayStart, Vector3.down, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (getRoadType(hitObject) != null) return hitObject;
        }
        
        return null;
    }
    
    RoadType? getRoadType(GameObject road)
    {   
        if (road.CompareTag("Straight"))
        {
            return RoadType.STRAIGHT;
        }
        else if (road.CompareTag("Turn"))
        {
            return RoadType.TURN;
        }
        else if (road.CompareTag("T-intersection"))
        {
            return RoadType.T_INTERSECTION;
        }
        else if (road.CompareTag("Intersection"))
        {
            return RoadType.INTERSECTION;
        }

        return null;
    }
}
