using System;
using Code.Scripts.Configs.Blackboards;

namespace Code.Scripts.Configs.Dialogs
{
    [Serializable]
    public class CountRequirement
    {
        public string TargetId;
        public IntKeyValue Requirement;
    }
}