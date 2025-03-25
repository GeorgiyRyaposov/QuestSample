using System.Collections.Generic;
using System.Linq;
using Code.Scripts.App.Common;
using Code.Scripts.Components;
using Code.Scripts.Configs.InteractionItems;
using Code.Scripts.Services.Common;
using Cysharp.Threading.Tasks;

namespace Code.Scripts.Services
{
    public class InteractionsService : IService
    {
        private readonly HashSet<InteractionItem> _sceneItems = new();
        private InteractionItem _activeItem;

        public void SetupItemsState(InteractionItem[] items)
        {
            ClearRegisteredItems();
            
            var inventory = Mediator.SessionState.InventoryItems;
            foreach (var item in items)
            {
                if (inventory.Contains(item.Info.Id))
                {
                    item.Dispose();
                }
                else
                {
                    RegisterSceneItem(item);
                }
            }
        }
        
        private void RegisterSceneItem(InteractionItem interactionItem)
        {
            _sceneItems.Add(interactionItem);
        }
        
        private void ClearRegisteredItems()
        {
            _activeItem = null;
            _sceneItems.Clear();
        }
        
        public void SetActiveItem(InteractionItem item)
        {
            var prevItem = _activeItem;
            _activeItem = item;

            if (prevItem)
            {
                prevItem.Highlight(false);
            }

            if (_activeItem)
            {
                _activeItem.Highlight(true);
                
                var hint = _activeItem.Info.Action.GetHint(_activeItem.Info);
                Mediator.HintsView.Show(hint);
            }
            else
            {
                Mediator.HintsView.Hide();
            }
        }

        public async UniTask Interact()
        {
            if (!_activeItem)
            {
                return;
            }

            await _activeItem.Info.Action.Interact(_activeItem.Info);
        }

        public InteractionItem FindSceneItem(InteractionItemInfo itemInfo)
        {
            return _sceneItems.FirstOrDefault(x => x.Info == itemInfo);
        }

        public void Destroy(InteractionItem item)
        {
            if (_activeItem == item)
            {
                _activeItem = null;
            }
            
            _sceneItems.Remove(item);
            item.Dispose();
        }
        
        public void SetPlayerCharacterActive(bool active)
        {
            if (active)
            {
                Mediator.Get<InputService>().EnablePlayerInput();
            }
            else
            {
                Mediator.Get<InputService>().DisablePlayerInput();
            }
            
            if (Mediator.PlayerCharacter)
            {
                Mediator.PlayerCharacter.SetActive(active);
            }
        }
    }
}