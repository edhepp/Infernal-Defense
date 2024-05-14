using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetDissabledInParent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UI_Manager.RestartEvent += () => RestartAll();
        UI_Manager.StartEvent += () => RestartAll();
    }

    private void RestartAll()
    {
        foreach(Transform t in transform) 
        {
            t.gameObject.SetActive(false);
            t.gameObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        UI_Manager.RestartEvent -= () => RestartAll();
        UI_Manager.StartEvent -= () => RestartAll();
    }
}
