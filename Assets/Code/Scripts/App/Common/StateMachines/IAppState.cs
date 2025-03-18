using System.Threading.Tasks;

namespace Code.Scripts.App.Common.StateMachines
{
    public interface IAppState
    {
        Task Enter();
        Task Exit();
    }
}