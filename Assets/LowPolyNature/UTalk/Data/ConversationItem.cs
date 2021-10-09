using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace UTalk.Data
{
    public enum EItemAction
    {
        ContextMenuItem,
        ContextMenuEdge,
        Drag,
        Select,

        None
    }

    //public class ConversationItemSO :  ScriptableObject
    //{
    //    public ConversationItem Item;
    //}

    [Serializable]
    public class ConversationItem : ISerializationCallbackReceiver
    {
        public string Text;

        [HideInInspector]
        public string ID;

        [HideInInspector]
        public int ActorId;

        [HideInInspector]
        public Rect Box;

        [SerializeField]
        [HideInInspector]
        private int mSelectedIndex = 0;

        public string Tag;

        public ConversationItem(Vector2 position, string text, int initialActorId)
        {
            Box = new Rect(position, new Vector2(200, 50));
            Text = text;
            ID = Guid.NewGuid().ToString();
            ActorId = initialActorId;
        }

        public EItemAction HandleEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDrag:
                    if (Box.Contains(e.mousePosition))
                    {
                        return EItemAction.Drag;
                    }
                    break;

                case EventType.MouseDown:
                    if (Box.Contains(e.mousePosition))
                    {
                        // Left click
                        if (e.button == 0)
                        {
                            return EItemAction.Select;
                        }

                        // Right click
                        if (e.button == 1)
                        {
                            return EItemAction.ContextMenuItem;
                        }
                    }

                    break;
            }
            return EItemAction.None;
        }

        public void Drag(Event e)
        {
            Box.position += e.delta;
        }

        public void Paint(ConversationDatabase database, bool isRoot)
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.padding.top = 10;

            if (isRoot)
            {
                PaintRootNode();
            }
            else
            {
                PaintDataNode(database);
            }

        }

        private void PaintRootNode()
        {
#if UNITY_EDITOR
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.padding.top = 15;

            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;

            GUIStyle font = new GUIStyle();
            font.fontStyle = FontStyle.Bold;
            font.alignment = TextAnchor.MiddleCenter;

            GUILayout.BeginArea(Box, style);

            EditorGUILayout.LabelField("< Start >", font);

            GUILayout.EndArea();

            GUI.backgroundColor = oldColor;
#endif
        }

        private void PaintDataNode(ConversationDatabase database)
        {
#if UNITY_EDITOR
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.padding.top = 10;

            Actor selectedActor = database.GetActor(ActorId);

            GUILayout.BeginArea(Box, style);

            GUILayout.BeginHorizontal();

            List<GUIContent> actors = new List<GUIContent>();

            int index = 0;

            foreach (var actor in database.Actors)
            {
                if (selectedActor != null)
                {
                    if (actor.Id == selectedActor.Id)
                        mSelectedIndex = index;
                }
                actors.Add(new GUIContent(actor.Name));
                index++;

            }

            int newIndex = EditorGUILayout.Popup(mSelectedIndex, actors.ToArray(), GUILayout.MaxWidth(60), GUILayout.MinWidth(60));
            if (newIndex != mSelectedIndex)
            {
                mSelectedIndex = newIndex;
                string actorName = actors[mSelectedIndex].text;

                ActorId = database.GetActor(actorName).Id;
            }

            Text = EditorGUILayout.TextField(Text);

            GUILayout.EndHorizontal();

            // Add Tag
            GUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Tag:", GUILayout.MaxWidth(60), GUILayout.MinWidth(60));
            Tag = EditorGUILayout.TextField(Tag);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
#endif
        }

        public void OnBeforeSerialize()
        {
        }

        public void OnAfterDeserialize()
        {
        }
    }
}
