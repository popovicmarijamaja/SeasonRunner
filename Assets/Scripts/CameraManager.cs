using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    private const float YPos = 0f;
    private const float Height = 1f;

    private Camera[] cameras;
    private void Update()
    {
        //SetCamera();
    }

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

    public void SetCamera()
    {
        cameras = FindObjectsOfType<Camera>();

        for (int i = 0; i < cameras.Length; i++)
        {
            float width = 1f / cameras.Length;
            float xPos = i * width;
            cameras[i].rect = new Rect(xPos, YPos, width, Height);
        }
    }
}
