﻿using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Scripts.App.AppState
{
    public class AppState : ScriptableObject, IAppState
    {
        public virtual UniTask Enter()
        {
            return UniTask.CompletedTask;
        }

        public virtual UniTask Exit()
        {
            return UniTask.CompletedTask;
        }
    }
}