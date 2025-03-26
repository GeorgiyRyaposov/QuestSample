using System;
using Code.Scripts.Configs.Blackboards;

namespace Code.Scripts.Configs.Dialogs
{
    [Serializable]
    public class FlagModifier
    {
        public string TargetId;
        public BoolKeyValue Flag;
    }
}