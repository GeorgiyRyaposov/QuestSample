using System;
using System.Collections.Generic;
using Code.Scripts.Configs.Blackboards;

namespace Code.Scripts.Persistence
{
    [Serializable]
    public class SessionStateData
    {
        public List<string> InventoryItems = new();

        public string PreviousStageId;
        public string CurrentStageId;
        public List<string> CompletedDialogues = new ();
        
        public BoolBlackboard BoolBlackboard = new();
        public IntBlackboard IntBlackboard = new();
    }
}