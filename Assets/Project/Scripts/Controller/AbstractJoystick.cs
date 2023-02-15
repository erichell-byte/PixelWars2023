using System;
using UnityEngine;

namespace Controller
{
    public abstract class AbstractJoystick : MonoBehaviour
    {
        public Action<Vector2> JoystickJustMove;
        public Action<Vector2> JoystickMoveUpdate;
        public Action JoystickActivated;
        public Action JoystickDisabled;
    }
}