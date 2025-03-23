using UnityEngine;

namespace Code.Scripts.Configs.InteractionItems
{
    [CreateAssetMenu(fileName = "TransferItemInfo", menuName = "Configs/Interactions/TransferItemInfo")]
    public class TransferItemInfo : InteractionItemInfo
    {
        public StageInfo TargetStage;
    }
}