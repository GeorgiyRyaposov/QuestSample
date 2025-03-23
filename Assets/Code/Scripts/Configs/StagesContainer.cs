using UnityEngine;

namespace Code.Scripts.Configs
{
    //[CreateAssetMenu(menuName = "Configs/Stages", fileName = "StagesContainer")]
    public class StagesContainer : ScriptableObject
    {
        public StageInfo StartStage;

        public StageInfo[] Stages;
    }
}