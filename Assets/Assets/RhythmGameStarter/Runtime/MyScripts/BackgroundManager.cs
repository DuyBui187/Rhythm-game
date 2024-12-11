using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour, ILastSpriteTracker
{
    [Header("Elements")]
    [SerializeField] private SpriteRenderer[] backgroundSprites; // Các sprite nền

    [Header("Settings")]
    [SerializeField] private float movementSpeed = 5; // Tốc độ di chuyển nền

    private Transform lastSpriteTransform; // Transform của sprite cuối cùng

    void Start()
    {
        lastSpriteTransform = backgroundSprites[backgroundSprites.Length - 1].transform;

        for (int i = 0; i < backgroundSprites.Length; i++)
            MoveBackgroundLoop(backgroundSprites[i], movementSpeed, this);
    }

    public void MoveBackgroundLoop(SpriteRenderer backgroundSprite, float speedValue,
        ILastSpriteTracker ilastSpriteTracker)
    {
        Bounds spriteBounds = backgroundSprite.bounds;

        // Tính vị trí mục tiêu di chuyển
        Vector3 targetPosition = backgroundSprite.transform.position + Vector3.left * spriteBounds.size.x;

        LeanTween.move(backgroundSprite.gameObject, targetPosition, spriteBounds.size.x / speedValue)
            .setEase(LeanTweenType.linear)
            .setOnComplete(() =>
            {
                if (IsSpriteOutOfCameraBounds(Camera.main, backgroundSprite))
                {
                    // Sau khi sprite ra khỏi màn hình, di chuyển nó về cuối
                    var lastSpriteTransform = ilastSpriteTracker.GetLastSpriteTransform();
                    Vector3 lastSpritePosition = lastSpriteTransform.position;
                    Bounds lastSpriteBounds = lastSpriteTransform.GetComponent<SpriteRenderer>().bounds;

                    backgroundSprite.transform.position = new Vector3(
                        lastSpritePosition.x + lastSpriteBounds.size.x,
                        backgroundSprite.transform.position.y,
                        backgroundSprite.transform.position.z
                    );

                    ilastSpriteTracker.SetLastSpriteTransform(backgroundSprite.transform);
                }

                // Tiếp tục lặp lại di chuyển
                MoveBackgroundLoop(backgroundSprite, speedValue, ilastSpriteTracker);
            });
    }

    public Bounds CalculateCameraBounds(Camera camera)
    {
        float cameraHeight = camera.orthographicSize * 2f; // Tổng chiều cao của camera
        float cameraWidth = cameraHeight * camera.aspect;  // Chiều rộng dựa trên tỉ lệ khung hình

        Vector3 cameraCenter = camera.transform.position;

        // Tạo một đối tượng Bounds với kích thước và tâm đã tính
        return new Bounds(cameraCenter, new Vector3(cameraWidth, cameraHeight, 0f));
    }

    public bool IsSpriteOutOfCameraBounds(Camera camera, SpriteRenderer spriteRenderer)
    {
        Bounds cameraBounds = CalculateCameraBounds(camera);
        Bounds spriteBounds = spriteRenderer.bounds;

        // Kiểm tra xem sprite đã ra khỏi hoặc chạm vào rìa trái của camera chưa
        return spriteBounds.max.x <= cameraBounds.min.x;
    }

    public Bounds GetSpriteBounds(SpriteRenderer spriteRenderer)
    {
        return spriteRenderer.bounds;
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

public interface ILastSpriteTracker
{
    Transform GetLastSpriteTransform();
    void SetLastSpriteTransform(Transform newTransform);
}