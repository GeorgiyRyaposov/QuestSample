using System;
using System.Collections.Generic;

namespace Code.Scripts.Persistence
{
    [Serializable]
    public class SessionStateData
    {
        public List<string> InventoryItems = new();

        public string PreviousStageId;
        public string CurrentStageId;
    }
}