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

        public void SetNextStage(StageInfo stageInfo)
        {
            _stageToLoad = stageInfo;
        }

        public override async UniTask Enter()
        {
            await Preloader.Show();

            await Mediator.Get<StageService>().LoadNextStage(_stageToLoad);
            
            await Preloader.Hide();
        }
    }
}