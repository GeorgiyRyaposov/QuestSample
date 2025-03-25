using Code.Scripts.Configs;
using Code.Scripts.Configs.Dialogs;
using Code.Scripts.Configs.InteractionItems;
using Code.Scripts.Services.Common;

namespace Code.Scripts.Services
{
    //[CreateAssetMenu(menuName = "Data/Services/AssetsService", fileName = "AssetsService")]
    public class AssetsService : ScriptableService
    {
        public PlayerCharacterConfig PlayerCharacterConfig;
        public StagesContainer StagesContainer;
        public CharactersContainer CharactersContainer;
    }
}