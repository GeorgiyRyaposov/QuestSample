using UnityEngine;

namespace Code.Scripts.Persistence
{
    public class InputState
    {
        public Vector2 Move;
        public Vector2 Look;
        public float Zoom;
        public bool Sprint;
        public bool Interact;

        public bool IsCurrentDeviceMouse = true;
    }
}