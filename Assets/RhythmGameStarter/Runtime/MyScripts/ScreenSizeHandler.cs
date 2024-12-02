using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ScreenSizeHandler : MonoBehaviour
{
    [SerializeField] private CanvasScaler scaler;
    private float lastSize;
    private int lastScreenWidth;
    private int lastScreenHeight;
    // Start is called before the first frame update
    void Start()
    {
        lastSize = Camera.main.orthographicSize;
        lastScreenHeight = Screen.height;
        lastScreenWidth = Screen.width;
        AdjustCamera();
    }

    private void AdjustCamera()
    {
        float screenAspect = (float)Screen.width / Screen.height;
        Camera.main.orthographicSize = lastSize / screenAspect;
        Debug.Log($"Screen change: {Screen.width}x{Screen.height} (Aspect: {screenAspect})");
        scaler.referenceResolution.Set(Screen.width, Screen.height);
    }

    // Update is called once per frame
    void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            AdjustCamera();
        }
    }

    //Event for btn
    public void ForceAdjustCamera()
    {
        AdjustCamera();
        //scaler.referenceResolution.Set(Screen.width, Screen.height);
    }
}
