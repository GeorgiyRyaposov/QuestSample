using UnityEngine;

namespace Code.Scripts.Persistence
{
    public class InputState
    {
        public Vector2 Move;
        public Vector2 Look;
        public bool Jump;
        public bool Walk;

        public bool IsCurrentDeviceMouse;
    }
}