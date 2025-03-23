using System;
using System.Linq;
using Code.Scripts.App.Common;
using Code.Scripts.GameplayStates;
using Code.Scripts.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Scripts.Configs.InteractionItems
{
    [CreateAssetMenu(fileName = "GoToStage", menuName = "Configs/Interactions/GoToStage")]
    public class GoToStage : InteractionAction
    {
        public override string GetHint(InteractionItemInfo itemInfo)
        {
            //todo: replace hardcoded F 
            return "Press 'F' to move next";
        }

        public override UniTask Interact(InteractionItemInfo itemInfo)
        {
            var stageInfo = GetStageInfo(itemInfo);
            if (!stageInfo)
            {
                Debug.LogError($"Failed to find stage with id: {itemInfo.Id}");
                return UniTask.CompletedTask;
            }
            
            Mediator.GameplayStateMachine
                .GoToStage(stageInfo)
                .Forget();
            
            return UniTask.CompletedTask;
        }

        private static StageInfo GetStageInfo(InteractionItemInfo itemInfo)
        {
            if (itemInfo is not TransferItemInfo transferItemInfo)
            {
                Debug.LogError($"ItemInfo should be TransferItemInfo");
                return null;
            }
            
            return transferItemInfo.TargetStage;
        }
    }
}