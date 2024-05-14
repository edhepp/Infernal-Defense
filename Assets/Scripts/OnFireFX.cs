using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnFireFX : MonoBehaviour
{
    [SerializeField] private GameObject _firePrefab;
    private void OnEnable()
    {
        _firePrefab.SetActive(false);
    }
    public void OnFire(bool isOnFire = false)
    {
        _firePrefab.SetActive(isOnFire);
    }
    
    public void DissableFX()
    {
        _firePrefab.SetActive(false);
    }
}
