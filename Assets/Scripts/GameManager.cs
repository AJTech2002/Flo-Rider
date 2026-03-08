using UnityEngine;

public enum GameState
{
    MENU,
    GAME,
    GAME_OVER
}

public class GameManager : MonoBehaviour
{
    public CameraController cameraController;
    public GameState state = GameState.GAME;
    
    public static GameManager Instance { get; private set; }
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    public void StartGame()
    {
        state = GameState.GAME;
        GameObject.FindAnyObjectByType<CarSpawner>().StartSpawning();
        GameObject.FindAnyObjectByType<RhythmEngine>().StartSong();
    }

    public void CarCrashed(Vector3 position)
    {
        state = GameState.GAME_OVER;
        Vector3 cameraOffset = 10.0f * cameraController.camera.transform.forward;
        cameraController.targetPosition = position - cameraOffset;
    }

    public bool isGameRunning()
    {
        return state == GameState.GAME;
    }

    public GameState getGameState()
    {
        return state;
    }
}