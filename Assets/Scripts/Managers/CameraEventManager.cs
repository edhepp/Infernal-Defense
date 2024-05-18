using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEventManager : MonoBehaviour
{
    public delegate void CameraBounds();
    public static event CameraBounds UpdateBounds;
    public static CameraEventManager Instance { get; private set; }
    public Camera CameraReff { get; private set; }
    private void Awake()
    {
        Instance = this;
        CameraReff = GetComponent<Camera>();
    }
    private bool isOrthographic;
    private float currentOrthoSize;
    private void Start()
    {
        isOrthographic = CameraReff.orthographic;
        currentOrthoSize = CameraReff.orthographicSize;
    }
    // Checking if Camera size should be updated with fixed update or update
    void FixedUpdate()
    {
        if(CameraReff.orthographic != isOrthographic)
        {
            isOrthographic = CameraReff.orthographic;
            UpdateBounds?.Invoke();
        }
        else if(currentOrthoSize != CameraReff.orthographicSize)
        {
            currentOrthoSize = CameraReff.orthographicSize;
            UpdateBounds?.Invoke();
        }
    }
}
