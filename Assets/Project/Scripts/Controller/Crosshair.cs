using System;
using Dependencies;
using UnityEngine;

namespace Controller
{
    public class Crosshair
    {
        public event Action<Vector3> PositionUpdated;
        public event Action ShootStarted;
        public event Action ShootStopped;
        public RectTransform transform { get; }

        private CrosshairReferences references;

        private float dragSpeed;
        private RectTransform canvasRect;
        private Canvas canvas;
        private Camera camera;
        
        // for video
        private RectTransform handTransform;
        
        public Crosshair(CrosshairReferences references)
        {
            this.references = references;
            transform = references.crosshairTransform;
            canvasRect = references.crosshairCanvasRect;
            canvas = canvasRect.GetComponent<Canvas>();
            dragSpeed = references.crosshairDragSpeed;
            camera = this.references.camera;

            handTransform = references.handTransform;
            
            references.joystick.JoystickMoveUpdate += MoveCrosshair;
            references.joystick.JoystickActivated += () => ShootStarted?.Invoke();
            references.joystick.JoystickDisabled += () => ShootStopped?.Invoke();

            // for Video
            // references.joystick.JoystickJustMove += MoveHand;
        }

        private void MoveHand(Vector2 direction)
        {
            // Vector2 anchoredPosition = handTransform.anchoredPosition + direction / canvas.scaleFactor;
            // anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, 0, canvasRect.sizeDelta.x);
            // anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, 0, canvasRect.sizeDelta.y);
            
            handTransform.anchoredPosition = direction;
        }

        public void UntrackedCrossHair()
        {
            references.joystick.JoystickMoveUpdate -= MoveCrosshair;
        }
        public void TrackedCrossHair()
        {
            references.joystick.JoystickMoveUpdate += MoveCrosshair;
        }
        private void MoveCrosshair(Vector2 direction)
        {
            Vector2 anchoredPosition = transform.anchoredPosition + direction / canvas.scaleFactor;
            anchoredPosition.x = Mathf.Clamp(anchoredPosition.x, 0, canvasRect.sizeDelta.x);
            anchoredPosition.y = Mathf.Clamp(anchoredPosition.y, 0, canvasRect.sizeDelta.y);
            
            transform.anchoredPosition = anchoredPosition;
            PositionUpdated?.Invoke(GetWorldPosition());
        }

        private Vector3 GetWorldPosition()
        {
            Vector3 startPoint = transform.position;
            startPoint.z = -camera.transform.position.z;

            startPoint = camera.ScreenToWorldPoint(startPoint);
            
            return startPoint;
        }
    }
}