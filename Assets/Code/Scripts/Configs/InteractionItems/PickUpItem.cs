using Code.Scripts.App.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Scripts.Configs.InteractionItems
{
    [CreateAssetMenu(fileName = "PickUpItem", menuName = "Configs/Interactions/PickUpItem")]
    public class PickUpItem : InteractionAction
    {
        public override async UniTask Interact(InteractionItemInfo itemInfo)
        {
            var sceneItem = InteractionsService.FindSceneItem(itemInfo);
            if (sceneItem != null)
            {
                await Mediator.PlayerCharacter.PlayPickUpAnimation(sceneItem);
                InteractionsService.Destroy(sceneItem);
            }
            
            Mediator.SessionState.InventoryItems.Add(itemInfo.Id);
        }
    }
}