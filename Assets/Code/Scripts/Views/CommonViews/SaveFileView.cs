using Code.Scripts.App.AppState;
using Code.Scripts.App.Common;
using Code.Scripts.Persistence;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Scripts.Views.CommonViews
{
    public class SaveFileView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Button _button;
        
        private SaveMetadata _saveFile;

        private void Start()
        {
            _button.onClick.AddListener(LoadFile);
        }

        public void Attach(SaveMetadata saveFile)
        {
            _saveFile = saveFile;
            
            _description.text = $"{saveFile.SaveName}-" +
                                $"{saveFile.SaveDate.ToLocalTime().ToShortDateString()} " +
                                $"{saveFile.SaveDate.ToLocalTime().ToLongTimeString()}";
        }
        
        private void LoadFile()
        {
            _button.interactable = false;
            Mediator.Get<AppStateMachine>().LoadGameplay(_saveFile).Forget();
        }
    }
}