using Code.Scripts.App.Common;
using Code.Scripts.Services;
using UnityEngine;

namespace Code.Scripts.Components
{
    public class InteractionItemsDetector : MonoBehaviour
    {
        [SerializeField] private float _radius;
        [SerializeField] private float _detectPeriod = 0.3f;
        [SerializeField] private LayerMask _itemsLayers;

        private readonly Collider[] _collidersBuffer = new Collider[42];
        private float _elapsed;
        private int _prevHits;

        private void Update()
        {
            _elapsed += Time.deltaTime;

            if (_elapsed >= _detectPeriod)
            {
                _elapsed = 0;
                DetectItems();
            }
        }

        private void DetectItems()
        {
            var hits = Physics.OverlapSphereNonAlloc(transform.position, _radius, _collidersBuffer, _itemsLayers, QueryTriggerInteraction.Ignore);
            if (_prevHits == hits)
            {
                return;
            }
            
            _prevHits = hits;
            UpdateActiveItem(_collidersBuffer, hits);
        }

        private void UpdateActiveItem(Collider[] colliders, int count)
        {
            var item = GetInteractionItem(colliders, count);
            Mediator.Get<InteractionsService>().SetActiveItem(item);
        }

        private InteractionItem GetInteractionItem(Collider[] colliders, int count)
        {
            InteractionItem bestItem = null;
            var bestPriority = int.MinValue;

            for (int i = 0; i < count; i++)
            {
                if (!colliders[i].TryGetComponent<InteractionItem>(out var item))
                {
                    continue;
                }

                if (item.Info.Priority > bestPriority)
                {
                    bestPriority = item.Info.Priority;
                    bestItem = item;
                }
            }
            
            return bestItem;
        }

        private void OnDrawGizmosSelected()
        {
            var color = Color.yellow;
            color.a = 0.5f;
            Gizmos.color = color;
            
            Gizmos.DrawSphere(transform.position, _radius);
        }
    }
}