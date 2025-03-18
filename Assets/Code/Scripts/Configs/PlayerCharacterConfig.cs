using UnityEngine;

namespace Code.Scripts.Configs
{
    [CreateAssetMenu(menuName = "Data/Configs/PlayerCharacterConfig", fileName = "PlayerCharacterConfig")]
    public class PlayerCharacterConfig : ScriptableObject
    {
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;
        
        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;
        
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;
    }
}