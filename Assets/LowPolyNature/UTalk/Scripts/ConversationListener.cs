using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UTalk
{
    public class ConversationListener : MonoBehaviour, IConversationProvider
    {
        public UnityEvent OnConversationStart;

        public UnityEvent OnConversationStop;

        public string ConversationName;

        // Use this for initialization
        void Start()
        {
            ConversationManager.Instance.ConversationStart += Instance_ConversationStart;
            ConversationManager.Instance.ConversationEnd += Instance_ConversationEnd;
        }

        private void Instance_ConversationStart(object sender, ConversationEventArgs e)
        {
            if (e.Conversation.Name == ConversationName)
            {
                OnConversationStart.Invoke();
            }
        }

        private void Instance_ConversationEnd(object sender, ConversationEventArgs e)
        {
            if (e.Conversation != null && e.Conversation.Name == ConversationName)
            {
                OnConversationStop.Invoke();
            }
        }

        public string GetConversationName()
        {
            return ConversationName;
        }

        public void SetConversationName(string name)
        {
            ConversationName = name;
        }
    }
}
