using System;
using Code.Scripts.Configs.Blackboards;

namespace Code.Scripts.Configs.Dialogs
{
    [Serializable]
    public class FlagRequirement
    {
        public string TargetId;
        public BoolKeyValue Requirement;
    }
}