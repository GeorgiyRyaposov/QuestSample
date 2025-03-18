using Code.Scripts.Configs;
using Code.Scripts.Services.Common;
using UnityEngine;

namespace Code.Scripts.Services
{
    [CreateAssetMenu(menuName = "Data/Services/AssetsService", fileName = "AssetsService")]
    public class AssetsService : ScriptableService
    {
        public PlayerHeroConfig PlayerHeroConfig;
    }
}