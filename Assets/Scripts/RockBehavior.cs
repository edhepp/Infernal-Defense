using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBehavior : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] private int _rockHealth = 1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Generates random value for scale
        float randomScaleValue = Random.Range(0.2f, 0.8f);
        // Assigns a random scale value
        rb.transform.localScale = new Vector3(randomScaleValue, randomScaleValue, 0f);
        // Assigns a random gravity value
        rb.gravityScale = Random.Range(0.2f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y <= -10)
        {
            Destroy(this.gameObject);
        }
    }

    private void GenerateRockAttributes()
    {

    }
}
