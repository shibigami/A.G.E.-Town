using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UTalk.Data;

namespace UTalk.Editor
{
    [Serializable]
    public class ConversationEditorWindow : EditorWindow
    {
        #region Private members

        private ConversationDatabase mDatabase = null;

        private Conversation mConversation = null;

        private ConversationItem mCurrentDragItem = null;

        private Edge mPendingConnection = null;

        private Vector2 mScrollPos = Vector2.zero;

        private static Texture2D mTexBackground;

        private GUIStyle mToolbarStyle;

        [HideInInspector]
        [SerializeField]
        private int mConversationIndex = 0;

        #endregion

        [MenuItem("Window/UTalk/Conversation Editor")]
        public static ConversationEditorWindow OpenConversationEditorWindow()
        {
            return EditorWindow.GetWindow<ConversationEditorWindow>("UTalk Editor");
        }

        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool OnOpenDatabase(int instanceID, int line)
        {
            ConversationDatabase database =
                EditorUtility.InstanceIDToObject(instanceID) as ConversationDatabase;
            if (database != null)
            {
                ConversationEditorWindow editorWindow = OpenConversationEditorWindow();
                editorWindow.mDatabase = database;

                if (editorWindow.mDatabase.Conversations.Count > 0)
                    editorWindow.mConversation = editorWindow.mDatabase.Conversations[0];

                return true;
            }
            return false;
        }

        private void SetCurrentConversation()
        {
            if( mConversationIndex < mDatabase.Conversations.Count)
            {
                mConversation = mDatabase.Conversations[mConversationIndex];
            }
            else
            {
                mConversationIndex = 0;
                mConversation = null;
                if (mDatabase.Conversations.Count > 0)
                {
                    mConversation = mDatabase.Conversations[0];

                }
            }

        }

        private void OnEnable()
        {
            mToolbarStyle = new GUIStyle();
            mToolbarStyle.padding.left = 5;
            mToolbarStyle.padding.top = 5;

            mTexBackground = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            mTexBackground.SetPixel(0, 0, new Color(0.65f, 0.65f, 0.65f));
            mTexBackground.Apply();

            mPendingConnection = null;
        }

        private void OnGUI()
        {
            if (mDatabase != null)
            {
                GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), mTexBackground, ScaleMode.StretchToFill);

                GUI.changed = false;

                PaintToolbar();

                SetCurrentConversation();

                // Scrollview
                mScrollPos = EditorGUILayout.BeginScrollView(mScrollPos, false, false);

                Vector2 bounds = GetWindowBounds();

                GUILayout.Label("", GUILayout.Width(bounds.x), GUILayout.Height(bounds.y));

                if (mConversation != null)
                {
                    HandleEvents(Event.current);

                    PaintNodes();

                    PaintEdges();
                }

                EditorGUILayout.EndScrollView();

                if (GUI.changed)
                {
                    EditorUtility.SetDirty(mDatabase);
                }

                // TODO: Optimize this -> Only repaint if needed
                Repaint();
            }
        }

        private void PaintToolbar()
        {
            EditorGUILayout.BeginVertical(mToolbarStyle, GUILayout.Height(18));

            EditorGUILayout.BeginHorizontal();

            List<GUIContent> guiConversations = new List<GUIContent>();

            foreach (var conversation in mDatabase.Conversations)
            {
                guiConversations.Add(new GUIContent(conversation.Name));              
            }

            mConversationIndex = EditorGUILayout.Popup(mConversationIndex, guiConversations.ToArray(), GUILayout.Width(160));

            if (GUILayout.Button("+", GUILayout.Width(18), GUILayout.Height(16)))
            {
                // BUtton pressed -> Add new conversation
                AddNewConversation();

            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void AddNewConversation()
        {
            Conversation newConversation = new Conversation();
            newConversation.Name = "New Conversation";

            mDatabase.Conversations.Add(newConversation);
        }

        private Vector2 GetWindowBounds()
        {
            Vector2 result = Vector2.zero;

            if (mConversation != null)
            {

                foreach (var item in mConversation.Items)
                {

                    if (item.Box.xMax > result.x)
                        result.x = item.Box.xMax;

                    if (item.Box.yMax > result.y)
                        result.y = item.Box.yMax;
                }
            }

            return result;
        }

        private void PaintNodes()
        {
            int i = 0;
            foreach (var item in mConversation.Items)
            {
                // 0 = Root item
                item.Paint(mDatabase, (i == 0));
                i++;
            }
        }

        private void PaintEdges()
        {
            if(mPendingConnection != null)
            {
                mPendingConnection.Paint(Event.current.mousePosition);
            }

            foreach (var edge in mConversation.Edges)
            {
                edge.Paint();
            }
        }

        private void HandleEvents(Event e)
        {
            EItemAction itemAction = EItemAction.None;
            int i = 0;
            foreach (ConversationItem item in mConversation.Items)
            {
                itemAction = item.HandleEvents(e);

                if(itemAction == EItemAction.ContextMenuItem)
                {
                    OpenContextMenuForNode(item, e.mousePosition, i == 0);
                    break;
                }
                else if(itemAction == EItemAction.Select)
                {
                    // TODO: Make object selectable in inspector?
                    //ConversationItemSO so = ScriptableObject.CreateInstance<ConversationItemSO>();
                    //so.Item = item;
                    //Selection.activeObject = so;

                    if(mPendingConnection != null)
                    {
                        mPendingConnection.ToId = item.ID;
                        mConversation.Edges.Add(mPendingConnection);
                        mPendingConnection.InitByIds(mConversation);
                        mPendingConnection = null;
                    }
                    break;
                }
                else if(itemAction == EItemAction.Drag)
                {
                    if(mCurrentDragItem == null)
                        mCurrentDragItem = item;

                    mCurrentDragItem.Drag(e);
                    break;
                }

                i++;
            }

            // Handle edge events
            if(itemAction == EItemAction.None)
            {
                foreach(var edge in mConversation.Edges)
                {
                    itemAction = edge.HandleEvents(e);
                    if(itemAction == EItemAction.ContextMenuEdge)
                    {
                        OpenContextMenuForEdge(edge, e.mousePosition);
                        break;
                    }
                }
            }

            // No item or edge action
            if (itemAction == EItemAction.None)
            {
                switch (e.type)
                {
                    case EventType.MouseDrag:
                        if(mCurrentDragItem != null)
                        {
                            mCurrentDragItem.Drag(e);
                        }
                        break;

                    case EventType.MouseUp:
                        mCurrentDragItem = null;
                        break;

                    case EventType.MouseDown:

                        // Right mouse button
                        if (e.button == 1)
                        {
                            OpenContextMenu(e.mousePosition);
                        }
                        // Left mouse button
                        else if(e.button == 0)
                        {
                            DeletePendingConnection();
                        }
                        break;
                    case EventType.KeyDown:

                        if (e.keyCode == KeyCode.Escape)
                        {
                            DeletePendingConnection();
                        }
                        break;
                }
            }
        }

        private void DeletePendingConnection()
        {
            if (mPendingConnection != null)
            {
                mPendingConnection = null;
            }
        }

        private void OpenContextMenuForNode(ConversationItem node, Vector2 mousePosition, bool isRoot)
        {
            GenericMenu genericMenu = new GenericMenu();

            if (!isRoot)
            {
                // avoid deleting the root node
                genericMenu.AddItem(new GUIContent("Delete Item"),
                    false, () => OnDeleteItem(node));
            }

            genericMenu.AddItem(new GUIContent("Create Item Connection"), 
                false, () => OnCreateConnection(node));

            genericMenu.ShowAsContext();
        }

        private void OpenContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add Item"), 
                false, () => OnAddItem(mousePosition));

            genericMenu.ShowAsContext();
        }

        void OnAddItem(Vector2 mousePosition)
        {
            Actor actor = mDatabase.GetFirstActor();
            if (actor != null)
            {
                ConversationItem node = new ConversationItem(mousePosition,
                    "Item " + (mConversation.Items.Count), actor.Id);

                mConversation.Items.Add(node);
            }
            else
            {
                Debug.LogWarning("UTalk: Please add actors to the conversation database!");
            }
        }

        void OnDeleteItem(ConversationItem node)
        {
            mConversation.RemoveItem(node);
        }

        private void OpenContextMenuForEdge(Edge edge, Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();


            genericMenu.AddItem(new GUIContent("Delete Connection"),
                false, () => OnDeleteConnection(edge));

            genericMenu.ShowAsContext();
        }

        void OnCreateConnection(ConversationItem node)
        {
            mPendingConnection = new Edge(node);
        }

        private void OnDeleteConnection(Edge edge)
        {
            mConversation.RemoveEdge(edge);
        }


    }
}
