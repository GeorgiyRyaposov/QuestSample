using System;
using UnityEngine;

namespace Code.Scripts.Utils
{
    public class ScriptableObjectIdAttribute : PropertyAttribute { }

#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(ScriptableObjectIdAttribute))]
    public class ScriptableObjectIdDrawer : UnityEditor.PropertyDrawer
    {
        private const string CHECKED_SCRIPTABLE_DUPLICATE_KEY = "CheckedScriptableDuplicate";
    
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label) 
        {
            GUI.enabled = false;
        
            if (string.IsNullOrEmpty(property.stringValue) || 
                (!IsDuplicateChecked(property) && HasDuplicateId(property)))
            {
                if (!UnityEditor.AssetDatabase.TryGetGUIDAndLocalFileIdentifier(property.serializedObject.targetObject, out var guid,
                        out long _))
                {
                    property.stringValue = guid;
                }
                else
                {
                    property.stringValue = Guid.NewGuid().ToString();
                }
            }
        
            UnityEditor.EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }

        //Check other instances ID, if any has same id
        //in case when scriptable was duplicated
        private bool HasDuplicateId(UnityEditor.SerializedProperty property)
        {
            var id = property.stringValue;

            var duplicates = 0;
        
            var type = property.serializedObject.targetObject.GetType();
            var otherObjects = Resources.FindObjectsOfTypeAll(type);
            foreach (var obj in otherObjects)
            {
                if (obj is not ScriptableObject)
                {
                    continue;
                }
            
                var serializedObj = new UnityEditor.SerializedObject(obj);
                var duplicateProperty = serializedObj.FindProperty(property.name);
                if (duplicateProperty == null)
                {
                    continue;
                }
            
                if (string.Equals(duplicateProperty.stringValue, id, StringComparison.Ordinal))
                {
                    duplicates++;
                
                    if (duplicates > 1)
                    {
                        return true;
                    }
                }
            }
        
            SetDuplicateAsChecked(property);

            return false;
        }

        private static bool IsDuplicateChecked(UnityEditor.SerializedProperty serializedProperty)
        {
            var checkedName = UnityEditor.EditorPrefs.GetString(CHECKED_SCRIPTABLE_DUPLICATE_KEY, string.Empty);
            return string.Equals(serializedProperty.serializedObject.targetObject.name, checkedName, StringComparison.Ordinal);
        }
    
        private static void SetDuplicateAsChecked(UnityEditor.SerializedProperty serializedProperty)
        {
            UnityEditor.EditorPrefs.SetString(CHECKED_SCRIPTABLE_DUPLICATE_KEY, serializedProperty.serializedObject.targetObject.name);
        }
    }
#endif
}