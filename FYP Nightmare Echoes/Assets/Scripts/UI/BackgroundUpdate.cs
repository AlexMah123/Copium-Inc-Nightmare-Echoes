using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundUpdate : MonoBehaviour
{
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        float currentOrthoSize = mainCam.orthographicSize;
        transform.localScale = new Vector3(1 + currentOrthoSize * 0.05f, 1 + currentOrthoSize * 0.05f, 1);
    }
}
