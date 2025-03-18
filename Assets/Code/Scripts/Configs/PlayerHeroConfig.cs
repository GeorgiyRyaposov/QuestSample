using UnityEngine;

namespace Code.Scripts.Configs
{
    [CreateAssetMenu(menuName = "Data/Configs/PlayerHeroConfig", fileName = "PlayerHeroConfig")]
    public class PlayerHeroConfig : ScriptableObject
    {
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;
    }
}