using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UTalk.Data;

namespace UTalk
{
    [CreateAssetMenu(fileName = "UTalk_DB", menuName = "UTalk/Database", order = 1)]
    public class ConversationDatabase : ScriptableObject
    {
        public List<Actor> Actors;

        public List<Conversation> Conversations;

        public float ContinueConversationTime = 4.0f;

        public Conversation GetConversation(string name)
        {
            if (Conversations == null)
                return null;

            return Conversations.Find((c => c.Name == name));
        }

        public Actor GetActor(string name)
        {
            if (Actors == null)
                return null;

            return Actors.Find((c => c.Name == name));
        }

        public Actor GetActor(int id)
        {
            if (Actors == null)
                return null;

            return Actors.Find((c => c.Id == id));
        }

        public Actor GetFirstActor()
        {
            if (Actors == null || Actors.Count == 0)
                return null;

            return Actors[0];
        }
    }


}
