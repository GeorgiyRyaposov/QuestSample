using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Scripts.Configs.Blackboards
{
    [Serializable]
    public class BoolBlackboard : ISerializationCallbackReceiver
    {
        [SerializeField] private BoolKeyValue[] _keyValues;
        
        [NonSerialized] private Dictionary<string, bool> _map = new();

        public bool Get(string key)
        {
            _map.TryGetValue(key, out var value);
            return value;
        }

        public void Set(string key, bool value)
        {
            _map[key] = value;
        }

        public void OnBeforeSerialize()
        {
            _keyValues = _map.Select(x => new BoolKeyValue
            {
                Key = x.Key,
                Value = x.Value
            }).ToArray();
        }

        public void OnAfterDeserialize()
        {
            _map = new Dictionary<string, bool>(_keyValues.Length);
            for (var i = 0; i < _keyValues.Length; i++)
            {
                var kvp = _keyValues[i];
                _map[kvp.Key] = kvp.Value;
            }
        }
    }
}