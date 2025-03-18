using System.Linq;
using UnityEngine.SceneManagement;

namespace Code.Scripts.App.ScenesManagement
{
    public class ScenesConsts
    {
        public const string EmptyScene = "Empty";
        public const string Preloader = "Preloader";
        public const string MainMenu = "MainMenu";
        public const string EntryPoint = "EntryPoint";
        public const string Gameplay = "Gameplay";
        public const string GameplayGui = "GameplayGui";

        public static string GetCurrentScene()
        {
            var scenes = new []
            {
                EmptyScene,
                MainMenu,
                Gameplay,
                GameplayGui,
            };
            
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scenes.Contains(scene.name))
                {
                    return scene.name;
                }
            }

            return string.Empty;
        }

        public static bool IsSceneLoaded(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (string.Equals(sceneName, scene.name))
                {
                    return true;
                }
            }

            return false;
        }
    }
}