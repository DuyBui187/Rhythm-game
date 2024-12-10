using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGManager : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private SpriteRenderer[] sprites;
    [SerializeField] private Transform[] transforms;

    [Header(" Settings ")]
    [SerializeField] private float moveSpeed;

    private float currentSpeed;

    private Transform tailTransform;

    void Start()
    {
        currentSpeed = moveSpeed;
        tailTransform = transforms[transforms.Length - 1];
    }

    void Update()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            MoveBG(transforms[i], currentSpeed);

            if (IsSpriteAtCameraBounds(Camera.main, sprites[i]))
            {
                Vector3 tailPosition = tailTransform.position;
                Bounds tailBounds = tailTransform.GetComponent<SpriteRenderer>().bounds;

                // Tính vị trí mới dựa trên bề rộng của sprite cuối cùng
                transforms[i].position = new Vector3(
                    tailPosition.x + tailBounds.size.x,
                    transforms[i].position.y,
                    transforms[i].position.z
                );

                tailTransform = transforms[i];
            }
        }
    }

    public void MoveBG(Transform transform, float speed)
    {
        transform.position += (Vector3)(Vector2.right * -speed * Time.deltaTime);
    }

    public Bounds GetCameraBounds(Camera camera)
    {
        float cameraHeight = camera.orthographicSize * 2f; // Total height of the camera
        float cameraWidth = cameraHeight * camera.aspect; // Width based on aspect ratio

        Vector3 cameraCenter = camera.transform.position;

        // Create a bounds object with the calculated size and center
        return new Bounds(cameraCenter, new Vector3(cameraWidth, cameraHeight, 0f));
    }

    public bool IsSpriteAtCameraBounds(Camera camera, SpriteRenderer spriteRenderer)
    {
        Bounds cameraBounds = GetCameraBounds(camera);
        Bounds spriteBounds = GetSpriteBounds(spriteRenderer);

        //// Check if the sprite's edges are outside or touching the camera's bounds
        //bool isAtLeft = spriteBounds.min.x <= cameraBounds.min.x;
        //bool isAtRight = spriteBounds.max.x >= cameraBounds.max.x;

        //return isAtLeft || isAtRight;
        return spriteBounds.max.x <= cameraBounds.min.x;
    }

    Bounds GetSpriteBounds(SpriteRenderer spriteRenderer)
    {
        return spriteRenderer.bounds;
    }
}
