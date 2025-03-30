using System;
using Code.Scripts.App.Common;
using UnityEngine;

namespace Code.Scripts.Views.CommonViews
{
    public class SaveStatusView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        private bool _isSaving;
        
        private void LateUpdate()
        {
            if (Mediator.GameState.SaveInProgress != _isSaving)
            {
                _isSaving = !_isSaving;
                _canvasGroup.alpha = _isSaving ? 1 : 0;
            }
        }
    }
}