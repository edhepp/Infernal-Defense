using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _volcanicRockPrefab;
    [SerializeField] private List<GameObject> _volcanicRockPool;
    [SerializeField] private Vector3 _volcanicRockPoolPosition = new Vector3(-5.0f, 5.0f, 0.0f);
    [SerializeField] private int _volcanicRockPoolSize = 10;
    [SerializeField] private float _volcanicRockSpawnRate = 3.0f;
    [SerializeField] private float _volcanicRockSpawnDelay = 2.0f;
    //Set max pool size using an algorithem difficulty X Spawn Rate

    private GameObject _lavaRock;

    // Start is called before the first frame update
    private void Awake()
    {
        _volcanicRockPoolSize = Mathf.Abs(_volcanicRockPoolSize);
        _volcanicRockPool = new List<GameObject>(_volcanicRockPoolSize);
        PopulatePool();
    }
    void Start()
    {
        StartCoroutine(VolcanicRockSpawnRoutine());
    }

    IEnumerator VolcanicRockSpawnRoutine()
    {
        yield return new WaitForSeconds(_volcanicRockSpawnDelay);
        while (true)
        {
            // Generates random spawn position
            Vector3 generateRandomPosition = new Vector3 (UnityEngine.Random.Range(-2f, 2f), 7f, 0);

            // Get a rock from pool
            GetLavaRock(generateRandomPosition);
            yield return new WaitForSeconds(_volcanicRockSpawnRate);
        }

    }
    private void GetLavaRock(Vector3 screenSpacePosition)
    {
        _lavaRock = _volcanicRockPool.FirstOrDefault(rock => !rock.gameObject.activeSelf && !rock.gameObject.activeInHierarchy);
        if ( _lavaRock == null )
        {
            _volcanicRockPool.Add(Instantiate(_volcanicRockPrefab, screenSpacePosition, Quaternion.identity, transform));
            _lavaRock = _volcanicRockPool.Last();
        }
        _lavaRock.transform.position = screenSpacePosition;
        _lavaRock.transform.gameObject.SetActive(true);

    }
    private void PopulatePool()
    {
        for (int i = 0; i < _volcanicRockPoolSize; i++)
        {
            GameObject tempRock = Instantiate(_volcanicRockPrefab, _volcanicRockPoolPosition, Quaternion.identity, transform);
            _volcanicRockPool.Add(tempRock);
            _volcanicRockPool.Last().transform.gameObject.SetActive(false);
        }
    }
}