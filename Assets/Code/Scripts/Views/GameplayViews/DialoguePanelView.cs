using System.Collections.Generic;
using System.Linq;
using Code.Scripts.App.Common;
using Code.Scripts.Configs.Dialogs;
using Code.Scripts.Services;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using CharacterInfo = Code.Scripts.Configs.InteractionItems.CharacterInfo;

namespace Code.Scripts.Views.GameplayViews
{
    public class DialoguePanelView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _dialogue;
        [SerializeField] private CanvasGroup _canvasGroup;
        
        [Header("Animation")]
        [SerializeField] private float _showDuration = 0.5f;
        [SerializeField] private float _textAwait = 0.01f;

        [SerializeField] private Transform _optionsRoot;
        [SerializeField] private DialogueOptionView _optionPrefab;
        
        private readonly List<DialogueOptionView> _options = new();
        private readonly Stack<DialogueOptionView> _optionsPool = new();
        private DialogueContainer _activeDialogue;

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
            _canvasGroup.DOFade(0f, _showDuration).SetEase(Ease.Flash);
            await UniTask.WaitForSeconds(_showDuration);
            
            _canvasGroup.alpha = 0;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        public async UniTask ShowDialogue(DialogueContainer dialogues)
        {
            _activeDialogue = dialogues;
            
            var startDialogue = _activeDialogue.Dialogues.FirstOrDefault(x => x.Guid == dialogues.StartDialogueGuid);
            if (startDialogue == null)
            {
                Debug.LogError($"Указан неверный стартовый диалог: {dialogues.name}", dialogues);
                return;
            }

            _canvasGroup.DOFade(1f, _showDuration).SetEase(Ease.Flash);
            await UniTask.WaitForSeconds(_showDuration);
            
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            
            await Show(startDialogue);
        }

        private async UniTask Show(DialogueData dialogue)
        {
            ClearOptions();
            
            var speaker = GetSpeaker(dialogue.SpeakerId);
            var characterName = $"{speaker.CharacterName}:";
            _dialogue.text = $"{characterName} {dialogue.Text}";
            _dialogue.maxVisibleCharacters = characterName.Length;
            
            for (int i = characterName.Length; i < _dialogue.text.Length + 1; i++)
            {
                _dialogue.maxVisibleCharacters++;
                await UniTask.WaitForSeconds(_textAwait);
            }

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
        }

        private void AddOption(DialogueOptionData dialogueOptionData)
        {
            var view = GetOption();
            view.Setup(dialogueOptionData, OnSelected);
            view.gameObject.SetActive(true);
        }

        private void OnSelected(DialogueOptionView view)
        {
            var targetId = view.DialogueOptionData.TargetDialogueGuid;
            var nextDialogue = _activeDialogue.Dialogues.FirstOrDefault(x => x.Guid == targetId);
            if (nextDialogue != null)
            {
                Show(nextDialogue).Forget();
            }
            else
            {
                Mediator.Get<DialoguesService>().OnDialogCompleted(_activeDialogue);
            }
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
    }
}