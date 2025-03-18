using Code.Scripts.App.ScenesManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.Scripts.App.Init
{
    public static class AppBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitializeBeforeSceneLoad()
        {
            SceneManager.LoadScene(ScenesConsts.EntryPoint, LoadSceneMode.Additive);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void InitializeAfterSceneLoad()
        {
            
        }
    }
}