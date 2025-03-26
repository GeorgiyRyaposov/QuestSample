﻿using Code.Scripts.Configs.Dialogs;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Code.Scripts.Views.GameplayViews
{
    public class DialogueOptionView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Button _button;
        
        private UnityAction<DialogueOptionView> _callback;

        public DialogueOptionData DialogueOptionData { get; private set; }
        
        public void Start()
        {
            _button.onClick.AddListener(OnSelected);
        }
        
        public void Setup(DialogueOptionData dialogueOptionData, UnityAction<DialogueOptionView> onSelected)
        {
            DialogueOptionData = dialogueOptionData;
            
            _text.text = dialogueOptionData.Text;
            _callback = onSelected;
        }
        
        public void Setup(string text, UnityAction<DialogueOptionView> onSelected)
        {
            DialogueOptionData = null;
            
            _text.text = text;
            _callback = onSelected;
        }

        public void SetInteractable(bool interactable)
        {
            if (_button.interactable != interactable)
            {
                _button.interactable = interactable;
            }
        }

        private void OnSelected()
        {
            _callback.Invoke(this);
        }
    }
}