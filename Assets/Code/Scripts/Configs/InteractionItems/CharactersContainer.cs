using UnityEngine;

namespace Code.Scripts.Configs.InteractionItems
{
    [CreateAssetMenu(fileName = "CharactersContainer", menuName = "Configs/Interactions/CharactersContainer")]
    public class CharactersContainer : ScriptableObject
    {
        public CharacterInfo[] Characters = new CharacterInfo[0];
    }
}