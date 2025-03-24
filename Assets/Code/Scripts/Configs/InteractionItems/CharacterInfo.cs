using UnityEngine;

namespace Code.Scripts.Configs.InteractionItems
{
    [CreateAssetMenu(fileName = "CharacterInfo", menuName = "Configs/Interactions/CharacterInfo")]
    public class CharacterInfo : InteractionItemInfo
    {
        public string CharacterName;
    }
}