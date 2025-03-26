using System;
using Code.Scripts.Configs.Blackboards;

namespace Code.Editor.DialogsEditor.Nodes
{
    public interface IHasFlagRequirement
    {
        Nullable<BoolKeyValue> FlagRequirement { get; set; }
    }
}