using System;
using System.Collections.Generic;
using UnityEngine;

namespace UTalk.Data
{
    [Serializable]
    public class Conversation : ISerializationCallbackReceiver
    {
        public string Name;

        [HideInInspector]
        public List<ConversationItem> Items;

        [HideInInspector]
        public List<Edge> Edges;

        public Conversation()
        {
            if (Items == null)
                Items = new List<ConversationItem>();

            if (Edges == null)
                Edges = new List<Edge>();

            AddStartItem();
        }

        public ConversationItem GetConversationItem(string conversationId)
        {
            if (Items == null || Items.Count == 0)
                return null;

            return Items.Find(c => c.ID == conversationId);
        }

        public bool HasConnectedItem(ConversationItem item)
        {
            return GetFirstConnectedItem(item) != null;

        }

        public ConversationItem GetFirstConnectedItem(ConversationItem item)
        {
            ConversationItem result = null;

            foreach (var edge in Edges)
            {
                if (edge.FromId == item.ID)
                {
                    result = GetConversationItem(edge.ToId);
                    break;
                }
            }
            return result;
        }


        public List<ConversationItem> GetConnectedItems(ConversationItem item)
        {
            List<ConversationItem> result = new List<ConversationItem>();

            foreach(var edge in Edges)
            {
                if(edge.FromId == item.ID)
                {
                    result.Add(GetConversationItem(edge.ToId));
                }
            }

            return result;
        }

        public void RemoveItem(ConversationItem item)
        {
            Edges.RemoveAll(edge => edge.ToId == item.ID);

            if (Items.Contains(item))
            {
                Items.Remove(item);
            }
        }

        public void OnAfterDeserialize()
        {
            // Add a new Start node if it doesnt exist
            AddStartItem();
            

            // Go through the edges, get the conversation item objects by the id
            // stored in the edge and add the conversation item objects to the edges
            if (Edges != null)
            {
                foreach (var edge in Edges)
                {
                    edge.InitByIds(this);
                }
            }
        }

        private void AddStartItem()
        {
            if (Items.Count == 0)
            {
                Items.Add(new ConversationItem(new Vector2(50, 10), "< Start >", 0));
            }
        }

        public void OnBeforeSerialize()
        {
        }

        public void RemoveEdge(Edge edge)
        {
            if(Edges != null && Edges.Contains(edge))
            {
                Edges.Remove(edge);
            }
        }
    }
}