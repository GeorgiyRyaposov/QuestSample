using Code.Scripts.Configs;
using Code.Scripts.Services.Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.Scripts.Services
{
    [CreateAssetMenu(menuName = "Data/Services/AssetsService", fileName = "AssetsService")]
    public class AssetsService : ScriptableService
    {
        [FormerlySerializedAs("PlayerHeroConfig")] public PlayerCharacterConfig playerCharacterConfig;
    }
}