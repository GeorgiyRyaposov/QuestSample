using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scripts.App.ScenesManagement
{
    public class Preloader
    {
        public static bool IsVisible { get; private set; }
        public static float Alpha { get; private set; }
        
        private static bool _preloaderLoaded;
        
        public static async UniTask Show(bool immediately = false)
        {
            if (IsVisible)
            {
                return;
            }
            
            await LoadPreloader();
            
            IsVisible = true;
            
            if (immediately)
            {
                Alpha = 1;
            }            
            else
            {
                await SetAlpha(0, 1);
            }

            await UniTask.WaitForSeconds(1f);
        }

        public static async UniTask Hide(bool immediately = false)
        {
            if (!IsVisible)
            {
                return;
            }
            
            if (immediately)
            {
                Alpha = 0;
            }
            else
            {
                await UniTask.WaitForSeconds(1f);
                await SetAlpha(1, 0);
            }
            
            IsVisible = false;
            
            await UnloadPreloader();
        }

        private static async UniTask LoadPreloader()
        {
            if (!_preloaderLoaded)
            {
                var op = SceneManager.LoadSceneAsync(ScenesConsts.Preloader, LoadSceneMode.Additive);
                await op.ToUniTask();

                if (op != null)
                {
                    op.allowSceneActivation = true;
                }
            }

            _preloaderLoaded = true;
        }
        
        private static async UniTask UnloadPreloader()
        {
            if (!_preloaderLoaded)
            {
                return;
            }
            _preloaderLoaded = false;
            
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (string.Equals(ScenesConsts.Preloader, scene.name))
                {
                    if (scene.IsValid())
                    {
                        var op = SceneManager.UnloadSceneAsync(scene);
                        await op.ToUniTask();
                    }
                }
            }
        }

        private static async UniTask SetAlpha(float from, float to)
        {
            for (var t = 0f; t < 1f; t += Time.deltaTime * 4)
            {
                Alpha = Mathf.Lerp(from, to, t);
                await UniTask.WaitForEndOfFrame();
            }

            Alpha = to;
        }
    }
}