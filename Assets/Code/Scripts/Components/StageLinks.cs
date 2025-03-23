using Unity.Collections;
using UnityEngine;

namespace Code.Scripts.Components
{
    public class StageLinks : MonoBehaviour
    {
        [ReadOnly] public InteractionItem[] InteractionItems;
        [ReadOnly] public PlayerSpawnPoint[] PlayerSpawnPoints;
        

        public void CollectLinks()
        {
            InteractionItems = FindObjectsByType<InteractionItem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            PlayerSpawnPoints = FindObjectsByType<PlayerSpawnPoint>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        }
        
        private void OnValidate()
        {
            CollectLinks();
        }
    }
}