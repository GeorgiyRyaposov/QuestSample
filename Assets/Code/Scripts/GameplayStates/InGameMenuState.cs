using Code.Scripts.App.Common;
using Cysharp.Threading.Tasks;

namespace Code.Scripts.GameplayStates
{
    public class InGameMenuState : State
    {
        public override UniTask Enter()
        {
            Mediator.HintsView.Hide();
            return base.Enter();
        }
    }
}