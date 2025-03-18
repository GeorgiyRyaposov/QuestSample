using System.Threading.Tasks;
using Code.Scripts.App.Common.StateMachines;
using UnityEngine;

namespace Code.Scripts.App.AppState
{
    public class AppState : ScriptableObject, IAppState
    {
        public virtual Task Enter()
        {
            return Task.CompletedTask;
        }

        public virtual Task Exit()
        {
            return Task.CompletedTask;
        }
    }
}