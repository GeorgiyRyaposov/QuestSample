using Code.Scripts.App.ScenesManagement;
using UnityEngine;

namespace Code.Scripts.Views
{
    public class PreloaderView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _rootCanvasGroup;
        [SerializeField] private RectTransform _preloaderIcon;
        
        [Space]
        [SerializeField] private float _rotationSpeed = 3;
        [SerializeField] private float _fadeSpeed = 5f;

        private void Update()
        {
            _preloaderIcon.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime);

            var from = Preloader.IsVisible ? 0f : 1f;
            var to = Preloader.IsVisible ? 1f : 0f;
            var t = Preloader.Immediately 
                ? 1f 
                : _fadeSpeed * Time.deltaTime;
            _rootCanvasGroup.alpha =  Mathf.Lerp(from, to, t);

            if (!Preloader.IsVisible && Mathf.Approximately(t, 1f))
            {
                Destroy(gameObject);
            }
        }
    }
}