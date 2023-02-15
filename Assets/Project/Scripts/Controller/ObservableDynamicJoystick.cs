using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;
using Vector2 = UnityEngine.Vector2;

namespace Controller
{
    public class ObservableDynamicJoystick : AbstractJoystick, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            JoystickActivated?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            JoystickDisabled?.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            JoystickMoveUpdate(eventData.delta);
        }

        // public void OnPointerMove(PointerEventData eventData)
        // {
        //     JoystickJustMove(eventData.position);
        // }
    }
}