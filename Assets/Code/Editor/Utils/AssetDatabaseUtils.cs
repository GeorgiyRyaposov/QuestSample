using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Code.Editor.Utils
{
    public static class AssetDatabaseUtils
    {
        public static List<T> FindAssets<T>() where T : Object
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

            return assets;
        }
        
        public static T FindAsset<T>(string fileName) where T : Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name} {fileName}");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath(path, typeof(T));
                if (asset is T assetT)
                {
                    return assetT;
                }
            }

            return null;
        }

        public static List<T> FindAssets<T>(string[] searchInFolders, bool includeSubfolders = true) where T : Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", searchInFolders);
            var assets = new List<T>(guids.Length);

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (!includeSubfolders)
                {
                    var directoryName = Path.GetDirectoryName(path).Replace(@"\", @"/");
                    if (!searchInFolders.Any(x => string.Equals(x, directoryName, StringComparison.Ordinal)))
                    {
                        continue;
                    }
                }

                var asset = AssetDatabase.LoadAssetAtPath(path, typeof(T));
                if (asset is T assetT)
                {
                    assets.Add(assetT);
                }
            }

            return assets;
        }

        public static List<string> FindAssetsPaths(string assetType, string[] searchInFolders, bool includeSubfolders = true)
        {
            var guids = AssetDatabase.FindAssets($"t:{assetType}", searchInFolders);
            var paths = new List<string>(guids.Length);

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (!includeSubfolders)
                {
                    var directoryName = Path.GetDirectoryName(path).Replace(@"\", @"/");
                    if (!searchInFolders.Any(x => string.Equals(x, directoryName, StringComparison.Ordinal)))
                    {
                        continue;
                    }
                }

                paths.Add(path);
            }

            return paths;
        }

        public static T FindAsset<T>() where T : Object
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath(path, typeof(T));
                if (asset is T assetT)
                {
                    return assetT;
                }
            }

            return null;
        }

        public static T FindPrefab<T>(string prefabName) where T : Object
        {
            var guids = AssetDatabase.FindAssets(prefabName);

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath(path, typeof(T));
                if (asset is T assetT)
                {
                    return assetT;
                }
            }

            return null;
        }


        public static T CreateInstance<T>() where T : ScriptableObject
        {
            return ScriptableObject.CreateInstance<T>();
        }

        public static void DeleteAsset(Object asset)
        {
            var path = AssetDatabase.GetAssetPath(asset);
            AssetDatabase.DeleteAsset(path);
        }

        public static string GetAssetFolder(Object asset)
        {
            var path = AssetDatabase.GetAssetPath(asset);
            var rootFolder = Directory.GetParent(path);
            if (rootFolder == null)
            {
                Debug.LogError($"<color=red>Failed to get folder root '{path}'</color>");
                return string.Empty;
            }

            var projectFolder = GetProjectFolderDirectoryInfo();
            return rootFolder.FullName.Substring(projectFolder.FullName.Length + 1);
        }

        /// <summary>
        /// Ensures path is valid and exists.
        /// </summary>
        public static void EnsurePath(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                Debug.LogError("Folder path is null or empty.");
                return;
            }

            string[] folders = folderPath.Split('/');
            if (folders.Length == 0 || folders[0] != "Assets")
            {
                Debug.LogError("Folder path must start with 'Assets'.");
                return;
            }

            string currentPath = "Assets";

            for (int i = 1; i < folders.Length; i++)
            {
                string folderName = folders[i];
                string newFolderPath = currentPath + "/" + folderName;

                if (!AssetDatabase.IsValidFolder(newFolderPath))
                {
                    string newFolderGuid = AssetDatabase.CreateFolder(currentPath, folderName);
                    if (string.IsNullOrEmpty(newFolderGuid))
                    {
                        Debug.LogError($"Failed to create folder: {newFolderPath}");
                        return;
                    }
                }

                currentPath = newFolderPath;
            }
        }

        public static void CreateAsset(Object asset, string assetPath)
        {
            var projectFolder = GetProjectFolderDirectoryInfo();

            var fullPath = Path.Combine(projectFolder.FullName, assetPath);
            fullPath = Path.GetDirectoryName(fullPath);
            if (string.IsNullOrEmpty(fullPath))
            {
                Debug.LogError($"<color=red>Failed to find path for '{asset}'</color>");
                return;
            }

            Directory.CreateDirectory(fullPath);
            AssetDatabase.CreateAsset(asset, assetPath);
        }

        public static AddressableAssetEntry AddAssetToAddressables(AddressableAssetGroup addressableAssetGroup, ScriptableObject asset)
        {
            var assetPath = AssetDatabase.GetAssetPath(asset);
            var assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
            return AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry(assetGuid, addressableAssetGroup);
        }

        public static DirectoryInfo GetProjectFolderDirectoryInfo()
        {
            return Directory.GetParent(Application.dataPath);
        }

        public static int GetNextFileIndex(string assetsRoot)
        {
            var maxNumber = 0;

            var guids = AssetDatabase.FindAssets("t:ScriptableObject", new[] { assetsRoot });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var fileName = Path.GetFileNameWithoutExtension(path);
                var index = fileName.LastIndexOf("_", StringComparison.Ordinal);
                if (index == -1)
                {
                    continue;
                }

                var fileNumber = fileName.Substring(index + 1);
                if (int.TryParse(fileNumber, out var result) && result > maxNumber)
                {
                    maxNumber = result;
                }
            }

            return maxNumber + 1;
        }
    }
}
