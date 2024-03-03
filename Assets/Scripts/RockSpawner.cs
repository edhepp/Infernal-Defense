using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _volcanicRockPrefab;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(VolcanicRockSpawnRoutine());
    }

    IEnumerator VolcanicRockSpawnRoutine()
    {
        while (true)
        {
            // Generates random spawn position
            Vector3 generateRandomPosition = new Vector3 (Random.Range(-2f, 2f), 7f, 0);
            // Instantiates volcanic rock prefab
            Instantiate(_volcanicRockPrefab, generateRandomPosition, Quaternion.identity);
            yield return new WaitForSeconds(3f);
        }

    }
}
