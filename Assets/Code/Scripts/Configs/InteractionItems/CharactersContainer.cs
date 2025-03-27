using UnityEngine;

namespace Code.Scripts.Configs.InteractionItems
{
    [CreateAssetMenu(fileName = "CharactersContainer", menuName = "Configs/Interactions/CharactersContainer")]
    public class CharactersContainer : ScriptableObject, IConfigsContainer
    {
        public CharacterInfo[] Characters = new CharacterInfo[0];
        
        public void UpdateItems(IAssetsFinder finder)
        {
            Characters = finder.GetAssets<CharacterInfo>();
        }
    }
}