using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UTalk.Data;

namespace UTalk
{
    public class ConversationEventArgs : EventArgs
    {
        private Conversation mConversation;

        public ConversationEventArgs(Conversation conversation) : base()
        {
            mConversation = conversation;
        }

        public Conversation Conversation
        {
            get { return mConversation; }
        }
    }

    public class ConversationItemEventArgs : EventArgs
    {
        private ConversationItem mConversationItem;

        public ConversationItemEventArgs(ConversationItem conversationItem) : base()
        {
            mConversationItem = conversationItem;
        }

        public ConversationItem ConversationItem
        {
            get { return mConversationItem; }
        }
    }
}
