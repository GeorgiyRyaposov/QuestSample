using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Scripts.Configs.Blackboards
{
    [Serializable]
    public class IntBlackboard : ISerializationCallbackReceiver
    {
        [SerializeField] private IntKeyValue[] _keyValues;
        
        [NonSerialized] private Dictionary<string, int> _map = new();

        public int Get(string key)
        {
            _map.TryGetValue(key, out var value);
            return value;
        }

        public void Set(string key, int value)
        {
            _map[key] = value;
        }

        public void OnBeforeSerialize()
        {
            _keyValues = _map.Select(x => new IntKeyValue
            {
                Key = x.Key,
                Value = x.Value
            }).ToArray();
        }

        public void OnAfterDeserialize()
        {
            _map = new Dictionary<string, int>(_keyValues.Length);
            for (var i = 0; i < _keyValues.Length; i++)
            {
                var kvp = _keyValues[i];
                _map[kvp.Key] = kvp.Value;
            }
        }
    }
}