using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace UTalk.Data
{
    [Serializable]
    public class Edge
    {
        [HideInInspector]
        public string FromId;

        [HideInInspector]
        public string ToId;

        private ConversationItem FromItem;

        private ConversationItem ToItem;

        [SerializeField]
        [HideInInspector]
        private Rect Box;

        public Edge(ConversationItem from)
        {
            FromId = from.ID;
            FromItem = from;
            ToItem = null;
        }

        public void InitByIds(Conversation conversation)
        {
            FromItem = conversation.GetConversationItem(FromId);
            ToItem = conversation.GetConversationItem(ToId);
        }

        public void Paint()
        {
#if UNITY_EDITOR
            if (FromItem == null || ToItem == null)
                return;

            Handles.color = Color.white;

            Vector2 pStart = new Vector2(FromItem.Box.width / 2 + FromItem.Box.x,
                FromItem.Box.y + FromItem.Box.height);

            Vector2 pEnd = new Vector2(ToItem.Box.width / 2 + ToItem.Box.x, ToItem.Box.y - 10);

            Vector3[] points = new Vector3[2];
            points[0] = pStart;
            points[1] = pEnd;

            Handles.DrawAAPolyLine(4, points);

            Box = new Rect(pEnd.x - 5, pEnd.y, 10, 10);

            Handles.DrawSolidRectangleWithOutline(Box, Color.white, Color.grey);

#endif
        }

        public void Paint(Vector2 mousePosition)
        {
#if UNITY_EDITOR
            if (FromItem == null)
                return;

            Handles.color = Color.white;
            Vector2 pStart = new Vector2(FromItem.Box.width / 2 + FromItem.Box.x, FromItem.Box.y + FromItem.Box.height);

            Handles.DrawLine(pStart, mousePosition);
#endif
        }

        public EItemAction HandleEvents(Event e)
        {
            EItemAction result = EItemAction.None;

            if (Box != null)
            {
                switch (e.type)
                {
                    case EventType.MouseDown:
                        // Right click
                        if (e.button == 1)
                        {
                            if (Box.Contains(e.mousePosition))
                            {
                                result = EItemAction.ContextMenuEdge;
                            }
                        }
                        break;
                }
            }
            return result;
        }
    }
}
