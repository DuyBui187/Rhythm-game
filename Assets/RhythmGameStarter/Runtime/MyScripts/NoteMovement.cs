using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteMovement : MonoBehaviour
{
    [Header(" Elements ")]
    private Transform endPoint;    // Điểm kết thúc
    private Collider boxCollider;

    [Header(" Settings ")]
    public float moveDuration = 1f; // Thời gian di chuyển
    public float rotationSpeed = 360f; // Tốc độ xoay vòng (độ/giây)

    private float jumpHeight; // Chiều cao nhảy
    private float elapsedTime;
    private bool isMoving;
    private Vector3 startPosition; // Vị trí bắt đầu (lấy từ vị trí hiện tại của note)

    void Start()
    {
        boxCollider = GetComponent<Collider>();
    }

    void Update()
    {
        if (isMoving)
        {
            // Cập nhật thời gian
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / moveDuration;

            if (t <= 1f)
            {
                // Tính toán vị trí theo quỹ đạo nhảy parabol
                Vector3 position = Vector3.Lerp(startPosition, endPoint.position, t);
                position.y += Mathf.Sin(t * Mathf.PI) * jumpHeight;

                // Tính toán xoay vòng
                float rotation = t * rotationSpeed * moveDuration;

                // Gán vị trí và xoay
                transform.position = position;
                transform.rotation = Quaternion.Euler(0, 0, rotation);
            }

            else
            {
                // Dừng chuyển động khi hoàn thành
                isMoving = false;
                boxCollider.enabled = true;
                transform.position = endPoint.position;
                transform.rotation = Quaternion.identity;
            }
        }
    }

    // Hàm để kích hoạt chuyển động
    public void StartMovement(Transform endPoint)
    {
        if (!isMoving)
        {
            if (this.endPoint == null) this.endPoint = endPoint;

            isMoving = true;
            boxCollider.enabled = false;
            elapsedTime = 0f;
            jumpHeight = Random.Range(3.5f, 4.5f);
            startPosition = transform.position;
        }
    }
}
