using System;
using UnityEngine;

namespace Code.Scripts.Persistence
{
    [Serializable]
    public class SaveMetadata : ISerializationCallbackReceiver
    {
        public string SaveKey;
        public string SaveName;
        public string DataFileName;
        public DateTime SaveDate;
        
        [SerializeField]
        private string _serializedDateTime;
        
        public void OnBeforeSerialize()
        {
            _serializedDateTime = SaveDate.ToString("o");
        }

        public void OnAfterDeserialize()
        {
            SaveDate = DateTime.Parse(_serializedDateTime, null, System.Globalization.DateTimeStyles.RoundtripKind);
        }
    }
}