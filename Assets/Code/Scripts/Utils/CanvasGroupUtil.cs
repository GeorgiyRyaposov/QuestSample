using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Code.Scripts.Utils
{
    public static class CanvasGroupUtil
    {
        public static async UniTask Show(CanvasGroup canvasGroup, float duration, Ease ease = Ease.Flash)
        {
            canvasGroup.DOFade(1f, duration).SetEase(ease);
            await UniTask.WaitForSeconds(duration);
            
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        
        public static async UniTask Hide(CanvasGroup canvasGroup, float duration, Ease ease = Ease.Flash)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            
            canvasGroup.DOFade(0f, duration).SetEase(Ease.Flash);
            await UniTask.WaitForSeconds(duration);
            
            canvasGroup.alpha = 0;
        }
    }
}