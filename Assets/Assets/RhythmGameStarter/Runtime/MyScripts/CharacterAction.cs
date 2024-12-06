using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmGameStarter
{
    public class CharacterAction : MonoBehaviour
    {
        [Header("Elements")]
        [SerializeField] private Transform lowPosition; // Vị trí thấp
        [SerializeField] private Transform highPosition; // Vị trí cao
        [SerializeField] private Animator anim; // Animator của nhân vật

        [Header("Settings")]
        [SerializeField] private float switchSpeed = 15f; // Tốc độ di chuyển giữa các vị trí
        [SerializeField] private float holdThreshold = 0.2f; // Thời gian tối thiểu để xem là "giữ nút"

        private int hitRandom;
        private float keyDownTime; // Thời gian bắt đầu nhấn phím
        private bool isOnHighPosition; // Theo dõi trạng thái: đang ở cao hay thấp
        private bool isHoldingKey;
        private bool useTouchInput;

        void Start()
        {
            hitRandom = 1;
        }

        void Update()
        {
            if (useTouchInput) return;

            HandleInput();
            UpdatePosition();
        }

        private void HandleInput()
        {
            // Kiểm tra tấn công khi không giữ phím
            if (!isHoldingKey)
            {
                if (Input.GetKeyDown(KeyCode.Q)) // Tấn công trên cao
                    PressToHighPosition();

                else if (Input.GetKeyDown(KeyCode.W)) // Tấn công ở thấp
                    PressToLowPosition();
            }

            // Kiểm tra nhấn nút Q hoặc W
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.W))
                keyDownTime = Time.time; // Lưu thời gian khi nhấn nút

            // Kiểm tra giữ phím để di chuyển nếu thời gian giữ đủ lâu
            if (Input.GetKey(KeyCode.Q))
            {
                if (Time.time - keyDownTime >= holdThreshold)
                    HoldToHighPosition();
            }

            else if (Input.GetKey(KeyCode.W))
            {
                if (Time.time - keyDownTime >= holdThreshold)
                    HoldToLowPosition();
            }

            // Xử lý khi phím không được nhấn
            else if (isHoldingKey)
                OnKeyUp();
        }

        public void PressToLowPosition()
        {
            isOnHighPosition = false;

            ResetTriggers();
            anim.SetTrigger("DownHit" + hitRandom);
        }

        public void PressToHighPosition()
        {
            isOnHighPosition = true;

            ResetTriggers();
            anim.SetTrigger("UpHit" + hitRandom);
        }

        public void HoldToHighPosition()
        {
            SetKeyState(true, true);
        }

        public void HoldToLowPosition()
        {
            SetKeyState(false, true);
        }

        public void OnKeyUp()
        {
            useTouchInput = false;

            SetKeyState(false, false);
        }

        private void SetKeyState(bool isHighPosition, bool isHolding)
        {
            isOnHighPosition = isHighPosition;
            isHoldingKey = isHolding;
            anim.SetBool("Pressing", isHolding);
        }

        private void ResetTriggers()
        {
            // Đặt lại triggers tấn công và chọn ngẫu nhiên hitRandom
            anim.ResetTrigger("UpHit" + hitRandom);
            anim.ResetTrigger("DownHit" + hitRandom);
            hitRandom = Random.Range(1, 5);
        }

        public void UpdatePosition()
        {
            // Di chuyển nhân vật đến vị trí mục tiêu (cao hoặc thấp)
            Vector2 targetPosition = isOnHighPosition ? highPosition.position : lowPosition.position;
            transform.position = Vector2.Lerp(transform.position, targetPosition, switchSpeed * Time.deltaTime);
        }

        // Gọi từ Animation Event: Kết thúc tấn công và quay về vị trí thấp
        public void OnAttackComplete()
        {
            isOnHighPosition = false;
            anim.SetTrigger("Running");
        }

        public void ActiveTouchInput()
        {
            useTouchInput = true;
        }

        public float GetHoldThreshold()
        {
            return holdThreshold;
        }
    }
}
