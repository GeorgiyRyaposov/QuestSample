using UnityEngine.SceneManagement;

namespace Code.Scripts.App.ScenesManagement
{
    public class Preloader
    {
        public static bool IsVisible { get; private set; }
        public static bool Immediately { get; private set; }
        
        private static bool _preloaderLoaded;
        
        public static void Show(bool immediately = false)
        {
            LoadPreloader();
            
            IsVisible = true;
            Immediately = immediately;
        }

        public static void Hide(bool immediately = false)
        {
            UnloadPreloader();
            
            IsVisible = false;
            Immediately = immediately;
        }

        private static void LoadPreloader()
        {
            if (!_preloaderLoaded)
            {
                SceneManager.LoadScene(ScenesConsts.Preloader, LoadSceneMode.Additive);
            }

            _preloaderLoaded = true;
        }
        
        private static void UnloadPreloader()
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
                        SceneManager.UnloadSceneAsync(scene);
                    }
                }
            }
        }
    }
}