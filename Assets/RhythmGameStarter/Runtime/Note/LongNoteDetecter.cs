using UnityEngine;
using UnityEngine.Events;

namespace RhythmGameStarter
{
    public class LongNoteDetecter : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;

        public UnityEvent OnNoteTouchDown, OnNoteTouchUp, OnNoteHoldProgress, OnNoteHoldComplete;

        [HideInInspector] public bool exitedLineArea = false;

        private bool isHolding = false;
        private float holdProgress = 0f;
        private float holdDuration = 1.5f; // Thời gian yêu cầu để hoàn thành LongPress

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void OnTouchDown()
        {
            OnNoteTouchDown.Invoke();
            var c = spriteRenderer.color;
            c.a = 0.5f;
            spriteRenderer.color = c;

            // Bắt đầu quá trình giữ
            isHolding = true;
            holdProgress = 0f;
            exitedLineArea = false;

            StartCoroutine(HandleHoldProgress());
        }


        public void OnTouchUp()
        {
            OnNoteTouchUp.Invoke();

            var c = spriteRenderer.color;
            c.a = 1f;
            spriteRenderer.color = c;

            isHolding = false; // Ngừng giữ
        }

        public void OnTouchHold()
        {
            // Đảm bảo khi giữ note, độ sáng thay đổi mượt mà
            var c = spriteRenderer.color;
            c.a = Mathf.Lerp(0.5f, 1f, Mathf.PingPong(Time.time * 0.5f, 1));  // Thay đổi độ sáng của màu sắc
            spriteRenderer.color = c;
        }

        void OnTriggerExit(Collider col)
        {
            if (col.tag == "LineArea")
            {
                exitedLineArea = true;  // Mark that the long note has exited the valid area
                isHolding = false;  // Stop holding the note
                OnNoteTouchUp.Invoke();  // Trigger the touch up event
            }
        }

        private System.Collections.IEnumerator HandleHoldProgress()
        {
            while (isHolding && !exitedLineArea)
            {
                holdProgress += Time.deltaTime;

                // Gọi sự kiện khi tiến trình cập nhật
                OnNoteHoldProgress.Invoke();

                if (holdProgress >= holdDuration)
                {
                    OnNoteHoldComplete.Invoke();
                    isHolding = false; // Kết thúc quá trình giữ
                }

                yield return null;
            }
        }
    }
}
