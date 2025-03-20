using System.Collections.Generic;
using System.Linq;
using Code.Scripts.App.Common;
using Code.Scripts.Components;
using Code.Scripts.Configs.InteractionItems;
using Code.Scripts.Services.Common;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Scripts.Services
{
    public class InteractionsService : IService
    {
        private readonly HashSet<InteractionItem> _sceneItems = new();
        private InteractionItem _activeItem;
        
        public void RegisterSceneItem(InteractionItem interactionItem)
        {
            _sceneItems.Add(interactionItem);
        }
        
        public void SetActiveItem(InteractionItem item)
        {
            var prevItem = _activeItem;
            _activeItem = item;

            if (prevItem != null)
            {
                prevItem.Highlight(false);
            }

            if (_activeItem != null)
            {
                _activeItem.Highlight(true);
                Mediator.HintsView.Show("Press 'F' to interact");
            }
            else
            {
                Mediator.HintsView.Hide();
            }
        }

        public async UniTask Interact()
        {
            if (_activeItem == null)
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
            
            item.Highlight(false, false);
            _sceneItems.Remove(item);
            Object.Destroy(item.gameObject);
        }
    }
}