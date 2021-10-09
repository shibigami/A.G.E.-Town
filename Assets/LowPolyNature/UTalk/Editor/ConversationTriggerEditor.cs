using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UTalk
{
    public class ConversationEditor : UnityEditor.Editor
    {
        private static int mIndex = 0;

        public void OnEnable()
        {
            IConversationProvider myTarget = this.target as IConversationProvider;

            if (myTarget == null || ConversationManager.Instance.Database == null ||
                ConversationManager.Instance.Database.Conversations == null)
                return;

            int i = 0;
            foreach (var conversation in ConversationManager.Instance.Database.Conversations)
            {
                if (conversation.Name == myTarget.GetConversationName())
                {
                    mIndex = i;
                    break;
                }
                i++;
            }
        }


        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
                return;

            IConversationProvider myTarget = this.target as IConversationProvider;

            if (myTarget == null || ConversationManager.Instance.Database == null ||
    ConversationManager.Instance.Database.Conversations == null)
                return;

            SerializedProperty script = serializedObject.FindProperty("m_Script");

            EditorGUILayout.PropertyField(script);

            SerializedProperty onConversationStart = serializedObject.FindProperty("OnConversationStart");

            EditorGUILayout.PropertyField(onConversationStart);

            SerializedProperty onConversationStop = serializedObject.FindProperty("OnConversationStop");

            EditorGUILayout.PropertyField(onConversationStop);

            List<string> guiConversations = new List<string>();

            foreach (var conversation in ConversationManager.Instance.Database.Conversations)
            {
                guiConversations.Add(conversation.Name);
            }

            mIndex = EditorGUILayout.Popup("Conversation", mIndex, guiConversations.ToArray(), new GUILayoutOption[0]);

            myTarget.SetConversationName(guiConversations[mIndex]);

            serializedObject.ApplyModifiedProperties();


            if (GUI.changed)
            {
                EditorUtility.SetDirty(this.target);
            }

        }

    }

    [CustomEditor(typeof(ConversationTrigger))]
    public class ConversationTriggerEditor : ConversationEditor
    {

    }

    [CustomEditor(typeof(ConversationListener))]
    public class ConversationListenerEditor : ConversationEditor
    {

    }
}
