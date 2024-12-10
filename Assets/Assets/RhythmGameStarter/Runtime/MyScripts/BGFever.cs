using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGFever : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private Transform startTransform;
    [SerializeField] private Transform endTransform;
    [SerializeField] private BGManager bGManager;
    [SerializeField] private Transform[] squares;

    private Transform tail;

    [Header(" Settings ")]
    private Vector3 startPosition;
    private float speed = 5f;

    void Start()
    {
        tail = squares[squares.Length - 1];
        startPosition = new Vector3(bGManager.GetCameraBounds(Camera.main).max.x, 0, 0);
    }

    void Update()
    {
        for (int i = 0; i < squares.Length; i++)
        {
            bGManager.MoveBG(squares[i], speed);

            if (bGManager.IsSpriteAtCameraBounds(Camera.main, squares[i].GetComponent<SpriteRenderer>()))
            {
                Vector3 tailPosition = tail.position;
                Bounds tailBounds = tail.GetComponent<SpriteRenderer>().bounds;

                // Tính vị trí mới dựa trên bề rộng của sprite cuối cùng
                squares[i].position = new Vector3(
                    tailPosition.x + tailBounds.size.x,
                    squares[i].position.y,
                    squares[i].position.z
                );

                tail = squares[i];
            }
        }
    }

    public void ReturnStartPosition()
    {
        this.transform.position = startPosition;
    }

    public void EnableSquares(bool isOn)
    {
        foreach (Transform child in squares)
            child.gameObject.SetActive(isOn);
    }
}
