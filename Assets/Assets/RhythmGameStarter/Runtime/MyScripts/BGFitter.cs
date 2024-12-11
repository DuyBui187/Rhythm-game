using RhythmGameStarter;
using System.Collections.Generic;
using UnityEngine;

public class BGFitter : MonoBehaviour
{
    public enum ScaleMode
    {
        ScaleBoth,   // Scale X and Y proportionally
        ScaleX,     // Scale only X-axis to fit width
        ScaleY,     // Scale only Y-axis to fit height
    }

    [System.Serializable]
    public class SpriteScaleConfig
    {
        public List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>(); // Sprite to scale
        public ScaleMode scaleMode = ScaleMode.ScaleBoth; // Scaling mode for this sprite
    }

    [SerializeField] private List<SpriteScaleConfig> spriteConfigs = new List<SpriteScaleConfig>();

    private void OnEnable()
    {
        CanvasHelper.OnResolutionOrOrientationChanged.AddListener(FitBackgrounds);
    }

    private void OnDisable()
    {

        CanvasHelper.OnResolutionOrOrientationChanged.RemoveListener(FitBackgrounds);
    }
    //private void Start()
    //{
    //    FitBackgrounds();
    //}

    private void FitBackgrounds()
    {
        float screenHeight = Camera.main.orthographicSize * 2;
        float screenWidth = screenHeight * Screen.width / Screen.height;

        foreach (var config in spriteConfigs)
        {
            foreach (var spriteRenderer in config.spriteRenderers)
            {
                if (spriteRenderer == null) continue;

                Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
                Vector3 scale = spriteRenderer.transform.localScale;

                switch (config.scaleMode)
                {
                    case ScaleMode.ScaleBoth:
                        scale.x = screenWidth / spriteSize.x;
                        scale.y = screenHeight / spriteSize.y;
                        break;
                    case ScaleMode.ScaleX:
                        scale.x = screenWidth / spriteSize.x;
                        break;

                    case ScaleMode.ScaleY:
                        scale.y = screenHeight / spriteSize.y;
                        break;
                }

                spriteRenderer.transform.localScale = scale;
            }
        }
    }
}