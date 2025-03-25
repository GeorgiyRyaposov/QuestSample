using Code.Scripts.Configs.Dialogs;
using UnityEngine;

namespace Code.Scripts.Configs.InteractionItems
{
    [CreateAssetMenu(fileName = "DialoguesInfo", menuName = "Configs/Interactions/DialoguesInfo")]
    public class DialoguesInfo : InteractionItemInfo
    {
        public DialogueContainer[] Dialogues = new DialogueContainer[0];
    }
}