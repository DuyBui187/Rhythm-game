using UnityEngine;

public class SawController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f; // Tốc độ di chuyển từ phải qua trái
    [SerializeField] private float rotateSpeed = 360f; // Tốc độ xoay (độ/giây)
    [SerializeField] private float moveDistance = 10f; // Khoảng cách di chuyển sang trái

    private Vector3 startPosition;
    private Vector3 endPosition;

    void Start()
    {
        // Lưu vị trí bắt đầu và tính toán vị trí kết thúc (dịch sang trái từ startPosition)
        startPosition = transform.position;
        endPosition = startPosition + Vector3.left * moveDistance;

        // Bắt đầu xoay liên tục và di chuyển
        RotateSaw();
        MoveSaw();
    }

    private void RotateSaw()
    {
        // Sử dụng LeanTween để xoay liên tục
        LeanTween.rotateAround(gameObject, Vector3.forward, 360f, 1f / (rotateSpeed / 360f)).setLoopClamp();
    }

    private void MoveSaw()
    {
        // Sử dụng LeanTween để di chuyển từ phải qua trái
        LeanTween.move(gameObject, endPosition, moveSpeed).setOnComplete(() => {
            // Khi kết thúc, reset về vị trí bắt đầu và lặp lại
            transform.position = startPosition;
            MoveSaw();
        });
    }
}
