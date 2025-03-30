using System.Collections.Generic;
using System.Linq;
using Code.Scripts.App.Common;
using Code.Scripts.Services;
using Code.Scripts.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Views.CommonViews
{
    public class LoadMenuView : MonoBehaviour
    {
        public bool IsVisible => _canvasGroup.alpha > 0;
        
        [SerializeField] private SaveFileView _saveFilePrefab;
        [SerializeField] private Transform _saveFilesRoot;
        [SerializeField] private Button _closeButton;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _showDuration = 0.5f;

        private readonly List<SaveFileView> _saveFiles = new();
        private readonly  Stack<SaveFileView> _saveFilesPool = new();

        private void Awake()
        {
            Mediator.LoadMenuView = this;
        }

        private void OnDestroy()
        {
            Mediator.LoadMenuView = null;
        }

        public async UniTask Show()
        {
            await PrepareSaveFiles();
            await CanvasGroupUtil.Show(_canvasGroup, _showDuration);
        }
        
        public async UniTask Hide()
        {
            await CanvasGroupUtil.Hide(_canvasGroup, _showDuration);
        }

        private async UniTask PrepareSaveFiles()
        {
            ClearSaveFiles();
            
            var saveFiles = await Mediator.Get<GameStateService>().GetSaveFiles();
            saveFiles = saveFiles.OrderBy(x => x.SaveDate).ToArray();
            
            foreach (var saveFile in saveFiles)
            {
                var view = GetSaveFileView();
                view.Attach(saveFile);
                view.gameObject.SetActive(true);
                
                _saveFiles.Add(view);
            }
        }

        private SaveFileView GetSaveFileView()
        {
            if (_saveFilesPool.TryPop(out var saveFileView))
            {
                return saveFileView;
            }
            
            return Instantiate(_saveFilePrefab, _saveFilesRoot);
        }

        private void ClearSaveFiles()
        {
            foreach (var saveFile in _saveFiles)
            {
                saveFile.gameObject.SetActive(false);
                _saveFilesPool.Push(saveFile);
            }
            
            _saveFiles.Clear();
        }
    }
}