using System;
using System.Collections.Generic;
using Code.Scripts.Configs.Blackboards;
using Code.Scripts.Utils;
using UnityEngine;

namespace Code.Scripts.Persistence
{
    [Serializable]
    public class SessionStateData
    {
        public string SessionId = ShortGuid.Generate();
        
        public List<string> InventoryItems = new();

        public string PreviousStageId;
        public string CurrentStageId;
        public List<string> CompletedDialogues = new ();
        
        public BoolBlackboard BoolBlackboard = new();
        
        public Vector3 LastGroundedPlayerPosition;
        public Vector3 PlayerPosition;
    }
}