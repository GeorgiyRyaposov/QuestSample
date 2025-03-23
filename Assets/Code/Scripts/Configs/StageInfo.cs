using Code.Scripts.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Code.Scripts.Configs
{
    [CreateAssetMenu(fileName = "StageInfo", menuName = "Configs/Stages/StageInfo")]
    public class StageInfo : ScriptableObject
    {
        [ScriptableObjectId]
        public string Id;
        public AssetReference Scene;
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(Id))
            {
                if (UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(this,
                        out var guid,
                        out long _))
                {
                    Id = guid;
                    UnityEditor.EditorUtility.SetDirty(this);
                }
            }
        }
#endif
    }
}