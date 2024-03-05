using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnFireFX : MonoBehaviour
{
    [SerializeField] private GameObject _firePrefab;
    [SerializeField] private EdgeCollider2D _EdgeCollider2D;

    private void Awake()
    {
        _EdgeCollider2D = GetComponent<EdgeCollider2D>();
    }
    private void OnEnable()
    {
        _firePrefab.SetActive(false);
        _EdgeCollider2D.enabled = true;
    }
    public void OnFire()
    {
        _EdgeCollider2D.enabled = false;
        _firePrefab.SetActive(true);
    }
    
    public void DissableFX()
    {
        _EdgeCollider2D.enabled = true;
        _firePrefab.SetActive(false);
    }
}
