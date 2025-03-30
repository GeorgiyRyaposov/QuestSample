using Code.Scripts.App.Common;
using Code.Scripts.Persistence;
using Cysharp.Threading.Tasks;

namespace Code.Scripts.GameplayStates
{
    public class PauseGameListener
    {
        private InputState InputState => Mediator.InputState;
        
        public void OnUpdate()
        {
            if (InputState.PauseGame)
            {
                InputState.PauseGame = false;

                if (Mediator.LoadMenuView.IsVisible)
                {
                    Mediator.LoadMenuView.Hide().Forget();
                }
                else if (Mediator.InGameMenuView.IsVisible)
                {
                    Mediator.InGameMenuView.Hide().Forget();
                }
                else
                {
                    Mediator.InGameMenuView.Show().Forget();
                }
            }
        }
    }
}