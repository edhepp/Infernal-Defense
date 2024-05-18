using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    public delegate void BoundsReset();
    public static event BoundsReset OnBoundsReset;

    public static CameraBounds Instance;
    public Vector2 XYBounds { get; private set; }
    private Camera cam;

    private void Awake()
    {
        if(CameraBounds.Instance == null)
        {
            Instance = this;
            if(CameraBounds.Instance != this)
            {
                Destroy(this);
                return;
            }
        }
    }
    void Start()
    {
        cam = CameraEventManager.Instance.CameraReff;
        CameraEventManager.UpdateBounds += UpdateBounds;
        //Register to events when Camera is change from ortho to perspective
        //Register to events when screen size is changed
        //Bug: might have to pause the game to apply Bound changes.
    }
    private void UpdateBounds()
    {
        if (cam.orthographic)
        {
            SetOrthoBounds();
        }
        if (!cam.orthographic)
        {
            SetPerspectiveBounds();
        }
        OnBoundsReset?.Invoke();
    }
    private float cameraHeight;
    private float cameraWidth;
    private void SetOrthoBounds()
    {
        if(cam != null)
        {
            cameraHeight = cam.orthographicSize;
            cameraWidth = cameraHeight * cam.aspect;
            XYBounds = new Vector2(cameraWidth, cameraHeight);
            Debug.Log($"Set Ortho Camera bounds set to X{XYBounds.x}, Y{XYBounds.y}");
        }
    }
    private float camDistance;
    private float frustumHeight;
    private float frustumWidth;
    private void SetPerspectiveBounds()
    {
        if(cam != null)
        {
            camDistance = Mathf.Abs(cam.transform.position.z);
            frustumHeight = camDistance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
            frustumWidth = frustumHeight * cam.aspect;
            XYBounds = new Vector2(frustumWidth, frustumHeight);
            Debug.Log($"Set Perspective Camera bounds to: {XYBounds.x} {XYBounds.y}");
        }
    }
    private void OnDisable()
    {
        CameraEventManager.UpdateBounds -= UpdateBounds;
    }
}
