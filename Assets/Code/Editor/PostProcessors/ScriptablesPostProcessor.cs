using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Code.Editor.Utils;
using Code.Scripts.Configs;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Code.Editor.PostProcessors
{
    public class ScriptablesPostProcessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths, bool didDomainReload)
        {
            foreach (string path in importedAssets)
            {
                if (path.EndsWith(".asset", StringComparison.OrdinalIgnoreCase))
                {
                    OnScriptableObjectAssetCreated(path);
                }
            }
            
            foreach (string path in deletedAssets)
            {
                if (path.EndsWith(".asset", StringComparison.OrdinalIgnoreCase))
                {
                    OnScriptableObjectAssetRemoved(path);
                }
            }
        }

        private static void OnScriptableObjectAssetCreated(string path)
        {
            UpdateContainers(path);
        }
        
        private static void OnScriptableObjectAssetRemoved(string path)
        {
            UpdateContainers(path);
        }

        private static void UpdateContainers(string path)
        {
            if (!path.StartsWith("Assets/Configs", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var assembly = Assembly.Load(new AssemblyName("Assembly-CSharp"));
            var configsContainersTypes = FindDerivedTypes(assembly, typeof(IConfigsContainer));
            var assetsFinder = new AssetsFinder();
            foreach (var containerType in configsContainersTypes)
            {
                var assets = AssetDatabaseUtils.FindAssets(containerType);
                foreach (var asset in assets)
                {
                    if (asset is IConfigsContainer configsContainer)
                    {
                        configsContainer.UpdateItems(assetsFinder);
                        EditorUtility.SetDirty(asset);
                    }
                }
            }
        }
        
        private static IEnumerable<Type> FindDerivedTypes(Assembly assembly, Type baseType)
        {
            return assembly.GetTypes().Where(baseType.IsAssignableFrom);
        }
        
        private class AssetsFinder : IAssetsFinder
        {
            public T[] GetAssets<T>() where T : Object
            {
                var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
                var assets = new List<T>(guids.Length);

                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var asset = AssetDatabase.LoadAssetAtPath(path, typeof(T));
                    if (asset is T assetT)
                    {
                        assets.Add(assetT);
                    }
                }

                return assets.ToArray();
            }
        }
    }
}