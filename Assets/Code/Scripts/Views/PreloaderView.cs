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

        private void Update()
        {
            _preloaderIcon.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime * Mathf.Sin(Time.time));

            _rootCanvasGroup.alpha =  Preloader.Alpha;

            if (!Preloader.IsVisible)
            {
                Destroy(gameObject);
            }
        }
    }
}