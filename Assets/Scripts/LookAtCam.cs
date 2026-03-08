using System;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    private void Update()
    {
        if (GameManager.Instance.isGameRunning())
        {
            transform.LookAt(Camera.main.transform);
        }
    }
}
