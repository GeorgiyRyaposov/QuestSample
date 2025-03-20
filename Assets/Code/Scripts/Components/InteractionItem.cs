using System;
using Code.Scripts.App.Common;
using Code.Scripts.Configs.InteractionItems;
using Code.Scripts.Services;
using UnityEngine;

namespace Code.Scripts.Components
{
    public class InteractionItem : MonoBehaviour
    {
        public InteractionItemInfo Info;

        public void Start()
        {
            Mediator.Get<InteractionsService>().RegisterSceneItem(this);
        }

        public void Highlight(bool on)
        {
            
        }
    }
}