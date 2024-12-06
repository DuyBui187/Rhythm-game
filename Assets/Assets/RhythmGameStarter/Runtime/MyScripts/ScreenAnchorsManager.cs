using System.Collections.Generic;
using UnityEngine;

public class ScreenAnchorsManager : MonoBehaviour
{
    public enum AnchorPosition
    {
        TopLeft,
        TopCenter,
        TopRight,
        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }

    [System.Serializable]
    public class AnchorObject
    {
        public GameObject gameObject;
        public AnchorPosition anchorPosition;
        public Vector3 offset;
    }

    public List<AnchorObject> anchoredObjects = new List<AnchorObject>();

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        UpdateAnchors();
    }

    private void Update()
    {
        UpdateAnchors(); // Continuously update for dynamic resizing
    }

    private void UpdateAnchors()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        foreach (var anchorObject in anchoredObjects)
        {
            if (anchorObject.gameObject == null) continue;

            Vector3 targetPosition = Vector3.zero;

            // Screen dimensions
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            // Calculate anchor position in screen space
            switch (anchorObject.anchorPosition)
            {
                case AnchorPosition.TopLeft:
                    targetPosition = new Vector3(0, screenHeight, 0);
                    break;
                case AnchorPosition.TopCenter:
                    targetPosition = new Vector3(screenWidth / 2, screenHeight, 0);
                    break;
                case AnchorPosition.TopRight:
                    targetPosition = new Vector3(screenWidth, screenHeight, 0);
                    break;
                case AnchorPosition.MiddleLeft:
                    targetPosition = new Vector3(0, screenHeight / 2, 0);
                    break;
                case AnchorPosition.MiddleCenter:
                    targetPosition = new Vector3(screenWidth / 2, screenHeight / 2, 0);
                    break;
                case AnchorPosition.MiddleRight:
                    targetPosition = new Vector3(screenWidth, screenHeight / 2, 0);
                    break;
                case AnchorPosition.BottomLeft:
                    targetPosition = new Vector3(0, 0, 0);
                    break;
                case AnchorPosition.BottomCenter:
                    targetPosition = new Vector3(screenWidth / 2, 0, 0);
                    break;
                case AnchorPosition.BottomRight:
                    targetPosition = new Vector3(screenWidth, 0, 0);
                    break;
            }

            // Convert screen space to world space
            targetPosition = mainCamera.ScreenToWorldPoint(targetPosition);
            targetPosition.z = 0; // Keep the object in the same plane

            // Apply offset
            targetPosition += anchorObject.offset;

            // Update GameObject position
            anchorObject.gameObject.transform.position = targetPosition;
        }
    }
}