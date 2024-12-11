using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using RhythmGameStarter;
public class ScreenSizeHandler : MonoBehaviour
{
    float targetOrthographicSize = 10f;
    private void OnEnable()
    {
        CanvasHelper.OnResolutionOrOrientationChanged.AddListener(AdjustCameraForVerticalScaling);
    }
    private void OnDisable()
    {
        CanvasHelper.OnResolutionOrOrientationChanged.RemoveListener(AdjustCameraForVerticalScaling);
    }

    private void AdjustCameraForVerticalScaling()
    {
        float screenAspect = (float)Screen.width / Screen.height;
        Camera.main.orthographicSize = targetOrthographicSize / screenAspect;
        Debug.Log($"Screen change: {Screen.width}x{Screen.height} (Aspect: {screenAspect})");
        //scaler.referenceResolution.Set(Screen.width, Screen.height);
    }

    //private void AdjustCameraForHorizontalScaling()
    //{
    //    float screenAspect = (float)Screen.width / Screen.height;

    //    // Adjust orthographic size for consistent horizontal scaling
    //    Camera.main.orthographicSize = targetOrthographicSize / (2 * screenAspect);

    //    Debug.Log($"Screen resolution: {Screen.width}x{Screen.height}, Aspect: {screenAspect}, Orthographic Size: {Camera.main.orthographicSize}");
    //}

}