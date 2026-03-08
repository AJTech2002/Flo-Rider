using System;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    private Transform originalParent;
    public float height = 1.0f;
    public AudioClip warningSound;
    private void Awake()
    {
        originalParent = transform.parent;
    }

    private void LateUpdate()
    {
        if (GameManager.Instance.isGameRunning())
        {
            GameObject cam = GameManager.Instance.cameraController.camera.gameObject;
            transform.LookAt(cam.transform);
            transform.position = originalParent.position + Vector3.up * height; // Adjust the height as needed
        }
    }

    public void PlayWarningSound()
    {
        AudioSource.PlayClipAtPoint(warningSound, transform.position);
    }
}