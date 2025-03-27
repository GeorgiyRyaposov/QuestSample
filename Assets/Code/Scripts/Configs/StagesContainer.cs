using UnityEngine;

namespace Code.Scripts.Configs
{
    //[CreateAssetMenu(menuName = "Configs/Stages", fileName = "StagesContainer")]
    public class StagesContainer : ScriptableObject, IConfigsContainer
    {
        public StageInfo StartStage;

        public StageInfo[] Stages;
        public void UpdateItems(IAssetsFinder finder)
        {
            Stages = finder.GetAssets<StageInfo>();
        }
    }
}