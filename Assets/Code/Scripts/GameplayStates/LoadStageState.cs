using Code.Scripts.App.Common;
using Code.Scripts.App.ScenesManagement;
using Code.Scripts.Configs;
using Code.Scripts.Services;
using Cysharp.Threading.Tasks;

namespace Code.Scripts.GameplayStates
{
    public class LoadStageState : State
    {
        private StageInfo _stageToLoad;

        public void SetStageToLoad(StageInfo stageInfo)
        {
            _stageToLoad = stageInfo;
        }

        public override async UniTask Enter()
        {
            await Preloader.Show();

            SetPlayerCharacterActive(false);
            
            await Mediator.Get<StageService>().LoadStage(_stageToLoad);
            
            SetPlayerCharacterActive(true);
            
            await Preloader.Hide();
        }
        
        private static void SetPlayerCharacterActive(bool active)
        {
            Mediator.Get<InteractionsService>().SetPlayerCharacterActive(active);
            if (Mediator.PlayerCharacter)
            {
                Mediator.PlayerCharacter.gameObject.SetActive(active);
            }
        }
    }
}