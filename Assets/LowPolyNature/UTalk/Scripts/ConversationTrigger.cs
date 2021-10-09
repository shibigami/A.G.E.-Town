using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UTalk
{
    public class ConversationTrigger : MonoBehaviour, IConversationProvider
    {
        public UnityEvent OnConversationStart;

        public UnityEvent OnConversationStop;

        public string ConversationName;

        private bool mIsActive = true;

        public string GetConversationName()
        {
            return ConversationName;
        }

        public void SetConversationName(string name)
        {
            ConversationName = name;
        }

        public void SetTriggerActive(bool active)
        {
            mIsActive = active;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && other is BoxCollider && mIsActive)
            {
                ConversationManager.Instance.StartConversation(ConversationName);

                OnConversationStart.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && other is BoxCollider)
            {
                ConversationManager.Instance.StopConversation();

                OnConversationStop.Invoke();
            }
        }
    }

}
