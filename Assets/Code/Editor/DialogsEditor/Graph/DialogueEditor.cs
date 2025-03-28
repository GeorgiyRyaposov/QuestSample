using Code.Scripts.Configs.Dialogs;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Code.Editor.DialogsEditor.Graph
{
    public class DialogueEditor : EditorWindow
    {
        private string _fileName = "NewDialogue";

        private DialogueGraphView _graphView;
        private DialogueContainer _dialogueContainer;
        private TextField _fileNameTextField;

        [MenuItem("Tools/Dialogue editor")]
        public static void CreateGraphViewWindow()
        {
            var window = GetWindow<DialogueEditor>();
            window.titleContent = new GUIContent("Dialogue editor");
        }

        private void ConstructGraphView()
        {
            _graphView = new DialogueGraphView(this)
            {
                name = "Dialogue editor",
            };
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);

            saveChangesMessage = "Есть несохраненные изменения в диалоге, хотите сохранить?";
        }

        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();
            
            _fileNameTextField = new TextField("Имя файла диалога:");
            _fileNameTextField.SetValueWithoutNotify(_fileName);
            _fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);

            _dialogueContainer = CreateInstance<DialogueContainer>();
            var objectField = new ObjectField
            {
                objectType = typeof(DialogueContainer)
            };
            objectField.SetValueWithoutNotify(_dialogueContainer);
            objectField.RegisterValueChangedCallback(evt =>
            {
                CheckUnsavedChanges();
                
                _dialogueContainer = evt.newValue as DialogueContainer;
                _fileNameTextField.value = evt.newValue?.name;

                Load();
            });
            toolbar.Add(objectField);
            
            toolbar.Add(new Button(SaveChanges) { text = "Сохранить" });
            
            toolbar.Add(_fileNameTextField);
            toolbar.Add(new Button(CreateNew) { text = "Создать новый" });
            
            rootVisualElement.Add(toolbar);
        }

        private void CreateNew()
        {
            CheckUnsavedChanges();
            
            _dialogueContainer = CreateInstance<DialogueContainer>();
            _graphView.ResetGraph();
            _graphView.AddPropertiesToBlackBoard(_dialogueContainer);
        }

        private void Save()
        {
            if (!string.IsNullOrEmpty(_fileName))
            {
                var serializer = new DialogueGraphSerializer(_graphView);
                serializer.SaveGraph(_fileName, _dialogueContainer);
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid File name", "Please Enter a valid filename", "OK");
            }
        }
        
        private void Load()
        {
            if (_dialogueContainer != null)
            {
                var serializer = new DialogueGraphSerializer(_graphView);
                serializer.LoadDialogues(_dialogueContainer);
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid File name", "Please Enter a valid filename", "OK");
            }
        }

        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
        }

        private void Update()
        {
            hasUnsavedChanges = HasUnsavedChanges();
        }
        
        private void CheckUnsavedChanges()
        {
            if (!HasUnsavedChanges())
            {
                return;
            }
            if (EditorUtility.DisplayDialog("Есть несохраненные изменения", saveChangesMessage, "Да", "Нет"))
            {
                SaveChanges();
            }
        }

        private bool HasUnsavedChanges()
        {
            var path = AssetDatabase.GetAssetPath(_dialogueContainer);
            if (string.IsNullOrEmpty(path) && !_graphView.HasChanges)
            {
                return false;
            }
            
            return EditorUtility.IsDirty(_dialogueContainer) || _graphView.HasChanges;
        }
        
        public override void SaveChanges()
        {
            Save();
            base.SaveChanges();
        }
    }
}