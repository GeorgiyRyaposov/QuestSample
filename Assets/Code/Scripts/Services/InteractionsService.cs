using Code.Scripts.Components;
using Code.Scripts.Services.Common;

namespace Code.Scripts.Services
{
    public class InteractionsService : IService
    {
        private InteractionItem _activeItem;
        
        public void SetActiveItem(InteractionItem item)
        {
            _activeItem = item;
            //todo: add outline to item, remove prev item outline
        }
    }
}