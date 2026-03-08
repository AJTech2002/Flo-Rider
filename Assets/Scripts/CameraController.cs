using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameManager gameManager;
    public Camera camera;
    public float zoomSpeed = 0.05f;
    public float maxSize = 20.0f;
    public float crashSize = 7.0f;

    public Vector3 targetPosition = new Vector3(-4.34f, 7.94f, -7.59f);
    

    // Update is called once per frame
    void Update()
    {
        GameState gameState = gameManager.getGameState();
        float newSize;
        switch (gameState)
        {
            case GameState.GAME:
                newSize = camera.orthographicSize + zoomSpeed * Time.deltaTime;
                camera.orthographicSize = Mathf.Min(newSize, maxSize);            
                break;
            case GameState.GAME_OVER:
                newSize = camera.orthographicSize - 0.2f * zoomSpeed * Time.deltaTime;
                camera.orthographicSize = Mathf.Max(newSize, crashSize);            
                break;
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.01f);
    }
}
