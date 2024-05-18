using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBehavior : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] private int _rockHealth = 1;
    [SerializeField] private LayerMask damageable;
    [SerializeField] private int _destroyProbabiilty = 25;

    private Vector2 _WorldBounds = Vector2.one;
    private float _worldboundsBuffer = 0.0f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        //Call attributes
        GenerateRockAttributes();
    }
    private void Start()
    {
        //Bug: will _WorldBounds update while prefab is dissabled
    }
    void FixedUpdate()
    {
        if(transform.position.y <= -CameraBounds.Instance.XYBounds.y)
        {
            gameObject.SetActive(false);
        }
    }

    private void GenerateRockAttributes()
    {
        // Generates random value for scale
        float randomScaleValue = Random.Range(0.2f, 0.8f);
        // Assigns a random scale value
        rb.transform.localScale = new Vector3(randomScaleValue, randomScaleValue, 0f);
        // Assigns a random gravity value
        rb.gravityScale = Random.Range(0.2f, 0.5f);

        rb.rotation = Random.Range(0.2f, 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HutBehaviour hutDamaged;
        bool isOnFire = false;
        if(collision.CompareTag("Hut"))
        {
            hutDamaged = collision.GetComponent<HutBehaviour>();
            isOnFire = hutDamaged.DamageHut();
            if (isOnFire && DamageHutProbability())
            {
                hutDamaged.DamageHut(1);
                gameObject.SetActive(false);
            }
            else if (!isOnFire)
            {
                hutDamaged.DamageHut(1);
                gameObject.SetActive(false);
            }

        }
    }
    private bool DamageHutProbability()
    {
        int probability = Random.Range(0, 101);
        return probability <= 25;
    }

    private void OnDestroy()
    {
        //MemoryLeak: Enable and dissable often 
        CameraBounds.OnBoundsReset -= () => { _WorldBounds = CameraBounds.Instance.XYBounds; };
    }
}
