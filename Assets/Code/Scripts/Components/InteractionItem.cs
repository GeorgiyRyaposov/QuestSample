using Code.Scripts.Configs.InteractionItems;
using DG.Tweening;
using UnityEngine;

namespace Code.Scripts.Components
{
    public class InteractionItem : MonoBehaviour
    {
        public InteractionItemInfo Info;

        [SerializeField] private Transform _visualsRoot;
        
        private bool _isHighlighted;

        public void Highlight(bool on, bool animate = true)
        {
            _isHighlighted = on;

            if (!_isHighlighted && animate)
            {
                _visualsRoot.DOLocalMove(Vector3.zero, 1f)
                    .SetEase(Ease.InOutSine);
            }
        }

        private void Update()
        {
            if (_isHighlighted)
            {
                var pos = _visualsRoot.localPosition;
                pos.y = Mathf.Lerp(pos.y, 0.5f + Mathf.Sin(Time.time) * 0.2f, Time.deltaTime * 4f);
                _visualsRoot.localPosition = pos;
            }
        }

        public void Dispose()
        {
            Highlight(false, false);
            Destroy(gameObject);
        }
    }
}