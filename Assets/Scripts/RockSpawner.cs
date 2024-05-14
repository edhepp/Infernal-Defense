using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class RockSpawner : MonoBehaviour
{
    //Todo: create a pool for lava rocks

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
        UI_Manager.RestartEvent += () => DissablePool();
        UI_Manager.ContinueEvent += GameStared;
        UI_Manager.EscButtonEvent += PlayPause;
        UI_Manager.StartEvent += GameStared;
        UI_Manager.QuitEvent += QuitGame;
        StartCoroutine(VolcanicRockSpawnRoutine());
    }
    private bool _quitGame = false;
    private void QuitGame()
    {
        GameStared();
        _quitGame = true;
    }
    private void PlayPause(bool isPlaying)
    {
        float timeScale = isPlaying ? 1.0f : 0.1f;
        Time.timeScale = timeScale;
    }

    private void UI_Manager_ContinueEvent()
    {
        throw new NotImplementedException();
    }

    private bool _gameStarted = false;
    private void GameStared()
    {
        _gameStarted = true;
        PlayPause(true);
        if (_quitGame)
        {
            _quitGame = false;
            DissablePool();
        }
    }
    IEnumerator VolcanicRockSpawnRoutine()
    {
        // Delay before game starts
        // Bug: On restart Must add _volcanicRockSpawnDelay in the while loop.
        yield return new WaitForSeconds(_volcanicRockSpawnDelay);
        while (true)
        {
            if (_gameStarted)
            {
                // Generates random spawn position
                Vector3 position = GenerateRandomPosition();

                // Get a rock from pool
                GetLavaRock(position); 
            }
            yield return new WaitForSeconds(_volcanicRockSpawnRate);
        }
    }
    private int _trackHutCount = 0;
    [SerializeField]
    private int _trackHutIntervals = 3;
    private Vector3 GenerateRandomPosition()
    {
        float xPosition = 0.0f;
        Vector3 ranPosition = new Vector3(UnityEngine.Random.Range(-2f, 2f), 7f, 0);
        _trackHutCount++;
        if(_trackHutCount % _trackHutIntervals == 0)
        {
            //Drop rock near or on hut.
            // must include rock size and a range point of hut size
            // spawn on huts X axes for now
            Debug.Log("Random Hut Selected", transform);
            xPosition = GetRandomActiveHut();
            ranPosition = new Vector3(xPosition, ranPosition.y, ranPosition.z);
        }
        return ranPosition;
    }
    [SerializeField]
    private Transform _hutsParent;
    private List<Transform> _hutsInParent = new List<Transform>();
    private float GetRandomActiveHut()
    {
        if(_hutsInParent.Count <= 0)
        {
            foreach(Transform t in _hutsParent) 
            {
                _hutsInParent.Add(t);
            }
        }
        // create a new list of active huts then select a random hut from list
        List<Transform> activeChildren = _hutsInParent.Where(x => x.gameObject.activeSelf).ToList();
        if(activeChildren.Count > 0)
        {
            int randHut = UnityEngine.Random.Range(0, activeChildren.Count);
            return activeChildren[randHut].transform.position.x;
        }
        return 0.0f;
        // return x position from selected hut.
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
    private void DissablePool()
    {
        foreach(Transform t in transform) { t.gameObject.SetActive(false); }
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
    private void OnDestroy()
    {
        UI_Manager.RestartEvent += () => DissablePool();
        UI_Manager.EscButtonEvent -= PlayPause;
        UI_Manager.ContinueEvent -= GameStared;
        UI_Manager.StartEvent -= GameStared;
        UI_Manager.QuitEvent += QuitGame;
    }
}
