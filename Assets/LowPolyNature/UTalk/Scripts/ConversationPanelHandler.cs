using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UTalk
{
    public class ConversationPanelHandler : MonoBehaviour {

        public void ContinueConversation()
        {
            ConversationManager.Instance.ConversationStep();
        }

        public void StopConversation()
        {
            ConversationManager.Instance.StopConversation();
        }
    }
}