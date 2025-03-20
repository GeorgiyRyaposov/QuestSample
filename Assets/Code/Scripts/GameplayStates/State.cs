using Cysharp.Threading.Tasks;

namespace Code.Scripts.GameplayStates
{
    public class State
    {
        public virtual UniTask Enter()
        {
            return UniTask.CompletedTask;
        }
        
        public virtual UniTask Exit()
        {
            return UniTask.CompletedTask;
        }
        
        public virtual void OnUpdate() { }
    }
}