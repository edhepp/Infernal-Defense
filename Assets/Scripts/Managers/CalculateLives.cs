using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngineInternal;

public class CalculateLives : MonoBehaviour
{

    //Temperary: Trigger Game over
    public delegate void GameState();
    public static event GameState GameOverEvent;

    private int _lives = -1;
    void Start()
    {
        HutBehaviour.AddHutEvent += AddLives;
        HutBehaviour.RemoveHutEvent += RemoveLives;
    }
    private void AddLives()
    {
        if(_lives < 0)
        {
            _lives = 0;
        }
        _lives++;
        //update UI
        UI_Manager.Instance.SetLives(_lives);
    }
    private void RemoveLives()
    {
        if(_lives < 0)
        {
            return;
        }
        _lives--;
        if(_lives <= 0)
        {
            GameOverEvent?.Invoke();
        }
        //Update UI
        UI_Manager.Instance.SetLives(_lives);
    }

    private void OnDisable()
    {
        HutBehaviour.AddHutEvent -= AddLives;
        HutBehaviour.RemoveHutEvent -= RemoveLives;
    }
}
