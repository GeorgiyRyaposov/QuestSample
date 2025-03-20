using Cysharp.Threading.Tasks;

namespace Code.Scripts.App.AppState
{
    public interface IAppState
    {
        UniTask Enter();
        UniTask Exit();
    }
}