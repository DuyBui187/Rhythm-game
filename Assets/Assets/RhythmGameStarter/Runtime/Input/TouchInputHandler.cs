using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RhythmGameStarter
{
    [HelpURL("https://bennykok.gitbook.io/rhythm-game-starter/hierarchy-overview/input")]
    public class TouchInputHandler : BaseTouchHandler
    {
        [Header(" Elements ")]
        [SerializeField] private CharacterAction characterAction;

        [Header("Settings")]
        private float touchHoldThreshold; // Thời gian tối thiểu để coi là "giữ"
        private bool isTouchHolding; // Trạng thái giữ cảm ứng
        private float touchDownTime; // Thời gian bắt đầu cảm ứng

        protected override void Start()
        {
            base.Start();

            touchHoldThreshold = characterAction.GetHoldThreshold();
        }

        void Update()
        {
            // Xử lý các cảm ứng
            for (int i = 0; i < Input.touchCount; ++i)
            {
                var touch = Input.GetTouch(i);

                // Kiểm tra xem cảm ứng có phải là đang trên UI không
                if (touch.phase == TouchPhase.Began && !isTouchHolding &&
                    !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    touchDownTime = Time.time; // Lưu thời gian khi bắt đầu cảm ứng
                    HandleTouchAction(touch); // Xử lý hành động khi bắt đầu cảm ứng
                }

                // Kiểm tra cảm ứng đang kéo dài (Hold)
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    if (Time.time - touchDownTime >= touchHoldThreshold)
                    {
                        isTouchHolding = true; // Đánh dấu trạng thái cảm ứng đang giữ
                        HandleTouchHoldAction(touch); // Xử lý hành động khi cảm ứng được giữ lâu
                    }
                }

                // Kiểm tra kết thúc cảm ứng (Touch End)
                else if (isTouchHolding)
                {
                    isTouchHolding = false; // Đặt lại trạng thái giữ
                    characterAction.OnKeyUp();
                }
            }

            characterAction.UpdatePosition();
        }

        // Xử lý hành động khi cảm ứng bắt đầu
        private void HandleTouchAction(Touch touch)
        {
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                var t = hit.rigidbody.GetComponent<TrackTriggerArea>();
                if (!t) return;

                t.TriggerNote(new TouchWrapper(touch));

                if (t.CompareTag("HighPosition"))
                    characterAction.PressToHighPosition(); // Triển khai hành động nhấp cảm ứng

                else characterAction.PressToLowPosition();
            }
        }

        // Xử lý hành động khi cảm ứng được giữ lâu
        private void HandleTouchHoldAction(Touch touch)
        {
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                var t = hit.rigidbody.GetComponent<TrackTriggerArea>();

                if (!t) return;

                t.TriggerNote(new TouchWrapper(touch));

                if (t.CompareTag("HighPosition"))
                {
                    characterAction.ActiveTouchInput();
                    characterAction.HoldToHighPosition(); // Triển khai hành động giữ cảm ứng
                }

                else
                {
                    characterAction.ActiveTouchInput();
                    characterAction.HoldToLowPosition();
                }
            }
        }

        // Lấy thông tin về cảm ứng theo ID
        public override TouchWrapper GetTouchById(int id)
        {
            var touch = Input.touches.Where(x => x.fingerId == id).FirstOrDefault();
            return new TouchWrapper(touch);
        }
    }
}
