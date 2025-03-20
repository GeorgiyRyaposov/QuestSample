using System;
using Code.Scripts.App.Common;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Code.Scripts.Views.GameplayViews
{
    public class HintsView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _hintLabel;
        [SerializeField] private float _fadeDuration = 0.2f;

        private void Awake()
        {
            Mediator.HintsView = this;
        }

        private void OnDestroy()
        {
            Mediator.HintsView = null;
        }

        public void Show(string text)
        {
            _hintLabel.text = text;
            _canvasGroup.DOFade(1f, _fadeDuration)
                .SetEase(Ease.InFlash);
        }

        public void Hide()
        {
            DOTween.Kill(gameObject);
            _canvasGroup.DOFade(0f, _fadeDuration)
                .SetEase(Ease.OutFlash);
        }
    }
}