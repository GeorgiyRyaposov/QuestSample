using System;
using System.IO;
using Code.Scripts.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.Scripts.Persistence
{
    public static class SaveManager
    {
        private static readonly string SavesFolder = Path.Combine(Application.persistentDataPath, "Saves");

        static SaveManager()
        {
            Directory.CreateDirectory(SavesFolder);
        }

        public static async UniTask SaveAsync(SessionStateData data, string saveName)
        {
            var saveKey = ShortGuid.Generate();
            var metadata = new SaveMetadata
            {
                SaveKey = saveKey,
                SaveDate = DateTime.UtcNow,
                SaveName = saveName,
                DataFileName = $"{saveKey}.data"
            };

            var dataSave = SaveDataAsync(data, metadata.DataFileName);
            var metaSave = SaveMetadataAsync(metadata);
        
            await UniTask.WhenAll(dataSave, metaSave);
        }

        private static async UniTask SaveDataAsync(SessionStateData data, string fileName)
        {
            var path = Path.Combine(SavesFolder, fileName);
            var json = JsonUtility.ToJson(data);

            await using var writer = new StreamWriter(path);
            await writer.WriteAsync(json);

            Debug.Log($"Saved data to {path}");
        }

        private static async UniTask SaveMetadataAsync(SaveMetadata metadata)
        {
            var path = Path.Combine(SavesFolder, $"{metadata.SaveKey}.meta");
            var json = JsonUtility.ToJson(metadata);

            await using var writer = new StreamWriter(path);
            await writer.WriteAsync(json);
        }
        
        public static async UniTask<(SessionStateData data, SaveMetadata metadata)> LoadAsync(string metadataPath)
        {
            var metadata = await LoadMetadataAsync(metadataPath);
            var data = await LoadDataAsync(metadata.DataFileName);
            return (data, metadata);
        }
        
        public static async UniTask<SaveMetadata> LoadMetadataAsync(string path)
        {
            using var reader = new StreamReader(path);
            var json = await reader.ReadToEndAsync();
            return JsonUtility.FromJson<SaveMetadata>(json);
        }

        public static async UniTask<SessionStateData> LoadDataAsync(string fileName)
        {
            var path = Path.Combine(SavesFolder, fileName);
            Debug.Log($"Loading data from {path}");

            using var reader = new StreamReader(path);
            var json = await reader.ReadToEndAsync();
            return JsonUtility.FromJson<SessionStateData>(json);
        }

        public static string[] GetAllSavePaths()
        {
            return Directory.GetFiles(SavesFolder, "*.meta");
        }
    }
}