using Code.Scripts.Configs.Dialogs;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Code.Editor.DialogsEditor.Graph
{
    public class DialogueEditor : EditorWindow
    {
        private string _fileName = "New Dialogue";

        private DialogueGraphView _graphView;
        private DialogueContainer _dialogueContainer;

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
            
            var fileNameTextField = new TextField("Имя файла выбранного диалога:");
            fileNameTextField.SetValueWithoutNotify(_fileName);
            fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
            toolbar.Add(fileNameTextField);

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
                fileNameTextField.value = evt.newValue.name;

                Load();
            });
            toolbar.Add(objectField);
            
            toolbar.Add(new Button(SaveChanges) { text = "Сохранить" });
            
            rootVisualElement.Add(toolbar);
        }

        private void CheckUnsavedChanges()
        {
            if (HasUnsavedChanges())
            {
                if (EditorUtility.DisplayDialog("Есть несохраненные изменения", saveChangesMessage, "Да", "Нет"))
                {
                    SaveChanges();
                }
            }
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