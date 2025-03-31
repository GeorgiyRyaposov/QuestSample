using System.Collections.Generic;
using System.Linq;
using Code.Scripts.App.Common;
using Code.Scripts.Configs.Dialogs;
using Code.Scripts.Services;
using Code.Scripts.Utils;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using CharacterInfo = Code.Scripts.Configs.InteractionItems.CharacterInfo;

namespace Code.Scripts.Views.GameplayViews
{
    public class DialoguePanelView : MonoBehaviour
    {
        public bool IsDialogueTyping { get; private set; }
        
        [SerializeField] private TMP_Text _dialogue;
        [SerializeField] private CanvasGroup _canvasGroup;
        
        [SerializeField] private LocalizedString _speakerName;
        [SerializeField] private LocalizedString _dialogText;
        
        [Header("Animation")]
        [SerializeField] private float _showDuration = 0.5f;
        [SerializeField] private float _textAwait = 0.01f;

        [SerializeField] private Transform _optionsRoot;
        [SerializeField] private DialogueOptionView _optionPrefab;
        
        private readonly List<DialogueOptionView> _options = new();
        private readonly Stack<DialogueOptionView> _optionsPool = new();
        private DialogueContainer _activeDialogue;
        private bool _subscribed;
        private bool _skipDialogue;

        private void Awake()
        {
            Mediator.DialoguePanelView = this;
        }

        private void OnDestroy()
        {
            Mediator.DialoguePanelView = null;
        }

        public async UniTask Hide()
        {
            UnsubscribeOnLocalizationChange();
            await CanvasGroupUtil.Hide(_canvasGroup, _showDuration);
        }
        
        public void SkipDialog()
        {
            _skipDialogue = true;
        }

        public async UniTask ShowDialogue(DialogueContainer dialogues)
        {
            SubscribeOnLocalizationChange();
            
            _activeDialogue = dialogues;
            
            var startDialogue = _activeDialogue.Dialogues.FirstOrDefault(x => x.Guid == dialogues.StartDialogueGuid);
            if (startDialogue == null)
            {
                Debug.LogError($"Указан неверный стартовый диалог: {dialogues.name}", dialogues);
                return;
            }
            
            ClearOptions();
            _dialogue.text = string.Empty;

            await CanvasGroupUtil.Show(_canvasGroup, _showDuration);
            
            await Show(startDialogue);
        }

        private void UpdateText(string _ = null)
        {
            _dialogue.text = $"{GetLocalizedSpeakerName()} {_dialogText.GetLocalizedString()}";
        }

        private string GetLocalizedSpeakerName()
        {
            return $"{_speakerName.GetLocalizedString()}:";
        }

        private async UniTask Show(DialogueData dialogue)
        {
            ClearOptions();
            
            var speaker = GetSpeaker(dialogue.SpeakerId);
            _speakerName.TableEntryReference = speaker.LocalizedName.TableEntryReference;
            _dialogText.TableEntryReference = dialogue.LocalizedText.TableEntryReference;
            
            var speakerNameLength = GetLocalizedSpeakerName().Length;
            UpdateText();
            _dialogue.maxVisibleCharacters = speakerNameLength;

            IsDialogueTyping = true;
            for (int i = speakerNameLength; i < _dialogue.text.Length + 1; i++)
            {
                if (_skipDialogue)
                {
                    _dialogue.maxVisibleCharacters = _dialogue.text.Length + 1;
                    break;
                }
                
                _dialogue.maxVisibleCharacters++;
                await UniTask.WaitForSeconds(_textAwait);
            }
            IsDialogueTyping = false;

            _skipDialogue = false;
            
            AddOptions(dialogue);
        }

        private void AddOptions(DialogueData dialogue)
        {
            var options = _activeDialogue.Options
                .Where(x => x.BaseDialogueGuid == dialogue.Guid);
            foreach (var option in options)
            {
                AddOption(option);
            }

            if (_options.Count(x => x.IsInteractable) == 0)
            {
                var view = GetOption();
                view.Setup("Завершить диалог", _ => CompleteDialogue());
                view.gameObject.SetActive(true);
            }
        }

        private void AddOption(DialogueOptionData dialogueOptionData)
        {
            var view = GetOption();
            view.Setup(dialogueOptionData, OnSelected);
            view.gameObject.SetActive(true);

            var hasUncompletedRequirements = Mediator.Get<DialoguesService>()
                .HasUncompletedRequirements(dialogueOptionData, _activeDialogue);
            view.SetInteractable(!hasUncompletedRequirements);
        }

        private void OnSelected(DialogueOptionView view)
        {
            Mediator.Get<DialoguesService>().OnOptionSelected(view.DialogueOptionData, _activeDialogue);
            
            var targetId = view.DialogueOptionData.TargetDialogueGuid;
            if (string.IsNullOrEmpty(targetId))
            {
                CompleteDialogue();
                return;
            }
            
            var nextDialogue = _activeDialogue.Dialogues.FirstOrDefault(x => x.Guid == targetId);
            if (nextDialogue != null)
            {
                Show(nextDialogue).Forget();
            }
            else
            {
                CompleteDialogue();
            }
        }

        private void CompleteDialogue()
        {
            Mediator.Get<DialoguesService>().OnDialogCompleted(_activeDialogue);
        }

        private CharacterInfo GetSpeaker(string speakerId)
        {
            return Mediator.Get<AssetsService>().CharactersContainer
                .Characters.FirstOrDefault(x => x.Id == speakerId);
        }

        private DialogueOptionView GetOption()
        {
            var view = _optionsPool.Count > 0 
                ? _optionsPool.Pop() 
                : Instantiate(_optionPrefab, _optionsRoot);
            
            _options.Add(view);
            return view;
        }

        private void ClearOptions()
        {
            foreach (var option in _options)
            {
                _optionsPool.Push(option);
                option.gameObject.SetActive(false);
            }
            
            _options.Clear();
        }

        private void SubscribeOnLocalizationChange()
        {
            if (_subscribed)
            {
                return;
            }

            _subscribed = true;
            
            _dialogText.StringChanged += UpdateText;
        }

        private void UnsubscribeOnLocalizationChange()
        {
            if (!_subscribed)
            {
                return;
            }

            _subscribed = false;
            
            _dialogText.StringChanged -= UpdateText;
        }
    }
}