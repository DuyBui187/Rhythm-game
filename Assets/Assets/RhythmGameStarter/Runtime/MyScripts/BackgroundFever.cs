using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundFever : MonoBehaviour, ILastSpriteTracker
{
    [Header("Elements")]
    [SerializeField] private Transform endTransform;
    [SerializeField] private BackgroundManager backgroundManager;
    [SerializeField] private SpriteRenderer[] backgroundSprites;

    [Header("Settings")]
    [SerializeField] private float movementSpeed = 10;

    private Transform lastSpriteTransform;
    private Vector3 initialPosition;

    void Start()
    {
        lastSpriteTransform = backgroundSprites[backgroundSprites.Length - 1].transform;
        initialPosition = new Vector3(backgroundManager.CalculateCameraBounds(Camera.main).max.x, 0, 0);
    }

    public void ActivateFeverMode()
    {
        // Đặt vị trí của background về vị trí ban đầu và bật các hình vuông
        transform.position = initialPosition;

        // Bật các sprite
        ToggleBackgroundSprites(true);

        // Đặt alpha của tất cả các background sprite về 1 (không mờ)
        foreach (SpriteRenderer sprite in backgroundSprites)
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);

        // Di chuyển tất cả các background sprite theo tốc độ đã cài đặt
        for (int i = 0; i < backgroundSprites.Length; i++)
            backgroundManager.MoveBackgroundLoop(backgroundSprites[i], movementSpeed, this);
    }

    public void ReturnToInitialPosition()
    {
        transform.position = initialPosition;
        // Làm mờ dần các background sprites
        foreach (SpriteRenderer sprite in backgroundSprites)
        {
            LeanTween.alpha(sprite.gameObject, 0f, 1f).setOnComplete(() =>
            {
                LeanTween.cancel(sprite.gameObject);

                for (int i = 0; i < backgroundSprites.Length; i++)
                {
                    var xOffset = (i == 0)
            ? initialPosition.x + backgroundSprites[i].bounds.size.x / 2
            : backgroundSprites[i - 1].transform.position.x + backgroundSprites[i].bounds.size.x;

                    backgroundSprites[i].transform.position = new Vector3(xOffset, transform.position.y, transform.position.z);
                }

                // Sau khi làm mờ xong, tắt các sprite
                ToggleBackgroundSprites(false);
            });
        }

        Debug.Log("Returning to initial position");
    }

    private void ToggleBackgroundSprites(bool isActive)
    {
        // Bật hoặc tắt tất cả background sprites
        foreach (SpriteRenderer sprite in backgroundSprites)
            sprite.gameObject.SetActive(isActive);
    }

    public Transform GetLastSpriteTransform()
    {
        return lastSpriteTransform;
    }

    public void SetLastSpriteTransform(Transform newTransform)
    {
        lastSpriteTransform = newTransform;
    }
}
