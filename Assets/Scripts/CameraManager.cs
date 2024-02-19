using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    private const float YPos = 0f;
    private const float Height = 1f;

    private Camera[] cameras;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCamera(int numberOfPlayers)
    {
        cameras = FindObjectsOfType<Camera>();
        for(int i = 0; i < numberOfPlayers; i++)
        {
            float width = 1f / numberOfPlayers;
            float xPos = i * width;
            cameras[i].rect = new Rect(xPos, YPos, width, Height);
        }
    }
}
