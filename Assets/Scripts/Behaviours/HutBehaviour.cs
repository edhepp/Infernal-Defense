using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HutBehaviour : MonoBehaviour
{
    [SerializeField]
    private bool _isOnFire = false;
    [SerializeField]
    private EdgeCollider2D _edgeCollider;
    // Start is called before the first frame update

    private OnFireFX _onFireFX;

    [SerializeField]
    private int _maxHealth = 2;
    [SerializeField]
    private int _health = 0;
    private int _onFireMinHealth = 1;
    private void Start()
    {
        //Listen for input events for events
        //
        _health = _maxHealth;
    }
    private void OnEnable()
    {
        // Left off: Bug found animation not playing.
        // each hut in Huts prefab no refference.
        _onFireFX = GetComponent<OnFireFX>();
        _edgeCollider = GetComponent<EdgeCollider2D>();
        _onFireFX.OnFire(false);
        _isOnFire = false;
        _health = _maxHealth;
        _edgeCollider.enabled = true;
    }
    private void RepairHut()
    {
        _onFireFX.OnFire(false);
        _isOnFire = false;
        _edgeCollider.enabled = false;
    }
    public bool DamageHut(int damage = 0)
    {
        if (damage > 0)
        {
            _health--;
            if (!_isOnFire && _health <= _onFireMinHealth)
            {
                IsOnFire();
            }
            else if (_isOnFire)
            {
                _edgeCollider.enabled = false;
                gameObject.SetActive(false);
            }
        }
        return _isOnFire;
    }
    private void IsOnFire()
    {
        _isOnFire = true;
        _onFireFX.OnFire(true);
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        //Check if hut ran out of lives
    }
}
