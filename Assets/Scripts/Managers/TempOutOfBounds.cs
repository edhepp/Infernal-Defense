using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempOutOfBounds : MonoBehaviour
{
    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position.y < -CameraBounds.Instance.XYBounds.y)
            gameObject.gameObject.SetActive(false);
    }
}
