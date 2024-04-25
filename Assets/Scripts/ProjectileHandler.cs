using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileHandler : MonoBehaviour
{
    //Todo: create a pool for projectiles

    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Rigidbody2D pivot;
    [SerializeField] private float detachDelay = .2f;
    [SerializeField] private float respawnDelay = 1f;

    private Rigidbody2D currentProjectileRigidbody;
    private SpringJoint2D currentProjectileSpringJoint;

    private Camera mainCamera;
    private bool isDragging;

    void Start()
    {
        mainCamera = Camera.main;

        SpawnNewProjectile();
    }

    void Update()
    {
        if (currentProjectileRigidbody == null) { return; }

        if (!Touchscreen.current.primaryTouch.press.isPressed)
        {
            if (isDragging)
            {
                LaunchProjectile();
            }

            isDragging = false;
            return;
        }

        isDragging = true;
        currentProjectileRigidbody.isKinematic = true;

        // Read touch position on screen
        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        // Convert from pixels to world position
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(touchPosition);
        // Move ball to touch position
        currentProjectileRigidbody.position = worldPosition;
        currentProjectileRigidbody.isKinematic = false;

    }

    private void SpawnNewProjectile()
    {
        GameObject projectileInstance = Instantiate(projectilePrefab, pivot.position, Quaternion.identity);
        currentProjectileRigidbody = projectileInstance.GetComponent<Rigidbody2D>();
        currentProjectileSpringJoint = projectileInstance.GetComponent <SpringJoint2D>();

        currentProjectileSpringJoint.connectedBody = pivot;
    }

    private void LaunchProjectile()
    {
        currentProjectileRigidbody.isKinematic = false;
        currentProjectileRigidbody = null;

        Invoke(nameof(DetachProjectile), detachDelay);
    }

    private void DetachProjectile()
    {
        currentProjectileSpringJoint.enabled = false;
        currentProjectileSpringJoint = null;

        Invoke(nameof(SpawnNewProjectile), respawnDelay);
    }
}
