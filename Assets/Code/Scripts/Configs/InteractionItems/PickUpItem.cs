using Code.Scripts.App.Common;
using Code.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Scripts.Configs.InteractionItems
{
    [CreateAssetMenu(fileName = "PickUpItem", menuName = "Configs/Interactions/PickUpItem")]
    public class PickUpItem : InteractionAction
    {
        public override string GetHint(InteractionItemInfo itemInfo)
        {
            //todo: replace hardcoded F 
            return "Press 'F' to pick up item";
        }
        
        public override async UniTask Interact(InteractionItemInfo itemInfo)
        {
            Mediator.Get<InteractionsService>().SetPlayerCharacterActive(false);
            
            var sceneItem = InteractionsService.FindSceneItem(itemInfo);
            if (sceneItem)
            {
                await Mediator.PlayerCharacter.PlayPickUpAnimation(sceneItem);
                InteractionsService.Destroy(sceneItem);
            }
            
            Mediator.SessionState.InventoryItems.Add(itemInfo.Id);
            
            Mediator.Get<InteractionsService>().SetPlayerCharacterActive(true);
        }
    }
}